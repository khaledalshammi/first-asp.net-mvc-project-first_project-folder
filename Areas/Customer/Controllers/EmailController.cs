using Microsoft.AspNetCore.Mvc;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace first_project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class EmailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string useremail)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string smtpServer = configuration["SmtpSettings:SmtpServer"];
            int smtpPort = int.Parse(configuration["SmtpSettings:SmtpPort"]);
            string smtpUsername = configuration["SmtpSettings:SmtpUsername"];
            string smtpPassword = configuration["SmtpSettings:SmtpPassword"];

            //string smtpServer = "smtp.gmail.com";
            //int smtpPort = 587;
            //string smtpUsername = "kkhhaa2002yl@gmail.com";
            //string smtpPassword = "loxkjlpdiuvkchib";


            var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("Khaled Alshammi", "kkhhaa2002yl@gmail.com"));
            //message.To.Add(new MailboxAddress(null, useremail));
            message.From.Add(new MailboxAddress(null, useremail));
            message.To.Add(new MailboxAddress("Khaled Alshammi", "kkhhaa2002yl@gmail.com"));
            message.Subject = "Help request from - " + useremail;
            message.Body = new TextPart("plain") { Text = "TEST" };

            using (var client = new SmtpClient())
            {
                client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(smtpUsername, smtpPassword);
                client.Send(message);
                client.Disconnect(true);
            }

            TempData["success"] = "Done";
            return RedirectToAction("index");
        }
    }
}
