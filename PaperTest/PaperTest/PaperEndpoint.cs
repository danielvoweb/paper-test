using System;
using System.Net;
using System.Net.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PaperTest
{
    public class PaperEndpoint
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired] public Uri Endpoint { get; set; }
        [BsonRequired] public HttpMethod Method { get; set; }
        [BsonRequired] public HttpStatusCode StatusCode { get; set; }
        public string Headers { get; set; }
        public string TrailingHeaders { get; set; }
        [BsonRequired] public string ReasonPhase { get; set; }
        public string Content { get; set; }

        [BsonRequired]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TimeToLive { get; set; }
    }
}