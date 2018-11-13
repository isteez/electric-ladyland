using System;
using System.Xml;
using ElectricLadyland.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectricLadyland.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(GetRssRoundRobin());
        }

        public IActionResult WOD()
        {
            //theme
            int rInt = RandomNumber();
            @ViewBag.Pallet = string.Format("pallet-{0}", rInt);
            @ViewBag.WorkoutFigure = GetMotionFigureURL(rInt);

            return View(GetWod());
        }

        #region Utilities

        int RandomNumber()
        {
            Random r = new Random();
            return r.Next(1, 3);
        }

        string GetMotionFigureURL(int index)
        {
            //TODO: move to xml file for easy updates
            if (index == 1)
            {
                return "https://cdn.dribbble.com/users/398490/screenshots/2039177/bench_gif.gif";
            }
            else
            {
                return "https://cdn.dribbble.com/users/398490/screenshots/2848209/pullups.gif";
            }
        }

        #endregion

        #region WOD

        WodModel GetWod()
        {
            WodModel wodModel = new WodModel();
            wodModel.Date = string.Format("{0} <br> {1}",
                                          DateTime.Now.ToString("dddd"),
                                          DateTime.Now.ToString("yyyyMMdd"));

            XmlDocument doc = new XmlDocument();
            doc.Load(@"wwwroot/Wods.xml");

            foreach (XmlNode wodNode in doc.DocumentElement.ChildNodes)
            {
                if (wodNode.Attributes["date"].Value == DateTime.Now.ToString("yyyyMMdd"))
                {
                    wodModel.Description = wodNode["description"]?.InnerText;
                    continue;
                }
            }

            if (string.IsNullOrEmpty(wodModel.Description))
            {
                wodModel.Description = "Rest Day";
            }

            return wodModel;
        }

        #endregion

        #region Rss

        RssFeedModel GetRssRoundRobin()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"../ElectricLadyland/RssFeeds.xml");

            Random r = new Random();
            int rInt = r.Next(0, doc.DocumentElement.ChildNodes.Count);

            XmlNode rssFeedRoundBobin = doc.DocumentElement.ChildNodes[rInt];
            RssFeedModel rssFeedModel = new RssFeedModel()
            {
                Name = rssFeedRoundBobin["name"]?.InnerText,
                Link = rssFeedRoundBobin["link"]?.InnerText
            };

            ParseRssFeed(rssFeedModel);
            return rssFeedModel;
        }

        void ParseRssFeed(RssFeedModel rssFeedModel)
        {
            XmlDocument rssXmlDoc = new XmlDocument();
            rssXmlDoc.Load(rssFeedModel.Link);

            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");
            rssFeedModel.Title = rssNodes[0].SelectSingleNode("title")?.InnerText;
            rssFeedModel.Description = rssNodes[0].SelectSingleNode("description")?.InnerText;
        }

        #endregion
    }
}
