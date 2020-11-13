#!/usr/bin/env bash
set -e

framework=net5.0
project_root=src/gitnstats
project_path=${project_root}/gitnstats.csproj
bin=${project_root}/bin/Release

echo "Cleaning ${bin}"
rm -rf ${bin}/**

# build the list of runtimes by parsing the *.csproj for runtime identifiers
IFS=';' read -ra runtimes <<< "$(grep '<RuntimeIdentifiers>' ${project_path} | sed -e 's,.*<RuntimeIdentifiers>\([^<]*\)</RuntimeIdentifiers>.*,\1,g')"

for runtime in ${runtimes[@]}; do
    echo "Restoring ${runtime}"
    dotnet restore -r ${runtime} ${project_path}
    
    echo "Packaging ${runtime}"
    dotnet publish -c release -r ${runtime} -p:PublishSingleFile=true ${project_path}
    
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
        7z a ${archive} ./
    )
done
exit 0