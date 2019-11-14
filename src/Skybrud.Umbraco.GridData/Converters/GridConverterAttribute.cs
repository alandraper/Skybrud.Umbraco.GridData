using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skybrud.Umbraco.GridData.Converters {
    /// <summary>
    /// Mark a class to be Auto-converted using the editor.Alias as a key
    /// </summary>
    public sealed class GridConverterAttribute : Attribute {
        /// <summary>
        /// Mark a class to be Auto-converted using the editor.Alias as a key
        /// </summary>
        /// <param name="editorAlias">The string which should match the editor.Alias of the grid control</param>
        /// <param name="configType">The type for the Config (optional)</param>
        public GridConverterAttribute(string editorAlias, Type configType = null) {
            this.EditorAlias = editorAlias;
            this.ConfigType = configType;
        }
        /// <summary>
        /// The string which should match the editor.Alias of the grid control
        /// </summary>
        public string EditorAlias { get; }
        /// <summary>
        /// The type for the Config (optional)
        /// </summary>
        public Type ConfigType { get; }
    }
    
}
