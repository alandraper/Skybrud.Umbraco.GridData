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
    internal class AutoGridTypeLoader {
        private TypeLoader TypeLoader { get; }
        private Type GridControlType = typeof(GridControl);
        private Type[] ConstructorArgs = new Type[] { typeof(GridControl), typeof(JToken) };

        public AutoGridTypeLoader(TypeLoader typeLoader) {
            this.TypeLoader = typeLoader;
        }


        /// <summary>
        /// this will be the function control.GetControlWrapper&lt;TValue&gt;() where TValue : IGridControlValue
        /// </summary>
        private MethodInfo CreateWrapperWithoutConfig { get; set; }

        /// <summary>
        /// this will be the function control.GetControlWrapper&lt;TValue, TConfig&gt;() where TValue : IGridControlValue where TConfig : IGridEditorConfig
        /// </summary>
        private MethodInfo CreateWrapperWithConfig { get; set; }

        void InitWrapperMethods() {
            foreach (var method in GridControlType.GetMethods().Where(m => m.IsGenericMethod && m.Name == "GetControlWrapper")) {
                var typeParams = method.GetGenericArguments();
                if (typeParams.Length == 1) {
                    CreateWrapperWithoutConfig = method; // todo can we typecheck here?
                }
                else if (typeParams.Length == 2) {
                    CreateWrapperWithConfig = method;
                }
            }
            Contract.Assert(CreateWrapperWithoutConfig != null, "CreateWrapperWithoutConfig should not be null");
            Contract.Assert(CreateWrapperWithConfig != null, "CreateWrapperWithConfig should not be null");
        }
        MethodInfo GetWrapperMethod(Type type, Type configType) {
            return configType == null ? CreateWrapperWithoutConfig.MakeGenericMethod(type) : CreateWrapperWithConfig.MakeGenericMethod(type, configType);
        }
        internal void ReadTypes(Dictionary<string, Type> types, Dictionary<string, Type> configTypes, Dictionary<string, Func<GridControl, GridControlWrapper>> wrapperFuncs) {

            InitWrapperMethods();



            foreach (var type in TypeLoader.GetAttributedTypes<GridConverterAttribute>()) {
                var attr = type.GetCustomAttribute<GridConverterAttribute>();
                Contract.Assert(typeof(IGridControlValue).IsAssignableFrom(type), $"Type {type.FullName} with GridConverterAttribute {attr.EditorAlias} must implement IGridControlValue");

                Contract.Assert(type.GetConstructor(ConstructorArgs) != null, "GridControlValue should have a constructor matching ");

                types[attr.EditorAlias] = type;
                configTypes[attr.EditorAlias] = attr.ConfigType; // this might be null
                wrapperFuncs[attr.EditorAlias] =
                    (GridControl gridControl) => GetWrapperMethod(type, attr.ConfigType).Invoke(gridControl, Type.EmptyTypes) as GridControlWrapper;
            }
        }
    }
}