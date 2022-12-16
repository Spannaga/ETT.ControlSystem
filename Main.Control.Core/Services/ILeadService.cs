using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;


namespace Main.Control.Core.Services
{
    public interface ILeadService
    {
        List<LeadLog> GetLeadLog(DateTime fromDate, DateTime toDate, long? adminName, string status, string email, string businessName, long? stateId, DateTime logDate, string contactName, string phonenumber, long? adminUserId, bool unassigned);
        //List<LeadLog> GetLeadLogAnyOneETTETFEIFTA(DateTime fromDate, DateTime toDate, string adminName, string status, string email, string businessName, long? stateId, DateTime logDate, bool isETT, bool isETF, bool isEIFTA);
        string GetStateCodebyStateId(string stateId);
        List<LeadLog> GetLeadEmailDetailsListbyAutoComplete(string str);
        List<LeadLog> GetLeadBusinessNameDetailsListbyAutoComplete(string str);
        List<LeadLog> GetLeadAdminUserNameDetailsListbyAutoComplete(string str);
        List<LeadLog> GetLeadAdminUserNameDetailsList(long _projectId);
        LeadLog SaveLeadLog(LeadLog lead);
        List<State> GetAllAdminState();
        LeadLog SaveNotes(LeadLog _adminLeadLog, long LeadId, long _followId);
        List<LeadLog> GetNotesDetails(long LeadLogId);
        LeadLog GetLeadLogById(long Id);
        List<LeadLog> GetRelatedNoteUpdate(long followupId);
        bool DeleteFollowUpMsg(long Id);
        List<AdminNotes> GetAllLeadStatus();
        List<AdminNotes> GetAllLeadStatusDetails(long LeadStatusId);
        List<AdminUserRole> GetAllAdminUserForTaxProjext();
        List<LeadLog> GetClientLeadLog(long? adminUserId);
        bool SaveAssignPerson(long assignPersonId, long leadLogId);
        string GetSalesPersonNamebyId(long Id);
        bool GetUnassignedbyId(long ProjectId, long leadLogId);
    }
}
