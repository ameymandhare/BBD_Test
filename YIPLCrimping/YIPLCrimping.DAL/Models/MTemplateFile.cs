using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimpingAPI.Models;

[Table("M_TemplateFile")]
public partial class MTemplateFile
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? MasterName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FileType { get; set; }

    public string? FilePath { get; set; }

    public byte[]? FileValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }
}

public class MTemplateFileDto
{
    public int? Id { get; set; } // Only for update
    public string? MasterName { get; set; }
    public IFormFile File { get; set; } = null!;
    public int? CreatedBy { get; set; }
}