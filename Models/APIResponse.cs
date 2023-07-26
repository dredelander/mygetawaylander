using System.Net;
namespace cr2Project.Models
{
	public class APIResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; } = true;
		public List<string> ErrorMessages { get; set; }
		public object Response { get; set; }
        public List<string> Message { get; set; }


    }
}

