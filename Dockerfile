# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files and build the application
COPY . ./
RUN dotnet publish -c Release -o /out

# Use the official ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose the port the application listens on
EXPOSE 8080
# Ensure the application listens on port 7777
ENV ASPNETCORE_URLS=http://+:8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "StrategoGameServer.dll"]
