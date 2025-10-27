# 评测机集成完成指南

## ✅ 已完成的工作

### 1. 后端集成评测机
- ✅ 在 `ProblemSetService.csproj` 中添加了 `JudgeMachine` 项目引用
- ✅ 添加了 `Docker.DotNet` 依赖包（版本 3.125.15）
- ✅ 创建了 `JudgeController.cs` 控制器，提供两个API：
  - `/api/Judge/run` - 运行单个测试用例
  - `/api/Judge/runBatch` - 批量运行多个测试用例
- ✅ 更新了 `Program.cs`：
  - 配置了 JSON camelCase 命名策略
  - 增强了 CORS 配置（支持 localhost 和局域网）

### 2. 前端更新
- ✅ 更新了 `useApi.ts`：
  - 将评测请求从直接调用评测机（5248端口）改为通过后端（5170端口）
  - 评测URL: `http://localhost:5170/api/Judge/run`

---

## 🚀 如何启动和测试

### 步骤 1: 确保 Docker Desktop 正在运行
```powershell
# 打开 Docker Desktop 应用
# 确认 Docker 状态为 "Running"
```

### 步骤 2: 构建并启动后端服务
```powershell
# 在 PowerShell 中执行
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet build
dotnet run
```

**预期输出**：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5170
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7170
```

### 步骤 3: 测试评测API (使用 Swagger)
1. 打开浏览器访问: `http://localhost:5170/swagger`
2. 找到 `Judge` 控制器
3. 展开 `POST /api/Judge/run`
4. 点击 "Try it out"
5. 输入测试数据：

```json
{
  "code": "#include <iostream>\nusing namespace std;\nint main() {\n    int a, b;\n    cin >> a >> b;\n    cout << a + b << endl;\n    return 0;\n}",
  "lang": "C++",
  "input": "1 2",
  "timeout": "5"
}
```

6. 点击 "Execute"

**预期响应**：
```json
{
  "standardOutput": "3\n",
  "errorOutput": "",
  "exitCode": 0,
  "executionTimeMs": 0,
  "memoryUsageKb": 0
}
```

### 步骤 4: 启动前端并测试
```powershell
# 在新的 PowerShell 窗口中
cd C:\Users\Uiharukazari\WebstormProjects\InitialThinkFrontend
npm run dev
```

**预期输出**：
```
  ➜  Local:   http://localhost:3000/
```

### 步骤 5: 在前端测试评测功能
1. 打开浏览器访问: `http://localhost:3000`
2. 选择任意题目进入
3. 在代码编辑器中写入代码
4. 点击 "运行" 按钮
5. 查看运行结果

---

## 🔍 调试方法

### 查看后端日志
后端会输出详细的日志信息：
```
[QuestionController] Loaded 150 questions
收到代码评测请求: Language=C++, CodeLength=123
开始执行代码: Language=C++, Timeout=5s
代码执行完成: ExitCode=0, OutputLength=2
```

### 查看前端日志
打开浏览器开发者工具 (F12)，在 Console 中查看：
```
🚀 调用评测接口: http://localhost:5170/api { lang: 'C++', ... }
🔧 评测URL: http://localhost:5170/api/Judge/run
📦 请求体: { code: '#include <iostream>...', lang: 'C++', ... }
📡 评测响应状态: 200 true
✅ 评测返回: { standardOutput: '3\n', ... }
```

---

## 📊 架构对比

### 旧架构（已废弃）
```
前端 (3000)  ──独立调用──>  评测机API (5248)
     └──────────────────>  后端API (5170)
```

### 新架构（当前）
```
前端 (3000)  ───────────>  后端API (5170)
                                 │
                                 └──>  JudgeMachine (库)
                                        └──>  Docker
```

**优势**：
- ✅ 统一入口，便于管理和监控
- ✅ 可以在后端添加权限控制
- ✅ 可以记录评测历史到数据库
- ✅ 便于实现评测队列

---

## 🛠️ 支持的语言

JudgeController 当前支持以下语言：
- C++
- C#
- Java
- Python3
- JavaScript
- TypeScript
- GoLang
- C
- Pascal

---

## ⚠️ 常见问题

### 1. 编译错误：找不到 JudgeMachine
**解决**：先编译 JudgeMachine 项目
```powershell
cd C:\Users\Uiharukazari\Desktop\InitialThinkJudgeMachine\JudgeMachine
dotnet build
```

### 2. Docker 连接失败
**错误**：`Cannot connect to Docker daemon`

**解决**：
1. 确认 Docker Desktop 正在运行
2. 检查 Docker 设置 - 确保启用了 "Expose daemon on tcp://localhost:2375"（如果使用 TCP）

### 3. CORS 错误
**错误**：`Access to fetch at 'http://localhost:5170' from origin 'http://localhost:3000' has been blocked by CORS policy`

**解决**：已在 `Program.cs` 中配置，如果仍有问题，重启后端服务

### 4. 404 Not Found
**错误**：`POST http://localhost:5170/api/Judge/run 404`

**检查**：
- 后端是否正在运行
- 访问 `http://localhost:5170/swagger` 确认 API 是否可用

---

## 📝 API 文档

### POST /api/Judge/run
运行单个测试用例

**请求体**：
```json
{
  "code": "代码内容",
  "lang": "语言名称 (C++, Python3, Java, etc.)",
  "input": "标准输入",
  "timeout": "超时时间（秒，字符串格式）"
}
```

**响应**：
```json
{
  "standardOutput": "标准输出",
  "errorOutput": "错误输出",
  "exitCode": 0,
  "executionTimeMs": 0,
  "memoryUsageKb": 0
}
```

### POST /api/Judge/runBatch
批量运行测试用例

**请求体**：
```json
{
  "code": "代码内容",
  "language": "语言名称",
  "testCases": [
    {
      "input": "测试输入1",
      "expectedOutput": "期望输出1",
      "ignoreWhitespace": true
    },
    {
      "input": "测试输入2",
      "expectedOutput": "期望输出2",
      "ignoreWhitespace": true
    }
  ],
  "timeout": 5,
  "stopOnFirstFailure": false
}
```

**响应**：
```json
{
  "totalCases": 2,
  "passedCases": 2,
  "results": [
    {
      "input": "测试输入1",
      "expectedOutput": "期望输出1",
      "actualOutput": "实际输出1",
      "errorOutput": "",
      "executionTimeMs": 0,
      "memoryUsageKb": 0,
      "exitCode": 0,
      "isAccepted": true
    }
  ]
}
```

---

## 🎯 下一步建议

1. **实现提交历史记录**
   - 在数据库中保存每次提交的结果
   - 创建 `SubmissionController` 处理提交相关逻辑

2. **添加评测队列**
   - 使用 Hangfire 或 RabbitMQ 实现异步评测
   - 避免长时间阻塞 HTTP 请求

3. **完善题目测试用例管理**
   - 在数据库中存储题目的测试用例
   - 提供测试用例的增删改查 API

4. **添加权限控制**
   - 实现用户认证和授权
   - 限制评测频率（防止滥用）

---

## 📞 联系信息

如有问题，请查看以下文档：
- `JUDGE_INTEGRATION_GUIDE.md` - 详细集成指南
- `MIGRATION_PLAN.md` - 迁移方案说明
- 项目 Issues 或联系开发团队

