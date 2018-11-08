using System;
using System.Collections.Generic;

using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    /// <summary>
    /// A PDF Dictionary type, a collection of named objects
    /// </summary>
    public class PdfDictionary : PdfObject
    {
        private readonly IList<PdfKeyValuePair> _dictionary;

        private PdfDictionary(IList<PdfKeyValuePair> dictionary)
            : base(PdfObjectType.Dictionary)
        {
            IsContainer = true;
            _dictionary = dictionary;
        }

        protected PdfDictionary(PdfDictionary dictionary, PdfObjectType type)
            : base(type)
        {
            IsContainer = true;
            _dictionary = dictionary._dictionary;
        }

        public IPdfObject this[string name]
        {
            get => TryGetValue(name, out IPdfObject value)
                ? value
                : throw new KeyNotFoundException(name + " was not found in PdfDictionary");
        }

        public void ExpectsType(string name)
        {
            PdfName type = this["Type"] as PdfName;
            if (type.Name != name)
            {
                throw new ParsingException($"Expected {name}, but got {type.Name}");
            }
        }

        public static PdfDictionary Parse(Lexer lexer)
        {
            var dictionaryItems = new List<PdfKeyValuePair>();

            PdfObject obj;

            while ((obj = PdfObject.ParseAny(lexer, ">>")) != null)
            {
                if (obj is PdfName name)
                {
                    PdfObject value = PdfObject.ParseAny(lexer);

                    dictionaryItems.Add(new PdfKeyValuePair(name, value));
                }
                else
                {
                    throw new ParsingException("The first item of a pair inside a dictionary must be a PDF name object");
                }
            }

            return new PdfDictionary(dictionaryItems);
        }

        public bool TryGetValue(string key, out IPdfObject value)
        {
            foreach (PdfKeyValuePair pair in _dictionary)
            {
                if (pair.Key.Text == key)
                {
                    value = pair.Value;

                    return true;
                }
            }

            value = null;

            return false;
        }

        /// <summary>
        /// Automatically dereference indirect references or returns the Pdf object
        /// after checking that it is of the expected type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Resolve<T>(string name)
            where T : class
        {
            IPdfObject value = this[name];

            if (value is PdfIndirectReference reference)
            {
                return reference.Dereference<T>();
            }
            else if (value is T)
            {
                return value as T;
            }
            else
            {
                throw new Exception($"Value is not of the expected type {typeof(T)}. Was {value.GetType()}'.");
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (PdfKeyValuePair pair in _dictionary)
                {
                    yield return pair.Key.Text;
                }
            }
        }

        public IEnumerable<IPdfObject> Values
        {
            get
            {
                foreach (PdfKeyValuePair pair in _dictionary)
                {
                    yield return pair.Value;
                }
            }
        }

        public override IReadOnlyList<IPdfObject> Items
        {
            get
            {
                var result = new IPdfObject[_dictionary.Count];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _dictionary[i];
                }

                return result;
            }
        }

        public string Type
        {
            get
            {
                if (TryGetValue("Type", out IPdfObject typeObject))
                {
                    PdfName type = (PdfName)typeObject;

                    return type.Name;
                }

                return null;
            }
        }

        public override string ToString()
        {
            return Type != null ? $"<<...>> ({Type})" : "<<...>>";
        }
    }
}