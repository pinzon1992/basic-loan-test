using System.Text.Json.Serialization;

namespace Fundo.Applications.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LoanStatus
    {
        Active = 0,
        Paid = 1,
    }
}
