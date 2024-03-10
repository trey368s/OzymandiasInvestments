#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 1433

ENV GPT_API_KEY=default_key_value

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OzymandiasInvestments/OzymandiasInvestments.csproj", "OzymandiasInvestments/"]
RUN dotnet restore "OzymandiasInvestments/OzymandiasInvestments.csproj"
COPY . .
WORKDIR "/src/OzymandiasInvestments"
RUN dotnet build "OzymandiasInvestments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OzymandiasInvestments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OzymandiasInvestments.dll"]
