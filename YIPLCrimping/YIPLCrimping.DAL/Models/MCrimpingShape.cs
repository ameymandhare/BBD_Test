using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_CrimpingShapes")]
public partial class MCrimpingShape
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    [NotMapped]
    public string? Base64Image { get; set; }

    [NotMapped]
    public string? Base64FileName { get; set; }

    //[InverseProperty("InsulationCrimpShape")]
    public virtual ICollection<CrimpingStandard> CrimpingStandards { get; set; } = new List<CrimpingStandard>();
}