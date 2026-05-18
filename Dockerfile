# 1. Force the official Microsoft .NET 10.0 SDK container to compile the app
FROM ://microsoft.com AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code and compile
COPY . .
RUN dotnet publish -c Release -o /app/out

# 2. Use the matching .NET 10.0 runtime image to execute the app online
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Open port 8080 (the default cloud web port)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "backend.dll"]
