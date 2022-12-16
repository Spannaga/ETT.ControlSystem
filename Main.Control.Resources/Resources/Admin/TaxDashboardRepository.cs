using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using Main.Control.Resources.Utilities;

namespace Main.Control.Resources
{
    public class TaxDashboardRepository : ITaxDashboardRepository
    {
        #region Declaration
        private MainControlDB_Entities _entities = new MainControlDB_Entities();

        #endregion


        #region Get LeadLog Status by LeadStatus

        public List<LeadLog> GetLeadLogStatusbyLeadStatus(long _adminUserId)
        {
            List<LeadLog> _leadStatusList = new List<LeadLog>();

            var _dbLeadStatus = (from n in _entities.Tax_Lead_Log
                                 where n.Is_Deleted == false
                                 && (_adminUserId > 0 ? n.Admin_User_Id == _adminUserId : true)
                                 select n).ToList();

            if (_dbLeadStatus != null && _dbLeadStatus.Count > 0)
            {
                foreach (var item in _dbLeadStatus)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.AdminFollowupId = item.Lead_Log_Id;
                    
                    long _leadStatusId = GetLastLeadLogFollow(item.Lead_Log_Id) != null ? GetLastLeadLogFollow(item.Lead_Log_Id).Lead_Status_Id ?? 0 : 0;

                    if (_leadStatusId > 0)
                    {
                        _leadLog.LeadStatus = GetLeadStatusValue(_leadStatusId).ToString();
                        _leadLog.LeadStatusId = _leadStatusId;
                    }
                   
                    _leadStatusList.Add(_leadLog);
                }
            }
            return _leadStatusList;

        }

        public List<LeadLog> GetLeadLogStatusbyAdminUser()
        {
            long client = (long)AdminLeadType.Client;
            List<LeadLog> _leadStatusList = new List<LeadLog>();

            var _dbLeadStatus = (from n in _entities.Tax_Lead_Log
                                 join o in _entities.Biz_Admin_Users on n.Admin_User_Id equals o.Admin_User_Id
                                 where n.Is_Deleted == false
                                 select new { n.Lead_Log_Id, n.Admin_User_Id, o.Admin_User_Name} ).ToList();

            if (_dbLeadStatus != null && _dbLeadStatus.Count > 0)
            {
                foreach (var item in _dbLeadStatus)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.AdminUserId = item.Admin_User_Id ?? 0;
                    _leadLog.AdminUserName = item.Admin_User_Name;

                    long _leadStatusId = GetLastLeadLogFollow(item.Lead_Log_Id) != null ? GetLastLeadLogFollow(item.Lead_Log_Id).Lead_Status_Id ?? 0 : 0;

                    if (client == _leadStatusId)
                    {
                        _leadStatusList.Add(_leadLog);
                    }
                }
            }
            return _leadStatusList;

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


    }
}
