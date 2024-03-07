using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SecilStoreCase.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreCase.Entities.Entities;

public class ConfigurationItemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public ConfigurationValueType ConfigurationValueType { get; set; }
    public string Value { get; set; }
    public bool IsActive { get; set; }
    public string ApplicationName { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}
