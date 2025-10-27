# 🎯 InitialThink 答疑系统使用文档

## 📖 项目概述

答疑系统（QAService）是 InitialThink 在线评测系统的智能辅助模块，为用户提供：
1. **做题前的渐进式提示** - 不直接给答案，引导思考
2. **做题后的智能解答** - 标准题解 + 个性化代码分析

---

## 🏗️ 系统架构

### 后端架构
```
QAService (Port: 5274)
├── Controllers
│   └── QAController.cs          # API 控制器
├── Services
│   └── AIService.cs              # AI 服务层（腾讯混元）
├── Models
│   ├── HintRequest.cs            # 提示请求模型
│   ├── HintResponse.cs           # 提示响应模型
│   ├── SolutionRequest.cs        # 题解请求模型
│   └── SolutionResponse.cs       # 题解响应模型
└── Program.cs                    # 启动配置
```

### 前端组件
```
app/components/QA/
├── HintPanel.vue                 # 提示面板
├── SolutionPanel.vue             # 解答面板
└── QAContainer.vue               # 容器组件

app/composables/
└── useQAApi.ts                   # API 调用封装

app/types/
└── QA.ts                         # TypeScript 类型定义
```

---

## 🚀 快速启动

### 1. 启动后端服务

```powershell
cd D:\SourceCode\CSharpProject\WebAPI\UserService\QAService
dotnet run
```

**访问地址：**
- API: http://localhost:5274
- Swagger 文档: http://localhost:5274/swagger

### 2. 配置 AI 密钥（必须）

编辑文件：`D:\SourceCode\CSharpProject\WebAPI\InitialThinkJudgeMachine\JudgeMachine\Judge\Keys.cs`

```csharp
public static class Keys
{
    public static readonly string HunYuanKey = "sk-YOUR_API_KEY_HERE";
}
```

获取 API Key：https://console.cloud.tencent.com/hunyuan/start

### 3. 前端集成

在题目页面中添加答疑组件：

```vue
<template>
  <div>
    <!-- 题目内容 -->
    <QuestionContainer :question="question" />
    
    <!-- 答疑系统 -->
    <QAContainer
      :question-id="question.id"
      :user-code="userCode"
      :language="selectedLanguage"
    />
  </div>
</template>

<script setup>
import QAContainer from '~/components/QA/QAContainer.vue'
</script>
```

---

## 📡 API 接口

### 1. 获取提示

**Endpoint:** `POST /api/qa/hint`

**请求体：**
```json
{
  "questionId": 1,
  "level": 1,
  "userCode": "可选的用户代码",
  "language": "C++"
}
```

**参数说明：**
- `questionId`: 题目ID（必填）
- `level`: 提示等级 1-3（必填）
  - 1: 轻度提示（大致方向）
  - 2: 中度提示（算法类型）
  - 3: 深度提示（详细思路+伪代码）
- `userCode`: 用户代码（可选，用于更精准的提示）
- `language`: 编程语言（可选）

**响应示例：**
```json
{
  "questionId": 1,
  "level": 1,
  "content": "提示内容...",
  "hasNextLevel": true,
  "generatedAt": "2025-10-27T12:00:00",
  "fromCache": false
}
```

### 2. 获取题解

**Endpoint:** `POST /api/qa/solution`

**请求体：**
```json
{
  "questionId": 1,
  "userCode": "用户的代码",
  "language": "C++"
}
```

**参数说明：**
- `questionId`: 题目ID（必填）
- `userCode`: 用户代码（可选，提供则分析用户代码）
- `language`: 编程语言（可选）

**响应示例：**
```json
{
  "questionId": 1,
  "standardSolution": {
    "approach": "解题思路...",
    "algorithm": "算法说明...",
    "timeComplexity": "O(n)",
    "spaceComplexity": "O(1)",
    "sampleCodes": {
      "C++": "代码示例..."
    },
    "keyPoints": ["关键点1", "关键点2"]
  },
  "userCodeAnalysis": {
    "overallReview": "整体评价",
    "correctness": "正确性分析",
    "timeComplexity": "O(n)",
    "spaceComplexity": "O(1)",
    "suggestions": ["优化建议1", "优化建议2"],
    "score": 85
  },
  "generatedAt": "2025-10-27T12:00:00"
}
```

### 3. 健康检查

**Endpoint:** `GET /api/qa/health`

**响应示例：**
```json
{
  "service": "QAService",
  "status": "healthy",
  "timestamp": "2025-10-27T12:00:00",
  "features": [
    "渐进式提示（3级）",
    "AI题解生成",
    "代码分析",
    "优化建议"
  ]
}
```

---

## 🎨 UI 设计特点

