using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C4_CombinationDetail")]
public partial class T1C4CombinationDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? WireCode { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? WireType { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? WireSizeCode { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? WireSize { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
}
