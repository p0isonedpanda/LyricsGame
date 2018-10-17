using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace LyricsGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Read in settings
            string file = File.ReadAllText("./settings.json");
            Settings s = JsonConvert.DeserializeObject<Settings>(file);

            // Find the web page we want
            Console.Write("Enter a song: ");
            string query = Console.ReadLine();
            var searchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = s.APIKey });
            var listRequest = searchService.Cse.List(query);
            listRequest.Cx = s.EngineID;
            string siteURL = listRequest.Execute().Items[0].Link;

            // Retrieve the web page
            string html;
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(siteURL);
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
    
            // Use XQuery to locate the lyrics of the song
            HtmlNodeCollection htmlBody = htmlDoc.DocumentNode.SelectNodes("//comment()/..");
		    
            // Print the lyrics
		    Console.WriteLine(htmlBody[8].InnerText);	
        }
    }
}