### 1. 提示面板（HintPanel）
- 🎨 **渐变紫色主题** - 代表智慧与思考
- 📊 **三级按钮** - 轻度、中度、深度依次解锁
- 💾 **缓存标识** - 显示是否来自缓存
- 🎯 **折叠面板** - 同时查看多级提示

### 2. 解答面板（SolutionPanel）
- 🎨 **渐变粉红主题** - 代表成就与收获
- 📈 **进度圆环** - 可视化代码评分
- 📋 **结构化展示** - 思路、算法、复杂度、代码
- 💡 **优化建议** - Alert 卡片形式，醒目实用

### 3. 响应式设计
- 📱 **移动端适配** - 全面支持移动设备
- 🌓 **深色模式** - 自动跟随系统主题
- ⚡ **加载动画** - 按钮 loading 状态

---

## 💡 AI Prompt 设计

### 提示生成策略

**Level 1 - 轻度提示：**
- 只提示大致方向
- 提及边界条件
- 3-5句话
- 不提具体算法

**Level 2 - 中度提示：**
- 提及数据结构类型
- 给出关键步骤
- 5-8句话
- 不给具体代码

**Level 3 - 深度提示：**
- 详细解题思路
- 伪代码或关键片段
- 复杂度分析
- 接近完整解法

### 题解生成格式

```markdown
# 解题思路
[详细思路说明]

# 算法说明
[算法和数据结构]

# 复杂度分析
时间复杂度：O(n)
空间复杂度：O(1)

# 关键点
- 关键点1
- 关键点2

# 示例代码
[完整代码]
```

### 代码分析格式

```markdown
# 整体评价
[30字评价]

# 正确性分析
[逻辑正确性]

# 复杂度分析
时间：O(n)
空间：O(1)

# 优化建议
- 建议1
- 建议2

# 评分
[0-100分]
```

---

## 🔧 配置选项

### appsettings.json

```json
{
  "AI": {
    "Model": "免费混元Lite",
    "MaxHintLevel": 3,
    "CacheEnabled": true
  }
}
```

**配置说明：**
- `Model`: AI 模型选择（参考 AI功能说明 文档）
- `MaxHintLevel`: 最大提示等级
- `CacheEnabled`: 是否启用缓存

---

## 🚨 常见问题

### 1. AI 调用失败

**可能原因：**
- API Key 未配置或无效
- 网络连接问题
- API 配额用尽

**解决方案：**
- 检查 `Keys.cs` 配置
- 查看控制台日志
- 使用降级提示（自动触发）

### 2. 提示内容不准确

**优化方法：**
- 调整 Prompt 模板（`AIService.cs`）
- 更换更强大的 AI 模型
- 提供用户代码以获得更精准提示

### 3. 题目数据加载失败

**检查：**
- `Questions.json` 文件路径
- 文件格式是否正确
- 控制台日志错误信息

---

## 📊 性能优化

### 1. 缓存策略
- ✅ 已实现内存缓存（ConcurrentDictionary）
- 🔄 可升级为 Redis 缓存
- ⏰ 可添加过期时间

### 2. 请求优化
- 🎯 按需加载（用户点击时才请求）
- 📦 响应数据精简
- ⚡ 异步处理

### 3. AI 调用优化
- 💰 使用免费模型（混元Lite）
- 📝 优化 Prompt 长度
- 🔄 实现降级方案

---

## 🎯 使用建议

### 对用户
1. **做题前先思考** - 不要立即查看提示
2. **渐进式使用** - 从 Level 1 开始
3. **提交后分析** - 查看题解和代码分析
4. **对比优化** - 学习更好的解法

### 对管理员
1. **监控 API 用量** - 避免超额
2. **定期清理缓存** - 释放内存
3. **收集用户反馈** - 改进提示质量
4. **优化 Prompt** - 提高准确度

---

## 📝 未来规划

### 短期（v1.1）
- [ ] Redis 缓存支持
- [ ] 提示质量评分
- [ ] 用户提示历史记录

### 中期（v1.2）
- [ ] 多语言代码示例
- [ ] 图解算法步骤
- [ ] 相似题目推荐

### 长期（v2.0）
- [ ] 自定义 AI 模型训练
- [ ] 社区贡献题解
- [ ] 视频讲解集成

---

## 📞 技术支持

- 项目地址：D:\SourceCode\CSharpProject\WebAPI\UserService\QAService
- API 文档：http://localhost:5274/swagger
- 相关文档：
  - `AI功能说明`：AI 模型配置
  - `QUICK_START.md`：快速开始
  - `API_CONTRACT.md`：API 约定

---

**祝您使用愉快！** 🎉

