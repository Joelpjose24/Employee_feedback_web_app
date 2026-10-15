using System.Text.Json.Serialization;

namespace EmployeeSurveyApp.Models
{
    public class SurveyQuestionModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "short_text"; // short_text, paragraph, multiple_choice, checkboxes, dropdown, linear_scale, date, time

        [JsonPropertyName("options")]
        public List<string> Options { get; set; } = new();

        [JsonPropertyName("isRequired")]
        public bool IsRequired { get; set; } = false;

        [JsonPropertyName("scaleMin")]
        public int ScaleMin { get; set; } = 1;

        [JsonPropertyName("scaleMax")]
        public int ScaleMax { get; set; } = 5;

        [JsonPropertyName("scaleMinLabel")]
        public string ScaleMinLabel { get; set; } = "Poor";

        [JsonPropertyName("scaleMaxLabel")]
        public string ScaleMaxLabel { get; set; } = "Excellent";
    }
}
