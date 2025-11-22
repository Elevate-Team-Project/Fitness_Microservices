using System.ComponentModel.DataAnnotations;

namespace ProgressTrackingService.Domain.Entity
{
    public class  BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; } = DateTime. Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }


    }
}
