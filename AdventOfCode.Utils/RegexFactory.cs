using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Regexes;
using AdventOfCode.Utils.Extensions.Types;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// RegexFactory helper class
/// </summary>
internal static class RegexFactoryHelper
{
    /// <summary>
    /// Expected type parameters for the Parse method
    /// </summary>
    private static readonly Type[] ParseMethodParameters = [typeof(string), typeof(IFormatProvider)];
    /// <summary>
    /// Parameter buffer for parse method invocation
    /// </summary>
    private static readonly object?[] ParseBuffer = new object[ParseMethodParameters.Length];

    /// <summary>
    /// Checks if a type is valid to be converted in a RegexFactory
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns><see langword="true"/> if the type can be converted in a RegexFactory, otherwise <see langword="false"/></returns>
    public static bool IsValidType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsImplementationOf<IConvertible>() || type.IsGenericImplementationOf(typeof(IParsable<>), type);
    }

    /// <summary>
    /// Converts a regex capture to the target type
    /// </summary>
    /// <param name="capture">Capture to convert</param>
    /// <param name="targetType">Type to convert the capture to</param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">When the valud in <paramref name="capture"/> could not be converted to <paramref name="targetType"/></exception>
    /// <exception cref="MissingMethodException">When the <see cref="IParsable{TSelf}.Parse"/> method could not be found on <paramref name="targetType"/></exception>
    public static object ConvertCapture(string capture, Type targetType)
    {
        if (targetType.IsImplementationOf<IConvertible>())
        {
            if (targetType.IsEnum)
            {
                targetType = Enum.GetUnderlyingType(targetType);
            }
            //Create and set the value
            return Convert.ChangeType(capture, targetType)
                ?? throw new InvalidCastException($"Could not convert {capture} to {targetType.Name}");
        }

        // Get the normal interface implementation
        MethodInfo? parse = targetType.GetMethod(nameof(IParsable<>.Parse),
                                                 BindingFlags.Static | BindingFlags.Public,
                                                 ParseMethodParameters);
        if (parse is null)
        {
            // Get the explicit interface implementation
            Type constrainedType = typeof(IParsable<>).MakeGenericType(targetType);
            parse = targetType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                              .FirstOrDefault(m => m.IsPrivate
                                                && m.ReturnType == targetType
                                                && m.Name.Contains($".{nameof(IParsable<>.Parse)}")
                                                && m.Name.Contains(nameof(IParsable<>))
                                                && m.GetParameters() is [{ } first, { } second]
                                                && first.ParameterType == ParseMethodParameters[0]
                                                && second.ParameterType == ParseMethodParameters[1]);
            if (parse is null)
            {
                parse = constrainedType.GetMethod(nameof(IParsable<>.Parse),
                                                  BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                  ParseMethodParameters);
                if (parse?.GetMethodBody() is null) throw new MissingMethodException(targetType.FullName, nameof(IParsable<>.Parse));
            }
        }
        ParseBuffer[0] = capture;
        object parsed = parse.Invoke(null, ParseBuffer)
                     ?? throw new InvalidCastException($"Could not parse {capture} to type {targetType.Name}");
        ParseBuffer[0] = null;
        return parsed;
    }
}

/// <summary>
/// Regex object creation factory
/// </summary>
/// <typeparam name="T">Type of objects created</typeparam>
[PublicAPI]
public sealed class RegexFactory<[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)] T> where T : notnull
{
    /// <summary>Stored object type</summary>
    private static readonly Type ObjectType = typeof(T);

    private readonly Regex regex;
    private readonly Dictionary<int, ConstructorInfo> constructors;
    private readonly Dictionary<string, FieldInfo> fields;

    /// <summary>
    /// Creates a new RegexFactory with a given pattern for the specified type.<br/>
    /// <b>NOTE</b>: Creating this object will analyze the target type with reflection, which could potentially be a slow process.
    /// </summary>
    /// <param name="regex">Regex matcher</param>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    public RegexFactory(Regex regex)
    {
        //Create Regex
        this.regex = regex;
        //Get types
        //Get potential constructors
        this.constructors = ObjectType.GetConstructors()
                                      .Where(c => c.GetParameters()
                                                   .All(p => RegexFactoryHelper.IsValidType(p.ParameterType)))
                                      .ToDictionary(c => c.GetParameters().Length, c => c);
        //Get potential fields
        this.fields = ObjectType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                                .Where(f => RegexFactoryHelper.IsValidType(f.FieldType))
                                .ToDictionary(f => f.Name, f => f);
    }

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
        string[] captures = this.regex.Match(input).CapturedGroups
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
            parameters[j] = RegexFactoryHelper.ConvertCapture(captures[j], paramType);
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
        MatchCollection matches = this.regex.Matches(input);

        //Go through them and create the objects
        T[] results = new T[matches.Count];
        foreach (int i in ..results.Length)
        {
            Group[] captures = matches[i].CapturedGroups.ToArray();
            object[] parameters = new object[captures.Length];
            ConstructorInfo constructor = this.constructors[captures.Length];
            ParameterInfo[] paramsInfo = constructor.GetParameters();
            foreach (int j in ..captures.Length)
            {
                //Get the underlying type if a nullable
                Type paramType = paramsInfo[j].ParameterType;
                paramType = Nullable.GetUnderlyingType(paramType) ?? paramType;
                //Create and set the value
                parameters[j] = RegexFactoryHelper.ConvertCapture(captures[j].Value, paramType);
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
                                         .Select(m => m.CapturedGroups.ToArray())
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
            object result = RegexFactoryHelper.ConvertCapture(value, fieldType);
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

    /// <summary>
    /// Constructs a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="regex">Regex matcher</param>
    /// <param name="input">Input strings</param>
    /// <returns>The created <typeparamref name="T"/> objects</returns>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ConstructObjects(Regex regex, IReadOnlyList<string> input)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new RegexFactory<T>(regex).ConstructObjects(input);
    }

    /// <summary>
    /// Constructs a <typeparamref name="T"/> object from a <see cref="Regex"/> match<br/>
    /// The construction finds a constructor on the type with the same amount of parameters as there are captures,<br/>
    /// then populates the parameters by converting the captures to the parameter type.<br/>
    /// Additionally, all the parameter types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to create</typeparam>
    /// <param name="regex">Regex matcher</param>
    /// <param name="input">Input string</param>
    /// <returns>The created <typeparamref name="T"/> objects</returns>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="KeyNotFoundException">If no matching constructor with the right amount of parameters is found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ConstructObjects(Regex regex, string input)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new RegexFactory<T>(regex).ConstructObjects(input);
    }

    /// <summary>
    /// Populates <typeparamref name="T"/> objects from a <see cref="Regex"/> match<br/>
    /// To populate, all matches from the regex are found in the input string, then are separated<br/>
    /// into key/value pairs if there are exactly two captures. The value is then applied to the public field matched with the key.<br/>
    /// Additionally, all the field types must implement <see cref="IConvertible"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to populate</typeparam>
    /// <param name="regex">Regex matcher</param>
    /// <param name="input">Input strings</param>
    /// <returns>An array of the populated <typeparamref name="T"/> objects</returns>
    /// <exception cref="ArgumentException">If the passed pattern has length 0</exception>
    /// <exception cref="InvalidCastException">If an error happens while casting the parameters</exception>
    /// <exception cref="MissingMethodException">If no default constructor is found</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] PopulateObjects(Regex regex, IReadOnlyList<string> input)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new RegexFactory<T>(regex).PopulateObjects(input);
    }
}
