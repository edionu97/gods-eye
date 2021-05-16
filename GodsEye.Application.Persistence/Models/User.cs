using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GodsEye.Application.Persistence.Models
{
    public class User
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [JsonProperty("Password")]
        public string PasswordHash { get; set; }

        [Column]
        public string Username { get; set; }
    }
}
