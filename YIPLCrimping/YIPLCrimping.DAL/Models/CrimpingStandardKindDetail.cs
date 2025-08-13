using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("CrimpingStandard_KindDetails")]
public partial class CrimpingStandardKindDetail
{
    [Key]
    public int Id { get; set; }

    public int CrimpingStandardId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? KindName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Size { get; set; }

    [Column("Crimping_ConductorHieight_upperLimit", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingConductorHieightUpperLimit { get; set; }

    [Column("Crimping_ConductorHieight_lowerLimit", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingConductorHieightLowerLimit { get; set; }

    [Column("Crimping_ConductorHieight_Average", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingConductorHieightAverage { get; set; }

    [Column("Crimping_InsulationHieight_upperLimit", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingInsulationHieightUpperLimit { get; set; }

    [Column("Crimping_InsulationHieight_lowerLimit", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingInsulationHieightLowerLimit { get; set; }

    [Column("Crimping_InsulationHieight_Average", TypeName = "decimal(10, 2)")]
    public decimal? CrimpingInsulationHieightAverage { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Figure { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? S { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TensileForceKgf { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TensileForceN { get; set; }

    [ForeignKey("CrimpingStandardId")]
    public virtual CrimpingStandard CrimpingStandard { get; set; } = null!;
}