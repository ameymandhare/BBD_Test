using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_Plant")]
public partial class MPlant
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PlantCode { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? PlantName { get; set; } = null!;
    public string? City { get; set; } = null;

    public int? CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }
}