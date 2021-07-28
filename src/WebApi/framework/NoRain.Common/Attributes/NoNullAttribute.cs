using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
   public class NoNullAttribute: ValidationAttribute
    {
        public bool IsNoEmpty { get; set; } = false;
        public NoNullAttribute(bool isemtpy=false)
        {
            IsNoEmpty = isemtpy;
        }
        public override bool IsValid(object value)
        {
            if (value==null)
            {
                return false;
            }
            if (value is DateTime)
            {
                var time = (DateTime)value;
                if (time==default(DateTime))
                {
                    return false;
                }
            }
            if (IsNoEmpty)
            {
                if (value is string)
                {
                    if (value.ToString().IsNullOrEmpty())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}
