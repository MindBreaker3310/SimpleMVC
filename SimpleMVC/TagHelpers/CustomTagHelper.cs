using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SimpleMVC.TagHelpers
{
    //自訂的tr tag helper -> 將tr元素設定為黑底白字
    //如要修改其他元素 如td class名稱應為TdTagHelper(沒有指定範圍的情況)
    //需要至_ViewImports.cshtml加入@addTagHelper *, SimpleMVC
    //將專案下的TagHelper Import進去

    //指定範圍 目標為tr 且需要有bg-color、text-color 且父元素為thead -> 才會影響
    //[HtmlTargetElement("tr", Attributes = "bg-color, text-color", ParentTag = "thead")]
    [HtmlTargetElement("tr", ParentTag = "tbody")]
    public class TrTagHelper : TagHelper
    {
        public string BgColor { get; set; } = "dark";
        public string TextColor { get; set; } = "white";


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", $"bg-{BgColor} text-center text-{TextColor}");
        }
    }

    [HtmlTargetElement("*", Attributes = "mytag")]
    public class MyTagHelper : TagHelper
    {
        public string BgColor { get; set; } = "primary";
        public string TextColor { get; set; } = "white";


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("class", $"bg-{BgColor} text-center text-{TextColor}");
        }
    }
}

