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
    public class TaxDashboardService : ITaxDashboardService
    {
        #region Declaration
        private readonly ITaxDashboardRepository _taxBoardRepository;
        #endregion


        #region Constructor
        public TaxDashboardService(ITaxDashboardRepository repository)
        {
            this._taxBoardRepository = repository;
        }
        #endregion

        public List<LeadLog> GetLeadLogStatusbyLeadStatus(long _adminUserId)
        {
            return this._taxBoardRepository.GetLeadLogStatusbyLeadStatus(_adminUserId);
        }

        public List<LeadLog> GetLeadLogStatusbyAdminUser()
        {
            return this._taxBoardRepository.GetLeadLogStatusbyAdminUser();
        }



    }
}
