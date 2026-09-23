using AdventOfCode;

using AdventOfCodeSetup setup = new();
if (!await setup.TrySetup()) return 1;

return await setup.RunProgram(args);
