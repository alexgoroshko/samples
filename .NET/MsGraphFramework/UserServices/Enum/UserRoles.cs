namespace UserServices.Enum
{
    public enum UserRole
    {
        Admin, 
        Employee, 
        CompanyAdmin, 
        CompanyEmployee, 
        LocalCountryAdmin, 
        LocalCountryEmployee
    }

    public static class RoleConstants
    {
        public const string Any = nameof(UserRole.Admin) + "," + 
                                  nameof(UserRole.Employee) + "," + 
                                  nameof(UserRole.CompanyAdmin) + "," + 
                                  nameof(UserRole.CompanyEmployee) + "," + 
                                  nameof(UserRole.LocalCountryAdmin) + "," +
                                  nameof(UserRole.LocalCountryEmployee);
        
        public const string Admins = nameof(UserRole.Admin) + "," + 
                                     nameof(UserRole.CompanyAdmin) + "," + 
                                     nameof(UserRole.LocalCountryAdmin);

        public const string Employees = nameof(UserRole.Employee) + "," + 
                                        nameof(UserRole.CompanyEmployee) + "," +
                                        nameof(UserRole.LocalCountryEmployee);
        
    }
}