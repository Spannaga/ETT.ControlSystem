using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;

namespace Main.Control.Core.Repositories
{
    public interface ITaxCustomerRepository
    {


        List<LeadLog> GetAdminLeadLogForDashboard(long _adminUserId);


    }
}
