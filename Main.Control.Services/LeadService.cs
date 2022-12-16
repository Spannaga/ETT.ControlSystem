using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;
using Main.Control.Core.Services;
using Main.Control.Core.Repositories;

namespace Main.Control.Services
{
    public class LeadService : ILeadService
    {
        #region Declaration
        private readonly ILeadRepository _leadRepository;
        #endregion

        #region Constructor
        public LeadService(ILeadRepository repository)
        {
            this._leadRepository = repository;
        }
        #endregion


        #region Get Lead Log
        public List<LeadLog> GetLeadLog(DateTime fromDate, DateTime toDate,long? adminName, string status, string email, string businessName, long? stateId, DateTime logDate, string contactName, string phonenumber, long? adminUserId, bool unassigned)
        {
            return _leadRepository.GetLeadLog(fromDate, toDate, adminName, status, email, businessName, stateId, logDate, contactName, phonenumber, adminUserId, unassigned);
        }
        #endregion

        #region Get Lead Log Any One ETT ETF EIFTA
        //public List<LeadLog> GetLeadLogAnyOneETTETFEIFTA(DateTime fromDate, DateTime toDate, string adminName, string status, string email, string businessName, long? stateId, DateTime logDate, bool isETT, bool isETF, bool isEIFTA)
        //{
        //    return _leadRepository.GetLeadLogAnyOneETTETFEIFTA(fromDate, toDate, adminName, status, email, businessName, stateId, logDate, isETT, isETF, isEIFTA);
        //}
        #endregion

        #region Get State Code by StateId
        public string GetStateCodebyStateId(string stateId)
        {
            return _leadRepository.GetStateCodebyStateId(stateId);
        }
        #endregion

        #region Get Lead Email Details List by AutoComplete
        public List<LeadLog> GetLeadEmailDetailsListbyAutoComplete(string str)
        {
            return _leadRepository.GetLeadEmailDetailsListbyAutoComplete(str);
        }
        #endregion

        public List<LeadLog> GetLeadAdminUserNameDetailsList(long _projectId)
        {
            return _leadRepository.GetLeadAdminUserNameDetailsList(_projectId);
        }


        #region Get Lead Business Name Details List by AutoComplete
        public List<LeadLog> GetLeadBusinessNameDetailsListbyAutoComplete(string str)
        {
            return _leadRepository.GetLeadBusinessNameDetailsListbyAutoComplete(str);
        }
        #endregion

        #region Get Lead Admin User Name Details List by AutoComplete
        public List<LeadLog> GetLeadAdminUserNameDetailsListbyAutoComplete(string str)
        {
            return _leadRepository.GetLeadAdminUserNameDetailsListbyAutoComplete(str);
        }
        #endregion


        #region Save Lead Log
        public LeadLog SaveLeadLog(LeadLog lead)
        {
            return _leadRepository.SaveLeadLog(lead);
        }
        #endregion

        #region  Get All Admin State
        public List<State> GetAllAdminState()
        {
            return _leadRepository.GetAllAdminState();
        }
        #endregion

        #region  Save Follow Up Msg
        public LeadLog SaveNotes(LeadLog _adminLeadLog, long LeadId, long _followId)
        {
            return _leadRepository.SaveNotes(_adminLeadLog, LeadId, _followId);
        }
        #endregion

        #region Get Notes Details
        public List<LeadLog> GetNotesDetails(long LeadLogId)
        {
            return _leadRepository.GetNotesDetails(LeadLogId);
        }
        #endregion

        #region Get Lead Details
        public LeadLog GetLeadLogById(long Id)
        {
            return _leadRepository.GetLeadLogById(Id);
        }
        #endregion

        #region Get Related Note Details
        public List<LeadLog> GetRelatedNoteUpdate(long LeadLogId)
        {
            return _leadRepository.GetRelatedNoteUpdate(LeadLogId);
        }
        #endregion

        #region Delete follow up
        public bool DeleteFollowUpMsg(long Id)
        {
            return _leadRepository.DeleteFollowUpMsg(Id);
        }
        #endregion

        #region Get All Lead Status
        public List<AdminNotes> GetAllLeadStatus()
        {
            return _leadRepository.GetAllLeadStatus();
        }
        #endregion

        #region Get All LeadStatus Details
        public List<AdminNotes> GetAllLeadStatusDetails(long LeadStatusId)
        {
            return _leadRepository.GetAllLeadStatusDetails(LeadStatusId);
        }
        #endregion


        #region Get All AdminUser For Tax Projext
        public List<AdminUserRole> GetAllAdminUserForTaxProjext()
        {
            return _leadRepository.GetAllAdminUserForTaxProjext();
        }
        #endregion

        #region Get Client LeadLog
        public List<LeadLog> GetClientLeadLog(long? adminUserId)
        {
            return _leadRepository.GetClientLeadLog(adminUserId);
        }
        #endregion

        #region Get Client LeadLog
        public bool SaveAssignPerson(long assignPersonId, long leadLogId)
        {
            return _leadRepository.SaveAssignPerson(assignPersonId, leadLogId);
        }
        #endregion

        #region Get Sales Person Name by Id
        public string GetSalesPersonNamebyId(long Id)
        {
            return _leadRepository.GetSalesPersonNamebyId(Id);
        }
        #endregion

        #region Get Unassigned by Id 
        public bool GetUnassignedbyId(long ProjectId,long leadLogId)
        {
            return _leadRepository.GetUnassignedbyId(ProjectId, leadLogId);
        }
        #endregion
         

    }
}
