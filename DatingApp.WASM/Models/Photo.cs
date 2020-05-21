using System;
using System.Net.Http;

namespace DatingApp.WASM.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public ByteArrayContent File { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
    }
}