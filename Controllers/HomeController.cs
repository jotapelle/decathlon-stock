using DecathlonStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net;
using System.Net.Mail;

namespace DecathlonStock.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static bool _isSendedEmail = false;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        #region [HttpPost]
        [HttpPost]
        public JsonResult CheckProduct(string url, string email)
        {
            DateTime now = DateTime.Now;
            ResultProductVM result = new()
            {
                LocalTimeStr = now.ToShortDateString() + " " + now.ToLongTimeString(),
                ResultTxt = "Empty URL or Email"
            };
            if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(email))
            {
                try
                {
                    bool isFinded = ScrapingWeb(url);
                    if (isFinded) SendEmail(url, email);
                    result.ResultTxt = isFinded ? "Product available" : "NO available";
                }
                catch (Exception e)
                {
                    result.ResultTxt = "Error URL/Email";
                }
            }
            return Json(result);
        }

        [HttpPost]
        public void ResetSendEmail()
        {
            _isSendedEmail = false;
        }
        #endregion

        #region [Private]
        private static bool ScrapingWeb(string url)
        {
            ChromeOptions chromeOptions = new();
            bool result = true;
            using (IWebDriver webDriver = new ChromeDriver(chromeOptions))
            {
                WebDriverWait wait = new(webDriver, TimeSpan.FromSeconds(15));
                webDriver.Navigate().GoToUrl(url);
                IWebElement onlineBuying = webDriver.FindElement(By.ClassName("online-buying"));
                if (onlineBuying.Text.ToLower().Contains("agotado")) result = false;
            }
            return result;
        }

        private static void SendEmail(string url, string email)
        {
            if (!_isSendedEmail)
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                var config = builder.Build();

                var smtpClient = new SmtpClient(config["Smtp:Host"])
                {
                    Port = int.Parse(config["Smtp:Port"]),
                    Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(config["Smtp:Username"]),
                    Subject = "Your Decathlon product is in stock!",
                    Body = "<h1>Hello</h1><p>Your product is available <a href='" + url + "'>Link</a></p>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);

                _isSendedEmail = true;
            }
        }
        #endregion
    }
}
