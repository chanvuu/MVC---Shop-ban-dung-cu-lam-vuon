using Shop.Models;

namespace Shop.N8.Models
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<ApplicationUser> applicationUsers { get; set; } = new List<ApplicationUser>();

    }
}
