using System.Text.RegularExpressions;

namespace QAService.Services;

/// <summary>
/// 知识图谱服务 - 负责识别算法知识点和代码模式
/// </summary>
public class KnowledgeGraphService
{
    // 知识点依赖关系（前置知识）
    private readonly Dictionary<string, List<string>> _knowledgeDependencies;
    
    // 关键词到知识点的映射
    private readonly Dictionary<string, List<string>> _keywordToKnowledge;
    
    // 代码模式识别规则
    private readonly List<CodePattern> _codePatterns;

    public KnowledgeGraphService()
    {
        _knowledgeDependencies = BuildKnowledgeDependencies();
        _keywordToKnowledge = BuildKeywordMapping();
        _codePatterns = BuildCodePatterns();
    }

    /// <summary>
    /// 从题目标题和描述中提取知识点
    /// </summary>
    public List<string> ExtractKnowledgeTags(string title, string description)
    {
        var tags = new HashSet<string>();
        var text = (title + " " + description).ToLower();

        // 遍历关键词映射
        foreach (var kvp in _keywordToKnowledge)
        {
            var keyword = kvp.Key;
            var knowledgePoints = kvp.Value;
            
            if (text.Contains(keyword))
            {
                foreach (var point in knowledgePoints)
                {
                    tags.Add(point);
                }
            }
        }

        return tags.ToList();
    }

    /// <summary>
    /// 获取知识点的前置知识
    /// </summary>
    public List<string> GetPrerequisites(IEnumerable<string> tags)
    {
        var prerequisites = new HashSet<string>();
        
        foreach (var tag in tags)
        {
            if (_knowledgeDependencies.ContainsKey(tag))
            {
                foreach (var prereq in _knowledgeDependencies[tag])
                {
                    prerequisites.Add(prereq);
                }
            }
        }

        // 移除已经在 tags 中的知识点
        prerequisites.ExceptWith(tags);
        
        return prerequisites.ToList();
    }

    /// <summary>
    /// 从代码中检测使用的算法模式
    /// </summary>
    public List<string> DetectAlgorithmPatterns(string code)
    {
        var detectedPatterns = new HashSet<string>();
        
        foreach (var pattern in _codePatterns)
        {
            if (pattern.Matches(code))
            {
                detectedPatterns.Add(pattern.KnowledgePoint);
            }
        }

        return detectedPatterns.ToList();
    }

    /// <summary>
    /// 获取相关概念（同一知识领域的其他概念）
    /// </summary>
    public List<string> GetRelatedConcepts(IEnumerable<string> tags)
    {
        var related = new HashSet<string>();
        
        // 构建知识领域
        var domains = new Dictionary<string, List<string>>
        {
            ["数据结构"] = new List<string> { "数组", "链表", "栈", "队列", "哈希表", "树", "图", "堆" },
            ["搜索算法"] = new List<string> { "二分查找", "DFS", "BFS", "回溯" },
            ["排序算法"] = new List<string> { "快速排序", "归并排序", "堆排序", "冒泡排序" },
            ["动态规划"] = new List<string> { "记忆化搜索", "状态压缩", "背包问题", "最长子序列" },
            ["图论"] = new List<string> { "最短路径", "最小生成树", "拓扑排序", "强连通分量" },
            ["字符串"] = new List<string> { "KMP", "字符串哈希", "Trie树", "后缀数组" }
        };

        foreach (var tag in tags)
        {
            foreach (var domain in domains.Values)
            {
                if (domain.Contains(tag))
                {
                    related.UnionWith(domain.Where(x => x != tag).Take(2));
                    break;
                }
            }
        }

        return related.ToList();
    }

    /// <summary>
    /// 构建知识点依赖关系
    /// </summary>
    private Dictionary<string, List<string>> BuildKnowledgeDependencies()
    {
        return new Dictionary<string, List<string>>
        {
            // 算法类
            ["动态规划"] = new List<string> { "递归思想", "数学归纳法" },
            ["记忆化搜索"] = new List<string> { "递归", "哈希表" },
            ["贪心算法"] = new List<string> { "排序", "优先队列" },
            ["回溯"] = new List<string> { "递归", "DFS" },
            ["分治"] = new List<string> { "递归" },
            
            // 图论
            ["最短路径"] = new List<string> { "图的表示", "BFS" },
            ["拓扑排序"] = new List<string> { "图的表示", "DFS或BFS" },
            ["最小生成树"] = new List<string> { "图的表示", "并查集或贪心" },
            ["强连通分量"] = new List<string> { "DFS", "图论基础" },
            
            // 数据结构
            ["线段树"] = new List<string> { "二叉树", "递归" },
            ["树状数组"] = new List<string> { "二进制运算", "前缀和" },
            ["并查集"] = new List<string> { "树的表示" },
            ["Trie树"] = new List<string> { "树的表示", "字符串处理" },
            ["堆"] = new List<string> { "完全二叉树", "优先队列概念" },
            
            // 字符串
            ["KMP"] = new List<string> { "字符串基础", "状态机" },
            ["字符串哈希"] = new List<string> { "哈希函数", "进制转换" },
            
            // 数学
            ["数论"] = new List<string> { "质数", "最大公约数" },
            ["组合数学"] = new List<string> { "排列组合", "递推" },
            ["概率论"] = new List<string> { "基础概率", "期望" }
        };
    }

