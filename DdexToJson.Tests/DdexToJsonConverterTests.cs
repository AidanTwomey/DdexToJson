using Xunit;
using aidantwomey.src.dotnetcore.DdexToJson.DdexToJson.Lambda;
using System.Xml.Linq;
using System;

namespace aidantwomey.src.dotnetcore.DdexToJson.DdexToJson.Tests
{
    public class GivenADdexXmlFile
    {
        [Fact]
        public void ProducesValidJson()
        {
            var converter = new DdexToJsonConverter();

            var testDoc = XDocument.Load( "TestData/663993503622.xml");

            var jsonNode = converter.ToJson(testDoc);

            Console.WriteLine( jsonNode );
        }
    }
}