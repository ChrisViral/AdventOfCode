using AdventOfCode.Resolver;

using AdventOfCodeSetup solverSetup = new();
if (!await solverSetup.TrySetup()) return 1;

return await solverSetup.RunProgram(args);
