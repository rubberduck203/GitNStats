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

### Mac

```bash
dotnet restore -r osx.10.12-x64
dotnet publish -c release -r -r osx.10.12-x64

# Make sure the resulting executable is executable
chmod +x rc/gitnstats/bin/Release/netcoreapp1.1/osx.10.12-x64/gitnstats
```

