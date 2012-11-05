﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionConverter.cs" company="Jörg Battermann">
//   Copyright © Jörg Battermann 2011
// </copyright>
// <summary>
//   Defines the PolygonConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Text;

namespace GeoJSON.Net.Converters
{
    using System;
    using System.Collections.Generic;

    using Exceptions;
    using Geometry;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converter to read and write the <see cref="GeographicPosition" /> type.
    /// </summary>
    public class PointConverter : JsonConverter
    {
        /// <summary>	
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
			writer.WriteStartArray();
			var g = value as List<IPosition>;
	        var geographicPosition =g[0] as GeographicPosition;
	        var s = string.Format("{0},{1}", geographicPosition.Latitude.ToString(CultureInfo.InvariantCulture), geographicPosition.Longitude.ToString(CultureInfo.InvariantCulture));
			writer.WriteRawValue(s);
	        writer.WriteEndArray();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var coordinates = serializer.Deserialize<JArray>(reader);
            if (coordinates == null || coordinates.Count != 2)
            {
                throw new ParsingException(
                    string.Format(
                        "Point geometry coordinates could not be parsed. Expected something like '[-122.428938,37.766713]' ([lon,lat]), what we received however was: {0}", 
                        coordinates));
            }

            string latitude;
            string longitude;
            try
            {
                longitude = coordinates.First.ToString();
                latitude = coordinates.Last.ToString();
            }
            catch (Exception ex)
            {
                throw new ParsingException("Could not parse GeoJSON Response. (Latitude or Longitude missing from Point geometry?)", ex);
            }

            return new List<IPosition> { new GeographicPosition(latitude, longitude) };
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GeographicPosition);
        }
    }
}