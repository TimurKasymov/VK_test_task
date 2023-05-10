using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class UserGroup
    {
        public UserGroup() { }
        public UserGroup(Role role, string description = null)
        {
            Description = description;
            Role = role;
        }
        [JsonIgnore]
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [JsonPropertyName("description")]
        [Column("description")]
        public string Description { get; set; }
        [JsonPropertyName("code")]
        [Column("code")]
        public Role Role { get; set; }
    }
    public enum Role
    {
        Admin = 0, 
        User = 1
    }
}
