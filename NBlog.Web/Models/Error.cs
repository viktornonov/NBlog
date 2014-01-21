using NBlog.Web.Application.Infrastructure;

namespace NBlog.Web.Models
{
    public class Error : LayoutModel
    {
        public string Heading { get; set; }
        public string Message { get; set; }
    }
}