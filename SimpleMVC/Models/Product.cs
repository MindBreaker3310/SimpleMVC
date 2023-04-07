using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleMVC.Validations;

namespace SimpleMVC.Models
{
    [MyCustomModelValidation]
    public class Product
    {
        //[BindNever]不綁定的意思 -> 前端表單就算有填寫，我們也不用將參數綁定到傳入的model
        [Required(ErrorMessage ="Id為必填")]
        public int ProductId { get; set; }


        [Required(ErrorMessage = "ProductName為必填")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "長度須在3~12之間")]
        [MyCustomPropertyValidation]
        public string ProductName { get; set; }


        [Required(ErrorMessage = "Quantity為必填")]
        public int Quantity { get; set; }


        [Required(ErrorMessage = "UnitInStock為必填")]
        public int UnitInStock { get; set; }


        [Required(ErrorMessage = "Cost為必填")]
        [Column(TypeName = "decimal(8,2")]
        [DisplayFormat(DataFormatString = "{0:c2}", ApplyFormatInEditMode = true)]
        [Range(100, 999999, ErrorMessage = "Must be between 100 and 999999")]
        public decimal Cost { get; set; }


        [Required(ErrorMessage = "Discount為必填")]
        public bool Discount { get; set; }

        [Required(ErrorMessage = "LaunchDate為必填")]
        [DataType(DataType.Date)]
        public DateTime LaunchDate { get; set; }



        //public int CategoryId { get; set; }

        //public Category Category { get; set; }

        //public string PaymnetType { get; set; }


    }
}

