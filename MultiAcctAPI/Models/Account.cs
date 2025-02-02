using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace MultiAcctAPI.Models
{
    [ExcludeFromCodeCoverage]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountId { get; set; }

        [Required]
        public required string AccountName { get; set; }

        [Required]
        public decimal CurrentBalance { get; set; } = 0;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserId { get; set; }
    }
}