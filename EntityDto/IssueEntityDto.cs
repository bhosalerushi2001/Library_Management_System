using Newtonsoft.Json;

namespace Library_Management_System.EntityDto
{
    public class IssueEntityDto
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }
    
        [JsonProperty(PropertyName = "uId", NullValueHandling = NullValueHandling.Ignore)]
        public string UId { get; set; }

        [JsonProperty(PropertyName = "bookId", NullValueHandling = NullValueHandling.Ignore)]
        public string BookId { get; set; }

        [JsonProperty(PropertyName = "memberId", NullValueHandling = NullValueHandling.Ignore)]
        public string MemberId { get; set; }

        [JsonProperty(PropertyName = "issueDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime IssueDate { get; set; }

        [JsonProperty(PropertyName = "dateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ReturnDate { get; set; }

        [JsonProperty(PropertyName = "isReturned", NullValueHandling = NullValueHandling.Ignore)]
        public Boolean IsReturned { get; set; }
    }
}
