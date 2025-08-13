using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YIPLCrimpingAPI.Models;

[Table("T1_Terminal")]
public partial class T1Terminal
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? Customer { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Flag { get; set; }

    public int? PlantCode { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? RegistrationNo { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? InsulationCrimpShape { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ManufacturinCrimpNo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TerminalNo { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? CommonTerminalNo { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string? TerminalName { get; set; }

    [Column(TypeName = "numeric(18, 0)")]
    public decimal? TerminalThickness { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ActionStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ActionDate { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }


    public virtual ICollection<T1C1Accessory> Accessories { get; set; } = new List<T1C1Accessory>();
    public virtual ICollection<T1C2ApplicatorDetail> ApplicatorDetails { get; set; } = new List<T1C2ApplicatorDetail>();
    public virtual ICollection<T1C4CombinationDetail> WireCombinations { get; set; } = new List<T1C4CombinationDetail>();
    public virtual ICollection<T1C5CrimpingStandardDetail> CrimpingStandardDetails { get; set; } = new List<T1C5CrimpingStandardDetail>();
    public virtual ICollection<T1C3StripingDetail> StripingDetails { get; set; } = new List<T1C3StripingDetail>();
    public virtual ICollection<T1C6CrimpingOtherParameter> CrimpingOtherParameters { get; set; } = new List<T1C6CrimpingOtherParameter>();
    public virtual ICollection<T1C7CrimpingDiesDetail> CrimpingDiesDetails { get; set; } = new List<T1C7CrimpingDiesDetail>();
    public virtual ICollection<T1C8ApprovalDetail> ApprovalDetails { get; set; } = new List<T1C8ApprovalDetail>();
    public virtual ICollection<T1C9TerminalSupplierCrimpingSpec> TerminalSupplierCrimpingSpec { get; set; } = new List<T1C9TerminalSupplierCrimpingSpec>();



    //public virtual ICollection<T1C5CrimpingStandardDetail> T1C5CrimpingStandardDetails { get; set; } = new List<T1C5CrimpingStandardDetail>();

    //public virtual ICollection<T1C7CrimpingDiesDetail> T1C7CrimpingDiesDetails { get; set; } = new List<T1C7CrimpingDiesDetail>();


    //public virtual ICollection<T1C5CrimpingStandardDetail> CrimpingStandardDetails { get; set; } = new List<T1C5CrimpingStandardDetail>();
    //public virtual ICollection<T1C7CrimpingDiesDetail> CrimpingDiesDetails { get; set; } = new List<T1C7CrimpingDiesDetail>();

    //// Additional navigation properties based on your query
    //public virtual ICollection<T1C1Accessory> Accessories { get; set; } = new List<T1C1Accessory>();
    //public virtual ICollection<T1C2ApplicatorDetail> ApplicatorDetails { get; set; } = new List<T1C2ApplicatorDetail>();
    //public virtual ICollection<T1C4CombinationDetail> WireCombinations { get; set; } = new List<T1C4CombinationDetail>();
    //public virtual ICollection<T1C3StripingDetail> StripingDetails { get; set; } = new List<T1C3StripingDetail>();
    //public virtual ICollection<T1C6CrimpingOtherParameter> CrimpingOtherParameters { get; set; } = new List<T1C6CrimpingOtherParameter>();
    //public virtual ICollection<T1C8ApprovalDetail> ApprovalDetails { get; set; } = new List<T1C8ApprovalDetail>();
    //public virtual ICollection<T1C9TerminalSupplierCrimpingSpec> TerminalSupplierCrimpingSpec { get; set; } = new List<T1C9TerminalSupplierCrimpingSpec>();
}
