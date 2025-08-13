using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_C7_CrimpingDiesDetail")]
public partial class T1C7CrimpingDiesDetail
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Terminal")]
    public int TerminalId { get; set; }
    public string TerminalNo { get; set; }

    public virtual T1Terminal Terminal { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CrimpingDieNo_AnvilA { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CrimpingDieNo_WireCrimperW { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CrimpingDieNo_InsulationCrimperI { get; set; }

    public decimal? CrimpingDieNo_StabilizerCrimperQ { get; set; }

    public decimal? DiesCrimpingWidth_ConductorAnvilA { get; set; }

    public decimal? DiesCrimpingWidth_InsulationAnvilA { get; set; }

    public decimal? DiesCrimpingWidth_WireCrimperW { get; set; }

    public decimal? DiesCrimpingWidth_InsulationCrimperI { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ConductorDieThickness { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? InsulationDieThickness { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    //[ForeignKey("TerminalNo")]
    //public virtual T1Terminal? TerminalNoNavigation { get; set; }
}
