using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C3_StripingDetails")]
public partial class T1C3StripingDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? MiddelStriping { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? MiddelStrippingUpperLimit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? MiddelStrippingLowerLimit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? EndStriping { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? EndStripingUpperLimit { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? EndStripingLowerLimit { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
}
