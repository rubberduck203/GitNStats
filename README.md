# GitNStats

Command line utility that reports back statistics about a git repository.

There are many tools out there, but most of them are interested in commits per author and other such things.
GitNStats is an experiment in analyzing how often particular files in a repositor change (and how big those changes were).
The idea is to discover "hotspots" in a code base by determining which files have a lot of churn.

[![Build status](https://ci.appveyor.com/api/projects/status/5ncbrrsob8t44bc5/branch/master?svg=true)](https://ci.appveyor.com/project/Rubberduck/gitnstats/branch/master)
[![codecov](https://codecov.io/gh/rubberduck203/GitNStats/branch/master/graph/badge.svg)](https://codecov.io/gh/rubberduck203/GitNStats)

## Quickstart

```bash
cd path/to/repo
gitnstats
```

For usage instruction run:

```bash
 dotnet run --project src/gitnstats/gitnstats.csproj -- --help

# or if you have a pre-packaged binary

gitnstats --help
```

### Philosophy

GitNStats tries to follow [the Unix philosophy](https://en.wikipedia.org/wiki/Unix_philosophy).

 - Make each program do one thing well.
 - Expect the output of every program to become the input to another, as yet unknown, program.

This means I've tried to stay light on the options and other bells and whistles.

### Options

#### Date Filter

The following command will return all a count of the number of times each file was committed on or after July 14th, 2017 on the develop branch.

```bash
gitnstats path/to/repo -b develop -d 2017-07-14
```

#### QuietMode

By default, GitNStats will print repository and branch information along with the history analysis.
However, this can be awkward when piping output into another program, like `sort` for further processing.
This extra info and column headers can be suppressed with the `-q` switch.

```bash
gitnstats -q | sort
```

### Common Operations

I've tried to provide instructions for common use cases below, but as tools differ from OS to OS,
I've not tested that they work everywhere.
If something doesn't work on your OS of choice, please open a pull request.
I don't have a Windows machine at the moment, so equivalent batch and powershell command would be appreciated.

#### Saving to file

Should work on basically every OS I know of, including Windows.

```bash
gitnstats > filename.txt
```

#### Sorting

Output is sorted ascending by default.

Descending:

```bash
gitnstats -q | sort -n
```

Ascending without headers & branch info:

```bash
gitnstats -q | sort -nr
```

#### Display Top N Files

Display the 10 most changed files:

```bash
gitnstats -q | sort -nr | head -n10
```

Display the 20 least changed files:

```bash
gitnstats -q | sort -nr | tail -n20
```

#### Display Only Files with More than N Commits

You can use awk to filter results.
The following command will print only records where the first column (the number of commits)
is greater than or equal to 15.

```bash
gitnstats -q | awk '$1 >= 15'
```

#### Filtering

Display files with a specific file extension:

```bash
gitnstats | grep \\.cs$
```

`$` indicates end of line.
The `.` must be escaped with a `\`, but since the shell uses the `\` character as a line continuation, we must escape it as well.

Filter on a directory:

```bash
gitnstats | grep tests/
```

## Installation

Obtain the [latest release](https://github.com/rubberduck203/GitNStats/releases/latest).

Unzip the distribution into your target directory.
The program can be run from this location, added to your PATH, 
or symbolic linked to a location that is already on your PATH.

Symoblic linking to a location already on the PATH (like `/usr/local/bin/`) is recommended as it keeps your path clean.

```bash
# Download release (replace version and runtime accordingly)
cd ~/Downloads
wget https://github.com/rubberduck203/GitNStats/releases/download/2.3.0/osx-x64.zip

# Create directory to keep package
mkdir -p ~/bin/gitnstats

# unzip
unzip osx-x64.zip -d ~/bin/gitnstats

# Create symlink
ln -s /Users/rubberduck/bin/gitnstats/gitnstats /usr/local/bin/gitnstats
```

Alternatively, you may want to keep the executable in the `/usr/local/share/` directory.

### .Net Dependencies

This project uses "self-contained" .Net Core deployment.
"Self contained" is quoted because although the *.zip archive includes the .Net runtime,
the .Net runtime has dependencies of it's own that need to be available.
Please see the [list of .Net Core runtime dependencies.][dotnet-deps] and make sure they're installed first. 

[dotnet-deps]: https://github.com/dotnet/core/blob/master/Documentation/prereqs.md

## Build

```bash
dotnet restore
dotnet build
```

## Tests

If you're using VS Code, there's a test task.
Cmd + P -> `> Tasks: Run Test Task`

Otherwise...

```bash
dotnet test
```

### Code Coverage

If you use the `./test.sh` or `test w/ coverage` Task Runner in VSCode, you can generate lcov reports and use the [lcov extension](https://marketplace.visualstudio.com/items?itemName=alexdima.vscode-lcov) to view the code coverage.

[Coverage Gutters](https://marketplace.visualstudio.com/items?itemName=ryanluker.vscode-coverage-gutters) will work too, but you need to tell it to look for the right file name.

## Publish

To maintain compatibility with our build server, we use 7zip to create the archive.

```bash
brew install p7zip
```

The publish script will package and zip a stand alone executable for each runtime specified in the *.csproj.

```bash
./publish.sh
```

### Integration Tests

The integration tests have two purposes.

1. Verify the self-contained publish works properly for an OS.
2. Document the .Net runtime dependencies for that OS.

If the tests is successful, you'll see output from the application and a get successful return code of 0.
All of the dockerfiles assume you have already run the `publish.sh` script. 

#### Ubuntu 14.04

```bash
docker build -f src/ubuntu14.dockerfile -t rubberduck/gitnstats:ubuntu14 src
docker run rubberduck/gitnstats:ubuntu14
```

#### Ubuntu 16.04

```bash
docker build -f src/ubuntu16.dockerfile -t rubberduck/gitnstats:ubuntu16 src
docker run rubberduck/gitnstats:ubuntu16
```