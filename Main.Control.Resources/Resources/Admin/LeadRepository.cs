using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using Main.Control.Resources.Utilities;

namespace Main.Control.Resources
{
    public class LeadRepository : ILeadRepository
    {
        #region Declaration
        MainControlDB_Entities _entities;
        #endregion

        #region Constructor
        public LeadRepository()
        {
            _entities = new MainControlDB_Entities();

        }
        #endregion

        #region Get Lead Log
        public List<LeadLog> GetLeadLog(DateTime fromDate, DateTime toDate, long? adminName, string status, string email, string businessName, long? stateId, DateTime logDate, string contactName, string phonenumber, long? adminUserId, bool unassigned)
        {
            bool _lastFollowUpDatecheck = true;
            long dataentry = (long)AdminRoleType.Team;
            string _stateId = DataUtility.GetStringValue(stateId);
            List<LeadLog> _leadLogList = new List<LeadLog>();
            toDate = toDate.AddDays(1).AddSeconds(-1);
            DateTime _templogDate = logDate.AddDays(1).AddSeconds(-1);
            toDate = toDate.AddDays(1).AddSeconds(-1);
            var _dbLeadLogList = (from l in _entities.Tax_Lead_Log
                                  join m in _entities.Biz_Admin_Users on l.Admin_User_Id equals m.Admin_User_Id
                                  join n in _entities.Biz_Admin_User_Roles on m.Admin_User_Id equals n.Admin_User_Id
                                  orderby l.Logged_Date ascending
                                  where l.Is_Deleted == false && l.Create_Time_Stamp >= fromDate && l.Create_Time_Stamp <= toDate && n.Admin_Role_Id != dataentry
                                  //&& (adminName != null && adminName != string.Empty ? l.Admin_User_Name == adminName : true)
                                  && (adminName != null && adminName > 0 ? l.Sales_Person_Id == adminName : true)
                                  && (email != null && email != string.Empty ? l.Email_Address == email : true)
                                  && (businessName != null && businessName != string.Empty ? l.Business_Name == businessName : true)
                                  && (stateId != null && stateId > 0 ? l.State == _stateId : true)
                                  && (adminUserId != null && adminUserId > 0 ? l.Admin_User_Id == adminUserId : true)
                                  && (contactName != null && contactName != string.Empty ? l.Contact_Name == contactName : true)
                                  && (phonenumber != null && phonenumber != string.Empty ? l.Phone_Number == phonenumber : true)
                                  select new
                                  {
                                      l.Lead_Log_Id,
                                      l.Email_Address,
                                      l.Website_Address,
                                      l.Contact_Name,
                                      l.Business_Name,
                                      l.Phone_Number,
                                      l.Create_Time_Stamp,
                                      l.State,
                                      l.Admin_User_Id,
                                      l.Admin_User_Name,
                                      role = "Normal"
                                  }).Union(from l in _entities.Tax_Lead_Log
                                           join m in _entities.Biz_Admin_Users on l.Admin_User_Id equals m.Admin_User_Id
                                           join n in _entities.Biz_Admin_User_Roles on m.Admin_User_Id equals n.Admin_User_Id
                                           orderby l.Logged_Date ascending
                                           where l.Is_Deleted == false && n.Admin_Role_Id == dataentry
                                           select new
                                           {
                                               l.Lead_Log_Id,
                                               l.Email_Address,
                                               l.Website_Address,
                                               l.Contact_Name,
                                               l.Business_Name,
                                               l.Phone_Number,
                                               l.Create_Time_Stamp,
                                               l.State,
                                               l.Admin_User_Id,
                                               l.Admin_User_Name,
                                               role = "DataEntry"

                                           }).ToList();


            if (_dbLeadLogList != null && _dbLeadLogList.Count() > 0)
            {
                foreach (var _dbLeadLog in _dbLeadLogList)
                {
                    LeadLog _leadLog = new LeadLog();
                    string _role = "";
                    _leadLog.LeadLogId = _dbLeadLog.Lead_Log_Id;
                    _leadLog.EmailAddress = _dbLeadLog.Email_Address;
                    _leadLog.WebSiteAddress = _dbLeadLog.Website_Address;
                    _leadLog.ContactName = _dbLeadLog.Contact_Name;
                    _leadLog.BusinessName = _dbLeadLog.Business_Name;
                    _leadLog.PhoneNumber = _dbLeadLog.Phone_Number;
                    _leadLog.AdminUserId = _dbLeadLog.Admin_User_Id ?? 0;
                    _leadLog.AdminUserName = _dbLeadLog.Admin_User_Name;
                    _leadLog.Createdon = _dbLeadLog.Create_Time_Stamp;
                    _leadLog.RoleType = _dbLeadLog.role;
                    _role = _dbLeadLog.role;

                    _leadLog.State = GetStateCodebyStateId(_dbLeadLog.State);
                    string _state = string.Empty;
                    if (_leadLog.State != null)
                    {
                        _state = _leadLog.State;
                    }
                    _leadLog.State = _state;
                    if (!_leadLog.IsDoNotFollowUp)
                    {
                        _leadLog.LastFollowUpDate = String.Format("{0:g}", _leadLog.FollowUpDate);
                    }
                    _leadLog.LeadLogFollowIdList = GetLeadLogFollowIdListbyLeadLogId(_leadLog.LeadLogId);


                    _leadLog.LastFollowUp = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? DataUtility.GetDateTime(GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Update_Time_Stamp) : DateTime.MinValue;



                    if (_leadLog.LastFollowUp == DateTime.MinValue)
                    {
                        _leadLog.LastFollowUpDate = "-";
                    }
                    else
                    {
                        _leadLog.LastFollowUpDate = String.Format("{0:g}", _leadLog.LastFollowUp);
                    }

                    long _leadStatusId = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Lead_Status_Id ?? 0 : 0;
                    long _leadStatusDetailId = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Lead_Status_SubcategoryId ?? 0 : 0;

                    if (_leadStatusId > 0)
                    {
                        _leadLog.LeadStatus = GetLeadStatusValue(_leadStatusId).ToString();
                    }

                    if (_leadStatusDetailId > 0)
                    {
                        _leadLog.LeadStatusDetail = GetSubLeadStatusValue(_leadStatusDetailId).ToString();
                    }

                    _leadLog.FollowUpDate = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).FollowUp_date : DateTime.MinValue;
                    if (_leadLog.FollowUpDate == DateTime.MinValue)
                    {
                        _leadLog.SFollowUpDate = "-";
                    }
                    else
                    {
                        _leadLog.SFollowUpDate = String.Format("{0:g}", _leadLog.FollowUpDate);
                    }


                    _lastFollowUpDatecheck = true;
                    if (logDate != null && logDate != DateTime.MinValue)
                    {
                        _leadLog.LastFollowUp = Utilities.DataUtility.GetDateTime(_leadLog.LastFollowUpDate);
                        if (_leadLog.LastFollowUp >= logDate && _leadLog.LastFollowUp <= _templogDate)
                        {
                            _leadLog.LastFollowUpDate = String.Format("{0:g}", _leadLog.LastFollowUp);
                        }
                        else
                        {
                            _lastFollowUpDatecheck = false;
                        }
                    }
                    if (_lastFollowUpDatecheck)
                    {
                        if (unassigned == true)
                        {
                            _leadLogList.Add(_leadLog);
                        }
                        else if (unassigned == false && _role == "Normal")
                        {
                            _leadLogList.Add(_leadLog);
                        }

                    }
                }
            }
            _leadLogList = _leadLogList.OrderBy(a => a.Createdon).ToList();
            return _leadLogList;
        }
        #endregion

