# InitialThink 后端启动指南

## 📋 项目结构

本解决方案包含以下服务：
- **UserService**: 用户服务 (端口: 5170/7084)
- **ProblemSetService**: 题集和判题服务 (端口: 5272/7206)
- **JudgeMachine**: 判题机库
- **ModelLibrary**: 数据模型库

## 🔧 前置要求

- ✅ .NET 9.0 SDK (已安装)
- ✅ Docker Desktop (已安装，但需启动)
- 数据库：PostgreSQL 和 Redis（通过 Docker 运行）

## 🚀 快速启动

### 方式一：使用启动脚本（推荐）

1. **启动 Docker Desktop**
   - 在开始菜单找到 Docker Desktop 并启动
   - 等待 Docker 完全启动（托盘图标不再转圈）

2. **运行启动脚本**
   ```powershell
   cd D:\SourceCode\CSharpProject\WebAPI\UserService
   .\启动服务.ps1
   ```

3. 脚本会自动：
   - 检查 Docker 状态
   - 启动 PostgreSQL 和 Redis
   - 提供服务启动命令

### 方式二：手动启动

1. **启动 Docker Desktop**

2. **启动数据库服务**
   ```powershell
   cd D:\SourceCode\CSharpProject\WebAPI\UserService
   docker-compose up -d
   ```

3. **启动 ProblemSetService（题目和判题服务）**
   ```powershell
   cd ProblemSetService
   dotnet run --launch-profile http
   ```
   访问: http://localhost:5272/swagger

4. **启动 UserService（用户服务）** - 在新的终端窗口
   ```powershell
   cd UserService
   dotnet run --launch-profile http
   ```
   访问: http://localhost:5170/swagger

## 📊 服务端口说明

| 服务 | HTTP | HTTPS | 说明 |
|------|------|-------|------|
| ProblemSetService | 5272 | 7206 | 题目、判题 |
| UserService | 5170 | 7084 | 用户管理 |
| PostgreSQL | 5432 | - | 主数据库 |
| Redis | 6379 | - | 缓存 |

## 🗄️ 数据库配置

**PostgreSQL**
- Host: localhost
- Port: 5432
- Database: db_it
- Username: postgres
- Password: 123456

**Redis**
- Host: localhost
- Port: 6379

## 🤖 AI 功能配置（可选）

如需使用 AI 代码分析功能，需要配置腾讯混元 API：

1. 访问 https://console.cloud.tencent.com/hunyuan/start 注册获取 API Key
2. 编辑文件：`D:\SourceCode\CSharpProject\WebAPI\InitialThinkJudgeMachine\JudgeMachine\Judge\Keys.cs`
3. 替换 `HunYuanKey` 的值

## 🛠️ 常用命令

**查看 Docker 容器状态**
```powershell
docker ps
```

**停止数据库服务**
```powershell
docker-compose down
```

**重启数据库服务**
```powershell
docker-compose restart
```

**查看数据库日志**
```powershell
docker-compose logs -f postgres
docker-compose logs -f redis
```

**清理并重建项目**
```powershell
dotnet clean
dotnet build
```

## 📝 前后端对接

前端项目位于: `D:\SourceCode\WebProject\VueProject\InitialThinkFrontend`

需要确保前端 API 配置指向正确的后端地址：
- UserService: http://localhost:5170
- ProblemSetService: http://localhost:5272

## ❗ 常见问题

**问题1: Docker 启动失败**
- 确保 Docker Desktop 已完全启动
- 检查 Hyper-V 或 WSL2 是否启用

**问题2: 端口被占用**
- 修改 `launchSettings.json` 中的端口配置
- 或者停止占用端口的程序

**问题3: 数据库连接失败**
- 检查 Docker 容器是否运行：`docker ps`
- 查看容器日志：`docker-compose logs`

**问题4: 项目构建失败**
- 清理项目：`dotnet clean`
- 恢复依赖：`dotnet restore`
- 重新构建：`dotnet build`

## 📚 相关文档

- `API_CONTRACT.md`: API 接口文档
- `QUICK_START.md`: 快速开始指南
- `JUDGE_INTEGRATION_GUIDE.md`: 判题系统集成指南

