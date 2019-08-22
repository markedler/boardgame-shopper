using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BoardGameShopper.Website.App 
{
    public static class StringExtensions 
    {   
        public static string[] TokenizeString(this string input)
        {
            if (input == null)
                return new string[] {};

            var stripWords = new string[] { "a", "the", "and", "of" };

            var rgx = new Regex("[^a-zA-Z0-9]");
            input = rgx.Replace(input, " ");
            return input.Split(' ').Where(x => stripWords.All(sw => sw != x)).ToArray();
        }
    }
}