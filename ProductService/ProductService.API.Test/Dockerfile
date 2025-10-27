FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ProductService/ProductService.API.Test/ProductService.API.Test.csproj ProductService/ProductService.API.Test/
COPY ProductService/ProductService.API/ProductService.API.csproj ProductService/ProductService.API/
COPY ProductService/ProductService.Application/ProductService.Application.csproj ProductService/ProductService.Application/
COPY ProductService/ProductService.Domain/ProductService.Domain.csproj ProductService/ProductService.Domain/
COPY ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj ProductService/ProductService.Infrastructure/

RUN dotnet restore ProductService/ProductService.API.Test/ProductService.API.Test.csproj

COPY ProductService/ ProductService/

WORKDIR /src/ProductService/ProductService.API.Test
RUN dotnet build ProductService.API.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "ProductService.API.Test.csproj", "-c", "Release", "--no-build", "--logger:console;verbosity=normal"]