    /// <summary>
    /// 构建关键词到知识点的映射
    /// </summary>
    private Dictionary<string, List<string>> BuildKeywordMapping()
    {
        return new Dictionary<string, List<string>>
        {
            // 动态规划相关
            ["动态规划"] = new List<string> { "动态规划" },
            ["dp"] = new List<string> { "动态规划" },
            ["最优子结构"] = new List<string> { "动态规划" },
            ["重叠子问题"] = new List<string> { "动态规划" },
            ["背包"] = new List<string> { "动态规划", "背包问题" },
            ["最长公共子序列"] = new List<string> { "动态规划", "最长子序列" },
            ["最长上升子序列"] = new List<string> { "动态规划", "最长子序列" },
            
            // 贪心
            ["贪心"] = new List<string> { "贪心算法" },
            ["greedy"] = new List<string> { "贪心算法" },
            ["局部最优"] = new List<string> { "贪心算法" },
            
            // 搜索
            ["二分"] = new List<string> { "二分查找" },
            ["binary search"] = new List<string> { "二分查找" },
            ["dfs"] = new List<string> { "DFS" },
            ["深度优先"] = new List<string> { "DFS" },
            ["bfs"] = new List<string> { "BFS" },
            ["广度优先"] = new List<string> { "BFS" },
            ["回溯"] = new List<string> { "回溯" },
            ["backtrack"] = new List<string> { "回溯" },
            
            // 图论
            ["最短路"] = new List<string> { "最短路径" },
            ["dijkstra"] = new List<string> { "最短路径" },
            ["floyd"] = new List<string> { "最短路径" },
            ["最小生成树"] = new List<string> { "最小生成树" },
            ["拓扑排序"] = new List<string> { "拓扑排序" },
            ["连通性"] = new List<string> { "图论基础" },
            
            // 数据结构
            ["栈"] = new List<string> { "栈" },
            ["stack"] = new List<string> { "栈" },
            ["队列"] = new List<string> { "队列" },
            ["queue"] = new List<string> { "队列" },
            ["哈希"] = new List<string> { "哈希表" },
            ["hash"] = new List<string> { "哈希表" },
            ["堆"] = new List<string> { "堆" },
            ["heap"] = new List<string> { "堆" },
            ["优先队列"] = new List<string> { "堆", "优先队列" },
            ["线段树"] = new List<string> { "线段树" },
            ["树状数组"] = new List<string> { "树状数组" },
            ["并查集"] = new List<string> { "并查集" },
            ["union find"] = new List<string> { "并查集" },
            ["trie"] = new List<string> { "Trie树" },
            ["前缀树"] = new List<string> { "Trie树" },
            
            // 排序
            ["排序"] = new List<string> { "排序算法" },
            ["sort"] = new List<string> { "排序算法" },
            
            // 字符串
            ["kmp"] = new List<string> { "KMP" },
            ["字符串匹配"] = new List<string> { "字符串处理" },
            ["子串"] = new List<string> { "字符串处理" },
            
            // 数学
            ["质数"] = new List<string> { "数论" },
            ["素数"] = new List<string> { "数论" },
            ["最大公约数"] = new List<string> { "数论" },
            ["gcd"] = new List<string> { "数论" },
            ["组合"] = new List<string> { "组合数学" },
            ["排列"] = new List<string> { "组合数学" },
            ["概率"] = new List<string> { "概率论" },
            ["期望"] = new List<string> { "概率论" },
            
            // 其他
            ["滑动窗口"] = new List<string> { "双指针", "滑动窗口" },
            ["双指针"] = new List<string> { "双指针" },
            ["前缀和"] = new List<string> { "前缀和" },
            ["差分"] = new List<string> { "差分数组" },
            ["位运算"] = new List<string> { "位运算" },
            ["bit"] = new List<string> { "位运算" }
        };
    }

