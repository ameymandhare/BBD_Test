using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C8_ApprovalDetail")]
public partial class T1C8ApprovalDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    public int? RevisionNo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RevisionDate { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? RevisionDetails { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? MadeBy { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CheckedBy { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ApprovedBy { get; set; }

    [Unicode(false)]
    public string? ManagementApproval { get; set; }

    [Unicode(false)]
    public string? Attachment { get; set; }

    [Unicode(false)]
    public string? ValidationReport { get; set; }

    [Unicode(false)]
    public string? ValidationReportName { get; set; }

    [Unicode(false)]
    public string? TerminalDrawing { get; set; }

    [Unicode(false)]
    public string? TerminalImage { get; set; }

    [Unicode(false)]
    public string? SupplierSpecReport { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }
}
