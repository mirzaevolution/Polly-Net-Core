using Newtonsoft.Json;

namespace TodoListApp.Models
{
    public class TodoViewModel
    {
        public int UserId { get; set; }
        [JsonProperty("id")]
        public int TodoId { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
