using LookGenerator.Domain.Entities;

namespace LookGenerator.Domain.Common ;

    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt{ get; set; }
        public Guid? CreatedBy{ get; set; }
        public User? Creator{ get; set; }
        public Guid? ModifiedBy{ get; set; }
        public DateTime? ModifiedAt{ get; set; }
    }