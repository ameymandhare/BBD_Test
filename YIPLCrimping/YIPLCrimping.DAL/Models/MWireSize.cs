using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimping.DAL.Models
{
    [Table("M_WireSize")]
    public partial class MWireSize
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? WireSizeCode { get; set; } = null!;

        [Column(TypeName = "numeric(18,2)")]
        public decimal WireSize { get; set; }

        public int? CreatedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }
    }
}