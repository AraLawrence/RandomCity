using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using RandomCityApi.Services;

namespace RandomCityApi.Tests.Services
{
    public class ProcessWikiData_ShouldSum
    {
        private readonly ProcessWikiData _processData;
        private readonly ITestOutputHelper output;
        private string _wikiData;
        public ProcessWikiData_ShouldSum(ITestOutputHelper output)
        {
            this.output = output;
           _processData = new ProcessWikiData();
           _wikiData = System.IO.File.ReadAllText("../../../WikiText.txt");
        }

        // MatchSummary should return a string with non-zero length
        [Fact]
        public void ShouldReturnString()
        {
            string wikiSummary = _processData.MatchSummary(_wikiData);
            Assert.IsType<string>(wikiSummary);
            Assert.NotEqual(0, wikiSummary.Length);
        }
        
        // MatchSummary should not return HTML
        [Fact]
        public void ShouldProcessHTML()
        {
            string wikiSummary = _processData.MatchSummary(_wikiData);
            Assert.DoesNotContain("<p>", wikiSummary);
            Assert.DoesNotContain("</p>", wikiSummary);
        }

        // MatchSummary should not contain unicode
        [Fact]
        public void ShouldProcessUnicode()
        {
            string wikiSummary = _processData.MatchSummary(_wikiData);
            Assert.DoesNotContain(".&#91;", _wikiData);
            Assert.DoesNotContain(".&#93;", _wikiData);
        }

        public static IEnumerable<object[]> GetCities()
        {
            yield return new object[] { System.IO.File.ReadAllText("../../../WikiTokyo.txt")};
            yield return new object[] { System.IO.File.ReadAllText("../../../WikiAmsterdam.txt")};
            yield return new object[] { System.IO.File.ReadAllText("../../../WikiNyc.txt") };
        }

        // MatchSummary should work on large cities
        [Theory]
        [MemberData(nameof(GetCities))]
        public void ShoudProcessLargeCities(string city)
        {
            string wikiSummary = _processData.MatchSummary(city);
            Assert.IsType<string>(wikiSummary);
            Assert.NotEqual(0, wikiSummary.Length);
            Assert.DoesNotContain("<p>", wikiSummary);
            Assert.DoesNotContain("</p>", wikiSummary);
            Assert.DoesNotContain(".&#91;", _wikiData);
            Assert.DoesNotContain(".&#93;", _wikiData);
        }
    }
}
