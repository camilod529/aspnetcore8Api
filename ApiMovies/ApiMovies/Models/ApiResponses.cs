using System.Net;

namespace ApiMovies.Models
{
    public class ApiResponses
    {
        public ApiResponses()
        {
            ErrorMessages = [];
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
