#!/usr/bin/env bash
set -e

project=src/gitnstats/gitnstats.csproj
framework=netcoreapp2.0
bin=src/gitnstats/bin/Release

echo "Cleaning ${bin}"
rm -rf ${bin}/**

# build the list of runtimes by parsing the *.csproj for runtime identifiers
runtimes=($(grep '<RuntimeIdentifier>' $project | sed -e 's,.*<RuntimeIdentifier>\([^<]*\)</RuntimeIdentifier>.*,\1,g'))

for runtime in ${runtimes[@]}; do
    echo "Packaging ${runtime}"
    dotnet publish -c release -r ${runtime} /p:ShowLinkerSizeComparison=true $project
    
    build=${bin}/${framework}/${runtime}
    publish=${build}/publish
    
    if [[ ${runtime} != win* ]]; then
        exe=${publish}/gitnstats
        echo "chmod +x ${exe}"
        chmod +x ${exe}
    fi
    
    # subshell so we can specify the archive's root directory
    (
        cd ${publish}
        archive=../../${runtime}.zip
        echo "Compressing to ${archive}"
        zip -r ${archive} ./
    )
done
exit 0
