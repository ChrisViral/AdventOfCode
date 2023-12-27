namespace AdventOfCode.Utils;

public class Ref<T>(T value) where T : struct
{
    public T Value { get; set; } = value;

    public static implicit operator T(Ref<T> value) => value.Value;

    public override string ToString() => this.Value.ToString()!;
}