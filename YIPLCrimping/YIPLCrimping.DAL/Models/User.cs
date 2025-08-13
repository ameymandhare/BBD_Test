using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YIPLCrimping.DAL.Models
{
    [Table("UserDetails")]
    public partial class UserDetails
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? WorkPhone { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public string? Manager { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? WorkMail { get; set; }
        public string? OfficeLocation { get; set; }
        public string? GradeCode { get; set; }
        public string? CostCenter { get; set; }
        public string? CostCenterCode { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? PAN { get; set; }
        public string? Location { get; set; }
        public string? Gender { get; set; }
        public string? Passport { get; set; }
        public DateTime? DOB { get; set; }
        public string? ReportTo { get; set; }
        public string? Phone { get; set; }
        public string? PRID { get; set; }
    }
}