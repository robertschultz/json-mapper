using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Json
{
    public class Mapper
    {
        /// <summary>
        /// Dictionary object used to hold the generated string contents.
        /// </summary>
        public Dictionary<string, List<string>> Contents { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public Mapper(string keyOneName, string keyTwoName, string data)
        {
            Contents = new Dictionary<string, List<string>>();

            // Use static method to deserialize the json data to an object
            var results = Deserialize(data);

            // Identify the first and the second languages from available
            var keyOne = ((Dictionary<string, object>)results.Where(t => t.Key == keyOneName).First().Value);
            var keyTwo = ((Dictionary<string, object>)results.Where(t => t.Key == keyTwoName).First().Value);

            MapPropertyContents(keyOne);
            MapPropertyContents(keyTwo);
        }

        /// <summary>
        /// Static method that just deserialized a json string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IDictionary<string, object> Deserialize(string data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<IDictionary<string, object>>(data);
        }

        /// <summary>
        /// Override to map property values to strings.
        /// </summary>
        /// <param name="props"></param>
        public void MapPropertyContents(Dictionary<string, object> props)
        {
            MapPropertyContents(props, string.Empty);
        }

        /// <summary>
        /// Maps property values to strings.
        /// </summary>
        /// <param name="props"></param>
        /// <param name="prefix"></param>
        public void MapPropertyContents(Dictionary<string, object> props, string prefix)
        {
            // Loop through each property on the object
            foreach (var prop in props)
            {
                // If the property value is a string, then identify it and add it to the contents.
                // If it is anything but a string, then prepend the key as the prefix and attach the dot notation
                if (prop.Value.GetType().Equals(typeof(string)))
                {
                    if (Contents.ContainsKey(prefix + prop.Key))
                    {
                        Contents.Where(item => item.Key == prefix + prop.Key).First().Value.Add(prop.Value.ToString());
                    }
                    else
                    {
                        Contents.Add(prefix + prop.Key, new List<string> { prop.Value.ToString() });
                    }
                }
                else
                {
                    prefix += prop.Key + ".";
                    MapPropertyContents((Dictionary<string, object>)prop.Value, prefix);
                }
            }
        }

        /// <summary>
        /// A ToString override to return the generated results.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // Loop through each composite key in the contents
            foreach (var item in Contents)
            {
                builder.Append("\"" + item.Key + "\"");

                // Loop through each translated value in the contents.  This would be dynamic to support multiple languages
                foreach (var val in item.Value)
                {
                    builder.Append(", \"" + val + "\"");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
