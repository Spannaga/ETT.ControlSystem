using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core;
using Main.Control.Core.Models;
using Main.Control.Core.Services;
using Main.Control.Core.Repositories;


namespace Main.Control.Services
{
    public class TaxNotesService:ITaxNotesService
    {

        #region Declaration
        private readonly ITaxNotesRepository _notesRepository;
        #endregion

        #region Constructor
        public TaxNotesService(ITaxNotesRepository notesRepository)
        {
            this._notesRepository = notesRepository;
        }
        #endregion


        #region Get Admin Notes for Dashboard
        /// <summary>
        /// Get Admin Notes for Dashboard
        /// </summary>
        /// <returns></returns>
        public List<AdminNotes> GetAdminNotesForDashboard()
        {
            return this._notesRepository.GetAdminNotesForDashboard();
        }
        #endregion


    }
}
