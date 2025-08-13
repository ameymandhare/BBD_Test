using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C9_TerminalSupplierCrimpingSpec")]
public partial class T1C9TerminalSupplierCrimpingSpec
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }


    [StringLength(100)]
    [Unicode(false)]
    public string? TerminalSupplierName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TerminalSupplierNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ApplicableWireSize { get; set; }

    [Column(TypeName = "numeric(18, 3)")]
    public decimal? ApplicableWireSizeMin { get; set; }

    [Column(TypeName = "numeric(18, 3)")]
    public decimal? ApplicableWireSizeMax { get; set; }

    public int? InsulationCrimpShape { get; set; }

    public string? CC_Height { get; set; }

    public decimal? CC_HeightStd { get; set; }

    public decimal? CC_HeightTolerance { get; set; }

    public string? IC_Height { get; set; }

    public decimal? IC_HeightStd { get; set; }

    public decimal? IC_HeightTolerance { get; set; }

    public string? CC_Width { get; set; }

    public decimal? CC_WidthMIn { get; set; }

    public decimal? CC_WidthMax { get; set; }

    [Column("IC_Width")]
    public string? IC_Width { get; set; }

    public decimal? IC_WidthMin { get; set; }

    public decimal? IC_WidthMax { get; set; }

    public decimal? TensileForce_KGF { get; set; }

    public decimal? TensileForce_N { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? StandardAsperSupplier { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("InsulationCrimpShape")]
    public virtual MCrimpingShape? InsulationCrimpShapeNavigation { get; set; }
}
//public partial class T1C9TerminalSupplierCrimpingSpec
//{
//    [Key]
//    public int Id { get; set; }

//    public int? TerminalNo { get; set; }

//    [StringLength(100)]
//    [Unicode(false)]
//    public string? TerminalSupplierName { get; set; }

//    [StringLength(100)]
//    [Unicode(false)]
//    public string? TerminalSupplierNumber { get; set; }

//    [StringLength(50)]
//    [Unicode(false)]
//    public string? ApplicableWireSize { get; set; }

//    [Column(TypeName = "numeric(18, 3)")]
//    public decimal? ApplicableWireSizeMin { get; set; }

//    [Column(TypeName = "numeric(18, 3)")]
//    public decimal? ApplicableWireSizeMax { get; set; }

//    [StringLength(100)]
//    [Unicode(false)]
//    public string? InsulationCrimpShape { get; set; }


//    public string? CcHeight { get; set; }
//    public decimal? CcHeightStd { get; set; }

//    [Column("CC_HeightTolerance", TypeName = "numeric(18, 3)")]
//    public decimal? CcHeightTolerance { get; set; }

//    public string? IcHeight { get; set; }
//    public decimal? IcHeightStd { get; set; }

//    [Column("IC_HeightTolerance", TypeName = "numeric(18, 3)")]
//    public decimal? IcHeightTolerance { get; set; }


//    public string? CcWidth { get; set; }

//    [Column("CC_WidthMIn", TypeName = "numeric(18, 3)")]
//    public decimal? CcWidthMin { get; set; }

//    [Column("CC_WidthMax", TypeName = "numeric(18, 3)")]
//    public decimal? CcWidthMax { get; set; }

//    public string? IcWidth { get; set; }

//    [Column("IC_WidthMin", TypeName = "numeric(18, 3)")]
//    public decimal? IcWidthMin { get; set; }

//    [Column("IC_WidthMax", TypeName = "numeric(18, 3)")]
//    public decimal? IcWidthMax { get; set; }

//    [Column("TensileForce_KGF", TypeName = "numeric(18, 3)")]
//    public decimal? TensileForceKgf { get; set; }

//    [Column("TensileForce_N", TypeName = "numeric(18, 3)")]
//    public decimal? TensileForceN { get; set; }

//    [StringLength(100)]
//    [Unicode(false)]
//    public string? StandardAsperSupplier { get; set; }

//    public bool? IsActive { get; set; }

//    public int? CreatedById { get; set; }

//    [Column(TypeName = "datetime")]
//    public DateTime? CreatedDate { get; set; }

//    public int? ModifiedById { get; set; }

//    [Column(TypeName = "datetime")]
//    public DateTime? ModifiedDate { get; set; }
//}
