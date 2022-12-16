using System.Web;
using System.Web.Optimization;

namespace Main.Control.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                       "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/DatePicker").Include(
                        "~/Scripts/moment.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/bootstrap-datetimepicker.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/ExpressACAForms").Include(
                     "~/Scripts/ExpressACAForms.common.js",
                     "~/Scripts/ExpressACAForms.HelpHead.js",
                     "~/Scripts/jquery.balloon.js",
                     "~/Scripts/ExpressACAForms.qtip.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/confirm-bootstrap.js",
                      "~/Scripts/bootbox.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/bootbox.min.js",
                      "~/Scripts/bootstrap-multiselect.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/CommonJquery").Include(
                      "~/Scripts/jquery.cookie.js",
                      "~/Scripts/jquery.resourcebundle.js",
                      "~/Scripts/jquery.meiomask.js",
                      "~/Scripts/jquery.maskedinput.js",
                      "~/Scripts/jvectormap.js",
                      "~/Scripts/plugins.js",
                      "~/Scripts/jquery.tipsy.js"));

            bundles.Add(new ScriptBundle("~/bundles/colorboxJs").Include(
                      "~/Scripts/jquery.colorbox-min.js",
                      "~/Scripts/jquery.colorbox.js"));

            bundles.Add(new ScriptBundle("~/bundles/uploadifiveJs").Include(
                      "~/Scripts/jquery.uploadifive.min.js",
                      "~/Scripts/jquery.qtip.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/spinJs").Include(
                  "~/Scripts/spin.min.js",
                  "~/Scripts/zoom.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
         "~/Scripts/jquery.dataTables.js"));


            bundles.Add(new ScriptBundle("~/bundles/scrollToJs").Include(
               "~/Scripts/jquery.scrollTo.js",
               "~/Scripts/jquery.scrollTo.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                 "~/Scripts/jquery.inputmask.js",
               "~/Scripts/jquery.inputmask.extensions.js",
               "~/Scripts/jquery.inputmask.date.extensions.js",
               "~/Scripts/jquery.inputmask.numeric.extensions.js",
               "~/Scripts/jquery.inputmask.phone.extensions.js",
               "~/Scripts/jquery.inputmask.regex.extensions.js"
                ));


            bundles.Add(new StyleBundle("~/Content/jqueryuiCSS").Include(
                "~/Content/jquery-ui.css",
                "~/Content/jquery-ui.min.css"));

            bundles.Add(new StyleBundle("~/Content/dataTablesCSS").Include(
                 "~/Content/jquery.dataTables.css"));

            bundles.Add(new StyleBundle("~/Content/colorboxCSS").Include(
                "~/Content/colorbox.css"));

            bundles.Add(new StyleBundle("~/Content/DatePickerCSS").Include(
                "~/Content/bootstrap-datepicker.css",
                "~/Content/bootstrap-datetimepicker.css"
                ));

            bundles.Add(new StyleBundle("~/Content/uploadifiveCSS").Include(
                "~/Content/uploadifive.css"));

            bundles.Add(new StyleBundle("~/Content/ButtonStyleCSS").Include(
                "~/Content/ButtonStyle.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/material-design-iconic-font.css",
                      "~/Content/font-awesome.css",
                       "~/Content/Accordion.css",
                       "~/Content/toastr.css",
                       "~/Content/Animate.css",
                        "~/Content/Common.css",
                        "~/Content/tipsy.css",
                      "~/Content/bootstrap-multiselect.css"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/tmplJs").Include(
                    "~/Scripts/jquery.tmpl.js",
                    "~/Scripts/jquery.shorten.js",
                    "~/Scripts/jquery.dirtyforms.js",
                    "~/Scripts/html2canvas.js",
                    "~/Scripts/summernote.js"));

            bundles.Add(new StyleBundle("~/Content/pluginCSS").Include(
              "~/Content/jquery.qtip.min.css",
              "~/Content/summernote.css",
              "~/Content/bootstrap-nav-wizard.css"));

            bundles.Add(new ScriptBundle("~/bundles/AccordianJs").Include(
               "~/Scripts/Accordian.js"));

            bundles.Add(new ScriptBundle("~/bundles/ToastrJs").Include(
               "~/Scripts/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/CreditValidJs").Include(
                 "~/Scripts/jquery.creditCardValidator"
                 ));
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}
