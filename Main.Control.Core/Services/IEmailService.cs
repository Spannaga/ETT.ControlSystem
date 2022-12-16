using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;
using System.Net.Mail;

namespace Main.Control.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(MailDetails mailDetail);

        MailDetails GetMailTemplateByTemplateId(int templateId);

    }
}
