FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ["OrderService/OrderService.API.Test/OrderService.API.Test.csproj", "OrderService/OrderService.API.Test/"]
COPY ["OrderService/OrderService.API/OrderService.API.csproj", "OrderService/OrderService.API/"]
COPY ["OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService/OrderService.Application/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService/OrderService.Domain/"]
COPY ["OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService/OrderService.Infrastructure/"]
RUN dotnet restore "OrderService/OrderService.API.Test/OrderService.API.Test.csproj"

COPY OrderService/ OrderService/

WORKDIR /src/OrderService/OrderService.API.Test
RUN dotnet build OrderService.API.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "OrderService.API.Test.csproj", "-c", "Release", "--no-build", "--logger:console;verbosity=normal"]