        #region Get Lead Status Value
        public string GetLeadStatusValue(long leadStatusId)
        {
            long _leadStatusId = DataUtility.GetLong(leadStatusId);
            var _dbleadStatus = (from st in _entities.Static_Biz_Lead_Status
                                 where st.Lead_StatusId == leadStatusId && !st.Is_Deleted
                                 select st).SingleOrDefault();
            string _LeadStatus = null;
            if (_dbleadStatus != null)
            {
                _LeadStatus = _dbleadStatus.Lead_Status;
            }

            //return state object
            return _LeadStatus;
        }
        #endregion

        #region Get Sub Lead Status Value
        public string GetSubLeadStatusValue(long subleadStatusId)
        {
            long _SubleadStatusId = DataUtility.GetLong(subleadStatusId);
            var _dbleadStatus = (from st in _entities.Static_Biz_Lead_Status_Details
                                 where st.Lead_Status_detailsId == _SubleadStatusId && !st.Is_Deleted
                                 select st).SingleOrDefault();
            string _SubLeadStatus = null;
            if (_dbleadStatus != null)
            {
                _SubLeadStatus = _dbleadStatus.Lead_Status_details;
            }

            //return state object
            return _SubLeadStatus;
        }
        #endregion

        #region Get Follow Up Date
        public List<LeadLog> GetFollowUpDate(DateTime? logDate)
        {
            var _dbFollowUpdateList = (from l in _entities.Tax_Lead_Log
                                       join t in _entities.Tax_Lead_Log_Follow
                                       on l.Lead_Log_Id equals t.LeadLog_Id
                                       where (t.FollowUp_date == logDate) && l.Is_Deleted == false && t.Is_Deleted == false
                                       select new
                                       {
                                           t.FollowUp_date
                                       }).ToList();

            List<LeadLog> _lstFollowUpDate = new List<LeadLog>();
            if (_dbFollowUpdateList != null && _dbFollowUpdateList.Count > 0)
            {
                foreach (var _dbFollowUpDate in _dbFollowUpdateList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.FFollowUpDate = _dbFollowUpDate.FollowUp_date;
                }
            }
            return _lstFollowUpDate;
        }
        #endregion

