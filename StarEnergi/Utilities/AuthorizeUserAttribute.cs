using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StarEnergi.Utilities
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private string returnUrl = "";
        private int[] _authorizedRoles;
        public int[] AuthorizedRoles
        {
            get { return _authorizedRoles ?? new int[0]; }
            set { _authorizedRoles = value; }
        }

        public AuthorizeUserAttribute(string returnUrl)
        {
            this.returnUrl = returnUrl;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            List<user_per_role> li = httpContext.Session["roles"] as List<user_per_role>;
            bool retVal = false;

            // if we don't need any roles to check if the users has that roles or not, return as authorized user
            if (AuthorizedRoles.Length == 0)
                return true;

            // if our session is empty, then return unauthorized
            if (li == null)
                return false;

            // check if the user that login has the roles that existed in authorizedRoles
            foreach (int i in AuthorizedRoles)
            {
                if (li.Exists(p => p.role == i))
                    retVal = true;
            }


            return retVal;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Account",
                                action = "LogOn",
                                returnUrl = this.returnUrl
                            })
                        );
        }
    }
}