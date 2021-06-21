using System;
using Microsoft.AspNetCore.Mvc.Razor;

namespace App.UI.Extensions
{
    public static class RazorExtensions
    {
        public static string FormatDocument(this RazorPage page, int personType, string document)
        {
            return personType == 1 ? 
                Convert.ToUInt64(document).ToString(@"000\.000\.000\-00") : 
                Convert.ToUInt64(document).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}