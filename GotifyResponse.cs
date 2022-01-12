using Newtonsoft.Json;

namespace LupuServ
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class GotifyResponse
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("appid")] public int Appid { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("priority")] public int Priority { get; set; }

        [JsonProperty("date")] public string Date { get; set; }
    }
}