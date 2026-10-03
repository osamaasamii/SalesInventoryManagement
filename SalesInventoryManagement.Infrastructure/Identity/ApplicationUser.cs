using Microsoft.AspNetCore.Identity;

namespace SalesInventoryManagement.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}