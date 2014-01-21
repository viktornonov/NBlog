using NBlog.Web.Application.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NBlog.Web.Models
{
    public class Contact : LayoutModel
    {
            [DisplayName("Your name")]
            [Required(ErrorMessage = "Please enter your name.")]
            public string SenderName { get; set; }

            [DisplayName("Your email address")]
            [Required(ErrorMessage = "Please enter your email address.")]
            public string SenderEmail { get; set; }

            [Required(ErrorMessage = "Please enter your message.")]
            public string Message { get; set; }
    }
}