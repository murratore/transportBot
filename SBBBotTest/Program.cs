using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SBBBotTest
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("Was möchtest Du?");

                string input = Console.ReadLine();
                if (input == string.Empty) { break; }

                RootObject my = new RootObject();
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                string jsonobj = wc.DownloadString(@"https://api.projectoxford.ai/luis/v2.0/apps/c126272e-cb6f-4659-8177-a397d15c1447?subscription-key=8028637619ce4a15bce46e21ddb6e92f&q=" + HttpUtility.UrlDecode(input));


                Object t = JsonConvert.DeserializeObject(jsonobj);
                my = JsonConvert.DeserializeObject<RootObject>(jsonobj);

                string von, nach, wann;
                von = string.Empty; nach = string.Empty; wann = string.Empty;


                foreach (var item in my.entities)
                {
                    if (item.type.ToUpper() == "VON") von = item.entity;
                    if (item.type.ToUpper() == "NACH") nach = item.entity;
                    if (item.type.ToUpper() == "WANN") wann = item.entity;

                }

                if (input.Contains("-d")) Console.WriteLine("Von {0} nach {1} {2}", von, nach, wann);

                string transapireturn = wc.DownloadString(string.Format("http://transport.opendata.ch/v1/connections?from={0}&to={1}", HttpUtility.UrlDecode(von),HttpUtility.UrlDecode(nach)));

                TransportObject conninfos = new TransportObject();
                conninfos = JsonConvert.DeserializeObject<TransportObject>(transapireturn);

                foreach (var item in conninfos.connections)
                {
                    Console.WriteLine("{3} von {0} nach {1}, Dauer {4}, Zwischenhalte {2}", item.from.station.name,item.to.station.name, item.transfers.ToString(),System.String.Concat(item.products),item.duration);
                    Console.WriteLine("Abfahrt: {0}, Abfahrtsort: {1} / Ankunft: {2}, Ankunftsort: Gleis {3}", System.Convert.ToDateTime(item.from.departure).ToShortTimeString(), item.from.platform, System.Convert.ToDateTime(item.to.arrival).ToShortTimeString(), item.to.platform);
                    if (item.transfers > 0)
                    {
                        Console.WriteLine("Zwischenhalte:");
                        foreach (var section in item.sections)
                        {
                            Console.WriteLine("  {0} - {1}", section.departure.location.name, System.Convert.ToDateTime(section.arrival.arrival).ToShortTimeString());
                        }
                        
                    }
                    
                }

            }
        }
    }


    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
    }

    public class RootObject
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
    }


    public class TransportObject
    {
       
            public List<Connection> connections { get; set; }
            public From from { get; set; }
            public To to { get; set; }
            public Stations stations { get; set; }
       

        public class Coordinate
        {
            public string type { get; set; }
            public double x { get; set; }
            public double y { get; set; }
        }

        public class Station
        {
            public string id { get; set; }
            public string name { get; set; }
            public object score { get; set; }
            public Coordinate coordinate { get; set; }
            public object distance { get; set; }
        }

        public class Prognosis
        {
            public object platform { get; set; }
            public object arrival { get; set; }
            public object departure { get; set; }
            public int? capacity1st { get; set; }
            public int? capacity2nd { get; set; }
        }

        public class Location
        {
            public string id { get; set; }
            public string name { get; set; }
            public object score { get; set; }
            public Coordinate coordinate { get; set; }
            public object distance { get; set; }
        }

        public class From
        {
            public Station station { get; set; }
            public object arrival { get; set; }
            public object arrivalTimestamp { get; set; }
            public string departure { get; set; }
            public int? departureTimestamp { get; set; }
            public object delay { get; set; }
            public string platform { get; set; }
            public Prognosis prognosis { get; set; }
            public string realtimeAvailability { get; set; }
            public Location location { get; set; }
        }

        public class To
        {
            public Station station { get; set; }
            public string arrival { get; set; }
            public int? arrivalTimestamp { get; set; }
            public object departure { get; set; }
            public object departureTimestamp { get; set; }
            public object delay { get; set; }
            public string platform { get; set; }
            public Prognosis prognosis { get; set; }
            public string realtimeAvailability { get; set; }
            public Location location { get; set; }
        }

        public class Service
        {
            public object regular { get; set; }
            public string irregular { get; set; }
        }

        public class PassList
        {
            public Station station { get; set; }
            public string arrival { get; set; }
            public int? arrivalTimestamp { get; set; }
            public string departure { get; set; }
            public int? departureTimestamp { get; set; }
            public object delay { get; set; }
            public string platform { get; set; }
            public Prognosis prognosis { get; set; }
            public string realtimeAvailability { get; set; }
            public Location location { get; set; }
        }

        public class Journey
        {
            public string name { get; set; }
            public string category { get; set; }
            public object subcategory { get; set; }
            public int? categoryCode { get; set; }
            public string number { get; set; }
            public string @operator { get; set; }
            public string to { get; set; }
            public List<PassList> passList { get; set; }
            public int? capacity1st { get; set; }
            public int? capacity2nd { get; set; }
        }

        public class Departure
        {
            public Station station { get; set; }
            public object arrival { get; set; }
            public object arrivalTimestamp { get; set; }
            public string departure { get; set; }
            public int? departureTimestamp { get; set; }
            public object delay { get; set; }
            public string platform { get; set; }
            public Prognosis prognosis { get; set; }
            public string realtimeAvailability { get; set; }
            public Location location { get; set; }
        }


        public class Arrival
        {
            public Station station { get; set; }
            public string arrival { get; set; }
            public int? arrivalTimestamp { get; set; }
            public object departure { get; set; }
            public object departureTimestamp { get; set; }
            public object delay { get; set; }
            public string platform { get; set; }
            public Prognosis prognosis { get; set; }
            public string realtimeAvailability { get; set; }
            public Location location { get; set; }
        }

        public class Section
        {
            public Journey journey { get; set; }
            public object walk { get; set; }
            public Departure departure { get; set; }
            public Arrival arrival { get; set; }
        }

        public class Stations
        {
            public List<From> from { get; set; }
            public List<To> to { get; set; }
        }

        public class Connection
        {
            public From from { get; set; }
            public To to { get; set; }
            public string duration { get; set; }
            public int? transfers { get; set; }
            public Service service { get; set; }
            public List<string> products { get; set; }
            public int? capacity1st { get; set; }
            public int? capacity2nd { get; set; }
            public List<Section> sections { get; set; }
        }
    }


}
