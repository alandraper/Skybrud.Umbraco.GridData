using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Linq;

using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.GridData.Converters {
    /// <summary>
    /// IGridConverter which looks through all types with the GridConverterAttribute, 
    /// and registers them to be auto-converted. 
    /// The type must have a constructor with a GridControl and JToken as parameters
    /// </summary>
    public sealed class AutoGridConverter : IGridConverter, IUserComposer {
        /// <summary>
        /// Register types with 
        /// </summary>
        /// <param name="composition"></param>
        public void Compose(Composition composition) {
            new AutoGridTypeLoader(composition.TypeLoader).ReadTypes(types, configTypes, wrapperFuncs);
        }

        static readonly Dictionary<string, Type> types = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> configTypes = new Dictionary<string, Type>();
        static readonly Dictionary<string, Func<GridControl, GridControlWrapper>> wrapperFuncs = new Dictionary<string, Func<GridControl, GridControlWrapper>>();

        /// <inheritdoc />
        public bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = types.ContainsKey(control.Editor.Alias)
                ? (IGridControlValue) Activator.CreateInstance(types[control.Editor.Alias], control, token)
                : null;
            return value != null;
        }
        /// <inheritdoc />
        public bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = configTypes.ContainsKey(editor.Alias)
                ? (IGridEditorConfig)Activator.CreateInstance(types[editor.Alias], editor, token)
                : null;
            return config != null;
        }
        /// <inheritdoc />
        public bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = wrapperFuncs.ContainsKey(control.Editor.Alias)
                ? wrapperFuncs[control.Editor.View](control)
                : null;
            return wrapper != null;
        }
    }
}
