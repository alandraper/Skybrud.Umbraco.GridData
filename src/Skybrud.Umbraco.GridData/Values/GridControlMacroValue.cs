using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Umbraco.GridData.Converters;

namespace Skybrud.Umbraco.GridData.Values {

    /// <summary>
    /// Class representing the macro value of a control.
    /// </summary>
    [GridConverter("macro")]
    public class GridControlMacroValue : GridControlObjectBase {

        /// <summary>
        /// Gets the syntax of the macro.
        /// </summary>
        [JsonProperty("syntax")]
        public string Syntax { get; set; }

        /// <summary>
        /// Gets the alias of the macro.
        /// </summary>
        [JsonProperty("macroAlias")]
        public string MacroAlias { get; set; }

        /// <summary>
        /// Gets a dictionary containing the macro parameters.
        /// </summary>
        [JsonProperty("macroParamsDictionary")]
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Gets whether the value is valid. For an instance of <see cref="GridControlMacroValue"/>, this means
        /// checking whether a macro alias has been specified.
        /// </summary>
        [JsonIgnore]
        public override bool IsValid => !String.IsNullOrWhiteSpace(MacroAlias);

        #region Constructors
        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        /// <param name="token">An instance of <see cref="JToken"/> representing the value of the control.</param>
        public GridControlMacroValue(GridControl control, JToken token) : base(control, token) {
            Syntax = JObject.GetString("syntax");
            MacroAlias = JObject.GetString("macroAlias");
            Parameters = JObject.GetObject("macroParamsDictionary").ToObject<Dictionary<string, object>>();
        }

        #endregion
    }
}