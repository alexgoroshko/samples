namespace UserServices.DTO
{
    public class ContactPersonDto
	{
		public string Id { get; set; }

		public string EmailLogin { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string PreferredLanguage { get; set; }

		public string DisplayName { get; set; }

        public string PhoneNumber { get; set; }

		public string Role { get; set; }

		public int CompanyId { get; set; }
		public bool IsCompanyContactPerson { get; set; }
		public string CountryCode { get; set; }
    }
}
