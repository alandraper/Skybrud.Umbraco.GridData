using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Json.Converters;

namespace Skybrud.Umbraco.GridData.Json {
    
    /// <summary>
    /// Class representing an object derived from an instance of <see cref="JToken"/>.
    /// </summary>
    [JsonConverter(typeof(GridJsonConverter))]
    public class GridJsonToken {

        #region Properties

        /// <summary>
        /// Gets a reference to the underlying instance of <see cref="JToken"/>.
        /// </summary>
        [JsonIgnore]
        public JToken JToken { get; private set; }

        #endregion

        #region Constructors

        /// <param name="token">The underlying instance of <see cref="JToken"/>.</param>
        public GridJsonToken(JToken token) {
            JToken = token;
        }

        #endregion

    }

}