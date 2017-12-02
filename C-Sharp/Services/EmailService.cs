using GSwap.Models.Requests.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using GSwap.Models.Requests.Tests;
using System.Data.SqlClient;
using GSwap.Data.Providers;
using GSwap.Models.Requests.Users;
using System.Web.Configuration;
using GSwap.Models.Requests;

namespace GSwap.Services
{
    public class EmailService : IEmailService
    {
        private IDataProvider _dataProvider;
        private ISiteConfigService _siteConfigService;

        public EmailService(IDataProvider dataProvider, ISiteConfigService siteConfigService)
        {
            _dataProvider = dataProvider;
            _siteConfigService = siteConfigService;
        }

        public async Task<bool> Send(EmailRequest request)
        {

            Response response = null;
            bool result = false;
            try
            {
                SendGridClient client = new SendGridClient(_siteConfigService.SendGridApiKey);
                SendGridMessage msg = new SendGridMessage();
                msg.AddTo(new EmailAddress(request.To));
                msg.From = new EmailAddress(request.From);
                msg.Subject = request.Subject;
                msg.HtmlContent = CreateBody(request);


                response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            //throw exception based on respoinse message if false
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                result = true;
            }
            else
            {
                throw new Exception(response.Body.ToString());
            }

            return result;

        }

        private string CreateBody(EmailRequest request)
        {
            string body;

            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~\Content\email_sendgrid.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{title}", request.Subject);
            body = body.Replace("{body}", request.Body);
           

            return body;
        }

       

        public async Task<bool> SendRecovery(RecoveryRequest request, Guid guidToken)
        {
            Response response = null;
            bool result = false;
            try
            {
                SendGridClient client = new SendGridClient(_siteConfigService.SendGridApiKey);
                SendGridMessage msg = new SendGridMessage();

                msg.AddTo(new EmailAddress(request.Email));
                msg.From = new EmailAddress(_siteConfigService.SiteAdminEmailAddress);
                msg.Subject = "Password Reset";
                msg.HtmlContent = RecoveryBody(request,guidToken);




                response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
          
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                result = true;
            }
            else
            {
                throw new Exception(response.Body.ToString());
            }

            return result;


        }
        private string RecoveryBody(RecoveryRequest request, Guid guidToken)
        {
            string body;
            
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~\Content\email_recovery.html")))
            {
                body = reader.ReadToEnd();
            }
            
            body = body.Replace("{body}", "Follow this link to reset your password.");
            body = body.Replace("{guid}", guidToken.ToString());
            body = body.Replace("{domain}", _siteConfigService.SiteDomain);
            return body;
        }


        public async Task<bool> ConfirmationSend(UserAddRequest request, Guid guidToken)
        {
            //return bool based on response code 202 or 200
            Response response = null;
            bool result = false;
            try
            {
                SendGridClient client = new SendGridClient(_siteConfigService.SendGridApiKey);
                SendGridMessage msg = new SendGridMessage();
                msg.AddTo(new EmailAddress(request.Email));
                msg.From = new EmailAddress(_siteConfigService.SiteAdminEmailAddress);
                msg.Subject = "Confirm Registration to GSwap";
                msg.HtmlContent = ConfirmationBody(request, guidToken);


                response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            //throw exception based on respoinse message if false
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                result = true;
            }
            else
            {
                throw new Exception(response.Body.ToString());
            }

            return result;

        }

        private string ConfirmationBody(UserAddRequest request, Guid guidToken)
        {
            string body;

            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~\Content\confirmationEmail_sendgrid.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{title}", "Account Confirmation");
            body = body.Replace("{token}", guidToken.ToString());
            body = body.Replace("{body}", "Please click the link below or copy and paste into your browser");
            body = body.Replace("{domain}", _siteConfigService.SiteDomain);
            return body;
        }

        public async Task<bool> ResetConfirmation(string userEmail)
        {
            Response response = null;
            bool result = false;
            try
            {
                SendGridClient client = new SendGridClient(_siteConfigService.SendGridApiKey);
                SendGridMessage msg = new SendGridMessage();

                msg.AddTo(new EmailAddress(userEmail));
                msg.From = new EmailAddress(_siteConfigService.SiteAdminEmailAddress);
                msg.Subject = "Reset Confirmation!";
                msg.HtmlContent = ConfirmationBody(userEmail);




                response = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                result = true;
            }
            else
            {
                throw new Exception(response.Body.ToString());
            }

            return result;


        }
        private string ConfirmationBody(string userEmail)
        {
            string body;
            

            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~\Content\email_resetConfirmation.html")))
            {
                body = reader.ReadToEnd();
            }
            
            body = body.Replace("{body}", "Password has been successfully reset!");
            //body = body.Replace("{title}", "Gourmet Swap ©");
           


            return body;
        }

       
        //Gets Id by Email
        public int GetIdByEmail(string email)
        {
            int id = 0;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@Email", email);

                SqlParameter outputParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                outputParameter.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(outputParameter);
            };

            Action<SqlParameterCollection> returnParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                Int32.TryParse(paramCollection["@Id"].Value.ToString(), out id);
               
            };

            string proc = "dbo.Users_SelectIdByEmail";
            _dataProvider.ExecuteNonQuery(proc, inputParamDelegate, returnParamDelegate);


            return id;

        }

       
   
       

        

    }
}
