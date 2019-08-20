FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /publish
COPY . /publish
EXPOSE 6000
ENTRYPOINT ["dotnet", "Yu.Api.dll"]