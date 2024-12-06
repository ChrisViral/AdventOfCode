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
    /// <summary>
    /// The file <see cref="Version"/> for the given assembly
    /// </summary>
    /// <param name="assembly">Assembly to get the file Version for</param>
    /// <returns>The file <see cref="Version"/> for the given assembly</returns>
    public static Version GetFileVersion(this Assembly assembly)
    {
        return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion!);
    }
}