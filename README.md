# GitNStats

Command line utility that reports back statistics about a git repository.

There are many tools out there, but most of them are interested in commits per author and other such things.
GitNStats is an experiment in analyzing how often particular files in a repositor change (and how big those changes were).
The idea is to discover "hotspots" in a code base by determining which files have a lot of churn.

## Publish

### Mac

```bash
dotnet restore -r osx.10.12-x64
dotnet publish -c release -r -r osx.10.12-x64

# Make sure the resulting executable is executable
chmod +x rc/gitnstats/bin/Release/netcoreapp1.1/osx.10.12-x64/gitnstats
```
