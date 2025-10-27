# 判题机迁移清单 ✅

完成以下步骤，将判题机从独立服务迁移到主后端。

---

## ☑️ 步骤 1: 添加引用 (5分钟)

```bash
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService

# 1.1 添加JudgeMachine项目引用
dotnet add reference ..\..\Desktop\InitialThinkJudgeMachine\JudgeMachine\JudgeMachine.csproj

# 1.2 添加Docker.DotNet包
dotnet add package Docker.DotNet --version 3.125.15
```

**验证**: 执行 `dotnet build`，应该成功编译（可能有警告，忽略即可）

---

## ☑️ 步骤 2: 创建JudgeController (10分钟)

在 `ProblemSetService/Controllers/` 创建 `JudgeController.cs`

**完整代码**: 见 `JUDGE_INTEGRATION_GUIDE.md` 中的"步骤2"

**关键点**:
- 命名空间: `namespace ProblemSetService.Controllers;`
- 路由: `[Route("api/[controller]")]`
- 包含两个端点: `POST /api/Judge/run` 和 `POST /api/Judge/runBatch`

**验证**: 文件创建成功，无语法错误

---

## ☑️ 步骤 3: 配置CORS (3分钟)

编辑 `ProblemSetService/Program.cs`

### 3.1 添加CORS服务
在 `var builder = WebApplication.CreateBuilder(args);` 之后添加：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            if (origin.Contains("localhost") || origin.Contains("127.0.0.1"))
                return true;
            
            var uri = new Uri(origin);
            var host = uri.Host;
            if (host.StartsWith("192.168.") || host.StartsWith("10."))
                return true;
                
            return false;
        })
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

### 3.2 启用CORS中间件
在 `var app = builder.Build();` 之后，`app.UseAuthorization();` 之前添加：

```csharp
app.UseCors("AllowFrontend");
```

### 3.3 配置JSON命名策略
找到 `builder.Services.AddControllers()`，修改为：

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

**验证**: 编译通过

---

## ☑️ 步骤 4: 前端配置 (已完成 ✅)

前端已自动修改：
- `useApi.ts` 中 `JUDGE_BASE_URL` 现在指向主后端
- API路径从 `/Judge/run` 改为 `/api/Judge/run`

**无需手动操作**

---

## ☑️ 步骤 5: 测试 (10分钟)

### 5.1 启动Docker Desktop
确保Docker正在运行

### 5.2 停止旧的判题机服务
如果之前的 `InitialThinkJudgeMachine/Api` 还在运行，关闭它

### 5.3 启动后端
```bash
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet run
```

**预期输出**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 5.4 测试Swagger
浏览器访问: `http://localhost:5000/swagger`

找到 `Judge` 分组，测试 `POST /api/Judge/run`：

**测试数据**:
```json
{
  "code": "#include <iostream>\nusing namespace std;\nint main() {\n    int a, b;\n    cin >> a >> b;\n    cout << a + b << endl;\n    return 0;\n}",
  "lang": "C++",
  "input": "1 2",
  "timeout": "5"
}
```

**预期结果**:
```json
{
  "standardOutput": "3\n",
  "errorOutput": "",
  "exitCode": 0,
  ...
}
```

### 5.5 启动前端并测试
```bash
cd C:\Users\Uiharukazari\WebstormProjects\InitialThinkFrontend
npm run dev
```

- 打开题目页面
- 编写代码
- 点击"运行"按钮
- 查看结果（应该在ResultDisplay的测试用例tab显示）

**验证**: 
- [ ] 能看到正确的输出
- [ ] 浏览器控制台无CORS错误
- [ ] 后端控制台有请求日志

---

## ☑️ 步骤 6: 清理（可选）

迁移成功后：

1. 停止并删除独立判题机API:
   - 关闭 `InitialThinkJudgeMachine/Api` 项目
   - 从解决方案中移除（保留JudgeMachine核心库）

2. 更新文档和配置，移除对独立判题机的引用

---

## 🔧 故障排查

### 问题1: 编译失败
```
错误: 找不到项目引用
```
**解决**: 先编译JudgeMachine，再编译ProblemSetService

```bash
cd C:\Users\Uiharukazari\Desktop\InitialThinkJudgeMachine\JudgeMachine
dotnet build
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet build
```

### 问题2: Docker连接失败
```
错误: Cannot connect to Docker daemon
```
**解决**: 
1. 打开Docker Desktop
2. 确认在系统托盘有Docker图标
3. 运行 `docker ps` 验证

### 问题3: 404错误
```
错误: POST http://localhost:5000/Judge/run 404
```
**原因**: 路径不对，应该是 `/api/Judge/run`

**解决**: 前端已自动修改，清除浏览器缓存重试

### 问题4: CORS错误
```
错误: Access-Control-Allow-Origin
```
**解决**: 确认Program.cs中已添加CORS配置（步骤3）

---

## ✅ 完成检查

- [ ] ProblemSetService可以成功编译
- [ ] Docker Desktop正在运行
- [ ] 后端启动在 `http://localhost:5000`
- [ ] Swagger测试通过
- [ ] 前端可以成功运行代码
- [ ] 无CORS错误
- [ ] 输出结果正确

**全部完成？恭喜！🎉 判题机迁移成功！**

---

## 📝 备注

- 原始判题机保存在 `InitialThinkJudgeMachine/` 作为备份
- 详细说明见 `JUDGE_INTEGRATION_GUIDE.md`
- 完整迁移计划见 `MIGRATION_PLAN.md`（在InitialThinkJudgeMachine目录）

