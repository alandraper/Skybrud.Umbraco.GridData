using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skybrud.Umbraco.GridData.Converters {
    public sealed class GridConverterAttribute : Attribute {
        public GridConverterAttribute(string editorViewName, Type configType = null) {
            this.ViewName = editorViewName;
            this.ConfigType = configType;
        }
        public string ViewName { get; }
        public Type ConfigType { get; }
    }
    
}
