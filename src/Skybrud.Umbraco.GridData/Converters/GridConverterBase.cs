using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;

namespace Skybrud.Umbraco.GridData.Converters {

    /// <summary>
    /// Abstract base implementation of <see cref="IGridConverter"/>.
    /// </summary>
    public abstract class GridConverterBase : IGridConverter {
        /// <summary>
        /// Will match the editorconfig.View
        /// </summary>
        public abstract string EditorViewName { get; }

        /// <summary>
        /// Check if the editor is a match (editor.Alias == Name)
        /// </summary>
        /// <param name="editor">The <see cref="GridEditor"/> to check</param>
        public virtual bool IsMatch(GridEditor editor) {
            return EqualsIgnoreCase(editor.View, this.EditorViewName);
        }
        /// <summary>
        /// Do the work of creating the control. 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract IGridControlValue CreateControlValue(GridControl control, JToken token);

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">A reference to the parent <see cref="GridControl"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted control value.</param>
        public virtual bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = IsMatch(control.Editor)
                ? CreateControlValue(control, token)
                : null;
            return value != null;
        }
        /// <summary>
        /// Do the work of creating the config. 
        /// </summary>
        /// <param name="editor">A reference to the parent <see cref="GridEditor"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the editor config.</param>
        /// <returns>The converted editor config.</returns>
        protected abstract IGridEditorConfig CreateEditorConfig(GridEditor editor, JToken token);

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridEditorConfig"/>.
        /// </summary>
        /// <param name="editor">A reference to the parent <see cref="GridEditor"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the editor config.</param>
        /// <param name="config">The converted editor config.</param>
        public virtual bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = IsMatch(editor) 
                ? CreateEditorConfig(editor, token)
                : null;
            return config != null;
        }
        /// <summary>
        /// Do the work of creating the config. 
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <returns>The wrapped control.</returns>
        protected abstract GridControlWrapper CreateControlWrapper(GridControl control);

        /// <summary>
        /// Gets an instance <see cref="GridControlWrapper"/> for the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <param name="wrapper">The wrapper.</param>
        public virtual bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = IsMatch(control.Editor)
                ? CreateControlWrapper(control)
                : null;
            return wrapper != null;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is contained in <paramref name="source"/> (case insensitive).
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns><code>true</code> if <paramref name="source"/> contains <paramref name="value"/>; otherwise <code>false</code>.</returns>
        protected bool ContainsIgnoreCase(string source, string value) {
            if (String.IsNullOrWhiteSpace(source)) return false;
            if (String.IsNullOrWhiteSpace(value)) return false;
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, value, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is equal <paramref name="source"/> (case insensitive).
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns><code>true</code> if <paramref name="value"/> equal to <paramref name="source"/>; otherwise <code>false</code>.</returns>
        protected bool EqualsIgnoreCase(string source, string value) {
            if (String.IsNullOrWhiteSpace(source)) return false;
            if (String.IsNullOrWhiteSpace(value)) return false;
            return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

    }
    /// <summary>
    /// Implements IGridConverter with the given TValue and no returned EditorConfig
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class GridConverter<TValue> : GridConverterBase where TValue : class, IGridControlValue {
        
        /// <inheritdoc />
        protected override IGridControlValue CreateControlValue(GridControl control, JToken token) {
            try {
                return Activator.CreateInstance(typeof(TValue), control, token) as IGridControlValue;
            } catch (ApplicationException err) {
                global::Umbraco.Core.Composing.Current.Logger.Error(GetType(), err, "Cannot create IGridControl {Type}", typeof(TValue).FullName);
                return null;
            }
        }
        /// <inheritdoc />
        protected override IGridEditorConfig CreateEditorConfig(GridEditor editor, JToken token) {
            return null;
        }
        /// <inheritdoc />
        protected override GridControlWrapper CreateControlWrapper(GridControl control) {
            return control.GetControlWrapper<TValue>();
        }
    }
    /// <summary>
    /// Implements IGridConverter with the given TValue and TEditorConfig
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TEditorConfig"></typeparam>
    public abstract class GridConverter<TValue, TEditorConfig> : GridConverter<TValue> where TValue : class, IGridControlValue where TEditorConfig : class, IGridEditorConfig {
        /// <inheritdoc />
        protected override IGridEditorConfig CreateEditorConfig(GridEditor editor, JToken token) {
            try {
                return Activator.CreateInstance(typeof(TEditorConfig), editor, token as JObject) as IGridEditorConfig;
            } catch (ApplicationException err) {
                global::Umbraco.Core.Composing.Current.Logger.Error(GetType(), err, "Cannot create IGridControl {Type}", typeof(TValue).FullName);
            }
            return null;
        }
    }

}