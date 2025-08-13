using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

public partial class CrimpingStandardDy
{
    [Key]
    public int Id { get; set; }

    public int CrimpingStandardId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Anumber { get; set; }

    [Column("ACoductorWidth")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AcoductorWidth { get; set; }

    [Column("AInsulationWidth")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AinsulationWidth { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Wnumber { get; set; }

    [Column("WConductorWidth", TypeName = "decimal(10, 2)")]
    public decimal? WconductorWidth { get; set; }

    [Column("WInsulationWidth", TypeName = "decimal(10, 2)")]
    public decimal? WinsulationWidth { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Inumber { get; set; }

    [Column("IConductorWidth", TypeName = "decimal(10, 2)")]
    public decimal? IconductorWidth { get; set; }

    [Column("IInsulationWidth", TypeName = "decimal(10, 2)")]
    public decimal? IinsulationWidth { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Qnumber { get; set; }

    [Column("QConductorWidth", TypeName = "decimal(10, 2)")]
    public decimal? QconductorWidth { get; set; }

    [Column("QInsulationWidth", TypeName = "decimal(10, 2)")]
    public decimal? QinsulationWidth { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CrimpingStandardId")]
    public virtual CrimpingStandard CrimpingStandard { get; set; } = null!;
}