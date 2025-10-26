FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ProductService/ProductService.Infrastructure.Test/ProductService.Infrastructure.Test.csproj ProductService/ProductService.Infrastructure.Test/
COPY ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj ProductService/ProductService.Infrastructure/
COPY ProductService/ProductService.Application/ProductService.Application.csproj ProductService/ProductService.Application/
COPY ProductService/ProductService.Domain/ProductService.Domain.csproj ProductService/ProductService.Domain/
RUN dotnet restore ProductService/ProductService.Infrastructure.Test/ProductService.Infrastructure.Test.csproj

COPY ProductService/ ProductService/

WORKDIR /src/ProductService/ProductService.Infrastructure.Test
RUN dotnet build ProductService.Infrastructure.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "ProductService.Infrastructure.Test.csproj", "-c", "Release", "--no-build", "--verbosity", "normal"]