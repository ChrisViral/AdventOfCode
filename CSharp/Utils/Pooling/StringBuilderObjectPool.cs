using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// StringBuilder object pool
/// </summary>
[PublicAPI]
public class StringBuilderObjectPool : DefaultObjectPool<StringBuilder>
{
    /// <inheritdoc />
    public StringBuilderObjectPool(StringBuilderPooledObjectPolicy policy) : base(policy) { }

    /// <inheritdoc />
    public StringBuilderObjectPool(StringBuilderPooledObjectPolicy policy, int maximumRetained) : base(policy, maximumRetained) { }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static StringBuilderObjectPool Shared { get; } = new(new StringBuilderPooledObjectPolicy());
}
