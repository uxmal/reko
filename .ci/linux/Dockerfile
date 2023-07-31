FROM mcr.microsoft.com/dotnet/sdk:6.0

RUN apt-get update && \
    apt-get install -y build-essential gcc g++ cmake libcapstone-dev pkg-config p7zip-full \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && dotnet nuget locals all --clear
    # reserved for cache eviction: 1
