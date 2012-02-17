using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using RestSharp;

namespace NathanielGame
{
    static class HighScores
    {
        private static readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        public static List<Score> scores = new List<Score>();
        public static bool downloaded;


        public static void GetScores()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:49304/ScoreService.svc/");
                request.BeginGetResponse(asyncResult =>
                                             {
                                                 try
                                                 {
                                                     HttpWebResponse response =
                                                         (HttpWebResponse)request.EndGetResponse(asyncResult);
                                                     XDocument doc = XDocument.Load(response.GetResponseStream());
                                                     if (doc.Root.Element("problem_cause") == null)
                                                     {
                                                         string[] list = doc.Root.Value.Split(',');
                                                         for (int i = 0; i < list.Length - 3; i += 3)
                                                         {
                                                             Score s = new Score();
                                                             s.name = list[i];
                                                             s.score = list[i + 1];
                                                             s.time = list[i + 2];
                                                             scores.Add(s);
                                                             Debug.WriteLine(s.name);
                                                         }
                                                     }
                                                 }
                                                 catch (WebException we)
                                                 {
                                                     Score s = new Score();
                                                     s.name = "Sorry Download Failed";
                                                     s.score = we.Message;
                                                     s.time = "";
                                                     scores.Add(s);
                                                     Score sc = new Score();
                                                     s.name = "Please try again later";
                                                     s.score = "";
                                                     s.time = "";
                                                     scores.Add(sc);
                                                     HighScoresPanel.Populated = true;
                                                 }





                                             }
                                         , null);
                downloaded = request.HaveResponse;
            }
            catch (WebException we)
            {
                HighScoresPanel.Message = "Sorry. Download Failed. " + we.Message;
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("Problem downloading scored " + e.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't load scores " + ex.Message);

            }
        }

        public static void UploadScore()
        {
           // var client = new RestClient("http://localhost:49304/ScoreService.svc");
            //var request = new RestRequest(new Score() { name = "John", score = "2", time = "333!"}, Method.POST);
            

            //client.ExecuteAsync<Score>(request, (response) =>
           // {
             //   var resource = response.Data;
               // Debug.WriteLine(resource.name);
            //});

        }
    }


    public class Score
    {
        public string name;
        public string score;
        public string time;
    }
}
