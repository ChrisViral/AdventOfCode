using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utils
{
    public static class RegexUtils
    {
        #region Constants
        /// <summary>
        /// IConvertible type cache
        /// </summary>
        private static readonly Type convertibleType = typeof(IConvertible);
        #endregion
        
        #region Static methods
        /// <summary>
        /// Constructs a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
        /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
        /// then populates the parameters by converting the captures to the parameter type.<br/>
        /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="input">Input strings</param>
        /// <param name="options">The applied Regex options, defaults to <see cref="RegexOptions.None"/></param>
        /// <returns>An array of the created <typeparamref name="T"/> objects</returns>
        /// <exception cref="ArgumentException">If the pattern string is invalid, or if no matching constructors of <typeparamref name="T"/> were found</exception>
        /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
        public static T[] ConstructObjects<T>(string pattern, IReadOnlyList<string> input, RegexOptions options = RegexOptions.None) where T : class
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentException("Cannot have a null or whitespace pattern string", nameof(pattern));
            if (input.Count == 0) return Array.Empty<T>();
            
            //Create match
            Regex match = new(pattern, options);
            
            //Get captures and matching Constructor
            string[] captures = GetCaptures(input[0], match);
            ConstructorInfo? constructor = typeof(T).GetConstructors()
                                                    .SingleOrDefault(c => c.GetParameters().Length == captures.Length);
            if (constructor is null) throw new ArgumentException($"Could not find a single matching constructor for type {typeof(T)} for the produced output of the regex", nameof(T));
            
            //Get parameters
            ParameterInfo[] paramsInfo = constructor.GetParameters();
            if (paramsInfo.Any(p => !convertibleType.IsAssignableFrom(p.ParameterType)
                                 || !convertibleType.IsAssignableFrom(Nullable.GetUnderlyingType(p.ParameterType)))) throw new ArgumentException($"Matching constructor for type {typeof(T)} has parameters which do not implement IConvertible", nameof(T));

            //Parse results
            T[] results = new T[input.Count];
            object[] parameters = new object[paramsInfo.Length];
            results[0] = CreateObject<T>(captures, parameters, constructor, paramsInfo);
            for (int i = 1; i < input.Count; i++)
            {
                results[i] = CreateObject<T>(GetCaptures(input[i], match), parameters, constructor, paramsInfo);
            }

            return results;
        }
        
        /// <summary>
        /// Gets all the Regex captures on a specific input
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="match">Matching Regex</param>
        /// <returns>All the matched captures</returns>
        private static string[] GetCaptures(string input, Regex match) => match.Match(input)
                                                                               .Groups.Cast<Group>().Skip(1)
                                                                               .Select(g => g.Value)
                                                                               .ToArray();

        /// <summary>
        /// Parses a specific line and creates a new <typeparamref name="T"/> from the data
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="captures">Captured strings for parameters</param>
        ///¸<param name="parameterCache">Parameters pre-allocated array</param>
        /// <param name="constructor">Constructor signature</param>
        /// <param name="paramsInfo">Parameter signatures</param>
        /// <returns>The parsed and created <typeparamref name="T"/> object</returns>
        /// <exception cref="InvalidCastException">If the conversion to a parameter failed</exception>
        private static T CreateObject<T>(IReadOnlyList<string> captures, object[] parameterCache, ConstructorInfo constructor, IReadOnlyList<ParameterInfo> paramsInfo)
        {
            for (int i = 0; i < paramsInfo.Count; i++)
            {
                //Get the underlying type if a nullable
                Type paramType = paramsInfo[i].ParameterType;
                Type type = Nullable.GetUnderlyingType(paramType) ?? paramType;
                //Create and set the value
                parameterCache[i] = Convert.ChangeType(captures[i], type) ?? throw new InvalidCastException($"Could not convert {captures[i]} to {type}");
            }

            return (T)constructor.Invoke(parameterCache);
        }

        /// <summary>
        /// Populates a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
        /// To populate, all matches from the regex are found in the input string, then are separated<br/>
        /// into key/value pairs if there are exactly two captures. The value is then applied to the public field matched with the key.<br/>
        /// Additionally, all the field types must implement <see cref="IConvertible"/>.
        /// </summary>
        /// <typeparam name="T">Type of object to populate</typeparam>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="input">Input strings</param>
        /// <param name="options">The applied Regex options, defaults to <see cref="RegexOptions.None"/></param>
        /// <returns>An array of the populated <typeparamref name="T"/> objects</returns>
        /// <exception cref="ArgumentException">If the pattern string is invalid, or if no matching constructors of <typeparamref name="T"/> were found</exception>
        /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
        public static T[] PopulateObjects<T>(string pattern, IReadOnlyList<string> input, RegexOptions options = RegexOptions.None) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentException("Cannot have a null or whitespace pattern string", nameof(pattern));
            if (input.Count == 0) return Array.Empty<T>();
            
            //Create match
            Regex match = new(pattern, options);
            T[] results = new T[input.Count];
            Dictionary<string, FieldInfo> fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                                            .Where(f => convertibleType.IsAssignableFrom(f.FieldType)
                                                                     || convertibleType.IsAssignableFrom(Nullable.GetUnderlyingType(f.FieldType)))
                                                            .ToDictionary(f => f.Name, f => f);
            for (int i = 0; i < input.Count; i++)
            {
                //Find all matches, extract key/value pairs
                (string, string)[] matches = match.Matches(input[i])
                                                  .Select(m => m.Groups.Cast<Group>().Skip(1).ToArray())
                                                  .Where(a => a.Length is 2)
                                                  .Select(a => (a[0].Value, a[1].Value))
                                                  .ToArray();
                //Create object and populate
                T obj = Activator.CreateInstance<T>();
                foreach ((string key, string value) in matches)
                {
                    //If the key matches to a field
                    if (fields.TryGetValue(key, out FieldInfo? field))
                    {
                        //Get the underlying type if a nullable
                        Type fieldType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                        //Create and set the value
                        object result = Convert.ChangeType(value, fieldType) ?? throw new InvalidCastException($"Could not convert {value} to {field.FieldType}");
                        field.SetValue(obj, result);
                    }
                }
                
                //Set the object
                results[i] = obj;
            }

            return results;
        }
        #endregion
    }
}
