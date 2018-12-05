using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//ReSharper disable StaticMemberInGenericType

namespace AdventOfCode.Tools
{
    /// <summary>
    /// Enum utility methods
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Generic Enum conversion class, hides all the type handling away
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        private static class EnumConverter<T> where T : struct, Enum, IConvertible
        {
            #region Properties
            /// <summary>
            /// Maximum numerical value of this enum type
            /// </summary>
            public static int Max { get; }

            /// <summary>
            /// Minimum numerical value of this enum type
            /// </summary>
            public static int Min { get; }

            /// <summary>
            /// Name to value enum conversion dictionary
            /// </summary>
            public static Dictionary<string, T> NameToValue { get; } = new Dictionary<string, T>();

            /// <summary>
            /// Value to name enum conversion dictionary
            /// </summary>
            public static Dictionary<T, string> ValueToName { get; } = new Dictionary<T, string>();

            /// <summary>
            /// A collection of the names of the values of this Enum type
            /// </summary>
            public static ReadOnlyCollection<string> Names { get; }

            /// <summary>
            /// A collection of the Values of this enum type
            /// </summary>
            public static ReadOnlyCollection<T> Values { get; }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes enum conversion for the given type
            /// </summary>
            static EnumConverter()
            {
                //Setup
                Type enumType = typeof(T);
                T[] values = (T[])Enum.GetValues(enumType);
                Min = int.MaxValue;
                Max = int.MinValue;
                List<string> tempNames = new List<string>(values.Length);
                List<T> tempValues = new List<T>(values.Length);


                //Loop through the values
                for (int i = 0; i < values.Length; i++)
                {
                    //Get current value
                    T value = (T)values.GetValue(i);
                    string name = Enum.GetName(enumType, value);

                    //Check if the value is valid
                    if (!string.IsNullOrEmpty(name) && !ValueToName.ContainsKey(value) && !NameToValue.ContainsKey(name))
                    {
                        //Get numerical value
                        try
                        {
                            int numValue = value.ToInt32(null);
                            Min = Math.Min(Min, numValue);
                            Max = Math.Max(Max, numValue);
                        }
                        catch { /* Error suppression is intended, values outside of range have undefined behaviour*/ }

                        tempNames.Add(name);
                        tempValues.Add(value);
                        ValueToName.Add(value, name);
                        NameToValue.Add(name, value);
                    }
                }

                //Set final data
                if (Max == int.MinValue) { Max = 0; }
                if (Min == int.MaxValue) { Min = 0; }

                Names = tempNames.AsReadOnly();
                Values = tempValues.AsReadOnly();
            }
            #endregion
        }

        #region Static methods
        /// <summary>
        /// Maximum numerical value of this enum type
        /// CAREFUL: If the underlying type is not Int32 and the value overflows, the assigned max value will be the closet valid value or zero
        /// </summary>
        public static int Max<T>() where T : struct, Enum => EnumConverter<T>.Max;
        
        /// <summary>
        /// Minimum numerical value of this enum type
        /// CAREFUL: If the underlying type is not Int32 and the value overflows, the assigned min value will be the closet valid value or zero
        /// </summary>
        public static int Min<T>() where T : struct, Enum => EnumConverter<T>.Min;

        /// <summary>
        /// Checks if the given value is a defined member of the enum type
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value is defined, false otherwise</returns>
        public static bool IsDefined<T>(T value) where T : struct, Enum => EnumConverter<T>.ValueToName.ContainsKey(value);

        /// <summary>
        /// Returns the string value of an Enum
        /// </summary>
        /// <param name="value">Enum value to convert to string</param>
        public static string GetName<T>(T value) where T : struct, Enum => EnumConverter<T>.ValueToName[value];

        /// <summary>
        /// Returns the name of the given enum member in CamelCase
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value of the enum member</param>
        /// <returns>Camel Case name</returns>
        public static string GetNameCamelCase<T>(T value) where T : struct, Enum
        {
            //Setup char buffer
            bool upper = true;
            char[] buffer = GetName(value).ToCharArray();
            int index = 0;

            //Loop through reference string
            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];
                //Add characters correctly
                if (c == '_') { upper = true; }
                else
                {
                    buffer[index++] = upper ? char.ToUpper(c) : char.ToLower(c);
                    upper = false;
                }
            }

            //Return the final string
            return new string(buffer, 0, index);
        }

        /// <summary>
        /// Parses the given string to the given Enum type 
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="name">String to parse</param>
        public static T GetValue<T>(string name) where T : struct, Enum => EnumConverter<T>.NameToValue[name];

        /// <summary>
        /// Tries and gets an enum value from it's name
        /// </summary>
        /// <param name="name">Name of the value to get</param>
        /// <param name="value">Parsed enum value</param>
        /// <returns>True if the name was a valid enum value, false otherwise</returns>
        public static bool TryGetValue<T>(string name, out T value) where T : struct, Enum => EnumConverter<T>.NameToValue.TryGetValue(name, out value);

        /// <summary>
        /// Gets the enum value at the given index
        /// Order is not guaranteed from declaration, but order will stay constant relative to this and to the similar GetNameAt method
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="index">Index of the element to get</param>
        public static T GetValueAt<T>(int index) where T : struct, Enum => EnumConverter<T>.Values[index];

        /// <summary>
        /// Finds the string name of the enum value at the given index
        /// Order is not guaranteed from declaration, but order will stay constant relative to this and to the similar GetValueAt method
        /// </summary>
        /// <typeparam name="T">Type of the enum</typeparam>
        /// <param name="index">Index of the name to find</param>
        public static string GetNameAt<T>(int index) where T : struct, Enum => EnumConverter<T>.Names[index];

        /// <summary>
        /// Returns the index of the Enum member of the given value
        /// The values here are not guaranteed from the declaration, but are in the same order as the GetValueAt method
        /// </summary>
        /// <typeparam name="T">Type of the Enum</typeparam>
        /// <param name="value">Value to find the index of</param>
        public static int IndexOf<T>(T value) where T : struct, Enum => EnumConverter<T>.Values.IndexOf(value);

        /// <summary>
        /// Returns the index of the Enum value of the given name
        /// The values here are not guaranteed from the declaration, but are in the same order as the GetNameAt method
        /// </summary>
        /// <typeparam name="T">Type of the Enum</typeparam>
        /// <param name="name">Name of the element to find</param>
        public static int IndexOf<T>(string name) where T : struct, Enum => EnumConverter<T>.Names.IndexOf(name);
        #endregion
    }
}