# Dockerfile for ServerManager
#
# Builds a container for running ServerManager.
#

# Build in a separate image.
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
RUN git clone https://github.com/hol430/server-manager /ServerManager
WORKDIR /ServerManager
RUN dotnet publish --nologo -c Release -r linux-x64 /ServerManager/src/ServerManager.csproj

# Main image from apsimng-server
FROM apsiminitiative/apsimng-server:latest
COPY --from=build /ServerManager/src/bin/Release/net6.0/linux-x64/publish /opt/server-manager/
ENV PATH $PATH:/opt/server-manager
ENTRYPOINT ["server-manager"]

