using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Models;
using System.Net.Mail;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
    public partial class ContactController : LayoutController
    {
        public ContactController(IServices services) : base(services) { }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new Contact());
        }

        [HttpPost]
        public ActionResult Index(Contact model)
        {
            if (!ModelState.IsValid)
                return View();

            var from = new MailAddress(model.SenderEmail, model.SenderName);
            var to = new MailAddress(Services.Config.Current.ContactForm.RecipientEmail, Services.Config.Current.ContactForm.RecipientName);
            var mailMessage = new MailMessage(from, to)
            {
                Subject = Services.Config.Current.ContactForm.Subject,
                Body = model.Message,
                IsBodyHtml = false
            };

            Services.Message.SendEmail(mailMessage);

            return RedirectToAction("Confirm");
        }

        [HttpGet]
        public ActionResult Confirm()
        {
            return View();
        }
    }
}
