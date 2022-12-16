using Main.Control.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Repositories
{
    public interface IEmailRepository
    {
        MailDetails GetMailTemplateByTemplateId(int templateId);
    }
}
