using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("CrimpingStandards_Debar")]
public partial class CrimpingStandardsDebar
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string Customer { get; set; } = null!;

    public int Plant { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? RegistrationNo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ControlNo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TerminalNo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CommonTerminal { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TerminalName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? AccessoryPartsNo { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? RubberSealPosition { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TerminalThickness { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Feed { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? TerminalSupplierName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TerminalSupplierNo { get; set; }

    [Column("ApplicableWireSize_Upper", TypeName = "decimal(10, 2)")]
    public decimal? ApplicableWireSizeUpper { get; set; }

    [Column("ApplicableWireSize_Lower", TypeName = "decimal(10, 2)")]
    public decimal? ApplicableWireSizeLower { get; set; }

    public int? InsulationCrimpShapeId { get; set; }

    [Column("ConductorCrimpingWidth_LowerLimit", TypeName = "decimal(10, 2)")]
    public decimal? ConductorCrimpingWidthLowerLimit { get; set; }

    [Column("ConductorCrimpingWidth_UpperLimit", TypeName = "decimal(10, 2)")]
    public decimal? ConductorCrimpingWidthUpperLimit { get; set; }

    [Column("ConductorCrimpingWidth_Average", TypeName = "decimal(10, 2)")]
    public decimal? ConductorCrimpingWidthAverage { get; set; }

    [Column("InsulationCrimpingWidth_LowerLimit", TypeName = "decimal(10, 2)")]
    public decimal? InsulationCrimpingWidthLowerLimit { get; set; }

    [Column("InsulationCrimpingWidth_UpperLimit", TypeName = "decimal(10, 2)")]
    public decimal? InsulationCrimpingWidthUpperLimit { get; set; }

    [Column("InsulationCrimpingWidth_Average", TypeName = "decimal(10, 2)")]
    public decimal? InsulationCrimpingWidthAverage { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MiddleStriping { get; set; }

    [Column("MiddleStriping_Upper", TypeName = "decimal(10, 2)")]
    public decimal? MiddleStripingUpper { get; set; }

    [Column("MiddleStriping_Lower", TypeName = "decimal(10, 2)")]
    public decimal? MiddleStripingLower { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? EndStriping { get; set; }

    [Column("EndStriping_Upper", TypeName = "decimal(10, 2)")]
    public decimal? EndStripingUpper { get; set; }

    [Column("EndStriping_Lower", TypeName = "decimal(10, 2)")]
    public decimal? EndStripingLower { get; set; }

    [Column("FrontCuttingCarryMm_Upper", TypeName = "decimal(10, 2)")]
    public decimal? FrontCuttingCarryMmUpper { get; set; }

    [Column("FrontCuttingCarryMm_Lower", TypeName = "decimal(10, 2)")]
    public decimal? FrontCuttingCarryMmLower { get; set; }

    [Column("RearCuttingCarryMm_Upper", TypeName = "decimal(10, 2)")]
    public decimal? RearCuttingCarryMmUpper { get; set; }

    [Column("RearCuttingCarryMm_Lower", TypeName = "decimal(10, 2)")]
    public decimal? RearCuttingCarryMmLower { get; set; }

    [Column("Wire_Upper", TypeName = "decimal(10, 2)")]
    public decimal? WireUpper { get; set; }

    [Column("Wire_Lower", TypeName = "decimal(10, 2)")]
    public decimal? WireLower { get; set; }

    [Column("FrontBellMouthMm_Upper", TypeName = "decimal(10, 2)")]
    public decimal? FrontBellMouthMmUpper { get; set; }

    [Column("FrontBellMouthMm_Lower", TypeName = "decimal(10, 2)")]
    public decimal? FrontBellMouthMmLower { get; set; }

    [Column("RearBellMouthMm_Upper", TypeName = "decimal(10, 2)")]
    public decimal? RearBellMouthMmUpper { get; set; }

    [Column("RearBellMouthMm_Lower", TypeName = "decimal(10, 2)")]
    public decimal? RearBellMouthMmLower { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? BendUpDeg { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? BendDownDeg { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? RollingDeg { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TwistDeg { get; set; }

    [Column(TypeName = "text")]
    public string? TerminalImage { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? MachineName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? MadeBy { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CheckedBy { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ApprovedBy { get; set; }

    public int? PageNumber { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CrimpingStandardCustomerTerminalDebar> CrimpingStandardCustomerTerminalDebars { get; set; } = new List<CrimpingStandardCustomerTerminalDebar>();

    public virtual ICollection<CrimpingStandardDiesDebar> CrimpingStandardDiesDebars { get; set; } = new List<CrimpingStandardDiesDebar>();

    public virtual ICollection<CrimpingStandardKindDetailsDebar> CrimpingStandardKindDetailsDebars { get; set; } = new List<CrimpingStandardKindDetailsDebar>();

    [ForeignKey("InsulationCrimpShapeId")]
    public virtual MCrimpingShape? InsulationCrimpShape { get; set; }

    public virtual ICollection<MCrimpingStandardWireInfoDebar> MCrimpingStandardWireInfoDebars { get; set; } = new List<MCrimpingStandardWireInfoDebar>();

    [ForeignKey("Plant")]
    public virtual MPlant PlantNavigation { get; set; } = null!;
}
