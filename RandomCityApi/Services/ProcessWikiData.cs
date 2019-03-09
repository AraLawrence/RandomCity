using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;

namespace RandomCityApi.Services
{
    public class ProcessWikiData
    {
        public int MatchPopulation(string wikiPage)
        {
            string popPattern = @"Population.*?Total</th><td>(.*?)</td>";
            Match popMatch = new Regex(popPattern).Match(wikiPage);
            int rtnVal = popMatch.Success && popMatch.Groups[1] != null
                ? int.Parse(popMatch.Groups[1].Value, NumberStyles.AllowThousands) : 0;
            return rtnVal;
        }

        public int MatchArea(string wikiPage)
        {
            string areaPattern = @"Area.*?Total</th><td>(.*?)\&.*?(km|mi)";
            Match areaMatch = new Regex(areaPattern).Match(wikiPage);
            if (areaMatch.Success && areaMatch.Groups[1] != null && areaMatch.Groups[2] != null)
            {
                double area = double.Parse(areaMatch.Groups[1].Value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
                string unit = areaMatch.Groups[2].Value;
                double areaConvert = unit == "km" ? area * 0.62 : area;
                return Convert.ToInt32(areaConvert);
            }
            return 0;
        }

        public string MatchSummary(string wikiPage)
        {
            // Only first paragraph needs processing
            int startIdx = wikiPage.IndexOf("<p>");
            int endIdx = wikiPage.IndexOf("</p>");

            StringBuilder summary = new StringBuilder();
            bool skipChars = false;
            for (var i = startIdx; i < endIdx; i++)
            {
                // Here remove HTML and unicode for brackets 
                // (plus whatever was between them)
                char currChar = wikiPage[i];
                if (currChar == Char.Parse("<")) skipChars = true;
                if (!skipChars) summary.Append(currChar);
                if (currChar == Char.Parse(">")) skipChars = false;
            }
            summary.Replace("\\n", String.Empty);
            summary.Replace("\n", String.Empty);
            summary.Replace("&#32;", String.Empty);
            summary.Replace("&#160;", String.Empty);

            string rtnString = summary.ToString();
            rtnString = Regex.Replace(rtnString, "&#91;.*&#93;", String.Empty);
            return rtnString;
        }
    }
}

