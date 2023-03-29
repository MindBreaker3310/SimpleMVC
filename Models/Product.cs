using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleMVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(8,2")]
        [DisplayFormat(DataFormatString = "{0:c2}", ApplyFormatInEditMode = true)]
        public decimal Cost { get; set; }

        public decimal? Discount { get; set; }

        public bool IsPartOfDeal { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string PaymnetType { get; set; }


    }
}

