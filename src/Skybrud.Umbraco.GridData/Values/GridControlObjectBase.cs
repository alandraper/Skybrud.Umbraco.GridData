using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Json;
using System;
using Umbraco.Web.Composing;
using Umbraco.Web;
using System.Diagnostics.Contracts;

namespace Skybrud.Umbraco.GridData.Values {
    
    /// <summary>
    /// Abstract class with a basic implementation of the <see cref="IGridControlValue"/> interface.
    /// Underlying value is a JObject
    /// </summary>
    public abstract class GridControlObjectBase : GridControlBase {
        #region Constructors
        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        /// <param name="token">An instance of <see cref="JToken"/> which should be a <see cref="JObject"/> representing the value of the control.</param>
        protected GridControlObjectBase(GridControl control, JToken token) : base(control, token) {
            Contract.Requires(token is JObject, "GridControlObjectBase requires jObject");
            this.JObject = token as JObject;
        }
        
        #endregion

        /// <summary>
        /// Underlying data JObject
        /// </summary>
        public JObject JObject { get; }

    }

}