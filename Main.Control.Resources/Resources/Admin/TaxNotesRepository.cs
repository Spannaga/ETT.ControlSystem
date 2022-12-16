using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;
using Main.Control.Core.Services;
using Main.Control.Core.Repositories;

using Main.Control.Resources.Utilities;

namespace Main.Control.Resources
{
    public class TaxNotesRepository :ITaxNotesRepository 
    {
        #region Declaration
        private MainControlDB_Entities _entities = new MainControlDB_Entities();
      
        #endregion

        #region Constructor
        public TaxNotesRepository()
        {
            _entities = new MainControlDB_Entities();
            
        }
        #endregion

        #region Get Admin Notes for Dashboard
        /// <summary>
        /// Get Admin Notes for Dashboard
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<AdminNotes> GetAdminNotesForDashboard()
        {
            List<AdminNotes> _NotesList = new List<AdminNotes>();

            string _status = NotesStatus.COMPLETED.ToString();

            var _dbAdminNotesList = (from n in _entities.Tax_Lead_Log_Follow
                                     where n.FollowUp_date != null && !n.Is_Deleted 
                                     orderby n.Update_Time_Stamp
                                     select n).ToList();

            _dbAdminNotesList = _dbAdminNotesList.OrderByDescending(n => n.FollowUp_date).ToList();

            if (_dbAdminNotesList != null && _dbAdminNotesList.Count() > 0)
            {
                foreach (var _dbAdminNotes in _dbAdminNotesList)
                {
                    AdminNotes _adminNotes = new AdminNotes();
                    _adminNotes.AdminNotesId = _dbAdminNotes.LeadLog_Follow_Id;
                    _adminNotes.AdminUserName = _dbAdminNotes.Admin_User_Name;
                    //_adminNotes.UserId = _dbAdminNotes.Lead_Log_Id;
                    _adminNotes.UserName = _dbAdminNotes.Admin_User_Name;
                    _adminNotes.DisplayDate = DataUtility.GetDateTime(_dbAdminNotes.FollowUp_date).ToShortDateString();
                    _adminNotes.CreateDate = DataUtility.GetDateTime(_dbAdminNotes.Create_Time_Stamp).ToShortDateString();
                    _NotesList.Add(_adminNotes);
                }
            }
            return _NotesList;
        }
        #endregion


    }
}