    /// <summary>
    /// 构建代码模式识别规则
    /// </summary>
    private List<CodePattern> BuildCodePatterns()
    {
        return new List<CodePattern>
        {
            // 动态规划模式
            new CodePattern
            {
                KnowledgePoint = "动态规划",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("dp[") || code.Contains("dp["),
                    code => code.Contains("memo[") || code.Contains("memo["),
                    code => Regex.IsMatch(code, @"for.*for.*dp\[")
                }
            },
            
            // 二分查找模式
            new CodePattern
            {
                KnowledgePoint = "二分查找",
                Patterns = new List<Func<string, bool>>
                {
                    code => Regex.IsMatch(code, @"while\s*\(\s*\w+\s*[<>]=\s*\w+\s*\)") && 
                            (code.Contains("mid") || code.Contains("middle")),
                    code => code.Contains("binary_search") || code.Contains("BinarySearch"),
                    code => code.Contains("lower_bound") || code.Contains("upper_bound")
                }
            },
            
            // DFS模式
            new CodePattern
            {
                KnowledgePoint = "DFS",
                Patterns = new List<Func<string, bool>>
                {
                    code => Regex.IsMatch(code, @"void\s+dfs\s*\(") || Regex.IsMatch(code, @"def\s+dfs\s*\("),
                    code => code.Contains("visited[") && code.Contains("递归"),
                    code => Regex.IsMatch(code, @"function\s+dfs")
                }
            },
            
            // BFS模式
            new CodePattern
            {
                KnowledgePoint = "BFS",
                Patterns = new List<Func<string, bool>>
                {
                    code => (code.Contains("queue") || code.Contains("Queue")) && 
                            (code.Contains("push") || code.Contains("Enqueue") || code.Contains("append")),
                    code => Regex.IsMatch(code, @"while.*(!|not).*empty\(\)") && code.Contains("front"),
                    code => code.Contains("bfs")
                }
            },
            
            // 哈希表模式
            new CodePattern
            {
                KnowledgePoint = "哈希表",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("unordered_map") || code.Contains("HashMap") || 
                            code.Contains("Dictionary") || code.Contains("dict"),
                    code => code.Contains("hash") || code.Contains("Hash"),
                    code => Regex.IsMatch(code, @"map\s*<")
                }
            },
            
            // 栈模式
            new CodePattern
            {
                KnowledgePoint = "栈",
                Patterns = new List<Func<string, bool>>
                {
                    code => (code.Contains("stack") || code.Contains("Stack")) && 
                            (code.Contains("push") || code.Contains("pop")),
                    code => code.Contains("stk")
                }
            },
            
            // 堆/优先队列模式
            new CodePattern
            {
                KnowledgePoint = "堆",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("priority_queue") || code.Contains("PriorityQueue"),
                    code => code.Contains("heap") || code.Contains("Heap"),
                    code => code.Contains("heapify")
                }
            },
            
            // 排序模式
            new CodePattern
            {
                KnowledgePoint = "排序算法",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("sort(") || code.Contains("Sort(") || 
                            code.Contains(".sort()") || code.Contains("sorted("),
                    code => code.Contains("quicksort") || code.Contains("mergesort")
                }
            },
            
            // 并查集模式
            new CodePattern
            {
                KnowledgePoint = "并查集",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("find(") && code.Contains("union("),
                    code => code.Contains("parent[") && Regex.IsMatch(code, @"find|union"),
                    code => code.Contains("UnionFind") || code.Contains("DSU")
                }
            },
            
            // 双指针模式
            new CodePattern
            {
                KnowledgePoint = "双指针",
                Patterns = new List<Func<string, bool>>
                {
                    code => Regex.IsMatch(code, @"(left|l)\s*=.*right|r\s*=") || 
                            Regex.IsMatch(code, @"while.*<.*\+\+.*--"),
                    code => code.Contains("left") && code.Contains("right") && 
                            (code.Contains("left++") || code.Contains("right--"))
                }
            },
            
            // 前缀和模式
            new CodePattern
            {
                KnowledgePoint = "前缀和",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("prefix") || code.Contains("preSum") || code.Contains("前缀和"),
                    code => Regex.IsMatch(code, @"sum\[\w+\]\s*[-+]=\s*sum\[\w+\s*[-+]\s*1\]")
                }
            },
            
            // 滑动窗口模式
            new CodePattern
            {
                KnowledgePoint = "滑动窗口",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("window") && (code.Contains("left") || code.Contains("right")),
                    code => Regex.IsMatch(code, @"while.*窗口")
                }
            },
            
            // 位运算模式
            new CodePattern
            {
                KnowledgePoint = "位运算",
                Patterns = new List<Func<string, bool>>
                {
                    code => Regex.IsMatch(code, @"[&|^~]\s*[&|^~]|<<|>>"),
                    code => code.Contains("bit") || code.Contains("Bit")
                }
            },
            
            // 贪心模式
            new CodePattern
            {
                KnowledgePoint = "贪心算法",
                Patterns = new List<Func<string, bool>>
                {
                    code => code.Contains("sort") && Regex.IsMatch(code, @"for.*greedy|贪心")
                }
            }
        };
    }

    /// <summary>
    /// 代码模式类
    /// </summary>
    private class CodePattern
    {
        public string KnowledgePoint { get; set; } = string.Empty;
        public List<Func<string, bool>> Patterns { get; set; } = new();

        public bool Matches(string code)
        {
            return Patterns.Any(pattern => 
            {
                try
                {
                    return pattern(code);
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}

