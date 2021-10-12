using System;
using System.Collections.Generic;
using System.Text;
using FuzzySharp;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using Humanizer;

namespace DuplicateFilter
{
    public static class InputSanitiser
    {

        // Iterate over given list and standardises each string
        // If stripBusinessTerms = true, remove all business terms; else, standardise them to full-length words
        public static void StandardiseInput(List<string> companyNames, bool stripBusinessTerms = false)
        {
            for (int index = 0; index < companyNames.Count; index++)
            {
                string name = companyNames[index];
                name = RemoveAccents(name);                
                name = StandardiseCorp(name, stripBusinessTerms);
                name = StandardiseLimited(name, stripBusinessTerms);
                name = StandardiseCompany(name, stripBusinessTerms);
                name = StandardiseInc(name, stripBusinessTerms);
                name = StandardiseAmpersand(name);
                name = StandardiseNumbers(name);
                name = RemoveExcessWhitespace(name);
                name = name.ToLower();
                companyNames[index] = name;
            }

        }

        // Standardize all whitespace to length one
        public static string RemoveExcessWhitespace(string text)
        {
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        // Standardise all numbers to their word equivalents (e.g. 22 to twenty-two)
        public static string StandardiseNumbers(string text)
        {
            string[] splitText = Regex.Split(text, "(\\d+)");
            var stringBuilder = new StringBuilder();
            int numericValue;

            foreach (string token in splitText)
            {
                if (int.TryParse(token, out numericValue)){
                    stringBuilder.Append(numericValue.ToWords());
                }
                else
                {
                    stringBuilder.Append(token);
                }
            }

            return stringBuilder.ToString();

        }

        // Replace any instances of 'corp.' with 'Corporation', or with " " if strip = true
        public static string StandardiseCorp(string text, bool strip = false)
        {
            string replacementText;
            if (strip) {
                replacementText = " ";
                text = Regex.Replace(text, "(?i)\\scorporation(\\W|$)", replacementText);
            }
            else
            {
                replacementText = " corporation ";
            }
            return Regex.Replace(text, "(?i)\\scorp\\.?(\\W|$)", replacementText);
        }

        // Replace any instances of 'ltd' with 'limited', or with " " if strip = true
        public static string StandardiseLimited(string text, bool strip = false)
        {
            string replacementText;
            if (strip)
            {
                replacementText = " ";
                text = Regex.Replace(text, "(?i)\\slimited(\\W|$)", replacementText);
            }
            else
            {
                replacementText = " limited ";
            }
            return Regex.Replace(text, "(?i)\\sltd\\.?(\\W|$)", replacementText);
        }

        // Replace any instance of 'co' with 'company', or with " " if strip = true
        public static string StandardiseCompany(string text, bool strip = false)
        {
            string replacementText;
            if (strip)
            {                
                replacementText = " ";
                text = Regex.Replace(text, "(?i)\\scompany(\\W|$)", replacementText);
            }
            else
            {
                replacementText = " company ";
            }
            return Regex.Replace(text, "(?i)\\sco\\.?(\\W|$)", replacementText);
        }

        // Replace any instances of '&' with 'and'
        public static string StandardiseAmpersand(string text)
        {
            return Regex.Replace(text, "(?i)\\s?&\\s?", " and ");
        }

        // Replace any instances of 'inc.' with 'incorporated', or with " " if strip = true
        public static string StandardiseInc(string text, bool strip = false)
        {
            string replacementText;
            if (strip)
            {
                replacementText = " ";
                text = Regex.Replace(text, "(?i)\\sincorporated(\\W|$)", replacementText);
            }
            else
            {
                replacementText = " incorporated ";
            }
            return Regex.Replace(text, "(?i)\\sinc\\.?(\\W|$)", replacementText);
        }

        // Replaces accented characters in a string with their base character counterpart
        public static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }



    }
}
