FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ["OrderService/OrderService.Infrastructure.Test/OrderService.Infrastructure.Test.csproj", "OrderService/OrderService.Infrastructure.Test/"]
COPY ["OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService/OrderService.Infrastructure/"]
COPY ["OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService/OrderService.Application/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService/OrderService.Domain/"]
RUN dotnet restore "OrderService/OrderService.Infrastructure.Test/OrderService.Infrastructure.Test.csproj"

COPY OrderService/ OrderService/

WORKDIR /src/OrderService/OrderService.Infrastructure.Test
RUN dotnet build OrderService.Infrastructure.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "OrderService.Infrastructure.Test.csproj", "-c", "Release", "--no-build", "--logger:console;verbosity=normal"]