        #region Get Lead Log Follow Id by Lead Log Id
        public List<LeadLog> GetLeadLogFollowIdListbyLeadLogId(long leadLogId)
        {
            var _dbLeadLogFollowList = (from t in _entities.Tax_Lead_Log_Follow
                                        where t.LeadLog_Id == leadLogId && !t.Is_Deleted
                                        select t).ToList();
            List<LeadLog> _leadLogList = new List<LeadLog>();
            if (_dbLeadLogFollowList != null && _dbLeadLogFollowList.Count > 0)
            {
                foreach (var _dbLeadLogFollow in _dbLeadLogFollowList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.LeadLogFollowId = _dbLeadLogFollow.LeadLog_Follow_Id;
                    _leadLog.NUpdatedDate = _dbLeadLogFollow.Update_Time_Stamp ?? DateTime.MinValue;
                    _leadLogList.Add(_leadLog);

                }
            }
            return _leadLogList;
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

        #region Get FollowUp Date Last by LeadLogId
        //private DateTime GetLastFollowUpDateLast(long AdminLeadLogId)
        //{
        //    DateTime _date = DateTime.MinValue;
        //    var _dbFollowUps = _entities.Tax_Lead_Log_Follow.Where(i => i.LeadLog_Id == AdminLeadLogId && !i.Is_Dont_Follow_Up && !i.Is_Deleted).OrderByDescending(i => i.FollowUp_date).ToList();
        //    var _newMsg = _dbFollowUps.Where(fu => fu.LeadLog_Follow_Id == (_dbFollowUps.Max(i => i.LeadLog_Follow_Id))).SingleOrDefault();
        //    if (_newMsg != null)
        //    {
        //        _date = DataUtility.GetDateTime(_newMsg.FollowUp_date);
        //    }
        //    return _date;
        //}
        #endregion

        #region Get State Code by StateId
        public string GetStateCodebyStateId(string stateId)
        {
            long _stateId = DataUtility.GetLong(stateId);
            var _dbState = (from st in _entities.Static_Biz_Admin_States
                            where st.State_Id == _stateId && !st.Is_Deleted
                            select st).SingleOrDefault();
            string _state = null;
            if (_dbState != null)
            {
                _state = _dbState.State_Code;
            }

            //return state object
            return _state;
        }
        #endregion

        #region Get Lead Email Details List by AutoComplete
        public List<LeadLog> GetLeadEmailDetailsListbyAutoComplete(string str)
        {
            var _dbLeadEmailDetailsList = (from l in _entities.Tax_Lead_Log
                                           where l.Is_Deleted == false
                                           && (l.Email_Address.ToLower().StartsWith(str.ToLower()))
                                           select new
                                           {
                                               l.Email_Address,
                                               l.Contact_Name
                                           }).ToList();
            List<LeadLog> _leadLogList = new List<LeadLog>();
            if (_dbLeadEmailDetailsList != null && _dbLeadEmailDetailsList.Count > 0)
            {
                foreach (var dbLeadEmailDetails in _dbLeadEmailDetailsList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.EmailAddress = dbLeadEmailDetails.Email_Address;
                    if (dbLeadEmailDetails.Contact_Name != null)
                    {
                        _leadLog.ContactName = dbLeadEmailDetails.Contact_Name;
                    }
                    else
                    {
                        _leadLog.ContactName = "-";
                    }
                    _leadLogList.Add(_leadLog);
                }
            }
            return _leadLogList;
        }
        #endregion

        #region Get Lead Business Name Details List by AutoComplete
        public List<LeadLog> GetLeadBusinessNameDetailsListbyAutoComplete(string str)
        {
            var _dbUserList = (from l in _entities.Tax_Lead_Log
                               where l.Is_Deleted == false
                               && (l.Business_Name.ToLower().StartsWith(str.ToLower()))
                               select new
                               {
                                   l.Business_Name,
                               }).ToList();
            List<LeadLog> _leadLogList = new List<LeadLog>();
            if (_dbUserList != null && _dbUserList.Count > 0)
            {
                foreach (var _dbUser in _dbUserList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.BusinessName = _dbUser.Business_Name;
                    _leadLogList.Add(_leadLog);
                }
            }
            return _leadLogList;
        }
        #endregion

        #region Get Lead Admin User Name Details List by AutoComplete
        public List<LeadLog> GetLeadAdminUserNameDetailsListbyAutoComplete(string str)
        {
            var _dbUserList = (from l in _entities.Biz_Admin_Users
                               where l.Is_Deleted == false
                               && (l.Admin_User_Name.ToLower().StartsWith(str.ToLower()))
                               select new
                               {
                                   l.Admin_User_Name
                               }).ToList();
            List<LeadLog> _leadLogList = new List<LeadLog>();
            if (_dbUserList != null && _dbUserList.Count > 0)
            {
                foreach (var _dbUser in _dbUserList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.AdminUserName = _dbUser.Admin_User_Name;
                    _leadLogList.Add(_leadLog);
                }
            }
            return _leadLogList;
        }
        #endregion

        #region Get Lead Admin User Name Details List
        public List<LeadLog> GetLeadAdminUserNameDetailsList(long _projectId)
        {
            long _dataentry = (long)AdminRoleType.Team;
            var _dbUserList = (from l in _entities.Biz_Admin_Users
                               join r in _entities.Biz_Admin_User_Roles
                               on l.Admin_User_Id equals r.Admin_User_Id
                               where l.Is_Deleted == false && r.Is_Deleted == false && r.Admin_Role_Id != _dataentry && r.Admin_Project_Id == _projectId
                               select new
                               {
                                   l.Admin_User_Name,
                                   l.Admin_User_Id
                               }).ToList();
            List<LeadLog> _leadLogList = new List<LeadLog>();
            if (_dbUserList != null && _dbUserList.Count > 0)
            {
                foreach (var _dbUser in _dbUserList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.AdminUserName = _dbUser.Admin_User_Name;
                    _leadLog.AdminUserId = _dbUser.Admin_User_Id;
                    _leadLogList.Add(_leadLog);
                }
            }
            return _leadLogList;
        }
        #endregion

        #region Save Lead Log

        public LeadLog SaveLeadLog(LeadLog adminLog)
        {
            Tax_Lead_Log _dbAdminLeadLog = null;
            bool IsRecordExists = false;

            if (adminLog.LeadLogId > 0)
            {
                _dbAdminLeadLog = _entities.Tax_Lead_Log.SingleOrDefault(au => au.Lead_Log_Id == adminLog.LeadLogId && au.Is_Deleted == false);
            }
            if (_dbAdminLeadLog != null && _dbAdminLeadLog.Lead_Log_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                _dbAdminLeadLog = new Tax_Lead_Log();
            }
            _dbAdminLeadLog.Business_Name = adminLog.BusinessName;
            _dbAdminLeadLog.Business_Type = adminLog.BusinessType;
            _dbAdminLeadLog.EIN = adminLog.EIN;
            _dbAdminLeadLog.DBA_Name = adminLog.DBAName;
            _dbAdminLeadLog.Type_of_User = adminLog.TypeOfUser;
            _dbAdminLeadLog.Contact_Name = adminLog.ContactName;
            _dbAdminLeadLog.Email_Address = adminLog.EmailAddress;
            _dbAdminLeadLog.Phone_Number = adminLog.PhoneNumber;
            _dbAdminLeadLog.Address = adminLog.Address;
            _dbAdminLeadLog.City = adminLog.City;
            _dbAdminLeadLog.Website_Address = adminLog.WebSiteAddress;
            _dbAdminLeadLog.Facebook = adminLog.FaceBook;
            _dbAdminLeadLog.Twitter = adminLog.Twitter;
            _dbAdminLeadLog.Preferred_Method_of_Contact = adminLog.PreferredMethodContact;
            _dbAdminLeadLog.State = adminLog.State;
            _dbAdminLeadLog.Zip_Code = adminLog.ZipCode;
            _dbAdminLeadLog.Lead_Source = adminLog.LeadSource;
            _dbAdminLeadLog.Logged_Date = null;
            _dbAdminLeadLog.Is_ETF = adminLog.IsETF;
            _dbAdminLeadLog.Is_ETT = adminLog.IsETT;
            _dbAdminLeadLog.Is_IFTA = adminLog.IsIFTA;
            _dbAdminLeadLog.Notes = adminLog.GeneralNotes;
            _dbAdminLeadLog.Admin_User_Id = adminLog.AdminUserId;
            _dbAdminLeadLog.Admin_User_Name = adminLog.AdminUserName;
            _dbAdminLeadLog.Other_Business_Type = adminLog.OtherBusinessType;
            _dbAdminLeadLog.Other_Lead_Source = adminLog.OthersLeadSource;
            _dbAdminLeadLog.Sales_Person_Id = adminLog.SalesPersonId;
            //Ett
            _dbAdminLeadLog.No_of_Trucks_Ett = adminLog.NoofTrucksETT;
            _dbAdminLeadLog.No_of_Owner_Operators_Ett = adminLog.NoofOwnerOperatorsETT;
            _dbAdminLeadLog.Feature_Looking_for = adminLog.FeatureLookingfor;
            _dbAdminLeadLog.No_of_Returns_Filed_per_year = adminLog.NoofReturnsFiledperyear;
            _dbAdminLeadLog.Interested_Forms = adminLog.InterestedForms;
            _dbAdminLeadLog.Others_Text_Ett = adminLog.OthersEtt;
            //Etf
            _dbAdminLeadLog.No_of_Contractors = adminLog.NoofContractors;
            _dbAdminLeadLog.Interested_Forms_Etf = adminLog.InterestedFormsEtf;
            _dbAdminLeadLog.Feature_Looking_for_Etf = adminLog.FeatureLookingforEtf;
            _dbAdminLeadLog.No_of_Employees = adminLog.NoofEmployees;
            _dbAdminLeadLog.Others_Text_Etf = adminLog.OthersEtf;


            //Ifta
            _dbAdminLeadLog.No_of_Trucks_Ifta = adminLog.NoofTrucksIfta;
            _dbAdminLeadLog.No_of_Owner_Operators_Ifta = adminLog.NoofOwnerOperatorsIfta;
            _dbAdminLeadLog.Feature_Looking_for_Ifta = adminLog.FeatureLookingforIfta;
            _dbAdminLeadLog.Interested_Forms_Ifta = adminLog.InterestedFormsIfta;
            _dbAdminLeadLog.Others_Text_Ifta = adminLog.OthersIfta;

            //if (adminLog.FollowUpDate != null && adminLog.FollowUpDate != DateTime.MinValue)
            //{
            //    _dbAdminLeadLog.Follow_up_Date = adminLog.FollowUpDate;
            //}
            _dbAdminLeadLog.Update_Time_Stamp = DateTime.Now;

            _dbAdminLeadLog.Is_Deleted = false;
            if (!IsRecordExists)
            {
                _dbAdminLeadLog.Create_Time_Stamp = DateTime.Now;
                _entities.Tax_Lead_Log.Add(_dbAdminLeadLog);
            }


            _entities.SaveChanges();
            adminLog.LeadLogId = _dbAdminLeadLog.Lead_Log_Id;


            return adminLog;
        }
        #endregion

        #region Get State
        public List<State> GetAllAdminState()
        {
            List<State> _stateList = new List<State>();
            var _dbStateList = (from s in _entities.Static_Biz_Admin_States
                                where s.Country_Id == 1 && s.Is_Deleted == false
                                select s).ToList();

            if (_dbStateList != null && _dbStateList.Count > 0)
            {
                foreach (var _dbState in _dbStateList)
                {
                    State _state = new State();
                    _state.StateId = _dbState.State_Id;
                    _state.CountryId = _dbState.Country_Id;
                    _state.StateName = _dbState.State_Name;
                    _state.StateCode = _dbState.State_Code;
                    _stateList.Add(_state);
                }
            }

            return _stateList;

        }
        #endregion

        #region Get All Lead Status
        public List<AdminNotes> GetAllLeadStatus()
        {
            List<AdminNotes> _statusList = new List<AdminNotes>();
            var _dbStatusList = (from s in _entities.Static_Biz_Lead_Status
                                 where s.Is_Deleted == false
                                 select s).ToList();
            if (_dbStatusList != null && _dbStatusList.Count > 0)
            {
                foreach (var _dbStatu in _dbStatusList)
                {
                    AdminNotes _status = new AdminNotes();
                    _status.LeadStatusId = _dbStatu.Lead_StatusId;
                    _status.LeadStatus = _dbStatu.Lead_Status;
                    _statusList.Add(_status);
                }
            }
            return _statusList;
        }
        #endregion

        #region Get All Lead Status Details
        public List<AdminNotes> GetAllLeadStatusDetails(long LeadStatusId)
        {
            List<AdminNotes> _statusDetailsList = new List<AdminNotes>();
            var _dbStatusList = (from s in _entities.Static_Biz_Lead_Status_Details
                                 where s.Is_Deleted == false && s.Lead_StatusId == LeadStatusId
                                 select s).ToList();
            if (_dbStatusList != null && _dbStatusList.Count > 0)
            {
                foreach (var _dbStatu in _dbStatusList)
                {
                    AdminNotes _status = new AdminNotes();
                    _status.LeadStatusdetailsId = _dbStatu.Lead_Status_detailsId;
                    _status.LeadStatusId = _dbStatu.Lead_StatusId ?? 0;
                    _status.LeadStatusdetails = _dbStatu.Lead_Status_details;
                    _statusDetailsList.Add(_status);
                }
            }
            return _statusDetailsList;
        }
        #endregion

        #region Save Follow Up
        public bool SaveFollowUpMsg(string followUpMsg, long Id, DateTime LogDate, string supportType, long leadlogFollowupId)
        {
            Tax_Lead_Log_Follow _dbAdminFollowUpMsg = null;
            bool IsRecordExists = false;

            if (leadlogFollowupId > 0)
            {
                _dbAdminFollowUpMsg = _entities.Tax_Lead_Log_Follow.SingleOrDefault(au => au.LeadLog_Follow_Id == leadlogFollowupId && !au.Is_Deleted);
            }
            if (_dbAdminFollowUpMsg != null && _dbAdminFollowUpMsg.LeadLog_Follow_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                _dbAdminFollowUpMsg = new Tax_Lead_Log_Follow();
                _dbAdminFollowUpMsg.LeadLog_Follow_Id = Id;
            }
            _dbAdminFollowUpMsg.FolowUp_Message = followUpMsg;
            _dbAdminFollowUpMsg.Support_Type = supportType;
            _dbAdminFollowUpMsg.Update_Time_Stamp = DateTime.Now;
            _dbAdminFollowUpMsg.FollowUp_date = LogDate;
            _dbAdminFollowUpMsg.Is_Deleted = false;
            if (!IsRecordExists)
            {
                _dbAdminFollowUpMsg.Create_Time_Stamp = DateTime.Now;

                _entities.Tax_Lead_Log_Follow.Add(_dbAdminFollowUpMsg);
            }

            _entities.SaveChanges();

            return true;

        }
        #endregion

        #region Save Assign Person
        public bool SaveAssignPerson(long assignPersonId, long leadLogId)
        {
            bool _isAssigned = false;
            Tax_Lead_Log _dbleadLog = new Tax_Lead_Log();
            if (assignPersonId > 0 && leadLogId > 0)
            {
                _dbleadLog = _entities.Tax_Lead_Log.Where(n => n.Lead_Log_Id == leadLogId && n.Is_Deleted == false).SingleOrDefault();
                _dbleadLog.Sales_Person_Id = assignPersonId;
                _dbleadLog.Admin_User_Id = assignPersonId;
                _dbleadLog.Is_Deleted = false;
                _dbleadLog.Update_Time_Stamp = DateTime.Now;
                _isAssigned = true;
                _entities.SaveChanges();
            }
            return _isAssigned;
        }
        #endregion

        #region Save Notes
        public LeadLog SaveNotes(LeadLog _adminLeadLog, long LeadId, long _followId)
        {
            bool _isExist = false;
            Tax_Lead_Log_Follow _dbadminNote = new Tax_Lead_Log_Follow();
            if (_followId > 0)
            {
                _dbadminNote = _entities.Tax_Lead_Log_Follow.Where(n => n.LeadLog_Follow_Id == _followId && !n.Is_Deleted).SingleOrDefault();
            }
            if (_dbadminNote != null && _dbadminNote.LeadLog_Follow_Id > 0)
            {
                _isExist = true;
            }
            else
            {
                _dbadminNote = new Tax_Lead_Log_Follow();
            }

            _dbadminNote.LeadLog_Id = LeadId;
            _dbadminNote.FolowUp_Message = _adminLeadLog.Comment;
            _dbadminNote.Support_Type = _adminLeadLog.NSupportType;

            if (_adminLeadLog.NFollowUpDate == DateTime.MinValue)
            {
                _dbadminNote.FollowUp_date = DateTime.Now;
            }
            else
            {
                _dbadminNote.FollowUp_date = _adminLeadLog.NFollowUpDate;
            }
            _dbadminNote.Lead_Status = _adminLeadLog.NLeadStatus;
            _dbadminNote.Lead_Status_Id = _adminLeadLog.LeadLogStatusId;
            _dbadminNote.Is_Dont_Follow_Up = _adminLeadLog.NIsDoNotFollowUp;
            _dbadminNote.Admin_User_Name = _adminLeadLog.AdminUserName;
            _dbadminNote.Lead_Status_SubcategoryId = _adminLeadLog.LeadLogStatusSubcategoryId;


            if (!_isExist)
            {
                _dbadminNote.Create_Time_Stamp = DateTime.Now;
                _entities.Tax_Lead_Log_Follow.Add(_dbadminNote);
            }
            _dbadminNote.Is_Deleted = false;
            _dbadminNote.Update_Time_Stamp = DateTime.Now;

            _entities.SaveChanges();

            _adminLeadLog.LeadLogId = _dbadminNote.LeadLog_Id;
            _adminLeadLog.AdminFollowupId = _dbadminNote.LeadLog_Follow_Id;
            _adminLeadLog.Comment = _dbadminNote.FolowUp_Message;
            _adminLeadLog.Createdon = _dbadminNote.Create_Time_Stamp ?? DateTime.Now;

            return _adminLeadLog;
        }
        #endregion

        #region Get Admin Note
        public List<LeadLog> GetNotesDetails(long LeadLogId)
        {


            var _dbNoteLst = (from a in _entities.Tax_Lead_Log_Follow
                              join b in _entities.Static_Biz_Lead_Status
                              on a.Lead_Status_Id equals b.Lead_StatusId into p
                              from b in p.DefaultIfEmpty()

                              join c in _entities.Static_Biz_Lead_Status_Details
                              on a.Lead_Status_SubcategoryId equals c.Lead_Status_detailsId into q
                              from c in q.DefaultIfEmpty()

                              orderby a.Create_Time_Stamp descending
                              where !a.Is_Deleted && a.LeadLog_Id == LeadLogId
                              select new
                              {
                                  a.FolowUp_Message,
                                  a.Admin_User_Name,
                                  a.Create_Time_Stamp,
                                  a.Support_Type,
                                  a.LeadLog_Follow_Id,
                                  a.FollowUp_date,
                                  b.Lead_Status,
                                  c.Lead_Status_details,
                                  a.Is_Dont_Follow_Up,
                                  a.Lead_Status_SubcategoryId

                              });

            List<LeadLog> _noteList = null;
            if (_dbNoteLst != null && _dbNoteLst.Count() > 0)
            {
                _noteList = new List<LeadLog>();
                foreach (var _thisadminUser in _dbNoteLst)
                {
                    LeadLog _adminuser = new LeadLog();
                    _adminuser.AdminUserName = _thisadminUser.Admin_User_Name;

                    _adminuser.Comment = _thisadminUser.FolowUp_Message;
                    _adminuser.Createdon = _thisadminUser.Create_Time_Stamp ?? DateTime.Today;
                    _adminuser.NSupportType = _thisadminUser.Support_Type;
                    _adminuser.NLeadStatus = _thisadminUser.Lead_Status;
                    _adminuser.LeadLogStatusbcategory = _thisadminUser.Lead_Status_details;
                    _adminuser.AdminFollowupId = _thisadminUser.LeadLog_Follow_Id;
                    _adminuser.NIsDoNotFollowUp = _thisadminUser.Is_Dont_Follow_Up;
                    if (_adminuser.NIsDoNotFollowUp)
                    {
                        _adminuser.LastFollowUpDate = "";
                    }
                    else
                    {
                        _adminuser.LastFollowUpDate = String.Format("{0:g}", _thisadminUser.FollowUp_date);
                    }
                    _adminuser.Screated = String.Format("{0:g}", _thisadminUser.Create_Time_Stamp);
                    _noteList.Add(_adminuser);
                }
            }
            return _noteList;
        }
        #endregion

        #region Get Lead Log by Id
        public LeadLog GetLeadLogById(long Id)
        {
            LeadLog _adminUser = new LeadLog();
            var _dbAdminUser = (from a in _entities.Tax_Lead_Log
                                where a.Lead_Log_Id == Id && a.Is_Deleted == false
                                select a).SingleOrDefault();

            if (_dbAdminUser != null && _dbAdminUser.Lead_Log_Id > 0)
            {
                _adminUser.BusinessName = _dbAdminUser.Business_Name;
                _adminUser.BusinessType = _dbAdminUser.Business_Type;
                _adminUser.EIN = _dbAdminUser.EIN;
                _adminUser.DBAName = _dbAdminUser.DBA_Name;
                _adminUser.TypeOfUser = _dbAdminUser.Type_of_User;
                _adminUser.ContactName = _dbAdminUser.Contact_Name;
                _adminUser.EmailAddress = _dbAdminUser.Email_Address;
                _adminUser.PhoneNumber = _dbAdminUser.Phone_Number;
                _adminUser.Address = _dbAdminUser.Address;
                _adminUser.City = _dbAdminUser.City;
                _adminUser.WebSiteAddress = _dbAdminUser.Website_Address;
                _adminUser.FaceBook = _dbAdminUser.Facebook;
                _adminUser.Twitter = _dbAdminUser.Twitter;
                _adminUser.PreferredMethodContact = _dbAdminUser.Preferred_Method_of_Contact;
                _adminUser.State = _dbAdminUser.State;
                _adminUser.ZipCode = _dbAdminUser.Zip_Code;
                _adminUser.LeadSource = _dbAdminUser.Lead_Source;
                _adminUser.LeadLogId = _dbAdminUser.Lead_Log_Id;
                _adminUser.SalesPersonId = _dbAdminUser.Sales_Person_Id ?? 0;
                _adminUser.AdminUserId = _dbAdminUser.Admin_User_Id ?? 0;
                //_adminUser.FollowUpDate = _dbAdminUser.Follow_up_Date ?? DateTime.MinValue;
                //_adminUser.LogDate = _dbAdminUser.Follow_up_Date ?? DateTime.MinValue;
                //_adminUser.LogTime = _dbAdminUser.Follow_up_Date ?? DateTime.MinValue;
                _adminUser.GeneralNotes = _dbAdminUser.Notes;
                _adminUser.OtherBusinessType = _dbAdminUser.Other_Business_Type;
                _adminUser.OthersLeadSource = _dbAdminUser.Other_Lead_Source;

                _adminUser.IsETF = _dbAdminUser.Is_ETF ?? false;
                _adminUser.IsETT = _dbAdminUser.Is_ETT ?? false;
                _adminUser.IsIFTA = _dbAdminUser.Is_IFTA ?? false;
                //Ett
                _adminUser.NoofTrucksETT = _dbAdminUser.No_of_Trucks_Ett ?? 0;
                _adminUser.NoofOwnerOperatorsETT = _dbAdminUser.No_of_Owner_Operators_Ett ?? 0;
                _adminUser.FeatureLookingfor = _dbAdminUser.Feature_Looking_for;
                _adminUser.NoofReturnsFiledperyear = _dbAdminUser.No_of_Returns_Filed_per_year ?? 0;
                _adminUser.InterestedForms = _dbAdminUser.Interested_Forms;
                _adminUser.OthersEtt = _dbAdminUser.Others_Text_Ett;

                //Etf
                _adminUser.NoofContractors = _dbAdminUser.No_of_Contractors ?? 0;
                _adminUser.InterestedFormsEtf = _dbAdminUser.Interested_Forms_Etf;
                _adminUser.FeatureLookingforEtf = _dbAdminUser.Feature_Looking_for_Etf;
                _adminUser.NoofEmployees = _dbAdminUser.No_of_Employees ?? 0;
                _adminUser.OthersEtf = _dbAdminUser.Others_Text_Etf;

                //Ifta
                _adminUser.NoofTrucksIfta = _dbAdminUser.No_of_Trucks_Ifta ?? 0;
                _adminUser.NoofOwnerOperatorsIfta = _dbAdminUser.No_of_Owner_Operators_Ifta ?? 0;
                _adminUser.FeatureLookingforIfta = _dbAdminUser.Feature_Looking_for_Ifta;
                _adminUser.InterestedFormsIfta = _dbAdminUser.Interested_Forms_Ifta;
                _adminUser.OthersIfta = _dbAdminUser.Others_Text_Ifta;

            }

            return _adminUser;
        }
        #endregion

        #region Get contact Note Update By NoteId
        public List<LeadLog> GetRelatedNoteUpdate(long followupId)
        {
            List<LeadLog> _noteLst = new List<LeadLog>();
            var _dbvaluelst = (from n in _entities.Tax_Lead_Log_Follow
                               where n.LeadLog_Follow_Id == followupId && n.Is_Deleted == false
                               select n).ToList();
            if (_dbvaluelst != null && _dbvaluelst.Count > 0)
            {
                foreach (var item in _dbvaluelst)
                {
                    LeadLog _notes = new LeadLog();
                    _notes.AdminFollowupId = item.LeadLog_Follow_Id;
                    _notes.Comment = item.FolowUp_Message;
                    _notes.NoteDate =  item.FollowUp_date.ToShortDateString();
                    _notes.NoteTime =  item.FollowUp_date.ToShortTimeString();
                    _notes.NSupportType = item.Support_Type;
                    _notes.NLeadStatus = item.Lead_Status;
                    _notes.NIsDoNotFollowUp = item.Is_Dont_Follow_Up;
                    _notes.LeadLogStatusSubcategoryId = item.Lead_Status_SubcategoryId ?? 0;
                    if (_notes.NIsDoNotFollowUp == true)
                    {
                        _notes.NoteDate = string.Empty;
                        _notes.NoteTime = string.Empty;
                    }
                    _noteLst.Add(_notes);
                }
            }
            return _noteLst;
        }
        #endregion

        #region Get All AdminUser For TaxProjext
        public List<AdminUserRole> GetAllAdminUserForTaxProjext()
        {
            List<AdminUserRole> _lstAdminUser = new List<AdminUserRole>();


            var _dbAdminList = (from a in _entities.Biz_Admin_Users
                                join r in _entities.Biz_Admin_User_Roles
                                on a.Admin_User_Id equals r.Admin_User_Id
                                where r.Admin_Project_Id == 8 && !a.Is_Deleted && !r.Is_Deleted
                                select new
                                {
                                    a.Admin_User_Name,
                                    a.Admin_User_Id
                                }).ToList();

            if (_dbAdminList != null && _dbAdminList.Count > 0)
            {
                foreach (var item in _dbAdminList)
                {
                    AdminUserRole _userLst = new AdminUserRole();

                    _userLst.AdminUserId = item.Admin_User_Id;
                    _userLst.AdminUserName = item.Admin_User_Name;
                    _lstAdminUser.Add(_userLst);
                }
            }
            return _lstAdminUser;
        }
        #endregion

        #region Delete FollowUp
        public bool DeleteFollowUpMsg(long Id)
        {
            bool _isDeleted = false;
            if (Id > 0)
            {
                var _dbAdminFollowUp = (from a in _entities.Tax_Lead_Log_Follow
                                        where a.LeadLog_Follow_Id == Id && !a.Is_Deleted
                                        select a).SingleOrDefault();
                if (_dbAdminFollowUp != null && _dbAdminFollowUp.LeadLog_Follow_Id > 0)
                {
                    _dbAdminFollowUp.Is_Deleted = true;
                    _dbAdminFollowUp.Update_Time_Stamp = DateTime.Now;
                    _entities.SaveChanges();
                    _isDeleted = true;
                }
            }
            return _isDeleted;
        }
        #endregion

        #region Get Lead Log
        public List<LeadLog> GetClientLeadLog(long? adminUserId)
        {
            List<LeadLog> _leadLogList = new List<LeadLog>();

            var _dbLeadLogList = (from l in _entities.Tax_Lead_Log
                                  orderby l.Logged_Date ascending
                                  where l.Is_Deleted == false
                                  && (adminUserId != null && adminUserId > 0 ? l.Admin_User_Id == adminUserId : true)
                                  select new
                                  {
                                      l.Lead_Log_Id,
                                      l.Email_Address,
                                      l.Website_Address,
                                      l.Contact_Name,
                                      l.Business_Name,
                                      l.Phone_Number,
                                      l.Create_Time_Stamp,
                                      l.State,
                                      l.Admin_User_Id,
                                      l.Admin_User_Name
                                  }).ToList();
            if (_dbLeadLogList != null && _dbLeadLogList.Count() > 0)
            {
                foreach (var _dbLeadLog in _dbLeadLogList)
                {
                    LeadLog _leadLog = new LeadLog();
                    _leadLog.LeadLogId = _dbLeadLog.Lead_Log_Id;
                    _leadLog.EmailAddress = _dbLeadLog.Email_Address;
                    _leadLog.WebSiteAddress = _dbLeadLog.Website_Address;
                    _leadLog.ContactName = _dbLeadLog.Contact_Name;
                    _leadLog.BusinessName = _dbLeadLog.Business_Name;
                    _leadLog.PhoneNumber = _dbLeadLog.Phone_Number;
                    _leadLog.AdminUserId = _dbLeadLog.Admin_User_Id ?? 0;
                    _leadLog.AdminUserName = _dbLeadLog.Admin_User_Name;
                    _leadLog.Createdon = _dbLeadLog.Create_Time_Stamp;

                    _leadLog.State = GetStateCodebyStateId(_dbLeadLog.State);
                    string _state = string.Empty;
                    if (_leadLog.State != null)
                    {
                        _state = _leadLog.State;
                    }
                    _leadLog.State = _state;
                    if (!_leadLog.IsDoNotFollowUp)
                    {
                        _leadLog.LastFollowUpDate = String.Format("{0:g}", _leadLog.FollowUpDate);
                    }
                    _leadLog.LeadLogFollowIdList = GetLeadLogFollowIdListbyLeadLogId(_leadLog.LeadLogId);


                    _leadLog.LastFollowUp = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? DataUtility.GetDateTime(GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Update_Time_Stamp) : DateTime.MinValue;



                    if (_leadLog.LastFollowUp == DateTime.MinValue)
                    {
                        _leadLog.LastFollowUpDate = "-";
                    }
                    else
                    {
                        _leadLog.LastFollowUpDate = String.Format("{0:g}", _leadLog.LastFollowUp);
                    }

                    long _leadStatusId = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Lead_Status_Id ?? 0 : 0;
                    long _leadStatusDetailId = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).Lead_Status_SubcategoryId ?? 0 : 0;

                    if (_leadStatusId > 0)
                    {
                        _leadLog.LeadStatus = GetLeadStatusValue(_leadStatusId).ToString();
                    }

                    if (_leadStatusDetailId > 0)
                    {
                        _leadLog.LeadStatusDetail = GetSubLeadStatusValue(_leadStatusDetailId).ToString();
                    }

                    _leadLog.FollowUpDate = GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id) != null ? GetLastLeadLogFollow(_dbLeadLog.Lead_Log_Id).FollowUp_date : DateTime.MinValue;
                    if (_leadLog.FollowUpDate == DateTime.MinValue)
                    {
                        _leadLog.SFollowUpDate = "-";
                    }
                    else
                    {
                        _leadLog.SFollowUpDate = String.Format("{0:g}", _leadLog.FollowUpDate);
                    }



                    if (_leadStatusId == (long)AdminLeadType.Client)
                    {
                        _leadLogList.Add(_leadLog);
                    }
                }
            }
            _leadLogList = _leadLogList.OrderBy(a => a.Createdon).ToList();
            return _leadLogList;
        }
        #endregion

        #region Get Sales person Name by  Id

        public string GetSalesPersonNamebyId(long Id)
        {
            string SalesPersonName = null;
            var salesperson = (from a in _entities.Biz_Admin_Users
                               where a.Admin_User_Id == Id && !a.Is_Deleted
                               select a).SingleOrDefault();
            if (salesperson != null)
            {
                SalesPersonName = salesperson.Admin_User_Name;
                
            }
            return SalesPersonName;
        }

        #endregion

        #region Get Unassigned Person by Project Id
        public bool GetUnassignedbyId(long ProjectId,long leadLogId)
        {
             
            bool _isAssigned = false;
            long Dataentry = (long)AdminRoleType.Team;
            var salesperson = (from a in _entities.Biz_Admin_User_Roles
                               where a.Admin_Project_Id == ProjectId && a.Admin_Role_Id == Dataentry && !a.Is_Deleted
                               select a.Admin_User_Id).FirstOrDefault();
            if (salesperson>0)
            {
                _isAssigned = SaveAssignPerson(salesperson, leadLogId);  
            }
            return _isAssigned;
        }

        #endregion



    }
}
