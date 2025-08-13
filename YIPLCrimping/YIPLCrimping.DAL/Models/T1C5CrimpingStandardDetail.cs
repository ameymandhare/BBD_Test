using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C5_CrimpingStandardDetail")]
public partial class T1C5CrimpingStandardDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ApplicableWireSize { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? WireMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? WireMax { get; set; }

    public int? InsulationCrimpShape { get; set; }


    public string? Ccheight { get; set; }

    [Column("CCHeightStd", TypeName = "numeric(18, 2)")]
    public decimal? CcheightStd { get; set; }

    [Column("CCHeightVariance", TypeName = "numeric(18, 2)")]
    public decimal? CcheightVariance { get; set; }

    public string? Icheight { get; set; }

    [Column("ICHeightStd", TypeName = "numeric(18, 2)")]
    public decimal? IcheightStd { get; set; }

    [Column("ICHeightVariance", TypeName = "numeric(18, 2)")]
    public decimal? IcheightVariance { get; set; }


    public string? Ccwidth { get; set; }

    [Column("CCWidthMin", TypeName = "numeric(18, 2)")]
    public decimal? CcwidthMin { get; set; }

    [Column("CCWidthMax", TypeName = "numeric(18, 2)")]
    public decimal? CcwidthMax { get; set; }


    public string? Icwidth { get; set; }

    [Column("ICWidthMin", TypeName = "numeric(18, 2)")]
    public decimal? IcwidthMin { get; set; }

    [Column("ICWidthMax", TypeName = "numeric(18, 2)")]
    public decimal? IcwidthMax { get; set; }

    [Column("TensileForceKGF", TypeName = "numeric(18, 2)")]
    public decimal? TensileForceKgf { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? TensileForceN { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PillShape { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Soldering { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("InsulationCrimpShape")]
    public virtual MCrimpingShape? InsulationCrimpShapeNavigation { get; set; }

    //[ForeignKey("TerminalNo")]
    //public virtual T1Terminal? TerminalNoNavigation { get; set; }
}
