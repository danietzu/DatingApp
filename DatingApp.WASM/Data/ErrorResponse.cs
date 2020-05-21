using Newtonsoft.Json;
using System;

namespace DatingApp.WASM.Data
{
    public class ErrorResponse
    {
        [JsonProperty("type")]
        public Uri Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("traceId")]
        public string TraceId { get; set; }

        [JsonProperty("errors")]
        public Errors Errors { get; set; }
    }

    public class Errors
    {
        [JsonProperty("password")]
        public string[] Password { get; set; }

        [JsonProperty("username")]
        public string[] Username { get; set; }
    }
}