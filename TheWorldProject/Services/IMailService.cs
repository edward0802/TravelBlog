using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TheWorldProject.Services
{
    public interface IMailService
    {
        void SendMail(string to, string from, string subject, string body);
        void SendMessage(string name, string email, string msg);
        Task ConfirmationByEmailMsgAsync(string email, string subject, string callbackUrl);
    }
}
