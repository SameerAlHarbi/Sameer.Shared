using System;
using System.ComponentModel.DataAnnotations;

namespace Sameer.Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxValueAttribute : ValidationAttribute
    {
        public double MaxValue { get; set; }

        public MaxValueAttribute(double maxValue)
        {
            this.MaxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType() == typeof(string))
            {
                var maxValAttr = new MaxLengthAttribute((int)MaxValue);

                return maxValAttr.IsValid(value);
            }

            if (Convert.ToDouble(value) > MaxValue)
            {
                return false;
            }

            return true;
        }
    }
}
