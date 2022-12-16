using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Main.Control.Core.Repositories;
using Main.Control.Resources;



namespace Main.Control.Utilities
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILeadRepository>().ImplementedBy<LeadRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxCustomerRepository>().ImplementedBy<TaxCustomerRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<IAdminRepository>().ImplementedBy<AdminRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ISpanControlRepository>().ImplementedBy<SpanControlRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxNotesRepository>().ImplementedBy<TaxNotesRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxDashboardRepository>().ImplementedBy<TaxDashboardRepository>().LifeStyle.PerWebRequest);
            container.Register(Component.For<IEmailRepository>().ImplementedBy<EmailRepository>().LifeStyle.PerWebRequest);
        }
    }
}
