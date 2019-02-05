using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Sameer.Shared.Mvc
{
    [HtmlTargetElement(Attributes = "is-active-controller")]
    public class ActiveControllerTagHelper : ActiveTagHelper
    {
        public ActiveControllerTagHelper()
        {
            this.tagHelperName = "is-active-controller";
        }
        protected override bool ShouldBeActive()
        {
            string currentController = ViewContext.RouteData.Values["Controller"].ToString();

            if (!string.IsNullOrWhiteSpace(Controller) && Controller.ToLower() != currentController.ToLower())
            {
                return false;
            }

            foreach (KeyValuePair<string, string> routeValue in RouteValues)
            {
                if (!ViewContext.RouteData.Values.ContainsKey(routeValue.Key) ||
                    ViewContext.RouteData.Values[routeValue.Key].ToString() != routeValue.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
