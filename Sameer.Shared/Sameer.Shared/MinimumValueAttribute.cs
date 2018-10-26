using System;
using System.ComponentModel.DataAnnotations;

namespace Sameer.Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinimumValueAttribute : ValidationAttribute
    {
        public double MinimumValue { get; set; }

        public MinimumValueAttribute(double minValue)
        {
            this.MinimumValue = minValue;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType() == typeof(string))
            {
                var minValAttr = new MinLengthAttribute((int)MinimumValue);

                return minValAttr.IsValid(value);
            }

            if (Convert.ToDouble(value) < MinimumValue)
            {
                return false;
            }

            return true;
        }
    }
}
