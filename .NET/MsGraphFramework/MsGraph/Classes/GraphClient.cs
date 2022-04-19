using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;
using MsGraph.GraphModels;
using MsGraph.Utils;

namespace MsGraph.Classes
{

    /*
     * Docs:
     * list users:  https://docs.microsoft.com/en-us/graph/api/user-list?view=graph-rest-1.0&tabs=http
     * get token: https://docs.microsoft.com/en-us/graph/auth-v2-service#4-get-an-access-token
     * register graph app: https://docs.microsoft.com/en-us/azure/active-directory-b2c/microsoft-graph-get-started?tabs=app-reg-ga
     * Manage Azure AD B2C with Microsoft Graph https://docs.microsoft.com/en-us/azure/active-directory-b2c/microsoft-graph-operations
     * use query parameters https://docs.microsoft.com/en-us/graph/query-parameters
     *
     * CUSTOM POLICIES: 
     * - with consent
     * https://justidm.wordpress.com/2018/10/01/add-a-terms-of-use-consent-page-to-azure-ad-b2c-user-journey-with-custom-policies/
     * https://github.com/pstapf/B2C-custom-policy-with-consent
     * 
     * - invite
     * https://github.com/azure-ad-b2c/samples/tree/master/policies/invite
     */
    
    public class GraphClient
    {
        
        #region private

        private readonly GraphConfig _graphConfig;
        private readonly GraphJson _graphJson;
        
        /// <summary>
        /// Returns response string after attempting to interpret it as an <see cref="GraphError"/> response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<string> ReadResponseStringSafeAsync(HttpResponseMessage response)
        {
            var s = await response.Content.ReadAsStringAsync();
            GraphError error;
            try
            {
                error = _graphJson.Deserialize<GraphError>(s);
            }
            catch (Exception e)
            {
                throw new GraphStatusCodeException((int)response.StatusCode, "Graph Error", e); 
            }

            if (error.Error != null) throw new GraphStatusCodeException((int)response.StatusCode, $"[{error.Error.Code}: {error.Error.Message}]");

            return s;
        }

        /// <summary>
        /// Makes sure response is 204 which means ok for delete and patch requests
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task CheckResponseForNoContent(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                var s = await ReadResponseStringSafeAsync(response); //in the hope of extrating a meaningful error from it and throwing http status exception

                throw new GraphStatusCodeException((int)response.StatusCode, "Graph error. Was expecting 204 but got response which could not be parsed", new Exception(s));
                
            }
        }

        private static string _token;
        private DateTime _tokenReceivedAt = DateTime.MinValue; 


        private async Task<string> GetToken()
        {
            
            //token usually valid for 1 hr but lets get it every 30 min to be safe
            if (!_token.IsBlank() && (DateTime.Now - _tokenReceivedAt).TotalMinutes < 30) return _token;

            var url = $"https://login.microsoftonline.com/{_graphConfig.TenantId}/oauth2/v2.0/token";


            using (var client = new HttpClient())
            {
                var data =
                    $"client_id={_graphConfig.GraphAppClientId}" +
                    "&scope=https%3A%2F%2Fgraph.microsoft.com%2F.default" +
                    $"&client_secret={_graphConfig.GraphAppClientSecret}" +
                    "&grant_type=client_credentials";

                var response = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"));
                var json = await response.Content.ReadAsStringAsync();
               
                var getTokenResponse = JsonSerializer.Deserialize<GraphTokenResponse>(json);
                _token = getTokenResponse?.access_token;
                if (_token.IsBlank()) throw new Exception($"Unalbe to get MS Graph access token", new Exception(json)); 
                //Console.WriteLine($"TOKEN:{_token}");
                _tokenReceivedAt = DateTime.Now;
                return _token;
            }
        }

