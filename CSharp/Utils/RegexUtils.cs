using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            //Get all valid constructors
            Dictionary<int, ConstructorInfo> constructors = typeof(T).GetConstructors()
                                                                     .Where(c => c.GetParameters()
                                                                                  .All(p => convertibleType.IsAssignableFrom(p.ParameterType)
                                                                                         || convertibleType.IsAssignableFrom(Nullable.GetUnderlyingType(p.ParameterType))))
                                                                     .ToDictionary(c => c.GetParameters().Length, c => c);
            if (constructors.Count is 0) throw new ArgumentException($"Could not find a single valid constructor for type {typeof(T)}", nameof(T));
            

            //Parse results
            T[] results = new T[input.Count];
            foreach (int i in ..results.Length)
            {
                string[] captures = match.Match(input[i])
                                         .GetCapturedGroups()
                                         .Select(c => c.Value)
                                         .ToArray();
                ConstructorInfo constructor;
                try
                {
                    constructor = constructors[captures.Length];
                }
                catch (KeyNotFoundException e)
                {
                    //Rethrow but with a clearer message
                    throw new KeyNotFoundException($"Could not find constructor with {captures.Length} parameters while parsing", e);
                }
                
                object[] parameters = new object[captures.Length];
                ParameterInfo[] paramsInfo = constructor.GetParameters();
                foreach (int j in ..captures.Length)
                {
                    //Get the underlying type if a nullable
                    Type paramType = paramsInfo[j].ParameterType;
                    Type type = Nullable.GetUnderlyingType(paramType) ?? paramType;
                    //Create and set the value
                    parameters[j] = Convert.ChangeType(captures[j], type) ?? throw new InvalidCastException($"Could not convert {captures[j]} to {type}");
                }

                results[i] = (T)constructor.Invoke(parameters);
            }

            return results;
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
                                                  .Select(m => m.Captures)
                                                  .Where(a => a.Count is 2)
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