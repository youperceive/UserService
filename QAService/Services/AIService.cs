using JudgeMachine.Judge;
using QAService.Models;
using System.Collections.Concurrent;
using System.Text;

namespace QAService.Services;

/// <summary>
/// AI 服务 - 负责生成提示和题解
/// </summary>
public class AIService
{
    // 简单的内存缓存
    private static readonly ConcurrentDictionary<string, HintResponse> _hintCache = new();
    private static readonly ConcurrentDictionary<int, SolutionResponse> _solutionCache = new();
    private readonly IConfiguration _configuration;
    private readonly KnowledgeGraphService _kgService;
    
    public AIService(IConfiguration configuration, KnowledgeGraphService kgService)
    {
        _configuration = configuration;
        _kgService = kgService;
    }
    
    /// <summary>
    /// 生成渐进式提示
    /// </summary>
    public async Task<HintResponse> GenerateHintAsync(int questionId, string questionTitle, string questionDesc, int level, string? userCode = null, string? language = null, bool forceRefresh = false)
    {
        var cacheKey = $"{questionId}_{level}";
        
        // 检查缓存（除非强制刷新）
        if (!forceRefresh && _hintCache.TryGetValue(cacheKey, out var cachedHint))
        {
            Console.WriteLine($"[AIService] 返回缓存提示: QuestionId={questionId}, Level={level}");
            cachedHint.FromCache = true;
            return cachedHint;
        }
        
        if (forceRefresh)
        {
            Console.WriteLine($"[AIService] 强制刷新，重新生成提示: QuestionId={questionId}, Level={level}");
        }
        else
        {
            Console.WriteLine($"[AIService] 首次生成提示: QuestionId={questionId}, Level={level}");
        }
        
        var prompt = BuildHintPrompt(questionTitle, questionDesc, level, userCode, language);
        
        try
        {
            var aiModel = Model.免费混元Lite;
            var aiResponse = await ArtificialIntelligence.AskAsync(aiModel, prompt);
            
            var hint = new HintResponse
            {
                QuestionId = questionId,
                Level = level,
                Content = aiResponse,
                HasNextLevel = level < 3,
                GeneratedAt = DateTime.Now,
                FromCache = false
            };
            
            // 更新缓存（强制刷新时直接覆盖，否则添加）
            _hintCache.AddOrUpdate(cacheKey, hint, (key, oldValue) => hint);
            Console.WriteLine($"[AIService] 提示已生成并缓存: QuestionId={questionId}, Level={level}, FromCache=false");
            
            return hint;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] Error generating hint: {ex.Message}");
            return new HintResponse
            {
                QuestionId = questionId,
                Level = level,
                Content = GetFallbackHint(level),
                HasNextLevel = level < 3,
                GeneratedAt = DateTime.Now,
                FromCache = false
            };
        }
    }
    
    /// <summary>
    /// 生成题解和代码分析
    /// </summary>
    public async Task<SolutionResponse> GenerateSolutionAsync(int questionId, string questionTitle, string questionDesc, string? userCode = null, string? language = null)
    {
        try
        {
            var aiModel = Model.免费混元Lite;
            
            // 生成标准题解
            var solutionPrompt = BuildSolutionPrompt(questionTitle, questionDesc, language);
            var solutionText = await ArtificialIntelligence.AskAsync(aiModel, solutionPrompt);
            var standardSolution = ParseStandardSolution(solutionText);
            
            CodeAnalysis? codeAnalysis = null;
            
            // 如果提供了用户代码，进行分析
            if (!string.IsNullOrEmpty(userCode))
            {
                var analysisPrompt = BuildCodeAnalysisPrompt(questionTitle, questionDesc, userCode, language ?? "C++");
                var analysisText = await ArtificialIntelligence.AskAsync(aiModel, analysisPrompt);
                codeAnalysis = ParseCodeAnalysis(analysisText);
            }
            
            return new SolutionResponse
            {
                QuestionId = questionId,
                StandardSolution = standardSolution,
                UserCodeAnalysis = codeAnalysis,
                GeneratedAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] Error generating solution: {ex.Message}");
            return new SolutionResponse
            {
                QuestionId = questionId,
                StandardSolution = GetFallbackSolution(questionTitle),
                UserCodeAnalysis = null,
                GeneratedAt = DateTime.Now
            };
        }
    }
    
    /// <summary>
    /// 构建提示的 Prompt
    /// </summary>
    private string BuildHintPrompt(string title, string desc, int level, string? userCode, string? language)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"题目：{title}");
        stringBuilder.AppendLine($"描述：{desc}");
        stringBuilder.AppendLine();
        
        var lang = language ?? "C++";
        
        // 🌟 知识图谱增强：提取题目涉及的知识点
        var knowledgeTags = _kgService.ExtractKnowledgeTags(title, desc);
        if (knowledgeTags.Any())
        {
            stringBuilder.AppendLine("【知识图谱分析】");
            stringBuilder.AppendLine($"这道题涉及的知识点：{string.Join("、", knowledgeTags)}");
            
            // 对于 Level 1 和 2，提供前置知识提示
            if (level <= 2)
            {
                var prerequisites = _kgService.GetPrerequisites(knowledgeTags);
                if (prerequisites.Any())
                {
                    stringBuilder.AppendLine($"需要掌握的前置知识：{string.Join("、", prerequisites)}");
                }
            }
            
            stringBuilder.AppendLine("请基于这些知识点给出提示。");
            stringBuilder.AppendLine();
        }
        
        switch (level)
        {
            case 1: // 轻度提示 - 只给方向，绝对不能有代码
                stringBuilder.AppendLine("【Level 1 - 轻度提示】只给思路方向，帮助理解题目。");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("严格要求：");
                stringBuilder.AppendLine("1. 只用白话说思路方向，像朋友间聊天");
                stringBuilder.AppendLine("2. 绝对禁止：任何代码、伪代码、代码片段、代码示例");
                stringBuilder.AppendLine("3. 绝对禁止：专业术语（如动态规划、哈希表、栈、队列、DFS、BFS等）");
                stringBuilder.AppendLine("4. 绝对禁止：具体的实现步骤");
                stringBuilder.AppendLine("5. 只能做：用通俗语言提示思考方向、提醒容易忽略的点");
                stringBuilder.AppendLine("6. 长度：2-3句话");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("示例好的回答：");
                stringBuilder.AppendLine("「这题关键是要记住之前算过的结果，避免重复计算。注意考虑一下输入为空的情况。」");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("示例错误回答（包含代码）：");
                stringBuilder.AppendLine("「可以这样写：int result = a + b; - 这是错的，不要模仿！」");
                break;
                
            case 2: // 中度提示 - 可以提方法，但不给代码
                stringBuilder.AppendLine("【Level 2 - 中度提示】可以说方法和步骤，但不给代码。");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("允许做的：");
                stringBuilder.AppendLine("1. 说明具体的解题思路和方法");
                stringBuilder.AppendLine("2. 可以提到适合的数据结构类型（如「需要用能快速查找的结构」）");
                stringBuilder.AppendLine("3. 用自然语言描述算法步骤（如「先遍历一遍，再从后往前处理」）");
                stringBuilder.AppendLine("4. 提到算法复杂度的大致要求");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("严格禁止：");
                stringBuilder.AppendLine("1. 任何代码、伪代码、代码片段");
                stringBuilder.AppendLine("2. 具体的API调用（如「使用map.get()」「调用Arrays.sort()」这些）");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("长度：4-6句话，分步骤说明");
                break;
                
            case 3: // 深度提示 - 可以给接近完整的方案
                stringBuilder.AppendLine("【Level 3 - 深度提示】给出详细方案，可以包含关键代码片段。");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("要求：");
                stringBuilder.AppendLine("1. 完整的解题思路和算法说明");
                stringBuilder.AppendLine("2. 可以给出关键代码片段或算法伪代码");
                stringBuilder.AppendLine($"3. 如果给代码，用{lang}语言，确保代码格式正确，用```包裹");
                stringBuilder.AppendLine("4. 说明时间和空间复杂度");
                stringBuilder.AppendLine("5. 接近完整解法，但留一些细节让用户自己实现");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("代码格式要求：");
                stringBuilder.AppendLine($"```{lang.ToLower()}");
                stringBuilder.AppendLine("// 代码示例");
                stringBuilder.AppendLine("```");
                break;
        }
        
        if (!string.IsNullOrEmpty(userCode))
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"他写的代码（{lang}）：");
            stringBuilder.AppendLine(userCode);
            stringBuilder.AppendLine("看看他的代码，针对性地给点建议。");
        }
        
        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// 构建题解的 Prompt
    /// </summary>
    private string BuildSolutionPrompt(string title, string desc, string? language = null)
    {
        var lang = language ?? "C++";
        return $@"写一个这道题的详细题解：

题目：{title}
描述：{desc}

按这个结构来写：

# 解题思路
[把解题的想法说清楚，怎么想到这个方法的]

# 算法说明
[具体用什么方法和技巧]

# 复杂度分析
时间复杂度：O(...)
空间复杂度：O(...)

# 关键点
- [重点1]
- [重点2]
- [重点3]

# {lang}示例代码
```{lang.ToLower()}
[完整可运行的代码]
```

注意：写得自然一些，就像在给同学讲解一样，别太官方。用简体中文，代码用{lang}。";
    }
    
    /// <summary>
    /// 构建代码分析的 Prompt
    /// </summary>
    private string BuildCodeAnalysisPrompt(string title, string desc, string userCode, string language)
    {
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("分析一下这段代码写得怎么样。");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine($"题目：{title}");
        promptBuilder.AppendLine($"描述：{desc}");
        promptBuilder.AppendLine($"语言：{language}");
        promptBuilder.AppendLine();
        
        // 🌟 知识图谱增强：分析题目要求的算法 vs 代码实际使用的算法
        var expectedKnowledge = _kgService.ExtractKnowledgeTags(title, desc);
        var detectedAlgorithms = _kgService.DetectAlgorithmPatterns(userCode);
        
        if (expectedKnowledge.Any() || detectedAlgorithms.Any())
        {
            promptBuilder.AppendLine("【知识图谱诊断】");
            
            if (expectedKnowledge.Any())
            {
                promptBuilder.AppendLine($"题目通常需要的知识点：{string.Join("、", expectedKnowledge)}");
            }
            
            if (detectedAlgorithms.Any())
            {
                promptBuilder.AppendLine($"代码中检测到的算法：{string.Join("、", detectedAlgorithms)}");
            }
            
            // 找出可能缺失的知识点
            var missingConcepts = expectedKnowledge.Except(detectedAlgorithms).ToList();
            if (missingConcepts.Any())
            {
                promptBuilder.AppendLine($"⚠️ 可能缺少的关键算法：{string.Join("、", missingConcepts)}");
                promptBuilder.AppendLine("在分析时请特别关注这些算法是否被正确实现（或者可能用了其他等效方法）。");
            }
            
            promptBuilder.AppendLine();
        }
        
        promptBuilder.AppendLine($"代码：");
        promptBuilder.AppendLine($"```{language.ToLower()}");
        promptBuilder.AppendLine(userCode);
        promptBuilder.AppendLine("```");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("帮我看看：");
        promptBuilder.AppendLine("1. 这代码能不能正确解决题目？算法逻辑对不对？");
        promptBuilder.AppendLine("2. 不用管代码风格、命名、注释这些，就看功能对不对");
        promptBuilder.AppendLine("3. 也不用管什么NULL检查、异常处理，就看算法本身");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("按这个格式给反馈：");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("# 整体评价");
        promptBuilder.AppendLine("[一句话总结，这代码行不行]");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("# 正确性分析");
        promptBuilder.AppendLine("[说说逻辑对不对，能不能通过测试，有没有bug]");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("# 复杂度分析");
        promptBuilder.AppendLine("时间复杂度：O(...) - [简单说说]");
        promptBuilder.AppendLine("空间复杂度：O(...) - [简单说说]");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("# 优化建议");
        promptBuilder.AppendLine("- [如果能改进就说说怎么改]");
        promptBuilder.AppendLine("- [如果有bug就说说怎么修]");
        promptBuilder.AppendLine("- [如果有更好的方法就提一下]");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("# 评分");
        promptBuilder.AppendLine("[给个0-100的分数，就写数字]");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("打分规则：");
        promptBuilder.AppendLine("- 90-100：算法对的，能AC");
        promptBuilder.AppendLine("- 70-89：大体对的，可能有小问题");
        promptBuilder.AppendLine("- 50-69：思路对但实现有问题");
        promptBuilder.AppendLine("- 30-49：思路不太对");
        promptBuilder.AppendLine("- 0-29：基本不对");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("记住：只看算法对不对，别管代码写得好不好看。语气自然点，别太正式。");
        
        return promptBuilder.ToString();
    }
    
    /// <summary>
    /// 解析标准题解
    /// </summary>
    private StandardSolution ParseStandardSolution(string aiResponse)
    {
        var solution = new StandardSolution();
        
        try
        {
            // 简单解析（实际项目中可以使用正则或更复杂的解析逻辑）
            var lines = aiResponse.Split('\n');
            var currentSection = "";
            var codeBuilder = new StringBuilder();
            var inCodeBlock = false;
            
            foreach (var line in lines)
            {
                if (line.StartsWith("# 解题思路"))
                {
                    currentSection = "approach";
                }
                else if (line.StartsWith("# 算法说明"))
                {
                    currentSection = "algorithm";
                }
                else if (line.StartsWith("# 复杂度分析"))
                {
                    currentSection = "complexity";
                }
                else if (line.StartsWith("# 关键点"))
                {
                    currentSection = "keypoints";
                }
                else if (line.Contains("```"))
                {
                    inCodeBlock = !inCodeBlock;
                    if (!inCodeBlock && codeBuilder.Length > 0)
                    {
                        solution.SampleCodes["C++"] = codeBuilder.ToString().Trim();
                        codeBuilder.Clear();
                    }
                }
                else if (inCodeBlock)
                {
                    codeBuilder.AppendLine(line);
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    switch (currentSection)
                    {
                        case "approach":
                            solution.Approach += line + "\n";
                            break;
                        case "algorithm":
                            solution.Algorithm += line + "\n";
                            break;
                        case "complexity":
                            if (line.Contains("时间复杂度"))
                                solution.TimeComplexity = line;
                            if (line.Contains("空间复杂度"))
                                solution.SpaceComplexity = line;
                            break;
                        case "keypoints":
                            if (line.TrimStart().StartsWith("-"))
                                solution.KeyPoints.Add(line.TrimStart('-').Trim());
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] Error parsing solution: {ex.Message}");
        }
        
        return solution;
    }
    
    /// <summary>
    /// 解析代码分析
    /// </summary>
    private CodeAnalysis ParseCodeAnalysis(string aiResponse)
    {
        var analysis = new CodeAnalysis
        {
            Score = 0  // 初始化为0，确保能正确解析
        };
        
        try
        {
            Console.WriteLine($"[AIService] Parsing code analysis response length: {aiResponse?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(aiResponse))
            {
                Console.WriteLine("[AIService] Empty AI response");
                analysis.Score = 60; // 只有在空响应时给默认分
                return analysis;
            }
            
            var lines = aiResponse.Split('\n');
            var currentSection = "";
            var sectionContent = new System.Text.StringBuilder();
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // 检测各种可能的标题格式
                if (trimmedLine.StartsWith("# 整体评价") || trimmedLine.StartsWith("#整体评价") || 
                    trimmedLine.StartsWith("**整体评价") || trimmedLine.Contains("整体评价："))
                {
                    SaveSection(analysis, currentSection, sectionContent.ToString());
                    currentSection = "overall";
                    sectionContent.Clear();
                    
                    // 如果标题行包含内容，直接提取
                    var colonIndex = trimmedLine.IndexOf('：');
                    if (colonIndex < 0) colonIndex = trimmedLine.IndexOf(':');
                    if (colonIndex >= 0 && colonIndex < trimmedLine.Length - 1)
                    {
                        sectionContent.Append(trimmedLine.Substring(colonIndex + 1).Trim());
                    }
                }
                else if (trimmedLine.StartsWith("# 正确性分析") || trimmedLine.StartsWith("#正确性分析") || 
                         trimmedLine.StartsWith("**正确性分析") || trimmedLine.Contains("正确性分析："))
                {
                    SaveSection(analysis, currentSection, sectionContent.ToString());
                    currentSection = "correctness";
                    sectionContent.Clear();
                    
                    var colonIndex = trimmedLine.IndexOf('：');
                    if (colonIndex < 0) colonIndex = trimmedLine.IndexOf(':');
                    if (colonIndex >= 0 && colonIndex < trimmedLine.Length - 1)
                    {
                        sectionContent.Append(trimmedLine.Substring(colonIndex + 1).Trim());
                    }
                }
                else if (trimmedLine.StartsWith("# 复杂度分析") || trimmedLine.StartsWith("#复杂度分析") || 
                         trimmedLine.StartsWith("**复杂度分析") || trimmedLine.Contains("复杂度分析："))
                {
                    SaveSection(analysis, currentSection, sectionContent.ToString());
                    currentSection = "complexity";
                    sectionContent.Clear();
                }
                else if (trimmedLine.StartsWith("# 优化建议") || trimmedLine.StartsWith("#优化建议") || 
                         trimmedLine.StartsWith("**优化建议") || trimmedLine.Contains("优化建议："))
                {
                    SaveSection(analysis, currentSection, sectionContent.ToString());
                    currentSection = "suggestions";
                    sectionContent.Clear();
                }
                else if (trimmedLine.StartsWith("# 评分") || trimmedLine.StartsWith("#评分") || 
                         trimmedLine.StartsWith("**评分") || trimmedLine.Contains("评分："))
                {
                    SaveSection(analysis, currentSection, sectionContent.ToString());
                    currentSection = "score";
                    sectionContent.Clear();
                    
                    // 尝试直接从标题行提取分数
                    var scoreMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"\d+");
                    if (scoreMatch.Success)
                    {
                        var score = int.Parse(scoreMatch.Value);
                        if (score >= 0 && score <= 100)
                        {
                            analysis.Score = score;
                            Console.WriteLine($"[AIService] Found score in title: {score}");
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    // 累积当前章节的内容
                    if (!string.IsNullOrEmpty(currentSection))
                    {
                        if (sectionContent.Length > 0)
                            sectionContent.AppendLine();
                        sectionContent.Append(trimmedLine);
                    }
                }
            }
            
            // 保存最后一个章节
            SaveSection(analysis, currentSection, sectionContent.ToString());
            
            // 如果没有解析到分数，给一个默认分
            if (analysis.Score == 0)
            {
                Console.WriteLine("[AIService] No score found, setting default 60");
                analysis.Score = 60;
            }
            
            Console.WriteLine($"[AIService] Parsed analysis - Score: {analysis.Score}, Correctness: {analysis.Correctness?.Length ?? 0}, TimeComplexity: {analysis.TimeComplexity}, SpaceComplexity: {analysis.SpaceComplexity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AIService] Error parsing analysis: {ex.Message}");
            Console.WriteLine($"[AIService] Stack trace: {ex.StackTrace}");
            if (analysis.Score == 0)
                analysis.Score = 60;
        }
        
        return analysis;
    }
    
    /// <summary>
    /// 保存章节内容到分析对象
    /// </summary>
    private void SaveSection(CodeAnalysis analysis, string section, string content)
    {
        if (string.IsNullOrEmpty(section) || string.IsNullOrWhiteSpace(content))
            return;
            
        var trimmedContent = content.Trim();
        
        switch (section)
        {
            case "overall":
                analysis.OverallReview = trimmedContent;
                Console.WriteLine($"[AIService] Saved overall: {trimmedContent.Substring(0, Math.Min(50, trimmedContent.Length))}");
                break;
                
            case "correctness":
                analysis.Correctness = trimmedContent;
                Console.WriteLine($"[AIService] Saved correctness: {trimmedContent.Substring(0, Math.Min(50, trimmedContent.Length))}");
                break;
                
            case "complexity":
                // 解析时间和空间复杂度
                var lines = trimmedContent.Split('\n');
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.Contains("时间复杂度"))
                    {
                        analysis.TimeComplexity = trimmedLine;
                        Console.WriteLine($"[AIService] Saved time complexity: {trimmedLine}");
                    }
                    if (trimmedLine.Contains("空间复杂度"))
                    {
                        analysis.SpaceComplexity = trimmedLine;
                        Console.WriteLine($"[AIService] Saved space complexity: {trimmedLine}");
                    }
                }
                break;
                
            case "suggestions":
                // 解析建议列表
                var suggestionLines = trimmedContent.Split('\n');
                foreach (var line in suggestionLines)
                {
                    var trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("-") || trimmedLine.StartsWith("•") || 
                        trimmedLine.StartsWith("*") || char.IsDigit(trimmedLine.FirstOrDefault()))
                    {
                        var suggestion = trimmedLine.TrimStart('-', '•', '*', ' ', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', ')').Trim();
                        if (!string.IsNullOrWhiteSpace(suggestion))
                        {
                            analysis.Suggestions.Add(suggestion);
                            Console.WriteLine($"[AIService] Added suggestion: {suggestion.Substring(0, Math.Min(30, suggestion.Length))}");
                        }
                    }
                }
                break;
                
            case "score":
                // 尝试提取分数
                var scoreMatch = System.Text.RegularExpressions.Regex.Match(trimmedContent, @"\d+");
                if (scoreMatch.Success)
                {
                    var score = int.Parse(scoreMatch.Value);
                    if (score >= 0 && score <= 100)
                    {
                        analysis.Score = score;
                        Console.WriteLine($"[AIService] Saved score: {score}");
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// 降级提示（AI 不可用时）
    /// </summary>
    private string GetFallbackHint(int level)
    {
        return level switch
        {
            1 => "提示：仔细阅读题目要求，思考需要处理的边界情况。",
            2 => "提示：考虑使用合适的数据结构来存储和处理数据。思考算法的时间复杂度要求。",
            3 => "提示：可以先写出解题的伪代码，然后逐步实现每个步骤。注意测试边界情况。",
            _ => "提示：建议先理解题意，再开始编码。"
        };
    }
    
    /// <summary>
    /// 降级题解（AI 不可用时）
    /// </summary>
    private StandardSolution GetFallbackSolution(string title)
    {
        return new StandardSolution
        {
            Approach = "题解暂时无法生成，请稍后重试。",
            Algorithm = "N/A",
            TimeComplexity = "N/A",
            SpaceComplexity = "N/A",
            KeyPoints = new List<string> { "请联系管理员" }
        };
    }
}

