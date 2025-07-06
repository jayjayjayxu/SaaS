# 使用微软官方的.NET SDK镜像作为构建环境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
# 复制.csproj文件并恢复依赖项
# 这种分层复制可以利用Docker的缓存机制，如果代码没变但依赖变了，可以只重新下载依赖
COPY ["CollaborativeOffice.Gateway/CollaborativeOffice.Gateway.csproj", "CollaborativeOffice.Gateway/"]
COPY ["CollaborativeOffice.IdentityService/CollaborativeOffice.IdentityService.csproj", "CollaborativeOffice.IdentityService/"]
COPY ["CollaborativeOffice.ProjectService/CollaborativeOffice.ProjectService.csproj", "CollaborativeOffice.ProjectService/"]
COPY ["CollaborativeOffice.Domain/CollaborativeOffice.Domain.csproj", "CollaborativeOffice.Domain/"]
RUN dotnet restore "CollaborativeOffice.ProjectService/CollaborativeOffice.ProjectService.csproj"

# 复制所有项目的源代码
COPY . .
WORKDIR "/src/CollaborativeOffice.ProjectService"
# 构建发布版本
RUN dotnet build "CollaborativeOffice.ProjectService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollaborativeOffice.ProjectService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用更轻量的ASP.NET运行时镜像作为最终的生产环境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollaborativeOffice.ProjectService.dll"]
