using System;
using Tweetinvi;
using System.Collections.Generic;
using Tweetinvi.Events;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;

namespace TwitterBot {
    class Program {
        static void Main(string[] striptargs) {
            string auth_key = "";
            string auth_secret = "";
            string app_key = "";
            string app_secret = "";
            string tweetmsg = "";

            List<long> followlist = new List<long>();
            List<long> deletelist = new List<long>();

            foreach (string arg in striptargs) {
                if (arg.StartsWith("/appkey=")) {
                    app_key = arg.Replace("/appkey=", "");
                }
                if (arg.StartsWith("/appsecret=")) {
                    app_secret = arg.Replace("/appsecret=", "");
                }
                if (arg.StartsWith("/authkey=")) {
                    auth_key = arg.Replace("/authkey=", "");
                }
                if (arg.StartsWith("/secret=")) {
                    auth_secret = arg.Replace("/secret=", "");
                }
                if (arg.StartsWith("/follow=")) {
                    string followStr = arg.Replace("/follow=", "");
                    List<string> followStrList = followStr.Split(",").ToList();
                    foreach (string follow in followStrList) {
                        bool success = long.TryParse(follow.Trim(), out long followid);
                        if (success) { followlist.Add(followid); }
                    }
                }
                if (arg.StartsWith("/delete=")) {
                    string str = arg.Replace("/delete=", "");
                    List<string> lstStr = str.Split(",").ToList();
                    foreach (string follow in lstStr) {
                        bool success = long.TryParse(follow.Trim(), out long id);
                        if (success) { deletelist.Add(id); }
                    }
                }

                if (arg.StartsWith("/tweet=")) {
                    tweetmsg = arg.Replace("/tweet=", "");
                }
            }

            if (auth_key == "") {
                Console.WriteLine("Enter auth key:");
                auth_key = Console.ReadLine();
            }

            if (auth_secret == "") {
                Console.WriteLine("Enter auth secret:");
                auth_secret = Console.ReadLine();
            }

            Auth.SetUserCredentials(app_key, app_secret, auth_key, auth_secret);

            if (deletelist.Count() > 0) {
                foreach (long id in deletelist) {
                    try {
                        Tweet.GetTweet(id).Destroy();
                        Console.WriteLine("Deleted Tweet ID " + id.ToString());
                    } catch {
                        Console.WriteLine("Failed to Delete Tweet ID" + id.ToString());
                    }
                }
            }

            if (tweetmsg != "") {
                Tweet.PublishTweet(tweetmsg);
                Console.WriteLine("Tweeted: " + tweetmsg);
            }

            if (followlist.Count() > 0) {
                var stream = Stream.CreateFilteredStream();
                foreach (long follow in followlist) {
                    Console.WriteLine("following " + follow.ToString());
                    stream.AddFollow(follow);
                }

                Console.WriteLine("Waiting on Tweets...");
                stream.MatchingTweetReceived += (sender, args) => {
                    //Check for Periscope Links...
                    if (args.Tweet.Urls.Any(t1 => t1.ExpandedURL.Contains("pscp.tv")) && args.Tweet.RetweetedTweet == null) {
                        Tweet.PublishTweet(HttpUtility.HtmlDecode(args.Tweet.FullText));
                        Console.WriteLine("Retweeted: " + HttpUtility.HtmlDecode(args.Tweet.ToString()));
                    }
                };

                stream.StartStreamMatchingAllConditions();
            }
        }

        private static void OnMatchedTweet(object sender, MatchedTweetReceivedEventArgs args) {
            //Do Stuff
            Console.WriteLine(args.Tweet);
        }

        private static string sanitize(string raw) {
            return Regex.Replace(raw, @"(@[A-Za-z0-9]+)|([^0-9A-Za-z \t])|(\w+:\/\/\S+)", " ").ToString();
        }
    }
}
