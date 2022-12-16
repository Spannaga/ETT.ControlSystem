using System;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace Main.Control.Utilities
{
    public class SEControllerFactory : DefaultControllerFactory
    {
         private readonly IUnityContainer _iocContainer;

         public SEControllerFactory(IUnityContainer container)
        {
            this._iocContainer = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }
            return _iocContainer.Resolve(controllerType) as IController;
        }
    }
}
