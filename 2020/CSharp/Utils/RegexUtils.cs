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
        /// Creates a <typeparamref name="T"/> object from a <see cref="Regex"/> match<p/>
        /// In order for the conversion to work, the type must have a constructor with the same amount of parameters as there are captures.
        /// Additionally, all the parameter types must implement <see cref="IConvertible"/>
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="input">Input strings</param>
        /// <param name="compile">If the <see cref="Regex"/> should be compiled or not, defaults to false</param>
        /// <returns>An array of the parsed <typeparamref name="T"/> objects</returns>
        /// <exception cref="ArgumentException">If the pattern string is invalid, or if no matching constructurs of <typeparamref name="T"/> were found</exception>
        /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
        public static T[] CreateObjects<T>(string pattern, string[] input, bool compile = false)
        {
            if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentException("Cannot have a null or whitespace pattern string", nameof(pattern));
            if (input.Length == 0) return Array.Empty<T>();
            
            //Create match
            Regex match = new (pattern, compile ? RegexOptions.Compiled : RegexOptions.None);
            
            //Get captures and matching Constructor
            string[] captures = GetCaptures(input[0], match);
            ConstructorInfo? constructor = typeof(T).GetConstructors()
                                                    .SingleOrDefault(c => c.GetParameters().Length == captures.Length);
            if (constructor is null) throw new ArgumentException($"Could not find a single matching constructor for type {typeof(T)} for the produced output of the regex", nameof(T));
            
            
            //Get parameters
            ParameterInfo[] paramsInfo = constructor.GetParameters();
            if (paramsInfo.Any(p => !convertibleType.IsAssignableFrom(p.ParameterType))) throw new ArgumentException($"Matching constructor for type {typeof(T)} has parameters which do not implement IConvertible", nameof(T));

            //Parse results
            T[] results = new T[input.Length];
            object[] parameters = new object[paramsInfo.Length];
            results[0] = ParseLine<T>(captures, parameters, constructor, paramsInfo);
            for (int i = 1; i < input.Length; i++)
            {
                results[i] = ParseLine<T>(GetCaptures(input[i], match), parameters, constructor, paramsInfo);
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
        private static T ParseLine<T>(IReadOnlyList<string> captures, object[] parameterCache, ConstructorInfo constructor, IReadOnlyList<ParameterInfo> paramsInfo)
        {
            for (int i = 0; i < paramsInfo.Count; i++)
            {
                parameterCache[i] = Convert.ChangeType(captures[i], paramsInfo[i].ParameterType) ?? throw new InvalidCastException($"Could not convert {captures[i]} to {paramsInfo[i].ParameterType}");
            }

            return (T)constructor.Invoke(parameterCache);
        }
        #endregion
    }
}
