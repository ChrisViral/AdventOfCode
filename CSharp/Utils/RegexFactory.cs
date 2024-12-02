using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.Regexes;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Regex object creation factory
/// </summary>
/// <typeparam name="T">Type of objects created</typeparam>
[PublicAPI]
public class RegexFactory<T> where T : notnull
{
    #region Constants
    /// <summary>Convertible type</summary>
    /// ReSharper disable once StaticMemberInGenericType
    private static readonly Type ConvertibleType = typeof(IConvertible);
    /// <summary>Stored object type</summary>
    private static readonly Type ObjectType      = typeof(T);
    #endregion

    #region Fields
    private readonly Regex regex;
    private readonly Dictionary<int, ConstructorInfo> constructors;
    private readonly Dictionary<string, FieldInfo> fields;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new RegexFactory with a given pattern for the specified type.<br/>
    /// <b>NOTE</b>: Creating this object will analyze the target type with reflection, which could potentially be a slow process.
    /// </summary>
    /// <param name="pattern">Pattern of the Regex match</param>
    /// <param name="options">Regex options</param>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    public RegexFactory(string pattern, RegexOptions options = RegexOptions.None)
    {
        if (pattern.Length is 0) throw new ArgumentException("Pattern length cannot be zero");

        //Create Regex
        this.regex = new(pattern, options);
        //Get types
        //Get potential constructors
        this.constructors = ObjectType.GetConstructors()
                                      .Where(c => c.GetParameters()
                                                   .All(p => ConvertibleType.IsAssignableFrom(Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType)))
                                      .ToDictionary(c => c.GetParameters().Length, c => c);
        //Get potential fields
        this.fields = ObjectType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .Where(f => ConvertibleType.IsAssignableFrom(Nullable.GetUnderlyingType(f.FieldType) ?? f.FieldType))
                                .ToDictionary(f => f.Name, f => f);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Constructs a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="input">Input string</param>
    /// <returns>The created <typeparamref name="T"/> object</returns>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    public T ConstructObject(string input)
    {
        string[] captures = this.regex.Match(input)
                                .GetCapturedGroups()
                                .Select(c => c.Value)
                                .ToArray();
        ConstructorInfo constructor = this.constructors[captures.Length];
        object[] parameters = new object[captures.Length];
        ParameterInfo[] paramsInfo = constructor.GetParameters();
        foreach (int j in ..captures.Length)
        {
            //Get the underlying type if a nullable
            Type paramType = paramsInfo[j].ParameterType;
            paramType = Nullable.GetUnderlyingType(paramType) ?? paramType;
            //Create and set the value
            parameters[j] = Convert.ChangeType(captures[j], paramType) ?? throw new InvalidCastException($"Could not convert {captures[j]} to {paramType}");
        }

        return (T)constructor.Invoke(parameters);
    }

    /// <summary>
    /// Constructs <typeparamref name="T"/> objects from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.<br/>
    /// This construction uses a single input string and finds all matches within it.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="input">Input string</param>
    /// <returns>An array of the created <typeparamref name="T"/> objects</returns>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    public T[] ConstructObjects(string input)
    {
        //Get all matches
        string[][] allCaptures = this.regex.Matches(input)
                                     .Select(m => m.GetCapturedGroups()
                                                   .Select(c => c.Value)
                                                   .ToArray())
                                     .ToArray();

        //Go through them and create the objects
        T[] results = new T[allCaptures.Length];
        foreach (int i in ..results.Length)
        {
            string[] captures = allCaptures[i];
            object[] parameters = new object[captures.Length];
            ConstructorInfo constructor = this.constructors[captures.Length];
            ParameterInfo[] paramsInfo = constructor.GetParameters();
            foreach (int j in ..captures.Length)
            {
                //Get the underlying type if a nullable
                Type paramType = paramsInfo[j].ParameterType;
                paramType = Nullable.GetUnderlyingType(paramType) ?? paramType;
                //Create and set the value
                parameters[j] = Convert.ChangeType(captures[j], paramType) ?? throw new InvalidCastException($"Could not convert {captures[j]} to {paramType}");
            }
            results[i] = (T)constructor.Invoke(parameters);
        }

        return results;
    }

    /// <summary>
    /// Constructs <typeparamref name="T"/> objects from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="input">Input strings</param>
    /// <returns>An array of the created <typeparamref name="T"/> objects</returns>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    /// ReSharper disable once MemberCanBePrivate.Global
    public T[] ConstructObjects(IReadOnlyList<string> input)
    {
        //Make sure some input is passed
        if (input.Count == 0) return [];

        //Parse results
        T[] results = new T[input.Count];
        foreach (int i in ..results.Length)
        {
            results[i] = ConstructObject(input[i]);
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
    /// <param name="input">Input string</param>
    /// <returns>The populated <typeparamref name="T"/> object</returns>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="MissingMethodException">If no default constructor is found</exception>
    /// ReSharper disable once MemberCanBePrivate.Global
    public T PopulateObject(string input)
    {
        //Find all matches, extract key/value pairs
        (string, string)[] matches = this.regex.Matches(input)
                                         .Select(m => m.GetCapturedGroups().ToArray())
                                         .Where(a  => a.Length is 2)
                                         .Select(a => (a[0].Value, a[1].Value))
                                         .ToArray();
        //Create object and populate
        T obj = Activator.CreateInstance<T>();
        foreach ((string key, string value) in matches)
        {
            //If the key matches to a field
            if (!this.fields.TryGetValue(key, out FieldInfo? field)) continue;

            //Get the underlying type if a nullable
            Type fieldType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
            //Create and set the value
            object result = Convert.ChangeType(value, fieldType) ?? throw new InvalidCastException($"Could not convert {value} to {fieldType}");
            field.SetValue(obj, result);
        }

        //Set the object
        return obj;
    }

    /// <summary>
    /// Populates <typeparamref name="T"/> objects from a <see cref="Regex"/> match<br/>
    /// To populate, all matches from the regex are found in the input string, then are separated<br/>
    /// into key/value pairs if there are exactly two captures. The value is then applied to the public field matched with the key.<br/>
    /// Additionally, all the field types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to populate</typeparam>
    /// <param name="input">Input strings</param>
    /// <returns>An array of the populated <typeparamref name="T"/> objects</returns>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="MissingMethodException">If no default constructor is found</exception>
    /// ReSharper disable once MemberCanBePrivate.Global
    public T[] PopulateObjects(IReadOnlyList<string> input)
    {
        //Make sure some input is passed
        if (input.Count == 0) return [];

        T[] results = new T[input.Count];
        for (int i = 0; i < input.Count; i++)
        {
            results[i] = PopulateObject(input[i]);
        }

        return results;
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Constructs a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="pattern">Pattern of the Regex match</param>
    /// <param name="input">Input strings</param>
    /// <param name="options">Regex options</param>
    /// <returns>The created <typeparamref name="T"/> objects</returns>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    public static T[] ConstructObjects(string pattern, IReadOnlyList<string> input, RegexOptions options = RegexOptions.None)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new RegexFactory<T>(pattern, options).ConstructObjects(input);
    }

    /// <summary>
    /// Populates <typeparamref name="T"/> objects from a <see cref="Regex"/> match<br/>
    /// To populate, all matches from the regex are found in the input string, then are separated<br/>
    /// into key/value pairs if there are exactly two captures. The value is then applied to the public field matched with the key.<br/>
    /// Additionally, all the field types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to populate</typeparam>
    /// <param name="pattern">Pattern of the Regex match</param>
    /// <param name="input">Input strings</param>
    /// <param name="options">Regex options</param>
    /// <returns>An array of the populated <typeparamref name="T"/> objects</returns>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="MissingMethodException">If no default constructor is found</exception>
    public static T[] PopulateObjects(string pattern, IReadOnlyList<string> input, RegexOptions options = RegexOptions.None)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new RegexFactory<T>(pattern, options).PopulateObjects(input);
    }
    #endregion
}