# GitNStats

Command line utility that reports back statistics about a git repository.

There are many tools out there, but most of them are interested in commits per author and other such things.
GitNStats is an experiment in analyzing how often particular files in a repositor change (and how big those changes were).
The idea is to discover "hotspots" in a code base by determining which files have a lot of churn.

For usage instruction run:

```bash
dotnet run -- --help

# or if you have a pre-packaged binary

gitnstats --help
```

## Installation

Unzip the distribution into your target directory.
The program can be run from this location, added to your PATH, 
or symbolic linked to a location that is already on your PATH.

Symoblic linking to a location already on the PATH (like `/usr/local/bin/`) is recommended as it keeps your path clean.

```bash
# Download release (replace version and runtime accordingly)
cd ~/Downloads
wget https://github.com/rubberduck203/GitNStats/releases/download/1.0.3/osx.10.12-x64.zip

# Create directory to keep package
mkdir -p ~/bin/gitnstats

# unzip
unzip osx.10.12-x64.zip -d ~/bin/gitnstats

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
dotnet test tests/gitnstats.tests/gitnstats.tests.csproj
```

## Publish

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