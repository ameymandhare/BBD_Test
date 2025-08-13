using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("M_CrimpingStandardWireInfo_Debar")]
public partial class MCrimpingStandardWireInfoDebar
{
    [Key]
    public int Id { get; set; }

    public int CrimpingStandardId { get; set; }

    public int WireIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? WireSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? WireSizeCode { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? WireType { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? WireTypeCode { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CrimpingStandardId")]
    public virtual CrimpingStandardsDebar CrimpingStandard { get; set; } = null!;
}
