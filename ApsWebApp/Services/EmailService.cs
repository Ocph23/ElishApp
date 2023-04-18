using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;

namespace ApsWebApp.Services
{

    public interface IEmailService
    {
      //  public Task SendEmailAsync(string email, string subject, string htmlMessage);
        public Task SendEmailAsync(Dictionary<string, string> emails, string subject, string htmlMessage, IEnumerable<Attachment> atts=null);
    }

    public class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }
        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task SendEmailAsync(Dictionary<string, string> emails, string subject, string htmlMessage, IEnumerable<Attachment> atts=null)
        {
            try
            {
                IConfigurationSection emailConfiguration = Configuration.GetSection("MailSettings");
                MailjetClient client = new(emailConfiguration["Mail"], emailConfiguration["Password"]);
                var to = new JArray { };
                foreach (var email in emails)
                {
                    to.Add(new JObject { { "Email", email.Key }, { "Name", email.Value } });
                }

                var attachments = new JArray();

                if(atts != null)
                {
                    foreach (var att in atts)
                    {
                        attachments.Add(new JObject {
                               {"ContentType", att.ContentType},
                               {"Filename", att.Filename},
                               {"Base64Content", att.Base64Content}
                               });
                    }

                }

                MailjetRequest request = new MailjetRequest
                {
                    Resource = SendV31.Resource,
                }
                 .Property(Send.Messages,
                    new JArray {
                         new JObject {
                          {
                           "From",
                           new JObject {
                            {"Email", emailConfiguration["Email"]},
                            {"Name", emailConfiguration["DisplayName"]}
                           }
                          }, {
                           "To", to
                          }, {
                           "Subject", subject
                          }, {
                           "TextPart",
                           "None"
                          }, {
                           "HTMLPart",
                           htmlMessage
                          }, {
                           "CustomID",
                           "AppGettingStartedTest"
                          },
                          {"Attachments", attachments}
                          }
                 });
                MailjetResponse response = await client.PostAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                    Debug.WriteLine(response.GetData());
                }
                else
                {
                    Debug.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                    Debug.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                    Debug.WriteLine(response.GetData());
                    Debug.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                }

            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Exception caught in CreateTestMessage2(): {0}",
           ex.ToString());
                throw new SystemException(ex.Message);
            }
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }

    public class Attachment
    {
        public string ContentType { get; set; }
        public string Filename { get; set; }
        public string Base64Content { get; set; }
    }
}
