using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Json;
using System;
using Umbraco.Web.Composing;
using Umbraco.Web;

namespace Skybrud.Umbraco.GridData.Values {
    
    /// <summary>
    /// Abstract class with a basic implementation of the <see cref="IGridControlValue"/> interface.
    /// Underlying value is a JToken
    /// </summary>
    public abstract class GridControlBase : GridJsonToken, IGridControlValue {

        #region Properties

        /// <summary>
        /// Gets a reference to the parent <see cref="GridControl"/>.
        /// </summary>
        [JsonIgnore]
        public GridControl Control { get; }
        
        /// <summary>
        /// Gets whether the control is valid (eg. whether it has a value).
        /// </summary>
        [JsonIgnore]
        public virtual bool IsValid => true;


        /// <summary>
        /// shortcut for the UmbracoContext
        /// </summary>
        public UmbracoContext UmbracoContext => Current.UmbracoContext;

        /// <summary>
        /// Shortcut for the UmbracoHelper
        /// </summary>
        public UmbracoHelper Umbraco => Current.UmbracoHelper;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        /// <param name="token">An instance of <see cref="JToken"/> representing the value of the control.</param>
        protected GridControlBase(GridControl control, JToken token) : base(token) {
            Control = control;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets the value of the control as a searchable text - eg. to be used in Examine.
        /// </summary>
        /// <returns>An instance of <see cref="System.String"/> with the value as a searchable text.</returns>
        public virtual string GetSearchableText() {
            return Environment.NewLine;
        }

        #endregion

    }

}