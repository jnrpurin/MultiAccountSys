using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MultiAcctAPI.Enums;

namespace MultiAcctAPI.Models
{
    [ExcludeFromCodeCoverage]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; } // "Deposit" or "Withdrawal"

        [Required]
        public DateTime TransactionDate { get; set; }
    }
}