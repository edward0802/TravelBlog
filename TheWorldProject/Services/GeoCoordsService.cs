﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TheWorldProject.Services
{
    public class GeoCoordsService
    {
        private ILogger<GeoCoordsService> _logger;
        private IConfigurationRoot _config;

        public GeoCoordsService(ILogger<GeoCoordsService> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<GeoCoordsResult> GetCoordsAsync(string name) // name of stop 
        {
            var result = new GeoCoordsResult()
            {
                Success = false,
                Message = "Failed to get coordinates"
            };

            // Use Bing Maps
            var apiKey = _config["Keys:BingKey"];
            var encodedName = WebUtility.UrlEncode(name);
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?q={encodedName}&key={apiKey}";  // look at // https://msdn.microsoft.com/en-us/library/ff701714.aspx

            var client = new HttpClient(); 
            var json = await client.GetStringAsync(url); 
                                                        
            var results = JObject.Parse(json);
            var resources = results["resourceSets"][0]["resources"]; 
            if (!resources.HasValues)
            {
                result.Message = $"Could not find '{name}' as a location";
            }
            else
            {
                var confidence = (string)resources[0]["confidence"]; 
                if (confidence != "High")
                {
                    result.Message = $"Could not find a confident match for '{name}' as a location"; 
                }
                else 
                {
                    var coords = resources[0]["geocodePoints"][0]["coordinates"];
                    result.Latitude = (double)coords[0];
                    result.Longitude = (double)coords[1];
                    result.Success = true;
                    result.Message = "Success";
                }
            }
            return result;
        }
    }
}
