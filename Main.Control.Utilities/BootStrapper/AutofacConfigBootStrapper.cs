using Autofac;
using Main.Control.Resources;
using Main.Control.Services;

namespace Main.Control.Utilities.BootStrapper
{
    public class AutofacConfigBootStrapper
    {
        public static ContainerBuilder RegisterInterfaces(ContainerBuilder builder)
        {
            //Repositories
            builder.RegisterType<AdminRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SpanControlRepository>().AsImplementedInterfaces().InstancePerLifetimeScope();

            //Services
            builder.RegisterType<AdminService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<EmailService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<SpanControlService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }
    }
}
