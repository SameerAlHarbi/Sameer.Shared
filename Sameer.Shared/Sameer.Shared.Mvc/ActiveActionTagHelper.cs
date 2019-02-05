using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Sameer.Shared.Mvc
{
    [HtmlTargetElement(Attributes = "is-active-action")]
    public class ActiveActionTagHelper: ActiveTagHelper
    {
        public ActiveActionTagHelper()
        {
            this.tagHelperName = "is-active-action";
        }
        protected override bool ShouldBeActive()
        {
            string currentAction = ViewContext.RouteData.Values["Action"].ToString();

            if (!string.IsNullOrWhiteSpace(Action) && Action.ToLower() != currentAction.ToLower())
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
