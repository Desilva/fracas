using System.Web.Mvc;

namespace StarEnergi.Utilities
{
    public class MyWebFormViewEngine : WebFormViewEngine
    {
        public MyWebFormViewEngine()
            : base()
        {
            MasterLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.master",
                "~/Views/%1/Shared/{0}.master"
            };

            AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/%1/{1}/{0}.master",
                "~/Areas/{2}/Views/%1/Shared/{0}.master",
            };

            ViewLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.aspx",
                "~/Views/%1/{1}/{0}.ascx",
                "~/Views/%1/Shared/{0}.aspx",
                "~/Views/%1/Shared/{0}.ascx"
            };

            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/%1/{1}/{0}.aspx",
                "~/Areas/{2}/Views/%1/{1}/{0}.ascx",
                "~/Areas/{2}/Views/%1/Shared/{0}.aspx",
                "~/Areas/{2}/Views/%1/Shared/{0}.ascx",
            };

            PartialViewLocationFormats = ViewLocationFormats;
            AreaPartialViewLocationFormats = AreaViewLocationFormats;
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            nameSpace = setPath(nameSpace);
            return base.CreatePartialView(controllerContext, partialPath.Replace("%1", nameSpace));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            nameSpace = setPath(nameSpace);
            return base.CreateView(controllerContext, viewPath.Replace("%1", nameSpace), masterPath.Replace("%1", nameSpace));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            nameSpace = setPath(nameSpace);
            return base.FileExists(controllerContext, virtualPath.Replace("%1", nameSpace));
        }

        private string setPath(string sNamespace)
        {
            string[] temp = sNamespace.Split('.');
            if (temp.Length > 2)
            {
                return temp[2];
            }
            else
            {
                return string.Empty;
            }
        }

    }

}