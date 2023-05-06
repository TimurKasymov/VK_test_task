

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class UserState : BaseEntity
    {
        public UserState() { }
        public UserState(State code, string description = null)
        {
            Description = description;
            State = code;
        }
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
