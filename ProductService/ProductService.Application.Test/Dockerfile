FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ProductService/ProductService.Application.Test/ProductService.Application.Test.csproj ProductService/ProductService.Application.Test/
COPY ProductService/ProductService.Application/ProductService.Application.csproj ProductService/ProductService.Application/
COPY ProductService/ProductService.Domain/ProductService.Domain.csproj ProductService/ProductService.Domain/

RUN dotnet restore ProductService/ProductService.Application.Test/ProductService.Application.Test.csproj

COPY ProductService/ ProductService/

WORKDIR /src/ProductService/ProductService.Application.Test
RUN dotnet build ProductService.Application.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "ProductService.Application.Test.csproj", "-c", "Release", "--no-build", "--verbosity", "normal"]