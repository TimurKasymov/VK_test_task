using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class User
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("created_date")]

        [JsonPropertyName("created_date")]
        [JsonIgnore]
        public DateTime CreationDate { get; set; }
        [JsonPropertyName("login")]
        [Column("login")]
        public string Login { get; set; }
        [JsonPropertyName("password")]
        [Column("password")]
        public string Password { get; set; }
        [JsonPropertyName("user_group")]
        [Column("user_group_id")]
        public UserGroup Group { get; set; }
        [JsonPropertyName("user_state")]
        [Column("user_state_id")]
        public UserState State { get; set; }

        public User()
        {

        }
        public User(string login, string password, UserGroup userGroup, UserState userState)
        {
            Login = login;
            Group = userGroup;
            State = userState;
            Password = password;
            CreationDate = DateTime.UtcNow;
        }
    }
}
