using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Resources
{
    public class EmailRepository : IEmailRepository
    {
        #region Declaration
        private MainControlDB_Entities _context;
        #endregion

        #region Constructor
        public EmailRepository()
        {
            _context = new MainControlDB_Entities();
        }
        #endregion

        public MailDetails GetMailTemplateByTemplateId(int templateId)
        {
            using (var _context = new MainControlDB_Entities())
            {
                MailDetails _mailDetails = null;
                Static_Biz_Mail_templates _dbMailTemplate = _context.Static_Biz_Mail_templates.SingleOrDefault(m => m.Mail_Template_Id == templateId && !m.Is_Deleted);
                if (_dbMailTemplate != null)
                {
                    _mailDetails = new MailDetails();
                    _mailDetails.MailTemplateName = _dbMailTemplate.Mail_Template_Name;

                    _mailDetails.MailSubject = _dbMailTemplate.Mail_Subject;
                    _mailDetails.MailBody = _dbMailTemplate.Mail_Body;
                }
            //return null if no data exists for such id.
            return _mailDetails;
            }
        }

    }
}
