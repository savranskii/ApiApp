using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiApp.Models;

public class Entity
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("operationDate")]
    public DateTime OperationDate { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    public Entity()
    {
        Id = Guid.NewGuid();
        OperationDate = DateTime.Now;
    }
}
