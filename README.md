# Advent of Code
My solutions to the Advent of Code challenges are found in this repository. The solutions try to keep as much as possible to the standard libraries of C#, but a few helper libraries are used for convenience, as well as the unfortunate occasional Z3.
The rest of the code found in here which isn't the solvers are my completely self-made personal libraries purely for the purpose of doing these challenges, from vectors, priority queues, extensions, etc.

My solutions are meant to work while being as clear as possible. Speed, efficiency, and algorithmic complexity are not my first concern, as long as it's not stupidly slow. Some answers might be naive ways to deal with certain problems, but they'd also be the simpler way in this case, and this is what I'm going for. Sometimes I do optimize down some solutions when I have a goo idea, but at the end of the day im mostly trying to have fun with these, so the actual thing I optimize from day to day may vary.

## Advent of Code Automation Compliance
This repo [fetches inputs automatically](AdventOfCode.CLI/InputFetcher.cs) from the Advent of Code website, and does follow the [automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation) on the [/r/adventofcode](https://www.reddit.com/r/adventofcode/) community wiki.

- Outbound calls are [throttled to every 900 seconds (15 minutes)](AdventOfCode.CLI/InputFetcher.cs#L119-L125) by storing the last call timestamp locally to preserve throttling between runs
- Once inputs are downloaded, they are [cached locally](AdventOfCode.CLI/InputFetcher.cs#L51-L71) and reused as needed
- The [User-Agent header is set to me](AdventOfCode.CLI/InputFetcher.cs#L134-L137), the maintainer of this repo
