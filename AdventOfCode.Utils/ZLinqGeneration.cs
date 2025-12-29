using ZLinq;

[assembly: ZLinqDropIn("", DropInGenerateTypes.Collection, GenerateAsPublic = true)]
[assembly: ZLinqDropInExternalExtension("AdventOfCode.Utils.Extensions.Ranges", "System.Range", "AdventOfCode.Utils.ValueEnumerators.FromRange", GenerateAsPublic = true)]
[assembly: ZLinqDropInExternalExtension("AdventOfCode.Utils.Extensions.Spans", "CommunityToolkit.HighPerformance.ReadOnlySpan2D`1", "AdventOfCode.Utils.ValueEnumerators.FromSpan2D`1", GenerateAsPublic = true)]
[assembly: ZLinqDropInExternalExtension("AdventOfCode.Utils.Extensions.Spans", "CommunityToolkit.HighPerformance.Span2D`1", "AdventOfCode.Utils.ValueEnumerators.FromSpan2D`1", GenerateAsPublic = true)]
