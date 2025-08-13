using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimping.DAL.Models
{
    public class WireCombinationDetails
    {
        [Key]
        public int Id { get; set; }

        public int? CreatedById { get; set; }

        public string? WireCode { get; set; }
        public string? WireType { get; set; }
        public string? WireSizeCode { get; set; }
        public decimal? WireSize { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }
    }
}