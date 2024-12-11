# Advent of Code
I'll be uploading my solutions to the Advent of Code challenges in this repository. The solutions try to be as much as possible using only the standard libraries from each language I'm using, but if absolutely needed I'll include an external library but give a reference to it.

My solutions are meant to work while being as clear as possible. Speed, efficiency, and algorithmic complexity are not my first concern, as long as it doesn't make it stupidly slow. Some answers might be naive ways to deal with certain problems, but they'd also be the simpler way in this case, and this is what I'm going for. This challenge isn't really the place to push the limits of performance really.

## Advent of Code Automation Compliance
This repo [fetches inputs automatically](CSharp/InputFetcher.cs) from the Advent of Code website, and does follow the [automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation) on the [/r/adventofcode](https://www.reddit.com/r/adventofcode/) community wiki.

- Outbound calls are [throttled to every 900 seconds (15 minutes)](CSharp/InputFetcher.cs#L119-L132) by storing the last call timestamp locally to preserve throttling between runs
- Once inputs are downloaded, they are [cached locally](CSharp/InputFetcher.cs#L58-L78) and reused as needed
- The [User-Agent header is set to me](CSharp/InputFetcher.cs#L141-L144), the maintainer of this repo
