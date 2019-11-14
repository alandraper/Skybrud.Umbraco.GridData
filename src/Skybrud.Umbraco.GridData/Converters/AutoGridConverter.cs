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
    public class AutoGridConverter : IGridConverter, IUserComposer {
        /// <summary>
        /// Save the TypeLoader for lazy loading later.
        /// </summary>
        /// <param name="composition"></param>
        public void Compose(Composition composition) {
            
            MethodInfo createWrapperWithoutConfig = null;
            MethodInfo createWrapperWithConfig = null;
            foreach( var method in typeof(GridControl).GetMethods().Where(m => m.IsGenericMethod && m.Name == "GetControlWrapper")) {
                var typeParams = method.GetGenericArguments();
                if(typeParams.Length == 1) {
                    createWrapperWithoutConfig = method;
                }
                else if(typeParams.Length == 2) {
                    createWrapperWithConfig = method;
                }
            }
            Contract.Assert(createWrapperWithConfig != null, "CreateWrapperWithConfig should not be null");
            Contract.Assert(createWrapperWithoutConfig != null, "CreateWrapperWithoutConfig should not be null");
                
            foreach ( var type in composition.TypeLoader.GetAttributedTypes<GridConverterAttribute>()) {
                var attr = type.GetCustomAttribute<GridConverterAttribute>();
                Contract.Assert(typeof(IGridControlValue).IsAssignableFrom(type), $"Type {type.FullName} with GridConverterAttribute {attr.ViewName} must implement IGridControlValue");
                _types[attr.ViewName] = type;
                MethodInfo wrapperMethod;
                if (attr.ConfigType != null) {
                    _configTypes[attr.ViewName] = attr.ConfigType;
                    wrapperMethod = createWrapperWithConfig.MakeGenericMethod(type, attr.ConfigType);
                }
                else {
                    wrapperMethod = createWrapperWithoutConfig.MakeGenericMethod(type);
                }
                _wrapperFuncs[attr.ViewName] = (gridControl) => wrapperMethod.Invoke(gridControl, Type.EmptyTypes) as GridControlWrapper;
            }
        }
        private static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> _configTypes = new Dictionary<string, Type>();
        private static Dictionary<string, Func<GridControl, GridControlWrapper>> _wrapperFuncs = new Dictionary<string, Func<GridControl, GridControlWrapper>>();

        /// <inheritdoc />
        public bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = _types.ContainsKey(control.Editor.View)
                ? (IGridControlValue) Activator.CreateInstance(_types[control.Editor.View], control, token)
                : null;
            return value != null;
        }
        /// <inheritdoc />
        public bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = _configTypes.ContainsKey(editor.View)
                ? (IGridEditorConfig)Activator.CreateInstance(_types[editor.View], editor, token)
                : null;
            return config != null;
        }
        /// <inheritdoc />
        public bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = _wrapperFuncs.ContainsKey(control.Editor.View)
                ? _wrapperFuncs[control.Editor.View](control)
                : null;
            return wrapper != null;
        }
    }
}
