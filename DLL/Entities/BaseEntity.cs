using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DLL.Entities
{
    public class BaseEntity
    {

        [JsonPropertyName("id")]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("created_date")]
        [Column("created_date")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public BaseEntity() { }
        public BaseEntity(int id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
}
