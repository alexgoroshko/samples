using System.Linq;
using AutoMapper;
using MsGraph.Classes;
using MsGraph.GraphModels;
using MsGraph.Utils;

namespace UserServices.Services.AzureUserServices
{
	public class AzureMapperProfile : Profile
    {
        private bool IsUserBeingCreated(User user)
        {
            return !string.IsNullOrEmpty(user.Password);
        }

        private GraphUserIdentity[] MakeIdentities(User user)
        {
            return new GraphUserIdentity[]
            {
                new()
                {
                    SignInType = GraphConstants.SignInTypeEmailAddress,
                    IssuerAssignedId = user.EmailLogin.Trim(),
                    Issuer = GraphConfig.The.TenantDomain,
                }
            };
        }

        private string GetEmailLogin(GraphUser graphUser)
        {
            return graphUser.Identities?.FirstOrDefault(i => i.SignInType == GraphConstants.SignInTypeEmailAddress)?.IssuerAssignedId;
        }

        public AzureMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<User, GraphUser>()
                .ForMember(gu => gu.GivenName, act => act.MapFrom(u => u.FirstName))
                .ForMember(gu => gu.Surname, act => act.MapFrom(u => u.LastName))
                .ForMember(gu => gu.MobilePhone, act => act.MapFrom(u => u.PhoneNumber ?? " "))
                .ForMember(gu => gu.ExtnEnvName, act => act.MapFrom(u => u.EnvName))
                .ForMember(gu => gu.ExtnRole, act => act.MapFrom(u => u.Role))
                .ForMember(gu => gu.ExtnGdprTermsAccepted, act => act.MapFrom(u => u.GdprTermsAccepted))
                .ForMember(gu => gu.ExtnCompanyId, act => act.MapFrom(u => u.CompanyId))
                .ForMember(gu => gu.ExtnIsCompanyContactPerson, act => act.MapFrom(u => u.IsCompanyContactPerson))
                .ForMember(gu => gu.ExtnCountryCode, act => act.MapFrom(u => u.CountryCode))
                .ForMember(gu => gu.DisplayName, act => act.MapFrom(u => u.FirstName.IsBlank() && u.LastName.IsBlank() ? null : $"{u.FirstName} {u.LastName}"))
                .ForMember(gu => gu.AccountEnabled, act => act.MapFrom(u => IsUserBeingCreated(u) ? true : default(bool?)))
                .ForMember(gu => gu.PasswordPolicies, act => act.MapFrom(u => IsUserBeingCreated(u) ? GraphConstants.DisablePasswordExpiration : default))
                .ForMember(gu => gu.PasswordProfile, act => act.MapFrom(u => IsUserBeingCreated(u) ? new GraphUserPasswordprofile { ForceChangePasswordNextSignIn = true, Password = u.Password.Trim()} : default))
                .ForMember(gu => gu.Identities, act => act.MapFrom(u =>  MakeIdentities(u)))
                ;

            CreateMap<GraphUser, User>()
                .ForMember(u => u.FirstName, act => act.MapFrom(gu => gu.GivenName))
                .ForMember(u => u.LastName, act => act.MapFrom(gu => gu.Surname))
                .ForMember(u => u.PhoneNumber, act => act.MapFrom(gu => gu.MobilePhone == " " ? null : gu.MobilePhone))
                .ForMember(u => u.Password, act => act.MapFrom(gu => default(string)))
                .ForMember(u => u.EnvName, act => act.MapFrom(gu => gu.ExtnEnvName))
                .ForMember(u => u.Role, act => act.MapFrom(gu => gu.ExtnRole))
                .ForMember(u => u.GdprTermsAccepted, act => act.MapFrom(gu => gu.ExtnGdprTermsAccepted))
                .ForMember(u => u.CompanyId, act => act.MapFrom(gu => gu.ExtnCompanyId))
                .ForMember(u => u.IsCompanyContactPerson, act => act.MapFrom(gu => gu.ExtnIsCompanyContactPerson))
                .ForMember(u => u.CountryCode, act => act.MapFrom(gu => gu.ExtnCountryCode))
                .ForMember(u => u.EmailLogin, act => act.MapFrom(gu => GetEmailLogin(gu)))
                ;
        }
    }

}
