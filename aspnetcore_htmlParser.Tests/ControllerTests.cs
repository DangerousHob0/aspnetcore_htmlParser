using Xunit;
using aspnetcore_htmlParser.Controllers;
using aspnetcore_htmlParser.Models;
using AngleSharp.Html.Parser;
using System.Net.Http;

namespace aspnetcore_htmlParser.Tests
{
    public class ControllerTests
    {
        [Fact]
        public async void CanGetRemoteData()
        {
            HttpResponseMessage testResp;

            using (HttpClient client = new HttpClient())
                testResp = await client.GetAsync(@"https://ru.wikipedia.org/wiki/%D0%9F%D0%B0%D0%BD%D0%B4%D0%B5%D0%BC%D0%B8%D1%8F_COVID-19");

            var controller = new HomeController();
            HttpResponseMessage controllerResp = await controller.TryGetRemoteData();


            Assert.Equal(await controllerResp.Content.ReadAsStringAsync(),
                        await testResp.Content.ReadAsStringAsync());
        }

        [Fact]
        public async void CorrectParseHtmlData()
        {
            var controller = new HomeController();
            HttpResponseMessage resp = await controller.TryGetRemoteData();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(await resp.Content.ReadAsStringAsync());
            var row = document.QuerySelectorAll("#covid19-container > table >tbody > *")[1];
            var cells = row.QuerySelectorAll("th");

            var stat = new Stat
            {
                TotalInfected = int.Parse(cells[1].TextContent.Replace(" ", "").Replace(" ", "")),
                TotalRecovered = int.Parse(cells[2].TextContent.Replace(" ", "").Replace(" ", "")),
                TotalDeath = int.Parse(cells[3].TextContent.Replace(" ", "").Replace(" ", "")),
                MortalityRate = double.Parse(cells[4].TextContent.Replace(" ", "").Replace(" ", "").Replace("%", "")),
                DeathPerMillion = double.Parse(cells[5].TextContent.Replace(" ", "").Replace(" ", ""))
            };

            var contollerStat = await controller.ParceHtmlData(resp);

            Assert.Equal(contollerStat, stat, Comparer.Get<Stat>((Stat s1, Stat s2) =>
            {
                return s1.TotalInfected == s2.TotalInfected &&
                s1.TotalRecovered == s2.TotalRecovered &&
                s1.TotalDeath == s2.TotalDeath &&
                s1.MortalityRate == s2.MortalityRate &&
                s1.DeathPerMillion == s2.DeathPerMillion;
            }));
        }
    }
}
