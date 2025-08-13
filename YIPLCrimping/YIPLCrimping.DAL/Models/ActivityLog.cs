using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimping.DAL.Models
{
    [Table("ActivityLog")]
    public partial class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? PlantId { get; set; }

        public string? Request { get; set; }

        public string? Response { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public string? ApiEndpoint { get; set; }

        public string? HttpMethod { get; set; }
    }
}