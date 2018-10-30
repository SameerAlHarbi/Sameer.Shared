using System;
using System.Collections.Generic;

namespace Sameer.Shared.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {
        public Type ErrorMessageResourceType { get; set; }
        public string ErrorMessageResourceName { get; set; }

        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                if (this.ErrorMessageResourceType != null)
                {
                    var r = new System.Resources.ResourceManager(ErrorMessageResourceType);
                    return r.GetString(ErrorMessageResourceName);
                }
                return errorMessage;
            }
            set
            {
                this.errorMessage = value;
            }
        }

        public List<string> ParentsPropertiesNames { get; set; }

        public UniqueAttribute(params string[] parentsNames)
        {
            ErrorMessage = string.Empty;
            ParentsPropertiesNames = new List<string>();
            if (parentsNames != null && parentsNames.Length > 0)
            {
                ParentsPropertiesNames.AddRange(parentsNames);
            }
        }
    }
}
