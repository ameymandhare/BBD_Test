using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_Role")]
public partial class MRole
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    //[InverseProperty("MRoleCode")]
    //public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}