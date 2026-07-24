namespace CMSApi.Contracts
{
    public class BaseProfile
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ReturnProfileDTO : BaseProfile
    {
        public Guid Id { get; set; }
        public Uri PictureUrl { get; set; }
    }

    public class RegisterProfileDTO : BaseProfile
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
