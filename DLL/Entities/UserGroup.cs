using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class UserGroup : BaseEntity
    {
        public UserGroup() { }
        public UserGroup(Role role, string description = null)
        {
            Description = description;
            Role = role;
        }

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
