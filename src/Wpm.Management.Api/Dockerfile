# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 as base
USER app
WORKDIR /app
# Expose the port the app runs on
EXPOSE 8080
EXPOSE 8081 

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wpm.Management.Api/Wpm.Management.Api.csproj", "Wpm.Management.Api/"]
RUN dotnet restore "Wpm.Management.Api/Wpm.Management.Api.csproj"
# Copy the project files and restore any dependencies
COPY . .
WORKDIR "/src/Wpm.Management.Api"
RUN dotnet build "./Wpm.Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wpm.Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wpm.Management.Api.dll"]