FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
COPY --from=datadog/serverless-init /datadog-init /App/datadog-init

ENV DD_SERVICE=curryware-rssfeedpuller
ENV DD_ENV=prod
ENV DD_VERSION=3.717.1237

ENTRYPOINT ["/App/datadog-init"]
CMD ["dotnet", "RSSFeedPuller.dll"]
# ENTRYPOINT ["dotnet", "RSSFeedPuller.dll"]
