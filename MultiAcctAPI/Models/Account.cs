using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiAcctAPI.Models
{
    public class Account
    {
        [Key]
        [JsonIgnore]
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