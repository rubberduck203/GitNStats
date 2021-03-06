#!/usr/bin/env bash
set -e

dotnet test tests/gitnstats.test/gitnstats.test.csproj \
    /p:CollectCoverage=true \
    /p:Exclude=\"[gitnstats.core]*,[gitnstats.core.tests]*,[gitnstats]GitNStats.CliView,[gitnstats]GitNStats.Program,[gitnstats]GitNStats.Options,[gitnstats]GitNStats.FileSystem\" \
    /p:CoverletOutput=./bin/ \
    /p:CoverletOutputFormat=\"json,opencover,lcov\"

dotnet test tests/gitnstats.core.tests/gitnstats.core.tests.csproj \
    /p:CollectCoverage=true \
    /p:CoverletOutput=./bin/ \
    /p:CoverletOutputFormat=\"json,opencover,lcov\"