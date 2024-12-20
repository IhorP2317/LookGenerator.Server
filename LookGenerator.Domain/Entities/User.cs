using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class User:BaseEntity
    {
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Country { get; set; } = default!;
        public ICollection<Look> Looks { get; set; } = new List<Look>();
    }