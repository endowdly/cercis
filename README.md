# Cercis

This is a lame .NET 6.0 ripoff of [solidiquis/erdtree](https://github.com/solidiquis/erdtree).

A bLazInGlY sLoW, skinnier version of the ancient [tree](https://en.wikipedia.org/wiki/Tree_(command)) command which displays a colorful depth indented listing of files
with their memory sizes adjacent.

![Cercis Screenshot](assets/cercis-screenshot.png)

## Usage

```text
Usage:
    cercis [directory] [options]

ARGUMENTS:
    directory     Directory to traverse. Defaults to current working directory.

OPTIONS:
    -l            Unsigned integer indicating many nested directory levels to display. Defaults to all.
    -p            Comma-separated list of prefixes. Directories containing any of
                  these prefixes will not be traversed. Their memory size will also be ignored.
    -s [asc|desc] Sort tree by memory-size. 
    -h            Displays help prompt.
```

## Installation

There is none yet.
This is lame, remember?

## Build

Snag a dotnet SDK, version 6.0.23 or higher.
Clone this repo and run `dotnet build`.
It has no dependencies, so, it should be quick.
The debug executable will run natively on Windows.

Use `dotnet publish -r <your runtime> --self-contained` if you are on another system.

See the [Runtime Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) for more information on your runtimes.

## Disambiguation about units for memory

As recommended in [IEC 80000-13](https://en.wikipedia.org/wiki/ISO/IEC_80000#cite_note-80000-13:2008-14), this utility will report memory sizes
using SI units rather than binary units. As such you can expect `1KB = 1000B` and not `1KiB = 1024B`.

(Good call solidiquis).

## Questions you might have

_Q: You ripped off erdtree!_

It's not a question.
But, yes I did.
As such, I included the original MIT license instead of my usual preference Unlicense.
If you fork this repo, be sure to do the same and credit the original coder.
This is not enough of a derivative, in my opinion.

_Q: What the hell is a Cercis anyway?_

Erdtree is an homage to Elden Ring.
[_Cercis_](https://en.wikipedia.org/wiki/Cercis) is a genus of trees (in real life) whose flowers erupt all over.
To me, their appearance is similar to the Erdtree, albeit tiny, non-luminous versions.

_Q: Cercis hangs when I call it on some folders?_

A: Yeah I ported this over in like 2 hours and this hasn't been tested much.
I am aware of unhandled FileAccess issue that may cause a hang and I'm "tracking" down _which_, but it's not a priority.
On very large folders (1 GB or larger), you'll see it takes bunch of time for it to output.

_Q: Why did you make this? It's totally, TOTALLY unnecessary._

Mostly because I couldn't build erdtree with cargo or rustup in two attempts.
Instead of trying to track down the issue with my tooling (or with erdtree's implementation), I decided to just bang it out in C#.
Also, I was bored.

_Q: Is it any good?_

Haha, no.
But, it does work!
And you don't need Rust.
Though you might want to get Rust.
Or I may want to rewrite this in C.

_Q: How do you know that this is blazingly slow?_

It's not written in Rust.
