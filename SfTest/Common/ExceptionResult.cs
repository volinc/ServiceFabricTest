using System.ComponentModel;
using System.Net;

namespace Common
{
    public class ExceptionResult
    {        
        public ExceptionResult(HttpStatusCode statusCode, string title, string detail = null)
        {
            StatusCode = statusCode;
            Title = title;
            Detail = detail;
        }

        public HttpStatusCode StatusCode { get; set; }

        [Localizable(true)]
        public string Title { get; set; }

        [Localizable(true)]
        public string Detail { get; set; }        
    }
}
