using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimping.DAL.Models
{
    [Table("M_WireType")]
    public partial class MWireType
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? WireTypeCode { get; set; } = null!;

        [StringLength(100)]
        [Unicode(false)]
        public string? WireTypeName { get; set; } = null!;

        public int? CreatedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }
    }
}