using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using bridgeMonitor.Models;

namespace VCubWatcher.Controllers
{
    public class StationController : Controller
    {
        private const string _cookieKey = "TimeFrame";

        public IActionResult Liste()
        {
            var TimeFrame = GetTimeFrameFromApi();



            return View(TimeFrame);
        }

        public IActionResult Carte()
        {
            var stations = GetTimeFrameFromApi();
            return View(stations);
        }

        public IActionResult Favoris()
        {
            //On r�cup�re les 183 stations
            var stations = GetBikeStationsFromApi();

            //On r�cup�re la liste des stations en favoris
            List<int> favoriteStationIds;
            if (Request.Cookies.ContainsKey(_cookieKey))
            {
                favoriteStationIds = JsonConvert.DeserializeObject<List<int>>(Request.Cookies[_cookieKey]);
            }
            else
            {
                favoriteStationIds = new List<int>();
            }

            //On cr�e une liste de station vide dans laquelle on copiera les stations dont les IDs sont dans le cookie
            List<BikeStation> favorites = new List<BikeStation>();
            foreach (var stationId in favoriteStationIds)
            {
                foreach (var station in stations)
                {
                    if (station.Id == stationId) favorites.Add(station);
                }
            }

            return View(favorites);
        }

        private static List<BikeStation> GetBikeStationsFromApi()
        {
            //Cr�ation un HttpClient (= outil qui va permettre d'interroger une URl via une requ�te HTTP)
            using (var client = new HttpClient())
            {
                //Interrogation de l'URL cens�e me retourner les donn�es
                var response = client.GetAsync("http://api.alexandredubois.com/vcub-backend/vcub.php");
                //R�cup�ration du corps de la r�ponse HTTP sous forme de cha�ne de caract�res
                var stringResult = response.Result.Content.ReadAsStringAsync();
                //Conversion de mon flux JSON (string) en une collection d'objets BikeStation
                //d'un flux de donn�es vers des objets => D�serialisation
                //d'objets vers un flux de donn�es => S�rialisation
                var result = JsonConvert.DeserializeObject<List<BikeStation>>(stringResult.Result);
                return result;
            }
        }
    }