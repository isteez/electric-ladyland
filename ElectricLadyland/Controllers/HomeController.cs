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
            @ViewBag.Pallet = string.Format("pallet-{0}", GetColorPallet());
            @ViewBag.WorkoutFigure = string.Format("person-{0}.png", GetWorkoutFigure());

            return View(GetWod());
        }

        #region Utilities

        int GetColorPallet()
        {
            return RandomNumber();
        }

        int GetWorkoutFigure()
        {
            return RandomNumber();
        }

        int RandomNumber()
        {
            Random r = new Random();
            return r.Next(1, 5);
        }

        #endregion

        #region WOD

        WodModel GetWod()
        {
            WodModel wodModel = new WodModel();
            wodModel.Date = string.Format("{0} <br> {1}", DateTime.Now.ToString("dddd"), DateTime.Now.ToString("MMMM dd, yyyy"));

            XmlDocument doc = new XmlDocument();
            doc.Load(@"../Wods.xml");

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
