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
    
    public AIService(IConfiguration configuration)
    {
        _configuration = configuration;
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
            cachedHint.FromCache = true;
            return cachedHint;
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
            
            // 缓存（如果是强制刷新，先移除旧缓存）
            if (forceRefresh)
            {
                _hintCache.TryRemove(cacheKey, out _);
            }
            _hintCache.TryAdd(cacheKey, hint);
            
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
        var sb = new StringBuilder();
        sb.AppendLine($"题目：{title}");
        sb.AppendLine($"描述：{desc}");
        sb.AppendLine();
        
        var lang = language ?? "C++";
        
        switch (level)
        {
            case 1: // 轻度提示 - 最严格
                sb.AppendLine("请给出轻度提示（Level 1）：");
                sb.AppendLine("【重要】绝对禁止事项：");
                sb.AppendLine("- 严禁提及任何具体的算法名称（如：动态规划、二分查找、DFS、BFS等）");
                sb.AppendLine("- 严禁提及任何具体的数据结构名称（如：栈、队列、哈希表、树等）");
                sb.AppendLine("- 严禁给出任何代码、伪代码或代码片段");
                sb.AppendLine("- 严禁给出具体的实现步骤");
                sb.AppendLine();
                sb.AppendLine("【允许的提示内容】：");
                sb.AppendLine("- 只能用通俗的语言描述解题的大致思考方向");
                sb.AppendLine("- 可以提醒需要注意的边界条件和特殊情况");
                sb.AppendLine("- 可以提示题目的关键突破点（但不能说具体怎么做）");
                sb.AppendLine("- 控制在2-4句话以内");
                sb.AppendLine("- 用简体中文回答，不要使用专业术语");
                sb.AppendLine();
                sb.AppendLine("示例（好的轻度提示）：");
                sb.AppendLine("\"思考一下如何记录之前看过的信息，避免重复计算。注意处理空值和边界情况。\"");
                break;
                
            case 2: // 中度提示 - 可提及技术但不给代码
                sb.AppendLine("请给出中度提示（Level 2）：");
                sb.AppendLine("【允许的内容】：");
                sb.AppendLine("- 可以提及适合使用的数据结构类型（如：需要先进先出的结构）");
                sb.AppendLine("- 可以提及算法的大类（如：遍历、搜索、优化等）");
                sb.AppendLine("- 给出解题的关键步骤（用文字描述）");
                sb.AppendLine();
                sb.AppendLine("【严格禁止】：");
                sb.AppendLine("- 绝对不能给出任何代码");
                sb.AppendLine("- 不能给出伪代码");
                sb.AppendLine("- 不能给出具体的API调用");
                sb.AppendLine();
                sb.AppendLine("- 控制在4-7句话");
                sb.AppendLine("- 用简体中文回答");
                break;
                
            case 3: // 深度提示 - 可以给伪代码和算法
                sb.AppendLine("请给出深度提示（Level 3）：");
                sb.AppendLine("- 详细说明完整的解题思路和算法");
                sb.AppendLine("- 给出清晰的伪代码或算法步骤");
                sb.AppendLine($"- 如果给出代码片段，使用{lang}语言");
                sb.AppendLine("- 说明时间和空间复杂度");
                sb.AppendLine("- 可以接近完整解法，但建议留一些细节让用户自己实现");
                sb.AppendLine($"- 用简体中文回答，代码注释也用中文，代码使用{lang}");
                break;
        }
        
        if (!string.IsNullOrEmpty(userCode))
        {
            sb.AppendLine();
            sb.AppendLine($"用户当前代码（{lang}）：");
            sb.AppendLine(userCode);
            sb.AppendLine("请根据用户代码给出更有针对性的提示，指出可能的问题方向");
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// 构建题解的 Prompt
    /// </summary>
    private string BuildSolutionPrompt(string title, string desc, string? language = null)
    {
        var lang = language ?? "C++";
        return $@"请为以下算法题目生成标准题解：

题目：{title}
描述：{desc}

请按以下格式输出：

# 解题思路
[详细的解题思路]

# 算法说明
[使用的算法和数据结构]

# 复杂度分析
时间复杂度：[如 O(n)]
空间复杂度：[如 O(1)]

# 关键点
- [关键点1]
- [关键点2]
- [关键点3]

# {lang}示例代码
```{lang.ToLower()}
[完整的{lang}代码]
```

用简体中文回答，代码部分使用{lang}。";
    }
    
    /// <summary>
    /// 构建代码分析的 Prompt
    /// </summary>
    private string BuildCodeAnalysisPrompt(string title, string desc, string userCode, string language)
    {
        return $@"作为一个算法评测系统，请从算法正确性角度分析以下用户提交的代码。

【重要】评分标准：
- 这是算法评测，不是代码审查
- 只关注：代码能否正确解决题目要求
- 不关注：代码风格、变量命名、注释、鲁棒性（如NULL检查、边界保护等）
- 评分依据：能否通过测试用例、算法逻辑是否正确

题目：{title}
描述：{desc}
编程语言：{language}

用户代码：
```{language.ToLower()}
{userCode}
```

请严格按以下格式输出（必须包含所有标题）：

# 整体评价
[一句话评价代码能否解决问题，30字以内]

# 正确性分析
[详细分析代码逻辑是否正确，能否满足题目要求，能否通过测试用例。只关注算法逻辑，不评价代码风格。]

# 复杂度分析
时间复杂度：O(n) - [简短说明]
空间复杂度：O(1) - [简短说明]

# 优化建议
- [如果有更优的算法，给出建议1]
- [如果有性能问题，给出建议2]
- [如果逻辑有误，给出修正建议]

# 评分
[0-100分的数字，只需要数字]

评分参考：
- 90-100分：算法完全正确，能通过所有测试用例
- 70-89分：算法基本正确，可能存在小的边界问题
- 50-69分：算法思路正确但实现有明显错误
- 30-49分：算法思路部分正确
- 0-29分：算法思路错误或无法运行

【再次强调】不要因为缺少NULL检查、缺少异常处理、变量命名不规范等代码风格问题扣分！只评价算法逻辑！

用简体中文回答。";
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

