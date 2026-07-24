using Microsoft.AspNetCore.Identity;

namespace Core.Models
{
    public class ProfileModel : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
