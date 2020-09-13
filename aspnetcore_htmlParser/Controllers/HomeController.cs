// using System.Runtime.CompilerServices;
// using System.Runtime.Intrinsics.X86;
// using System.Net.Http.Headers;
// using AngleSharp;
// using AngleSharp.Dom;
// using System.Security.AccessControl;
//using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aspnetcore_htmlParser.Models;

using AngleSharp.Html.Parser;

namespace aspnetcore_htmlParser.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ViewResult> Index()
        {
            var resp = await TryGetRemoteData();

            if (resp != null)
            {
                var result = await ParceHtmlData(resp);

                if (result != null)
                    return View(result);
            }
            return View();
        }

        public async Task<HttpResponseMessage> TryGetRemoteData()
        {
            using (HttpClient client = new HttpClient())
                return await client.GetAsync(@"https://ru.wikipedia.org/wiki/%D0%9F%D0%B0%D0%BD%D0%B4%D0%B5%D0%BC%D0%B8%D1%8F_COVID-19");
        }

        public async Task<Stat> ParceHtmlData(HttpResponseMessage resp)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(await resp.Content.ReadAsStringAsync());
            var row = document.QuerySelectorAll("#covid19-container > table >tbody > *")[1];
            var cells = row.QuerySelectorAll("th");

            return new Stat
            {
                TotalInfected = int.Parse(cells[1].TextContent.Replace(" ", "").Replace(" ", "")),
                TotalRecovered = int.Parse(cells[2].TextContent.Replace(" ", "").Replace(" ", "")),
                TotalDeath = int.Parse(cells[3].TextContent.Replace(" ", "").Replace(" ", "")),
                MortalityRate = double.Parse(cells[4].TextContent.Replace(" ", "").Replace(" ", "").Replace("%", "")),
                DeathPerMillion = double.Parse(cells[5].TextContent.Replace(" ", "").Replace(" ", ""))
            };
        }
    }
}