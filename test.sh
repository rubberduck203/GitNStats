#! /bin/bash

dotnet test tests/gitnstats.test/gitnstats.test.csproj \
    /p:CollectCoverage=true \
    /p:Exclude=\"[gitnstats.core]*,[gitnstats.core.tests]*\" \
    /p:CoverletOutput=./bin/ \
    /p:CoverletOutputFormat=\"json,opencover,lcov\"

dotnet test tests/gitnstats.core.tests/gitnstats.core.tests.csproj \
    /p:CollectCoverage=true \
    /p:CoverletOutput=./bin/ \
    /p:CoverletOutputFormat=\"json,opencover,lcov\"