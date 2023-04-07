using System;
using SimpleMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleMVC.Validations
{
    public class MyCustomPropertyValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var prod = (Product)validationContext.ObjectInstance;

            if (prod.ProductName == null)
            {
                return new ValidationResult("不得為空");
            }

            if (prod.ProductName.ToLower().Contains("shit") || prod.ProductName.ToLower().Contains("fuck"))
            {
                return new ValidationResult("不可以講髒話！");
            }

            return ValidationResult.Success;
        }
    }
}

