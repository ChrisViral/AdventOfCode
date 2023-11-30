using System;
using System.Linq;

namespace AdventOfCode.Extensions;

public static class TypeExtensions
{
    public static bool IsImplementationOf(this Type type, Type interfaceType)
    {
        return type.GetInterfaces()
                   .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
    }
}