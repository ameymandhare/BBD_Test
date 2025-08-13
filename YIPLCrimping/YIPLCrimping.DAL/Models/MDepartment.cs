using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_Department")]
public partial class MDepartment
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string DeptName { get; set; } = null!;

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    //[InverseProperty("Department")]
    //public virtual ICollection<UserAccount> UserAccount { get; set; } = new List<UserAccount>();
}