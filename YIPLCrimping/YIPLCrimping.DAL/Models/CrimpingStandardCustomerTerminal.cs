using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("CrimpingStandardCustomerTerminal")]
public partial class CrimpingStandardCustomerTerminal
{
    [Key]
    public int Id { get; set; }

    public int CrimpingStandardId { get; set; }

    public int CustomerId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string CustomerName { get; set; } = null!;

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CrimpingStandardId")]
    public virtual CrimpingStandard CrimpingStandard { get; set; } = null!;

    [ForeignKey("CustomerId")]
    public virtual MCustomer Customer { get; set; } = null!;
}