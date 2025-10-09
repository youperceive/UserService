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
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; }

    /// <summary>
    /// 关联的题目数据库ID（外键）
    /// </summary>
    public int ProblemId { get; set; }

    /// <summary>
    /// 示例序号（用于区分同一题目的多组示例，如 1、2、3）
    /// </summary>
    public int IoIndex { get; set; } = 1;

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
