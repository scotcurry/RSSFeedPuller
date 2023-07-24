FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN apt-get update
RUN apt-get install -y curl

WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=datadog/serverless-init /datadog-init /App/datadog-init

ENV DD_SERVICE=rssfeedpuller
ENV DD_ENV=prod
ENV DD_VERSION=3.724.611

# Download and install the Tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

ENTRYPOINT ["/App/datadog-init"]
CMD ["dotnet", "RSSFeedPuller.dll"]
