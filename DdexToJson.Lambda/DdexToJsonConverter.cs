using System;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace aidantwomey.src.dotnetcore.DdexToJson.DdexToJson.Lambda
{
    public class DdexToJsonConverter
    {
        public string ToJson(XDocument ddex)
        {
            return JsonConvert.SerializeXNode(ddex);
        }
    }
}