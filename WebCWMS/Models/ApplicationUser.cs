using Microsoft.AspNetCore.Identity;

namespace WebCWMS.Models
{
	public class ApplicationUser : IdentityUser
	{
		public bool IsApproved { get; set; }
		public byte[] ProfilePicture { get; set; }
	}
}
