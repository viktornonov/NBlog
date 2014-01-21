using NBlog.Web.Application.Infrastructure;

namespace NBlog.Web.Models
{
    public class Login : LayoutModel
    {
        public string OpenID_Identifier { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }
}