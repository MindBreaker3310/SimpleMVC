using System;
using System.ComponentModel.DataAnnotations;
using SimpleMVC.Models;

namespace SimpleMVC.Validations
{
    public class MyCustomModelValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            Product prod = value as Product;

            if (prod.ProductName.Contains("習大大"))
                return new ValidationResult("不得有敏感字元");

            return ValidationResult.Success;
        }

    }
}

