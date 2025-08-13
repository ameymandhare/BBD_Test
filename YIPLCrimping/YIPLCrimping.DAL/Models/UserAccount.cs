using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("UserAccount")]
public partial class UserAccount
{
    [Key]
    public int? Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? EmployeeId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? UserName { get; set; }

    public int? RoleCode { get; set; }

    public int? Plant { get; set; }

    public int? DepartmentId { get; set; }

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [ForeignKey("DepartmentId")]
    public virtual MDepartment? Department { get; set; }

    [ForeignKey("Plant")]
    public virtual MPlant? MPlant { get; set; }

    [ForeignKey("RoleCode")]
    public virtual MRole? MRoleCode { get; set; }
}