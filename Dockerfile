# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia arquivos do projeto e restaura pacotes
COPY *.sln ./
COPY WebApplication1/*.csproj WebApplication1/
RUN dotnet restore

# Copia o restante e faz build
COPY . .
WORKDIR /src/WebApplication1
RUN dotnet publish -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Define as portas para o Kestrel (5033 e 80)
ENV ASPNETCORE_URLS="http://+:80"

# Expõe as portas
EXPOSE 80

ENTRYPOINT ["dotnet", "WebApplication1.dll"]
