FROM mcr.microsoft.com/dotnet/sdk:8.0 AS testrunner

WORKDIR /src

COPY ["OrderService/OrderService.Application.Test/OrderService.Application.Test.csproj", "OrderService/OrderService.Application.Test/"]
COPY ["OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService/OrderService.Application/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService/OrderService.Domain/"]
RUN dotnet restore "OrderService/OrderService.Application.Test/OrderService.Application.Test.csproj"

COPY OrderService/ OrderService/

WORKDIR /src/OrderService/OrderService.Application.Test
RUN dotnet build OrderService.Application.Test.csproj -c Release --no-restore

CMD ["dotnet", "test", "OrderService.Application.Test.csproj", "-c", "Release", "--no-build", "--logger:console;verbosity=normal"]