        private async Task<HttpClient> CreateClient()
        {
            var token = await GetToken();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task<string> GraphGet(string url)
        {
            using var client = await CreateClient();
            using var response = await client.GetAsync(url);
            return await ReadResponseStringSafeAsync(response);
        }

        private async Task GraphDelete(string url)
        {
            using var client = await CreateClient();
            using var response = await client.DeleteAsync(url);
            await CheckResponseForNoContent(response);
        }
        
        private async Task GraphPatch(string url, string json)
        {
            using var client = await CreateClient();
            using var response = await client.PatchAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            await CheckResponseForNoContent(response);
        }


        private async Task<string> GraphPost(string url, string json)
        {
            using var client = await CreateClient();
            using var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return await ReadResponseStringSafeAsync(response);
        }


        private static void CheckUserId(GraphUser user)
        {
            if (user.Id.IsBlank()) throw new ArgumentException("Missing User ID");
        }

        #endregion private

        #region public


        public GraphClient(GraphConfig graphConfig, GraphJson graphJson)
        {
            _graphJson = graphJson;
            _graphConfig = graphConfig;
        }


        /// <summary>
        /// Returns full blown <see cref="GraphCustomAttributes"/> list of custom attributes
        /// </summary>
        /// <returns></returns>
        private async Task<GraphCustomAttributes> GetCustomAttributes()
        {
            var json = await GraphGet(Url.Combine(GraphConstants.GraphApplicationsUrl, _graphConfig.B2CExtensionAppObjectId, "extensionProperties")); //
            return _graphJson.Deserialize<GraphCustomAttributes>(json);
        }

        /// <summary>
        /// Returns only names of custom attributes
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> GetCustomAttributeNames()
        {
            var attribs = await GetCustomAttributes();
            return attribs.Items.Select(i => _graphConfig.CustomAttributeShortName(i.Name)).ToList();
        }

        private string DefaultSelectQuery => "$select=" + string.Join(",", _graphConfig.B2CPropertyList<GraphUser>());

        public async Task<GraphUser> GetUser(string id) 
        {
            var json = await GraphGet(Url.Combine(GraphConstants.GraphUsersUrl, id + "?" + DefaultSelectQuery));
            var x = _graphJson.Deserialize<GraphUser>(json);
            return x;
        }

        public async Task<GraphUsers> GetUsers(int top, string filterQuery = "",  bool autoFollowNextLink = false)
        {
            var selectQuery = DefaultSelectQuery;
            var topQuery = $"$top={top}";
            var query = GraphQuery.Combine(selectQuery, filterQuery, topQuery);
            var json = await GraphGet(GraphConstants.GraphUsersUrl + query);

            var result = _graphJson.Deserialize<GraphUsers>(json);

            var nextLink = result?.OdataNextLink;

            if (result?.Items != null && !nextLink.IsBlank() && autoFollowNextLink)
            {
                var iterations = 1;
                var combinedList = result.Items?.ToList();
                while (!nextLink.IsBlank())
                {
                    var nextJson = await GraphGet(nextLink);
                    var nextList = _graphJson.Deserialize<GraphUsers>(nextJson);
                    combinedList.AddRange(nextList?.Items ?? Array.Empty<GraphUser>());
                    iterations++;
                    nextLink = nextList?.OdataNextLink;
                }

                result.Items = combinedList.ToArray();
                result.OdataNextLink = null;
                result.Iterations = iterations;
            }
            return result;
        }

        public async Task<GraphUsers> GetUsersNextPage(string nextLink)
        {
            return _graphJson.Deserialize<GraphUsers>(await GraphGet(nextLink));
        }

        public async Task UpdateUser(GraphUser user)
        {
            CheckUserId(user);
            await GraphPatch(Url.Combine(GraphConstants.GraphUsersUrl, user.Id), _graphJson.Serialize(user));
        }
        
        /// <summary>
        /// Creates new B2C user. Required properties are DisplayName, Identities and PasswordProfile.
        /// Use this method when you must pass additional properties (such as phone number) when creating a new user. Otherwise use the overload which is much safer.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<GraphUser> CreateUser(GraphUser user)
        {
            if (user.Surname.IsBlank()) throw new ArgumentException("User must have at least a last name");
            if (user.DisplayName.IsBlank()) throw new ArgumentException("User must have a DisplayName");
            if (user.Identities == null || user.Identities.Length < 1) throw new ArgumentException("No Identities specified");
            var identitiy = user.Identities.FirstOrDefault(i => i.SignInType == GraphConstants.SignInTypeEmailAddress);
            if (identitiy == null) throw new ArgumentException($"No Identity found with signin type '{GraphConstants.SignInTypeEmailAddress}'");
            if (identitiy.IssuerAssignedId.IsBlank()) throw new ArgumentException($"No email specified!");
            if (user.PasswordProfile == null) throw new ArgumentException("User must have a PasswordProfile");
            if (user.PasswordProfile.Password.IsBlank()) throw new ArgumentException("New user must have a password");
            user.Id = null;
            user.AccountEnabled = true;
            user.PasswordPolicies = "DisablePasswordExpiration";

            var json = _graphJson.Serialize(user);
            return _graphJson.Deserialize<GraphUser>(await GraphPost(GraphConstants.GraphUsersUrl, json));
        }

      
        public async Task<GraphUser> CreateUser(string email, string firstName, string lastName, string password, string displayNamePrefix = "", bool forceChangePasswordNextSignIn = false)
        {
            var user = new GraphUser
            {
                GivenName = firstName,
                Surname = lastName,
                DisplayName = $"{displayNamePrefix}{firstName} {lastName}",
                AccountEnabled = true,
                PasswordPolicies = "DisablePasswordExpiration",
                Identities = new GraphUserIdentity[]
                {
                    new()
                    {
                        SignInType = GraphConstants.SignInTypeEmailAddress,
                        IssuerAssignedId = email,
                        Issuer = _graphConfig.TenantDomain,
                    }
                },
                PasswordProfile = new GraphUserPasswordprofile { ForceChangePasswordNextSignIn = forceChangePasswordNextSignIn, Password = password },
            };
            return await CreateUser(user);
        }

        //If this returns "Insufficient privileges to complete the operation" then do this:
        //Navigate to Azure Portal > Azure AD > Roles and administrators > User Administrator > Click on Add Assignments > select the application > click on Add button.
        //(https://docs.microsoft.com/en-us/answers/questions/9024/error-while-updating-the-password-profile.html)
        public async Task ResetPassword(string id, string newTempPassword)
        {
            var user = new GraphUser()
            {
                Id = id,
                PasswordProfile = new GraphUserPasswordprofile()
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = newTempPassword
                }
            };
            
            await UpdateUser(user);
        }
        
        public async Task DisableUser(string id)
        {
            var cleanId = Guid.Parse(id).ToString("N");
            var user = new GraphUser
            {
                Id = id,
                GivenName = "Deleted",
                Surname = "Deleted",
                DisplayName = $"Deleted {DateTime.Now}",
                ExtnGdprTermsAccepted = false,
                ExtnIsCompanyContactPerson = false,
                MobilePhone = "+5(555)-555-5555",
                AccountEnabled = false,

                Identities = new GraphUserIdentity[]
                {
                    new()
                    {
                        SignInType = GraphConstants.SignInTypeEmailAddress,
                        IssuerAssignedId = $"deleted_{cleanId}@{cleanId}.com",
                        Issuer = _graphConfig.TenantDomain,
                    }
                },
                PasswordProfile = new GraphUserPasswordprofile()
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = "DELETED_" + Guid.NewGuid().ToString("N").ToLower()
                }
            };
            await UpdateUser(user);
        }

        public async Task DeleteUser(string userId)
        {
            await GraphDelete(Url.Combine(GraphConstants.GraphUsersUrl, userId));
        }


        public async Task DeleteUser(GraphUser user)
        {
            CheckUserId(user);
            await DeleteUser(user.Id);
        }
        
        #endregion public
    }
}
