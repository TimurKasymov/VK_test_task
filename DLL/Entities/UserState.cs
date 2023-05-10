

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class UserState
    {
        public UserState() { }
        public UserState(State code, string description = null)
        {
            Description = description;
            State = code;
        }
        [Key]
        [JsonIgnore]
        [Column("id")]
        public int Id { get; set; }
        [JsonPropertyName("description")]
        [Column("description")]
        public string Description { get; set; }
        [JsonPropertyName("code")]
        [Column("code")]
        public State State { get; set; } = State.Active;

    }
    public enum State
    {
        Active = 0, 
        Blocked = 1
    }
}
