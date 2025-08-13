using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C6_CrimpingOtherParameters")]
public partial class T1C6CrimpingOtherParameter
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FrontCuttingCarry { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FronCuttingCarryMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FrontCuttingCarryMax { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RearCuttingCarry { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? RearCuttingCarryMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? RearCuttingCarryMax { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BrushLength { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? BrushLengthMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? BrushLengthMax { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FrontBellMouth { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FrontBellMouthMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FrontBellMouthMax { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RearBellMouth { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? RearBellMouthMin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? RearBellMouthMax { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? BendUp { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BendUpUnit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? BendDown { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? BendDownUnit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Rolling { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RollingUnit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Twist { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TwistUnit { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
}
