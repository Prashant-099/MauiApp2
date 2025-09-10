using System.Text.Json.Serialization;

namespace MauiApp2.Models
{
    public class ImageModel
    {
        [JsonPropertyName("imgDataId")]
        public int ImgDataId { get; set; }

        [JsonPropertyName("imgDataExtractedText")]
        public string? ImgDataExtractedText { get; set; }

        [JsonPropertyName("imgDataImg")]
        public string? ImgDataImg { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("imgDataTimedate")]
        public DateTime? ImgDataTimedate { get; set; }

        [JsonPropertyName("imgDataLocation")]
        public string? ImgDataLocation { get; set; }
        // Optional properties (based on JSON):
        [JsonPropertyName("imgDataUserUploaded")]
        public string? ImgDataUserUploaded { get; set; } = "1";

        [JsonPropertyName("imgDataCreated")]
        public DateTime? ImgDataCreated { get; set; } = DateTime.Now;

        [JsonPropertyName("imgDataUpdated")]
        public DateTime? ImgDataUpdated { get; set; } = DateTime.Now;

        [JsonPropertyName("imgDataCreateUid")]
        public string? ImgDataCreateUid { get; set; } 

        [JsonPropertyName("imgDataEditedUid")]
        public string? ImgDataEditedUid {get; set;} 
    }
    public class ImageResponse
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<ImageModel> data { get; set; } = new();
    }
}
 