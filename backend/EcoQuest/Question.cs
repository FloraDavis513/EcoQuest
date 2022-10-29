using System.Text.Json.Serialization;

namespace EcoQuest
{
    public partial class Question
    {
        public long Id { get; set; }
        public string? Answer { get; set; }
        [JsonPropertyName("QuestionType")]
        public string? Type { get; set; }
        public string? ShortText { get; set; }
        public string? Text { get; set; }
        public long? ProductId { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }
    }
}