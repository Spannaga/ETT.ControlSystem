using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Main.Control.Core.Services;
using Main.Control.Services;


namespace Main.Control.Utilities
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(Component.For<ILoger>().ImplementedBy<NLogger>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ILeadService>().ImplementedBy<LeadService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxCustomerService>().ImplementedBy<TaxCustomerService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<IAdminService>().ImplementedBy<AdminService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ISpanControlService>().ImplementedBy<SpanControlService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxNotesService>().ImplementedBy<TaxNotesService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<ITaxDashboardService>().ImplementedBy<TaxDashboardService>().LifeStyle.PerWebRequest);
            container.Register(Component.For<IEmailService>().ImplementedBy<EmailService>().LifeStyle.PerWebRequest);
        }
    }
}
