# Advent of Code
My solutions to the Advent of Code challenges are found in this repository. The solutions try to keep as much as possible to the standard libraries of C#, but a few helper libraries are used for convenience, as well as the unfortunate occasional Z3.
Also used here are my completely self-made personal [libraries](https://github.com/ChrisViral/ChallengeLibraries) purely for the purpose of doing these challenges, from vectors, priority queues, extensions, etc.

My solutions are meant to work while being as clear as possible. Speed, efficiency, and algorithmic complexity are not my first concern, as long as it's not stupidly slow. Some answers might be naive ways to deal with certain problems, but they'd also be the simpler way in this case, and this is what I'm going for. Sometimes I do optimise down some solutions when I have a good idea, but at the end of the day I'm mostly trying to have fun with these, so the actual thing I optimise from day to day may vary.

## Advent of Code Automation Compliance
This repo [fetches inputs automatically](AdventOfCode/SolverResolver.cs) from the Advent of Code website, and does follow the [automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation) on the [/r/adventofcode](https://www.reddit.com/r/adventofcode/) community wiki.

- Outbound calls are [throttled to every 900 seconds (15 minutes)](AdventOfCode/SolverResolver.cs#L127-L132) by storing the last call timestamp locally to preserve throttling between runs
- Once inputs are downloaded, they are [cached locally](AdventOfCode/SolverResolver.cs#L62-88) and reused as needed
- The [User-Agent header is set to me](AdventOfCode/SolverResolver.cs#L141-144), the maintainer of this repo
