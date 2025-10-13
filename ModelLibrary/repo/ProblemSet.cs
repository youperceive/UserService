using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace ModelLibrary.repo;

/// <summary>
/// 题目核心信息表
/// </summary>
public class Problem
{
    /// <summary>
    /// 数据库内部主键（自增）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 业务对外唯一标识（如 P1972）
    /// </summary>
    [SugarColumn(Length = 50)] // 唯一约束，确保业务标识不重复
    public string Uuid { get; init; } = "";
    
    [SugarColumn(Length = 50)] // 题目标题
    public string Title { get; init; } = "";

    /// <summary>
    /// 题目描述
    /// </summary>
    [SugarColumn(ColumnDataType = "text")] // 长文本类型，支持多行描述
    public string Description { get; set; } = "";

    /// <summary>
    /// 输入格式说明
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string InputFormat { get; set; } = "";

    /// <summary>
    /// 输出格式说明
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string OutputFormat { get; set; } = "";

    /// <summary>
    /// 样例解释
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Explanation { get; set; } = "";

    /// <summary>
    /// 难度等级（如 提高+/省选-）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Difficulty { get; set; } = "";

    /// <summary>
    /// 时间限制（数值，默认单位毫秒）
    /// </summary>
    public int TimeLimit { get; set; } = 2000;

    /// <summary>
    /// 内存限制（数值，默认单位 MB）
    /// </summary>
    public int MemoryLimit { get; set; } = 512;
    
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToMany, nameof(Submission.ProblemId))]
    public List<Submission>? Submissions { get; set; }
}

/// <summary>
/// 题目-标签关联表（多对多）
/// </summary>
public class TagOfProblem
{
    /// <summary>
    /// 关联的题目数据库ID（外键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)] // 联合主键之一
    public int ProblemId { get; set; }

    /// <summary>
    /// 标签名称（如 主席树、莫队）
    /// </summary>
    [SugarColumn(Length = 50, IsPrimaryKey = true)] // 联合主键之一，确保 (ProblemId, Tag) 唯一
    public string Tag { get; set; } = "";

    /// <summary>
    /// 导航属性：关联的题目（可选，用于 SqlSugar 导航查询）
    /// </summary>
    [SugarColumn(IsIgnore = true)] // 忽略数据库映射，仅用于内存导航
    public Problem? Problem { get; set; }
}

/// <summary>
/// 题目-输入输出示例表
/// </summary>
public class IoFileOfProblem
{
    /// <summary>
    /// 数据库内部主键（自增）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 关联的题目数据库ID（外键）
    /// </summary>
    public int ProblemId { get; set; }

    /// <summary>
    /// 示例序号（用于区分同一题目的多组示例，如 1、2、3）
    /// </summary>
    public int Index { get; set; } = 1;

    /// <summary>
    /// 输入内容（长字符串，包含换行等格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string In { get; set; } = "";

    /// <summary>
    /// 输出内容（长字符串，包含换行等格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Out { get; set; } = "";

    /// <summary>
    /// 导航属性：关联的题目（可选，用于 SqlSugar 导航查询）
    /// </summary>
    [SugarColumn(IsIgnore = true)] // 忽略数据库映射，仅用于内存导航
    public Problem? Problem { get; set; }
}

/// <summary>
/// 题目-输入输出示例详细表（包含样例解释）
/// </summary>
public class IoExampleProblem
{
    /// <summary>
    /// 数据库内部主键（自增）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 关联的题目数据库ID（外键）
    /// </summary>
    public int ProblemId { get; set; }

    /// <summary>
    /// 示例序号（用于区分同一题目的多组示例，如 1、2、3）
    /// </summary>
    public int Index { get; set; } = 1;

    /// <summary>
    /// 输入内容（长字符串，包含换行等格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string InExample { get; set; } = "";

    /// <summary>
    /// 输出内容（长字符串，包含换行等格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string OutExample { get; set; } = "";

    /// <summary>
    /// 样例解释（说明该示例的具体含义和解题思路）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")]
    public string Explanation { get; set; } = "";

    /// <summary>
    /// 导航属性：关联的题目（用于 SqlSugar 导航查询）
    /// </summary>
    [SugarColumn(IsIgnore = true)] // 忽略数据库映射，仅用于内存导航
    public Problem? Problem { get; set; }
}

/// <summary>
/// 提交记录表（存储用户对题目的代码提交信息）
/// </summary>
public class Submission
{
    /// <summary>
    /// 数据库内部主键（自增，仅用于后端表间关联）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 提交对外唯一标识（哈希值，如 "33112468"，供前端展示和查询）
    /// </summary>
    [SugarColumn(Length = 50)] // 唯一约束，确保对外标识不重复
    public string Uuid { get; set; } = Guid.NewGuid().ToString("N"); // 默认生成短哈希值

    /// <summary>
    /// 关联用户的数据库ID（外键，对应 User.Id）
    /// </summary>
    [SugarColumn] // 隐含外键关联逻辑，与 User 表的内部主键匹配
    public int UserId { get; set; }

    /// <summary>
    /// 关联题目的数据库ID（外键，对应 Problem.Id）
    /// </summary>
    [SugarColumn] // 与 Problem 表、IoFileOfProblem 等的关联逻辑一致
    public int ProblemId { get; set; }

    /// <summary>
    /// 提交时间（如 "Oct/13/2025 11:36"，后端存储为标准时间格式）
    /// </summary>
    [SugarColumn]
    public DateTime SubmissionTime { get; set; } = DateTime.Now; // 默认值为当前提交时间

    /// <summary>
    /// 编程语言（如 "c++23" "java17" "python3.11"）
    /// </summary>
    [SugarColumn(Length = 50)] // 长度足够覆盖主流语言标识
    public string Lang { get; set; } = "";

    /// <summary>
    /// 提交状态（如 "TLE" "AC" "WA" "RE" "Pending"）
    /// </summary>
    [SugarColumn(Length = 50)] // 长度覆盖所有可能的评测状态
    public string State { get; set; } = "Pending"; // 默认状态为"待评测"

    /// <summary>
    /// 运行时间（单位：毫秒，如 "4000"，未运行时为0）
    /// </summary>
    [SugarColumn]
    public int Time { get; set; } = 0;

    /// <summary>
    /// 内存使用（单位：MB，如 "0"，未运行时为0）
    /// </summary>
    [SugarColumn]
    public int Memory { get; set; } = 0;

    /// <summary>
    /// 提交的代码内容（长文本存储）
    /// </summary>
    [SugarColumn(ColumnDataType = "text")] // 支持大段代码存储（如数千行代码）
    public string Code { get; set; } = "";

    #region 导航属性（用于 SqlSugar 关联查询，不映射到数据库）
    /// <summary>
    /// 关联的用户信息（可选，用于快速查询提交所属用户）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.ManyToOne, nameof(UserId))] // 修正为 ManyToOne
    public User? User { get; set; }

    /// <summary>
    /// 关联的题目信息（可选，用于快速查询提交所属题目）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.ManyToOne, nameof(UserId))] // 修正为 ManyToOne
    public Problem? Problem { get; set; }
    #endregion
}