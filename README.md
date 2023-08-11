# Overview

## Structure

```
│   # Solution Folder
|   # RSSFeedPuller
|       # Project Folder.  Simple .NET Framework API template was used to build project.
|--     RSSFeedPuller
|       # A command line tool that updates the version information in the Dockerfile
|--     VersionUpdater
```

## Important files in the project

### Dockerfile - There are some key requirements to get the Datadog tracer into the container.

```
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0.20
RUN apt-get update
RUN apt-get install -y curl

WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=datadog/serverless-init /datadog-init /App/datadog-init

ENV DD_SERVICE=rssfeedpuller
ENV DD_ENV=prod
ENV DD_VERSION=3.724.994

# Download and install the Tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

ENTRYPOINT ["/App/datadog-init"]
CMD ["dotnet", "RSSFeedPuller.dll"]
```

### az container build - This moves the file to the Azure Container Registry

```
# This builds the image and stores it in the Azure Container Registry
az acr build --image datadogcurryware/rssfeedpuller:v3.724.994 \
  --registry curryware.azurecr.io \
  --file Dockerfile .
```

### az container up -- This "launches" the container in the Azure environment.

```
# This starts the container using the image from above.
az containerapp up \
  --name curryware-rssfeedpuller \
  --resource-group Curryware_Azure_Resources \
  --ingress external \
  --target-port 80 \
  --env-vars "DD_API_KEY=$DD_API_KEY" "DD_TRACE_ENABLED=true" "DD_SITE=datadoghq.com" "LOG_PATH=/var/log" \
    "DD_LOGS_ENABLED=true" "DD_APM_ENABLED=true" "CORECLR_ENABLE_PROFILING=1" \
    "CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}" \
    "CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so" \
    "DD_DOTNET_TRACER_HOME=/opt/datadog" "DD_LOGS_INJECTION=true" "DD_RUNTIME_METRICS_ENABLED=true" \
    "DD_ENV=prod" "DD_VERION=3.724.994" "DD_SERVICE=rssfeedpuller" "DD_RUNTIME_SECURITY_CONFIG_ENABLED=true" \
  --image curryware.azurecr.io/datadogcurryware/rssfeedpuller:v3.724.994
  ```