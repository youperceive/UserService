# Question API 已完成 ✅

## 完成的工作

### 1. 后端API (ProblemSetService)

已创建以下文件：

- ✅ `Models/Question.cs` - 题目数据模型
- ✅ `Controllers/QuestionController.cs` - 题目API控制器
- ✅ `Data/Questions.json` - 题目数据（从前端复制）
- ✅ `ProblemSetService.csproj` - 配置文件（已添加Questions.json复制规则）

### 2. 前端修改

- ✅ `useApi.ts` - 修复了判题机路径重复问题（`/api/api/Judge/run` → `/api/Judge/run`）

---

## API端点

### 1. 获取题目详情
```
GET /api/question/{id}
```

**示例**:
```bash
curl http://localhost:5000/api/question/1
```

**返回**:
```json
{
  "id": 1,
  "title": "两数之和",
  "descriptionH5": "<p>给定两个整数 a 和 b，请输出它们的和。</p>",
  "inputFormatH5": "<p>输入两个整数。</p>",
  "outputFormatH5": "<p>输出它们的和。</p>",
  "noticeH5": "<p>时间限制：1秒</p>",
  "tags": [
    {"name": "简单", "color": "...", "backgroundColor": "..."},
    {"name": "加法", "color": "...", "backgroundColor": "..."}
  ],
  "examples": [
    {"input": "1 1", "output": "2", "explanation": "1+1=2"}
  ],
  "enabledLanguages": ["C++", "Python3", "Java", "JavaScript", "C#"],
  "allowTemplate": true
}
```

### 2. 获取题目列表（支持筛选）
```
GET /api/question/list
```

**参数**:
- `difficulty` (可选): 难度筛选，支持多选（逗号分隔）。值: `easy`, `normal`, `hard`
- `tags` (可选): 标签筛选，支持多选（逗号分隔）
- `keyword` (可选): 关键词搜索（标题）
- `minId` (可选): 最小ID
- `maxId` (可选): 最大ID
- `page` (可选): 页码，默认1
- `pageSize` (可选): 每页数量，默认20

**示例**:
```bash
# 获取简单题目
curl "http://localhost:5000/api/question/list?difficulty=easy"

# 获取中等和困难题目
curl "http://localhost:5000/api/question/list?difficulty=normal,hard"

# 搜索包含"链表"的题目
curl "http://localhost:5000/api/question/list?tags=链表"

# ID范围筛选
curl "http://localhost:5000/api/question/list?minId=1&maxId=10"

# 分页
curl "http://localhost:5000/api/question/list?page=2&pageSize=10"
```

**返回**:
```json
{
  "total": 150,
  "page": 1,
  "pageSize": 20,
  "items": [
    {
      "id": 1,
      "title": "两数之和",
      "difficulty": "简单",
      "tags": ["加法", "基础数学"],
      "acceptedCount": 523,
      "submissionCount": 1234,
      "acceptanceRate": 42.37
    }
  ]
}
```

### 3. 搜索题目
```
GET /api/question/search?keyword=链表
```

**返回**:
```json
[
  {"id": 2, "title": "反转链表"},
  {"id": 15, "title": "链表排序"}
]
```

---

## 启动服务

### 1. 编译并运行

```bash
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet build
dotnet run
```

**预期输出**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
[QuestionController] Loaded 150 questions
```

### 2. 测试Swagger

访问: `http://localhost:5000/swagger`

找到 `Question` 分组，测试各个端点。

---

## 前端使用

前端的 `useApi.ts` 已经有 `question.getQuestionDetail(id)` 方法，现在会自动调用后端API。

```typescript
// 在Question页面中
const response = await api.question.getQuestionDetail(questionID)
```

**如果后端未启动**，前端会自动回退到mock数据，并显示警告。

---

## 数据流

### 之前（Mock数据）
```
前端页面 → TestValue.ts → Questions.json (前端) → 直接显示
```

### 现在（后端API）
```
前端页面 → useApi.ts → GET /api/question/{id} → QuestionController → Questions.json (后端) → 返回数据
```

**回退机制**: 如果API失败，自动使用前端mock数据。

---

## 测试清单

- [ ] 启动ProblemSetService（5000端口）
- [ ] 访问 `http://localhost:5000/api/question/1`，确认返回JSON
- [ ] 在Swagger测试 `/api/question/list`
- [ ] 启动前端，访问题目页面（如 `/Question/1`）
- [ ] 确认题目正常显示，无mock数据警告
- [ ] 测试题目列表页 `/QuestionGit`，确认筛选功能正常

---

## 常见问题

### 1. 404 Not Found

**问题**: `GET /api/question/1 404`

**原因**: 
- ProblemSetService未启动
- 端口不是5000

**解决**: 
```bash
cd C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService
dotnet run
```

### 2. Questions.json not found

**错误**: `[QuestionController] Error loading questions: Could not find file`

**原因**: Questions.json未复制到输出目录

**解决**:
1. 确认 `Data/Questions.json` 存在
2. 确认 `.csproj` 文件已添加 `<CopyToOutputDirectory>` 配置
3. 重新编译: `dotnet build`

### 3. 前端仍显示Mock数据

**问题**: 前端显示"后端API未就绪，使用Mock数据"

**原因**:
- 后端未启动在5000端口
- CORS未配置

**解决**:
1. 确认后端运行在 `http://localhost:5000`
2. 检查 `Program.cs` 中的CORS配置

### 4. 判题机路径错误

**问题**: `POST /api/api/Judge/run 404`

**原因**: 路径重复了 `/api`

**解决**: 已修复，`useApi.ts` 中 `JUDGE_BASE_URL` 现在正确指向 `http://localhost:5000`

---

## 下一步

- [ ] **集成判题机** - 按照 `JUDGE_INTEGRATION_GUIDE.md` 和 `MIGRATION_CHECKLIST.md`
- [ ] **测试运行代码** - 在题目页面测试"运行"和"提交"按钮
- [ ] **数据库集成** - 将Questions.json数据导入数据库
- [ ] **用户提交记录** - 保存用户的代码提交和评测结果

---

## 文件位置

**后端**:
- `C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService\Controllers\QuestionController.cs`
- `C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService\Models\Question.cs`
- `C:\Users\Uiharukazari\WebstormProjects\UserService\ProblemSetService\Data\Questions.json`

**前端**:
- `C:\Users\Uiharukazari\WebstormProjects\InitialThinkFrontend\app\composables\useApi.ts`

**文档**:
- `JUDGE_INTEGRATION_GUIDE.md` - 判题机集成详细指南
- `MIGRATION_CHECKLIST.md` - 判题机迁移清单
- `QUESTION_API_READY.md` - 本文档

---

✅ **Question API已完成！现在可以启动后端测试了。**

