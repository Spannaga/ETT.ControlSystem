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
    public class TaxCustomerService : ITaxCustomerService
    {
        #region Declaration
        private readonly ITaxCustomerRepository _customerRepository;
        #endregion


        #region Constructor
        public TaxCustomerService(ITaxCustomerRepository repository)
        {
            this._customerRepository = repository;
        }
        #endregion

        #region Dashboard -> Lead Log FollowUps
        /// <summary>
        /// Dashboard -> Lead Log FollowUps
        /// </summary>
        /// <returns></returns>
        public List<LeadLog> GetAdminLeadLogForDashboard(long _adminUserId)
        {
            return this._customerRepository.GetAdminLeadLogForDashboard(_adminUserId);
        }
        #endregion

    }

}
