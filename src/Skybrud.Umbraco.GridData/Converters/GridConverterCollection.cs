using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.GridData.Interfaces;
using Umbraco.Core.Composing;

namespace Skybrud.Umbraco.GridData.Converters {

    /// <summary>
    /// Collection of <see cref="IGridConverter"/>.
    /// </summary>
    public class GridConverterCollection : IEnumerable<IGridConverter> {
        /// <summary>
        /// Look through the TypeLoader and find types implementing IGridConverter
        /// </summary>
        public class ConverterLoader : IUserComposer {
            /// <summary>
            /// Look through the TypeLoader and find types implementing IGridConverter
            /// </summary>
            public void Compose(Composition composition) {
                foreach(var type in composition.TypeLoader.GetTypes<IGridConverter>()) {
                    try {
                        var converter = Activator.CreateInstance(type) as IGridConverter;
                        _converters.Add(converter);
                    }
                    catch(ApplicationException err) {
                        composition.Logger.Error(GetType(), err, "Cannot create IGridConverter {Type}", type.FullName);
                    }
                }
            }
        }

        #region Private fields
        private static List<IGridConverter> _converters = new List<IGridConverter>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the amount of converters added to the collection.
        /// </summary>
        public int Count => _converters.Count;

        #endregion

        #region Member methods

        /// <summary>
        /// Adds the specified <paramref name="converter"/> to the collection.
        /// </summary>
        /// <param name="converter">The converter to be added.</param>
        public void Add(IGridConverter converter) {
            _converters.Add(converter);
        }

        /// <summary>
        /// Adds the specified <paramref name="converter"/> to the collection at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="converter">The converter to be added.</param>
        public void AddAt(int index, IGridConverter converter) {
            _converters.Insert(index, converter);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear() {
            _converters.Clear();
        }

        /// <summary>
        /// Removes the specified <paramref name="converter"/> from the collection.
        /// </summary>
        /// <param name="converter">The converter to be removed.</param>
        public void Remove(IGridConverter converter) {
            _converters.Remove(converter);
        }

        /// <summary>
        /// Removes all converters of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the converters to be removed.</typeparam>
        public void Remove<T>() where T : IGridConverter {
            foreach (T converter in _converters.ToArray().OfType<T>()) {
                _converters.Remove(converter);
            }
        }

        /// <summary>
        /// Removes the converter at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the converter to be removed.</param>
        public void RemoveAt(int index) {
            _converters.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{IGridConverter}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="List{IGridConverter}"/>.</returns>
        public IEnumerator<IGridConverter> GetEnumerator() {
            return _converters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

    }

}