using Microsoft.AspNetCore.Identity;

namespace Reports.Api.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public Role() { }

        public Role(string name) : base(name) { }
    }
}
