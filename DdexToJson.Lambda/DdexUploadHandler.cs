using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.S3;
using System.IO;
using Amazon.S3.Model;
using Amazon.Lambda.S3Events;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace aidantwomey.src.dotnetcore.DdexToJson.DdexToJson.Lambda
{
    public class DdexUploadHandler
    {

        private readonly IAmazonS3 s3client;
        private readonly Func<Stream, TextReader> createStreamReader;

        public DdexUploadHandler()
        {
            this.s3client = new AmazonS3Client();
            this.createStreamReader = s => new StreamReader(s);
        }

        public DdexUploadHandler(IAmazonS3 s3Client, TextReader streamReader)
        {
            this.s3client = s3Client;
            this.createStreamReader = s => streamReader;
        }

        [LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<string> ConvertToJson(S3Event s3event, ILambdaContext context)
        {
            var logger = context.Logger;
            var s3Record = s3event.Records[0].S3;

            logger.LogLine( String.Format( "bucket {0}, key {1}", s3Record.Bucket.Name, s3Record.Object.Key) );

            var request = new GetObjectRequest(){
                BucketName = s3Record.Bucket.Name,
                Key = s3Record.Object.Key,
                ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.None
            };

            var response = await s3client.GetObjectAsync(request);

            var ddexAsJson = ParseResponse(response, logger);

            logger.LogLine(ddexAsJson);

            var putResponse = await s3client.PutObjectAsync( new PutObjectRequest( ){
                BucketName="ddex-samples-as-json",
                Key=s3Record.Object.Key + ".json",
                ContentBody= ddexAsJson
            } );

            return putResponse.HttpStatusCode.ToString();
        }

        private string ParseResponse(GetObjectResponse response, ILambdaLogger logger)
        {
            using (var stream = response.ResponseStream)
            {
                return new DdexToJsonConverter().ToJson( XDocument.Load(stream) );
            }
        }
    }
}