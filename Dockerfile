ARG LL_REGISTRY

FROM ${LL_REGISTRY}/ti/container/images/netcore8/x-build:stable as build
ARG ARTIFACTSTOKENNUGET
WORKDIR /src
COPY src .
WORKDIR /src/CustomerManagementApi.Api
RUN dotnet publish CustomerManagementApi.Api.csproj -c Release -o /app/publish

FROM ${LL_REGISTRY}/ti/container/images/netcore8/x-runtime/debian:stable
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_HTTP_PORTS=8080 
EXPOSE 8080
EXPOSE 443
ENTRYPOINT ["dotnet", "CustomerManagementApi.Api.dll"]