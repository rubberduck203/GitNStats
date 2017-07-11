#!/usr/bin/env bash
set -e

framework=netcoreapp1.1
bin=src/gitnstats/bin/Release

echo "Cleaning ${bin}"
rm -rf ${bin}/**

## It would be smarter to grep the RuntimeIdentifiers to build this list
## grep '<RuntimeIdentifier>' src/gitnstats/gitnstats.csproj | sed -e 's,.*<RuntimeIdentifier>\([^<]*\)</RuntimeIdentifier>.*,\1,g'

runtimes=(osx.10.12-x64 ubuntu.14.04-x64 ubuntu.16.04-x64 win10-x64)

for runtime in ${runtimes[@]}; do
    echo "Restoring ${runtime}"
    dotnet restore -r ${runtime}
    
    echo "Packaging ${runtime}"
    dotnet publish -c release -r ${runtime}
    
    build=${bin}/${framework}/${runtime}
    
    if [[ ${runtime} != win* ]]; then
        exe=${build}/gitnstats
        echo "chmod +x ${exe}"
        chmod +x ${exe}
    fi
    
    archive=${build}.zip
    echo "Compressing to ${archive}"
    zip ${archive} ${build}/publish/
done
exit 0