# From base image for build (with sdk)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS src
WORKDIR /src
# Copy src to image
COPY /src .

# Run building the project with packets restoring
RUN dotnet build Minibank.Web -c Release -r linux-x64 --self-contained
#Run tests
RUN dotnet test Tests/Minibank.Core.Tests --no-build
# Publish projects' dll to /dist directory
RUN dotnet publish Minibank.Web -c Release -r linux-x64 --self-contained --no-build -o /dist

# From base image with runtime only
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Set timezone
ENV TZ=Europe/Moscow

# Copy from src image to final image
COPY --from=src /dist .
# Declare enviroment variables
ENV ASPNETCORE_URLS=http://+:5001;http://+:5000
ENV AUTHENTICATION_AUTHORITY=https://demo.duendesoftware.com
ENV ISSUER_TOKEN=https://demo.duendesoftware.com/connect/token
EXPOSE 5000 5001
# Start the application
ENTRYPOINT ["dotnet", "Minibank.Web.dll"]
