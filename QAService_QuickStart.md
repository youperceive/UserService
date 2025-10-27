# ⚡ QAService 快速开始

## 🎯 30秒启动答疑系统

### 第1步：启动服务

```powershell
cd D:\SourceCode\CSharpProject\WebAPI\UserService\QAService
dotnet run
```

✅ 看到以下输出即成功：
```
=========================================
   InitialThink QA Service
=========================================
  环境: Development
  端口: http://localhost:5274
  Swagger: http://localhost:5274/swagger
=========================================
```

### 第2步：测试 API

浏览器访问：**http://localhost:5274/swagger**

### 第3步：前端集成

在题目页面中引入答疑组件：

```vue
<QAContainer
  :question-id="1"
  :user-code="code"
  :language="'C++'"
/>
```

---

## 📋 完整服务列表

| 服务 | 端口 | 用途 |
|------|------|------|
| UserService | 5170 | 用户管理 |
| ProblemSetService | 5272 | 题目和判题 |
| **QAService** | **5274** | **答疑系统** ⭐ |

---

## 🔑 AI 配置（重要）

编辑文件：
```
D:\SourceCode\CSharpProject\WebAPI\InitialThinkJudgeMachine\JudgeMachine\Judge\Keys.cs
```

替换 API Key：
```csharp
public static readonly string HunYuanKey = "sk-你的密钥";
```

**获取密钥：** https://console.cloud.tencent.com/hunyuan/start

> 💡 **提示**：不配置 AI 密钥也能运行，会使用降级提示

---

## 💻 快速测试

### 使用 PowerShell 测试 API

**1. 健康检查：**
```powershell
curl http://localhost:5274/api/qa/health
```

**2. 获取提示：**
```powershell
$body = @{
    questionId = 1
    level = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5274/api/qa/hint" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

**3. 获取题解：**
```powershell
$body = @{
    questionId = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5274/api/qa/solution" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

---

## 🎨 前端效果预览

### 提示面板
- 🎨 紫色渐变主题
- 3个等级按钮（轻度→中度→深度）
- 折叠面板展示多级提示

### 解答面板
- 🎨 粉色渐变主题  
- 标准题解（思路、算法、代码）
- 代码分析（评分、建议）

---

## ⚠️ 常见问题

**Q: 端口被占用？**
```powershell
# 修改 Properties/launchSettings.json 中的端口
```

**Q: AI 返回错误？**
```
检查：
1. Keys.cs 中的 API Key 是否正确
2. 网络连接是否正常
3. 控制台日志错误信息
```

**Q: 题目数据找不到？**
```
确保 Questions.json 在相对路径：
../ProblemSetService/Data/Questions.json
```

---

## 📚 更多文档

- 📖 完整文档：`QAService使用文档.md`
- 🔧 配置指南：`appsettings.json`
- 🚀 API 文档：http://localhost:5274/swagger

---

**开始使用答疑系统，让学习更高效！** 🎉

