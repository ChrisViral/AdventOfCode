using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Assemblies;

/// <summary>
/// Assembly extensions
/// </summary>
[PublicAPI]
public static class AssemblyExtensions
{
    extension(Assembly assembly)
    {
        /// <summary>
        /// The file <see cref="Version"/> for the given assembly
        /// </summary>
        /// <returns>The file <see cref="Version"/> for the given assembly</returns>
        public Version GetFileVersion => new(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion!);
    }
}
