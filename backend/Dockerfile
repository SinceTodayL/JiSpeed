# ------------------
# 第一阶段：构建 (Build)
# ------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 将所有项目文件复制到 /src 目录
# 这样可以利用 Docker 缓存，如果项目文件没有改变，就不需要重新还原依赖
COPY ["JISpeed.Api/JISpeed.Api.csproj", "JISpeed.Api/"]
# 如果有多个项目文件，可以按需添加，例如:
# COPY ["JISpeed.Shared/JISpeed.Shared.csproj", "JISpeed.Shared/"]

# 还原项目依赖
RUN dotnet restore "JISpeed.Api/JISpeed.Api.csproj"

# 将所有源文件复制到工作目录
COPY . .

# 更改工作目录到 API 项目
WORKDIR /src/JISpeed.Api

# 发布项目，生成最终的可执行文件和依赖
RUN dotnet publish -c Release -o /app --no-restore

# ------------------
# 第二阶段：发布 (Publish)
# ------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 从构建阶段复制发布的文件到最终镜像
COPY --from=build /app/publish .

# 设置环境变量以确保应用程序监听所有网络接口的 80 端口
# ENV ASPNETCORE_URLS=http://+:80

# 暴露 8080 和 8081 端口，避免和 nginx 80 端口冲突
EXPOSE 8080
EXPOSE 8081

# 定义容器启动时要执行的命令
# "JISpeed.Api.dll" 应该是你项目的 DLL 文件名
ENTRYPOINT ["dotnet", "JISpeed.Api.dll"]