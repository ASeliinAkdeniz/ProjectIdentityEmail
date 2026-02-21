using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using ProjectIdentityEmail.Dtos;

namespace ProjectIdentityEmail.Controllers
{
    public class EmailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(MailRequestDto mailRequestDto)
        {
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin Identity", "akdenizasliselin@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);
            MailboxAddress mailboxAddressTo = new MailboxAddress("User Identity", mailRequestDto.ReceiverEmail);
            mimeMessage.To.Add(mailboxAddressTo);
            mimeMessage.Subject = mailRequestDto.Subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = mailRequestDto.MessageDetail;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("akdenizasliselin@gmail.com", "zlzf rkzy puuk tneh");
            client.Send(mimeMessage);
            client.Disconnect(true);

            return RedirectToAction("Index");

        }
    }
}
