using Main.Control.Core.Services;
using System.Web.Mvc;
using System.Linq;
using Main.Control.Web.Utilities;
using System.Web.Routing;

namespace Main.Control.Web.Controllers
{
    [TraceFilter()]
    public class BaseController : Controller
    {
        //private readonly ISpanControlService _mainControlService;
        //#region Constructor
        ///// <summary>
        ///// Base Controller
        ///// </summary>
        //public BaseController()
        //{

        //}
        //public BaseController(ISpanControlService mainControlService,IAdminService _adminService)
        //{
        //    this._mainControlService = mainControlService;
        //}
        //#endregion


        //#region Initialize
        ///// <summary>
        ///// Initialize
        ///// </summary>
        ///// <param name="requestContext"></param>
        //protected override void Initialize(RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
        //    var ipAddressList = _mainControlService.GetAllValidIpAddress(0);
        //    string _controller = requestContext.RouteData.Values["Controller"].ToString().ToLower();
        //    string _action = requestContext.RouteData.Values["Action"].ToString().ToLower();
        //    if (ipAddressList != null && ipAddressList.Any() && !ipAddressList.Any(ip => ip == Utility.GetIPAddress()))
        //    {
        //        if ((_controller == "noaccess" && _action == "signin"))
        //        {
        //            requestContext.HttpContext.Session["IsValidIP"] = "true";
        //        }
        //        else
        //        {
        //            requestContext.HttpContext.Session["IsValidIP"] = "false";
        //        }
        //    }
        //    else
        //    {
        //        requestContext.HttpContext.Session["IsValidIP"] = "true";
        //    }
        //}
        //#endregion

        //#region OnActionExecuting
        ///// <summary>
        ///// OnActionExecuting
        ///// </summary>
        ///// <param name="filterContext"></param>
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // call the base method first
        //    base.OnActionExecuting(filterContext);
        //    if (filterContext.HttpContext.Session["IsValidIP"] != null && filterContext.HttpContext.Session["IsValidIP"].ToString() == "false")
        //    {
        //        filterContext.Result = new RedirectResult(Url.Action("SignIn", "NoAccess"));
        //        return;
        //    }
        //}
        //#endregion
    }
}