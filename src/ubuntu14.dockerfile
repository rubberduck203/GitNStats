FROM ubuntu:14.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        \
# .NET Core dependencies
        libc6 \
        libcurl3 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu52 \
        liblttng-ust0 \
        libssl1.0.0 \
        libstdc++6 \
        libunwind8 \
        libuuid1 \
        zlib1g \
 # Install git for testing purposes
        git \
 # Clean up
    && rm -rf /var/lib/apt/lists/*
 WORKDIR /root/
 RUN git clone https://github.com/rubberduck203/GitNStats
 COPY gitnstats/bin/Release/net5.0/ubuntu.14.04-x64/publish/ /root/bin/
 CMD bin/gitnstats GitNStats/