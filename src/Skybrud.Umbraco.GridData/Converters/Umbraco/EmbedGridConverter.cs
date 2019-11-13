using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Config;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;
using Skybrud.Umbraco.GridData.Values;

namespace Skybrud.Umbraco.GridData.Converters.Umbraco {

    /// <summary>
    /// Converter for handling the default editors (and their values and configs) of Umbraco.
    /// </summary>
    public class EmbedGridConverter : IGridConverter {
        private bool IsEmbedEditor(GridEditor editor) {
            return editor?.View == "embed";
        }

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">The parent control.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted value.</param>
        public virtual bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = null;
            if (IsEmbedEditor(control.Editor)) {
                value = GridControlEmbedValue.Parse(control, token);
            }
            return value != null;
        }

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridEditorConfig"/>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the editor config.</param>
        /// <param name="config">The converted config.</param>
        public virtual bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = null;
            return false;
        }

        /// <summary>
        /// Gets an instance <see cref="GridControlWrapper"/> for the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <param name="wrapper">The wrapper.</param>
        public virtual bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = null;
            if (IsEmbedEditor(control.Editor)) {
                wrapper = control.GetControlWrapper<GridControlEmbedValue>();
            }
            return wrapper != null;

        }
    }
}