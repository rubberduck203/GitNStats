#!/usr/bin/env bash
set -e

framework=netcoreapp1.1
bin=src/gitnstats/bin/Release

echo "Cleaning ${bin}"
rm -rf ${bin}/**

# build the list of runtimes by parsing the *.csproj for runtime identifiers
runtimes=($(grep '<RuntimeIdentifier>' src/gitnstats/gitnstats.csproj | sed -e 's,.*<RuntimeIdentifier>\([^<]*\)</RuntimeIdentifier>.*,\1,g'))

for runtime in ${runtimes[@]}; do
    echo "Restoring ${runtime}"
    dotnet restore -r ${runtime}
    
    echo "Packaging ${runtime}"
    dotnet publish -c release -r ${runtime}
    
    build=${bin}/${framework}/${runtime}
    publish=${build}/publish
    
    if [[ ${runtime} != win* ]]; then
        exe=${publish}/gitnstats
        echo "chmod +x ${exe}"
        chmod +x ${exe}
    fi
    
    archive=${build}.zip
    echo "Compressing to ${archive}"
    zip ${archive} ${publish}/**
done
exit 0