using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimpleMVC.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public List<SelectListItem> Categories { get; } = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "1", Text="流行"},
            new SelectListItem(){ Value = "2", Text="家電"},
            new SelectListItem(){ Value = "3", Text="電腦"},
            new SelectListItem(){ Value = "4", Text="運動"}

        };

    }
}

