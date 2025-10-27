# 🚀 快速启动指南

## 方式一：使用测试脚本（推荐）

```powershell
# 在 UserService 目录下执行
.\test-judge-integration.ps1
```

这个脚本会：
1. ✅ 检查 Docker 状态
2. ✅ 构建项目
3. ✅ 启动后端服务
4. ✅ 自动测试评测接口
5. ✅ 显示测试结果

---

## 方式二：手动启动

### 1. 启动后端
```powershell
cd ProblemSetService
dotnet run
```

等待显示：
```
Now listening on: http://localhost:5170
```

### 2. 访问 Swagger 测试
打开浏览器：http://localhost:5170/swagger

### 3. 启动前端
```powershell
# 新开一个终端
cd C:\Users\Uiharukazari\WebstormProjects\InitialThinkFrontend
npm run dev
```

访问：http://localhost:3000

---

## 🎯 测试评测功能

### 在 Swagger 中测试

1. 访问 http://localhost:5170/swagger
2. 找到 `POST /api/Judge/run`
3. 点击 "Try it out"
4. 输入：

```json
{
  "code": "#include <iostream>\nusing namespace std;\nint main() {\n    int a, b;\n    cin >> a >> b;\n    cout << a + b << endl;\n    return 0;\n}",
  "lang": "C++",
  "input": "1 2",
  "timeout": "5"
}
```

5. 点击 "Execute"
6. 查看响应

**预期结果**：
```json
{
  "standardOutput": "3\n",
  "errorOutput": "",
  "exitCode": 0
}
```

---

## ⚠️ 注意事项

### Docker 必须运行
- 确保 Docker Desktop 正在运行
- 首次运行会下载 Docker 镜像（可能需要几分钟）

### 端口占用
- 后端使用 5170 端口
- 前端使用 3000 端口
- 确保这些端口没有被占用

### 构建错误
如果遇到编译错误，先构建 JudgeMachine：
```powershell
cd C:\Users\Uiharukazari\Desktop\InitialThinkJudgeMachine\JudgeMachine
dotnet build
```

---

## 📚 更多文档

- `INTEGRATION_COMPLETE.md` - 完整集成说明
- `JUDGE_INTEGRATION_GUIDE.md` - 集成指南
- `MIGRATION_PLAN.md` - 迁移计划

---

## 🐛 遇到问题？

1. 检查 Docker 是否运行
2. 查看后端日志输出
3. 访问 Swagger UI 确认 API 可用
4. 查看浏览器 Console（F12）
5. 参考 `INTEGRATION_COMPLETE.md` 中的常见问题部分

