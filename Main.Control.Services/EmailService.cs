using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Services;
using Main.Control.Core.Models;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Main.Control.Service.Utilities;
using Main.Control.Services.Utilities;
using Main.Control.Core.Repositories;
namespace Main.Control.Services
{
    public class EmailService : IEmailService
    {
        #region Declaration
        private readonly IEmailRepository _repository;

        #endregion

        #region Constructor

        public EmailService()
        {
        }

        public EmailService(IEmailRepository repository)
        {
            this._repository = repository;

        }
        #endregion
        public void SendEmail(MailDetails mailDetails)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailmsg = new MailMessage();

            //from address and host details    
            mailDetails.MailHost = ConfigurationManager.AppSettings[Constants.MailHost].ToString();

            //Check whether its local or Using SMTP(LIVE), if not use gmail credentials to send email
            bool _isAWS_SES_Email = Convert.ToBoolean(ConfigurationManager.AppSettings[Constants.IsAWS_SES_Email].ToString());
            //check for from address to be present already
            if (string.IsNullOrEmpty(mailDetails.MailFromAddress) || _isAWS_SES_Email)
            {
                mailDetails.MailFromAddress = ConfigurationManager.AppSettings[Constants.MailFromAddress].ToString();
            }

            if (mailDetails != null)
            {
                if (!string.IsNullOrEmpty(mailDetails.MailHost)
                    && !string.IsNullOrEmpty(mailDetails.MailToAddress)
                    && !string.IsNullOrEmpty(mailDetails.MailFromAddress))
                {
                    //replace Email Subject global legends
                    mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.MailSiteName, ConfigurationManager.AppSettings[Constants.SiteName].ToString());

                    //replace email global constants with app settings
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.MailSiteName, ConfigurationManager.AppSettings[Constants.SiteName].ToString());
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.MailSupportPhone, ConfigurationManager.AppSettings[Constants.SupportPhoneNumber].ToString());
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.MailSupportEmail, ConfigurationManager.AppSettings[Constants.SupportEmail].ToString());
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.MailSiteURL, ConfigurationManager.AppSettings[Constants.SiteURL].ToString());
                    if (mailDetails.MailBody.Contains(Constants.EmailAd))
                    {
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.EmailAd, ConfigurationManager.AppSettings[Constants.EmailAdKey].ToString());
                    }
                    if (mailDetails.MailBody.Contains(Constants.CurrentYear))
                    {
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CurrentYear, DateTime.Now.Year.ToString());
                    }
                  
                    mailDetails.IsAddBCCMail = Utilities.Utilities.GetBool(ConfigurationManager.AppSettings[Constants.IsMailBCC].ToString());
                    string _mailBcc = ConfigurationManager.AppSettings[Constants.MailBCCIds].ToString();
                   

                    if (_isAWS_SES_Email)
                    {
                        string _awsAccessKey = ConfigurationManager.AppSettings[Constants.AWSAccessKey].ToString();
                        string _awsSecretKey = ConfigurationManager.AppSettings[Constants.AWSSecretKey].ToString();
                        AmazonSESWrapper _aws = new AmazonSESWrapper(_awsAccessKey, _awsSecretKey);
                        List<string> _toAddress = new List<string>();

                        if (!string.IsNullOrEmpty(mailDetails.MailToAddress))
                        {
                            if (mailDetails.MailToAddress.Contains(","))
                            {
                                string[] _mailTos = mailDetails.MailToAddress.Split(',');
                                if (_mailTos != null && _mailTos.Length > 0)
                                {
                                    foreach (string _item in _mailTos)
                                    {
                                        _toAddress.Add(_item.Trim());
                                    }
                                }
                            }
                            else
                            {
                                _toAddress.Add(mailDetails.MailToAddress);
                            }
                        }

                        List<string> _bccAddress = new List<string>();
                        if (!string.IsNullOrEmpty(_mailBcc))
                        {
                            if (_mailBcc.Contains(","))
                            {
                                string[] _mailBccs = _mailBcc.Split(',');
                                if (_mailBccs != null && _mailBccs.Length > 0)
                                {
                                    foreach (string _item in _mailBccs)
                                    {
                                        _bccAddress.Add(_item.Trim());
                                    }
                                }
                            }
                            else
                            {
                                _bccAddress.Add(_mailBcc);
                            }
                        }
                        _aws.SendEmail(_toAddress, new List<string>(), _bccAddress, mailDetails.MailFromAddress, mailDetails.MailFromAddress, mailDetails.MailSubject, mailDetails.MailBody);
                    }
                    else
                    {
                        //use gmail as SMTP      
                        mailDetails.MailFromAddress = ConfigurationManager.AppSettings[Constants.MailFromAddress].ToString();
                        //to send email from Vista machine
                        string pswd = ConfigurationManager.AppSettings[Constants.PasswordVista].ToString();

                        //to send email from Vista machine
                        smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings[Constants.MailFromAddress].ToString(), pswd); //  Assign your username and password to connect to gmail     
                        smtpClient.EnableSsl = true;
                        smtpClient.Port = 587;

                        //sends the mail 
                        mailmsg = new MailMessage();
                        mailmsg.IsBodyHtml = true;
                        mailmsg.To.Add(mailDetails.MailToAddress);

                        //add BCC if BCC required is selected
                        if (mailDetails.IsAddBCCMail)
                        {
                            mailmsg.Bcc.Add(_mailBcc);
                        }
                        mailmsg.From = new MailAddress(mailDetails.MailFromAddress);
                        mailmsg.Subject = mailDetails.MailSubject;
                        mailmsg.Body = mailDetails.MailBody;
                        smtpClient.Host = mailDetails.MailHost;

                        smtpClient.Send(mailmsg);
                    }
                }
            }
        }

        public MailDetails GetMailTemplateByTemplateId(int templateId)
        {
            return this._repository.GetMailTemplateByTemplateId(templateId);
        }

    }
}
