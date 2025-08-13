using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_Customer")]
public partial class MCustomer
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? CustomerName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? CustomerCode { get; set; } = null!;

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    //[InverseProperty("Customer")]
    //public virtual ICollection<CrimpingStandardCustomerTerminal> CrimpingStandardCustomerTerminals { get; set; } = new List<CrimpingStandardCustomerTerminal>();
}