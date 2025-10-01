using E_Model.Authentication.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace E_Common
{
    // 👉 Bọc extension vào 1 class static
    public static class MailHelperExtensions
    {
        public static IServiceCollection AddMailHelper(this IServiceCollection services, IConfiguration configuration)
        {
            // Init MailHelper từ appsettings.json
            MailHelper.Init(configuration);
            return services;
        }
    }

    public static class MailHelper
    {
        private static EmailSettings _settings;

        public static void Init(IConfiguration configuration)
        {
            var section = configuration.GetSection("EmailSettings");
            _settings = new EmailSettings
            {
                FromEmail = section["FromEmail"],
                Password = section["Password"],
                SmtpHost = section["SmtpHost"],
                SmtpPort = int.Parse(section["SmtpPort"]),
                EnableSsl = bool.Parse(section["EnableSsl"])
            };
        }

        public static async Task<bool> SendMailAsync(MailModel mail)
        {
            if (_settings == null)
                throw new InvalidOperationException("MailHelper chưa được cấu hình. Hãy gọi AddMailHelper() trong Program.cs.");

            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail),
                    Subject = mail.Subject,
                    Body = mail.Body,
                    IsBodyHtml = mail.IsBodyHtml
                };

                message.To.Add(mail.To);

                using (var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort))
                {
                    client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);
                    client.EnableSsl = _settings.EnableSsl;

                    await client.SendMailAsync(message);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Gửi OTP với giao diện Bootstrap 5
        /// </summary>
        public static async Task<bool> SendOtpAsync(string toEmail, string code, int minute)
        {
            var subject = "Mã xác thực OTP";
            var body = $@"<!DOCTYPE html>
                          <html lang='vi'>
                          <head>
                              <meta charset='UTF-8'>
                              <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
                          </head>
                          <body style='background-color:#f8f9fa;padding:20px;'>
                              <div class='container'>
                                  <div class='row justify-content-center'>
                                      <div class='col-md-6'>
                                          <div class='card shadow-lg border-0 rounded-3'>
                                              <div class='card-body text-center'>
                                                  <h4 class='card-title mb-3 text-primary'>Xác thực OTP</h4>
                                                  <p class='mb-2'>Xin chào,</p>
                                                  <p class='mb-4'>
                                                      Mã OTP của bạn là:
                                                  </p>
                                                  <h2 class='fw-bold text-white bg-primary d-inline-block px-4 py-2 rounded'>
                                                      {code}
                                                  </h2>
                                                  <p class='mt-4 text-muted'>
                                                      Mã có hiệu lực trong <strong>{minute} phút</strong>. 
                                                      Vui lòng không chia sẻ mã này cho bất kỳ ai.
                                                  </p>
                                                  <hr />
                                                  <p class='small text-muted mb-0'>Đây là email tự động, vui lòng không trả lời.</p>
                                              </div>
                                          </div>
                                      </div>
                                  </div>
                              </div>
                          </body>
                          </html>";

            var mail = new MailBO
            {
                Subject = subject,
                Content = body,
                From = "SendOTP@mektec.nokgrp.com",
                To = toEmail,
                CC = ""
            };

            return await MailHelper_Api.SendMailByApiAsync(mail);
        }
    }

    public static class MailHelper_Api
    {
        private static readonly string ApiUrl = "http://10.80.1.75:8080/api/v1/mail/sendmail";


        /// <summary>
        /// Hàm chung gửi mail qua API
        /// </summary>
        public static async Task<bool> SendMailByApiAsync(MailBO mailBO)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var jsonData = JsonConvert.SerializeObject(mailBO);
                    var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                    {
                        Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
                    };

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent.Contains("successfully", StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

    }
}
