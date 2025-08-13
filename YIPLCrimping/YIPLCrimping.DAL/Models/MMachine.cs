using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Models
{
    [Table("M_Machine")]
    public class MMachine
    {
        [Key]
        public int Id { get; set; }

        public string MachineName { get; set; }

        //public float MachineCost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MachineCost { get; set; }

        public int PlantId { get; set; }

        [ForeignKey("PlantId")]
        public int? CreatedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        //[JsonIgnore]
        public virtual MPlant? Plant { get; set; }
    }
}