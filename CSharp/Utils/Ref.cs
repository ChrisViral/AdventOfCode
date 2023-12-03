namespace AdventOfCode.Utils;

public class Ref<T> where T : struct
{
    public T Value { get; set; }

    public Ref(T value) => this.Value = value;

    public static implicit operator T(Ref<T> value) => value.Value;

    public override string ToString() => this.Value.ToString()!;
}