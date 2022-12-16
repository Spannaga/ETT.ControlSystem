using CampaignBulkUploadService.BLL;
using ProcessBulkUpload;
using ProcessBulkUpload.Utilities;
using Main.Control.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CampaignBulkUploadService
{
    public partial class CampaignUpload : ServiceBase
    {
        private CampaignUploadBLL campaignUploadBLL = new CampaignUploadBLL();
        private BulkUploadCommon bulkUploadCommon = new BulkUploadCommon();
        Timer _timer = new Timer();
        public bool _isServiceInProcess = false;
        public bool _isTimerDisabled = false;
        EventLog _eventLog = new EventLog();
        public int timerCount = 2;

        public CampaignUpload()
        {
            InitializeComponent();
            // StartService();
            _timer.Enabled = true;
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Interval = Utility.GetDouble(ConfigurationManager.AppSettings[Constants.TimerInterval]);
            _eventLog.Source = ConfigurationManager.AppSettings[Constants.EventLogSource];
        }

        #region Timer Elapsed Event
        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //disabe the timer to avoid unwanted call before completion of process.
            _timer.Enabled = false;

            //Start the Service for the Time Interval given
            StartService();

            //enable the timer, if the process completes.
            if (!_isTimerDisabled)
            {
                _timer.Enabled = true;
            }
        }
        #endregion

        #region Start Service
        public void StartService()
        {
            try
            {
                if (!_isServiceInProcess && timerCount > 0)
                {
                    _isServiceInProcess = true;

                    CampaignDetails campaignDetails = campaignUploadBLL.GetCampaignInitDetails();

                    if (campaignDetails != null && campaignDetails.CampaignDetailsId > 0)
                    {
                        campaignDetails.IsBatchProcess = true;
                        campaignDetails.IsBatchStatus = false;
                        campaignUploadBLL.CampaignSetInProgress(campaignDetails);
                        campaignDetails.BatchStatus = BatchUploadStatus.IN_PROGRESS.ToString();
                        campaignDetails.IsManagerFollowUp = false;
                        campaignDetails = campaignUploadBLL.SetSupportAdminUserList(campaignDetails);
                        campaignDetails = bulkUploadCommon.CommonExcelValuesAssignedCampaignDetails(campaignDetails);
                        campaignUploadBLL.SaveCampaignDetails(campaignDetails);
                        campaignUploadBLL.UpdateUploadStatus(campaignDetails);
                    }
                    //DateTime systemDate = DateTime.Now;
                    string[] tempTime = DateTime.Now.TimeOfDay.ToString(@"hh\:mm").Split(':');
                    TimeSpan rime = new TimeSpan(Utility.GetInt(tempTime[0]), Utility.GetInt(tempTime[1]), 0);
                    string[] configHourMinute = Utility.GetAppSettings("DefaultEntryTime").ToString().Split(':');
                    DateTime configDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Utility.GetInt(configHourMinute[0]), Utility.GetInt(configHourMinute[1]), 0);
                    int isTimeToRunService = TimeSpan.Compare(rime, configDate.TimeOfDay);   // will return 0 if both are equal
                    if (isTimeToRunService == 0)
                    {
                        CampaignDetails FollowupCampaignDetails = new CampaignDetails();
                        FollowupCampaignDetails.CampaignAssignedDetailsList = campaignUploadBLL.GetManagerFollowUpDetails();
                        if (FollowupCampaignDetails != null && FollowupCampaignDetails.CampaignAssignedDetailsList.Any() && FollowupCampaignDetails.CampaignAssignedDetailsList.Count() > 0)
                        {
                            FollowupCampaignDetails = campaignUploadBLL.FollowUpCampaignDetails(FollowupCampaignDetails.CampaignAssignedDetailsList);
                            FollowupCampaignDetails.IsManagerFollowUp = true;
                            FollowupCampaignDetails = campaignUploadBLL.SaveCampaignDetails(FollowupCampaignDetails);
                            campaignUploadBLL.UpdateUploadStatus(FollowupCampaignDetails);
                        }
                    }
                    _isServiceInProcess = false;
                }
                timerCount++;
            }
            catch (Exception ex)
            {
                _isServiceInProcess = true;
                _timer.Stop();
                timerCount = 0;
                //Send the Exceptions as Email
                SendExceptionEmail(ex);
                //log exception
                string _exceptMsg = "Campaign Upload \n" + ex.Message + "\n" + ex.ToString();
                _eventLog.WriteEntry(_exceptMsg, EventLogEntryType.Error);
                this.Stop();
            }
        }

        private void SendExceptionEmail(Exception exception)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailmsg = new MailMessage();
            //from address and host details           
            string _mailHost = ConfigurationManager.AppSettings[Constants.MailHost];

            //get the from address 
            string _mailFromAddress = ConfigurationManager.AppSettings[Constants.ExceptionFromMail];
            string _mailToAddress = ConfigurationManager.AppSettings[Constants.ExceptionToMail];

            if (!string.IsNullOrEmpty(_mailHost) && !string.IsNullOrEmpty(_mailToAddress) && !string.IsNullOrEmpty(_mailFromAddress))
            {
                string pswd = ConfigurationManager.AppSettings[Constants.ExceptionMailPassword];
                smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings[Constants.ExceptionFromMail], pswd);
                smtpClient.EnableSsl = true;
                smtpClient.Port = 587;
                //replace Email Subject global legends
                string _mailSubject = ConfigurationManager.AppSettings[Constants.ExceptionMailSubject].ToString();
                //replace email global constants with app settings
                string _mailBody = exception.ToString();
                //sends the mail 
                mailmsg = new MailMessage();
                mailmsg.IsBodyHtml = true;
                mailmsg.To.Add(_mailToAddress);

                mailmsg.From = new MailAddress(_mailFromAddress);
                mailmsg.Subject = _mailSubject;
                mailmsg.Body = _mailBody;
                smtpClient.Host = _mailHost;

                try
                {
                    smtpClient.Send(mailmsg);
                    if (mailmsg.Attachments != null)
                    {
                        mailmsg.Dispose();
                    }
                }
                catch (Exception ex) //we just need to write to event log as there is not much we can do here
                {
                    _isServiceInProcess = true;
                    //Stop the timer
                    _timer.Stop();

                    System.Diagnostics.EventLog _eventLog = new System.Diagnostics.EventLog("Application", Environment.MachineName, ConfigurationManager.AppSettings[Constants.EventLogSource]);
                    _eventLog.WriteEntry(ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                    //stop the service
                    this.Stop();
                }
            }
        }
        #endregion


    }
}
