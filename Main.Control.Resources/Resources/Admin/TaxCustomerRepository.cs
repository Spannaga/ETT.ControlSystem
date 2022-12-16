using System;
using System.Collections.Generic;
using System.Linq;
using Main.Control.Core.Models;
using Main.Control.Core.Repositories;

using Main.Control.Resources.Utilities;



namespace Main.Control.Resources
{
    public class TaxCustomerRepository : ITaxCustomerRepository
    {
        MainControlDB_Entities _entities = new MainControlDB_Entities();

        #region Dashboard -> Lead Log FollowUps
        /// <summary>
        /// Dashboard -> Lead Log FollowUps
        /// </summary>
        /// <returns></returns>
        public List<LeadLog> GetAdminLeadLogForDashboard(long _adminUserId)
        {
            List<LeadLog> _adminLeadLogList = new List<LeadLog>();

            DateTime _startDate = DateTime.Now.AddDays(-7);

            var _dbAdminLeadLogList = (from b in _entities.Tax_Lead_Log
                                           where b.Is_Deleted == false 
                                           && (_adminUserId > 0 ? b.Admin_User_Id == _adminUserId : true)
                                           select b).ToList();

            if (_dbAdminLeadLogList != null && _dbAdminLeadLogList.Count() > 0)
            {
                foreach (var _dbAdminLeadLog in _dbAdminLeadLogList)
                {
                    bool _IsNote = false;
                    LeadLog _adminLeadLog = new LeadLog();
                    _adminLeadLog.LeadLogId = _dbAdminLeadLog.Lead_Log_Id;
                    _adminLeadLog.BusinessName = _dbAdminLeadLog.Business_Name;
                    _adminLeadLog.ContactName = _dbAdminLeadLog.Contact_Name;
                    _adminLeadLog.EmailAddress = _dbAdminLeadLog.Email_Address;
                    _adminLeadLog.PhoneNumber = _dbAdminLeadLog.Phone_Number;

                    long _leadStatusId = GetLastLeadLogFollow(_dbAdminLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbAdminLeadLog.Lead_Log_Id).Lead_Status_Id ?? 0 : 0;
                    long _leadStatusDetailId = GetLastLeadLogFollow(_dbAdminLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbAdminLeadLog.Lead_Log_Id).Lead_Status_SubcategoryId ?? 0 : 0;

                    if (_leadStatusId > 0)
                    {
                        _adminLeadLog.LeadStatus = GetLeadStatusValue(_leadStatusId).ToString();
                    }

                    if (_leadStatusDetailId > 0)
                    {
                        _adminLeadLog.LeadStatusDetail = GetSubLeadStatusValue(_leadStatusDetailId).ToString();
                    }

                    //_adminLeadLog.FollowUpDate = _dbAdminLeadLog.FollowUp_date;
                    Tax_Lead_Log_Follow _tax_Lead_Log_Follow= GetLastFollowUpDateLast(_dbAdminLeadLog.Lead_Log_Id);
                    if(_tax_Lead_Log_Follow != null)
                    {
                        _adminLeadLog.FollowUpDate = _tax_Lead_Log_Follow.FollowUp_date;
                        //_adminLeadLog.LastNoteAdded = _tax_Lead_Log_Follow.Create_Time_Stamp;
                        _IsNote = true;
                    }
                    else
                    {
                        _IsNote = false;
                    }

                    if (_IsNote)
                    {
                        _adminLeadLogList.Add(_adminLeadLog);
                    }
                }

                _adminLeadLogList = _adminLeadLogList.Where(a=>a.FollowUpDate>_startDate).OrderBy(a => a.FollowUpDate).ToList();
            }
            return _adminLeadLogList;
        }
        #endregion

        #region Get FollowUp Date by LeadLogId
        public Tax_Lead_Log_Follow GetLastLeadLogFollow(long AdminLeadLogId)
        {
            DateTime _date = DateTime.MinValue;

            var _dbFollowUps = _entities.Tax_Lead_Log_Follow.Where(i => i.LeadLog_Id == AdminLeadLogId && !i.Is_Dont_Follow_Up && !i.Is_Deleted).OrderByDescending(i => i.FollowUp_date).ToList();
            var FollowUp = _dbFollowUps.Where(fu => fu.LeadLog_Follow_Id == (_dbFollowUps.Max(i => i.LeadLog_Follow_Id))).SingleOrDefault();
            if (FollowUp != null)
            {
                return FollowUp;
            }
            return null;
        }
        #endregion

        #region Get Lead Status Value
        public string GetLeadStatusValue(long leadStatusId)
        {
            long _leadStatusId = DataUtility.GetLong(leadStatusId);
            var _dbleadStatus = (from st in _entities.Static_Biz_Lead_Status
                                 where st.Lead_StatusId == leadStatusId && !st.Is_Deleted
                                 select st.Lead_Status).SingleOrDefault();
         
            return _dbleadStatus;
        }
        #endregion

        #region Get Sub Lead Status Value
        public string GetSubLeadStatusValue(long subleadStatusId)
        {
            long _SubleadStatusId = DataUtility.GetLong(subleadStatusId);
            var _dbleadStatus = (from st in _entities.Static_Biz_Lead_Status_Details
                                 where st.Lead_Status_detailsId == _SubleadStatusId && !st.Is_Deleted
                                 select st.Lead_Status_details).SingleOrDefault();
            

            return _dbleadStatus;
        }
        #endregion

        #region Get FollowUp Date Last by LeadLogId
        public Tax_Lead_Log_Follow GetLastFollowUpDateLast(long AdminLeadLogId)
        {
            DateTime _date = DateTime.MinValue;
            List<Tax_Lead_Log_Follow> _dbFollowUps = _entities.Tax_Lead_Log_Follow.Where(i => i.LeadLog_Id == AdminLeadLogId && !i.Is_Dont_Follow_Up && !i.Is_Deleted).OrderByDescending(i => i.FollowUp_date).ToList();
            Tax_Lead_Log_Follow _newMsg = _dbFollowUps.Where(fu => fu.LeadLog_Follow_Id == (_dbFollowUps.Max(i => i.LeadLog_Follow_Id))).SingleOrDefault();
            if (_newMsg != null)
            {

                //_date = DataUtility.GetDateTime(_newMsg.FollowUp_date);
                _date = DataUtility.GetDateTime(_newMsg.FollowUp_date);
            }
            return _newMsg;
        }
        #endregion

    }
}
