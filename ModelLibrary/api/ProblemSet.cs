// Models/ProblemSetModels.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace ModelLibrary.api
{
    /// <summary>
    /// 题目输入输出样例数据传输对象（用于封装单组输入输出数据）
    /// </summary>
    public class IoFileDto
    {
        /// <summary>
        /// 输入内容
        /// </summary>
        [JsonPropertyName("in")]
        public string In { get; set; } = string.Empty;

        /// <summary>
        /// 输出内容
        /// </summary>
        [JsonPropertyName("out")]
        public string Out { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IoFileDto() { }

        /// <summary>
        /// 带参构造函数（快速初始化输入输出内容）
        /// </summary>
        /// <param name="inContent">输入内容</param>
        /// <param name="outContent">输出内容</param>
        public IoFileDto(string inContent, string outContent)
        {
            In = inContent;
            Out = outContent;
        }
    }

    #region 创建题目相关模型
    /// <summary>
    /// 创建题目请求模型（接收客户端传递的题目信息）
    /// </summary>
    public class CreateProblemRequest
    {
        /// <summary>
        /// 业务对外唯一标识（如 P1972）
        /// </summary>
        [Required(ErrorMessage = "业务唯一标识（uuid）不能为空")]
        [StringLength(50, ErrorMessage = "业务唯一标识（uuid）长度不能超过50个字符")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 题目标题
        /// </summary>
        [Required(ErrorMessage = "题目标题不能为空")]
        [StringLength(100, ErrorMessage = "题目标题长度不能超过100个字符")]
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 题目描述
        /// </summary>
        [Required(ErrorMessage = "题目描述不能为空")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 输入格式说明
        /// </summary>
        [Required(ErrorMessage = "输入格式说明不能为空")]
        [JsonPropertyName("inputFormat")]
        public string InputFormat { get; set; } = string.Empty;

        /// <summary>
        /// 输出格式说明
        /// </summary>
        [Required(ErrorMessage = "输出格式说明不能为空")]
        [JsonPropertyName("outputFormat")]
        public string OutputFormat { get; set; } = string.Empty;

        /// <summary>
        /// 样例解释（非必填）
        /// </summary>
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        /// <summary>
        /// 难度等级（如 提高+/省选-）
        /// </summary>
        [Required(ErrorMessage = "难度等级不能为空")]
        [StringLength(50, ErrorMessage = "难度等级长度不能超过50个字符")]
        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = string.Empty;

        /// <summary>
        /// 时间限制（如 2000(默认毫秒)）
        /// </summary>
        [Required(ErrorMessage = "时间限制不能为空")]
        [StringLength(20, ErrorMessage = "时间限制格式长度不能超过20个字符")]
        [JsonPropertyName("timeLimit")]
        public string TimeLimit { get; set; } = string.Empty;

        /// <summary>
        /// 内存限制（如 512(默认MB)）
        /// </summary>
        [Required(ErrorMessage = "内存限制不能为空")]
        [StringLength(20, ErrorMessage = "内存限制格式长度不能超过20个字符")]
        [JsonPropertyName("memoryLimit")]
        public string MemoryLimit { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CreateProblemRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化题目核心信息）
        /// </summary>
        public CreateProblemRequest(string uuid, string title, string description, 
                                    string inputFormat, string outputFormat, string difficulty, 
                                    string timeLimit, string memoryLimit, string explanation = "")
        {
            Uuid = uuid;
            Title = title;
            Description = description;
            InputFormat = inputFormat;
            OutputFormat = outputFormat;
            Explanation = explanation;
            Difficulty = difficulty;
            TimeLimit = timeLimit;
            MemoryLimit = memoryLimit;
        }
    }

    /// <summary>
    /// 创建题目响应模型（返回创建结果给客户端）
    /// </summary>
    public class CreateProblemResponse : BaseResponse
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CreateProblemResponse() { }

        /// <summary>
        /// 成功响应构造函数（默认状态码200，消息“题目创建成功”）
        /// </summary>
        /// <param name="message">自定义成功消息</param>
        public CreateProblemResponse(string message = "题目创建成功") : base(200, message) { }

        /// <summary>
        /// 自定义状态码响应构造函数（如参数错误、uuid重复等场景）
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">错误/提示消息</param>
        public CreateProblemResponse(int statusCode, string message) : base(statusCode, message) { }
    }
    #endregion

    #region 添加题目输入输出样例相关模型
    /// <summary>
    /// 添加题目输入输出样例请求模型（接收客户端传递的样例信息）
    /// </summary>
    public class AddIoFileOfProblemRequest
    {
        /// <summary>
        /// 关联题目的业务唯一标识（uuid）
        /// </summary>
        [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
        [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 输入内容（字符串形式）
        /// </summary>
        [Required(ErrorMessage = "输入内容（inFile）不能为空")]
        [JsonPropertyName("inFile")]
        public string InFile { get; set; } = string.Empty;

        /// <summary>
        /// 输出内容
        /// </summary>
        [Required(ErrorMessage = "输出内容（outFile）不能为空")]
        [JsonPropertyName("outFile")]
        public string OutFile { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AddIoFileOfProblemRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化样例信息）
        /// </summary>
        /// <param name="uuid">题目uuid</param>
        /// <param name="inFile">输入内容</param>
        /// <param name="outFile">输出内容</param>
        public AddIoFileOfProblemRequest(string uuid, string inFile, string outFile)
        {
            Uuid = uuid;
            InFile = inFile;
            OutFile = outFile;
        }
    }

    /// <summary>
    /// 添加题目输入输出样例响应模型（返回添加结果给客户端）
    /// </summary>
    public class AddIoFileOfProblemResponse : BaseResponse
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AddIoFileOfProblemResponse() { }

        /// <summary>
        /// 成功响应构造函数（默认状态码200，消息“输入输出样例添加成功”）
        /// </summary>
        /// <param name="message">自定义成功消息</param>
        public AddIoFileOfProblemResponse(string message = "输入输出样例添加成功") : base(200, message) { }

        /// <summary>
        /// 自定义状态码响应构造函数（如题目不存在、样例重复等场景）
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">错误/提示消息</param>
        public AddIoFileOfProblemResponse(int statusCode, string message) : base(statusCode, message) { }
    }
    #endregion
    
    #region 获取带解释的题目输入输出样例相关模型
    /// <summary>
    /// 获取带解释的题目输入输出样例请求模型（接收客户端传递的题目uuid）
    /// </summary>
    public class GetIoExampleOfProblemRequest
    {
        /// <summary>
        /// 目标题目的业务唯一标识（uuid）
        /// </summary>
        [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
        [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetIoExampleOfProblemRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化题目uuid）
        /// </summary>
        /// <param name="uuid">题目uuid</param>
        public GetIoExampleOfProblemRequest(string uuid)
        {
            Uuid = uuid;
        }
    }

    /// <summary>
    /// 获取带解释的题目输入输出样例响应模型（返回带解释的样例列表给客户端）
    /// </summary>
    public class GetIoExampleOfProblemResponse : BaseResponse
    {
        /// <summary>
        /// 带解释的题目输入输出样例列表
        /// </summary>
        [JsonPropertyName("ioExamples")]
        public List<IoExampleDto> IoExamples { get; set; } = new List<IoExampleDto>();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetIoExampleOfProblemResponse() { }

        /// <summary>
        /// 成功响应构造函数（带样例列表数据）
        /// </summary>
        /// <param name="ioExamples">带解释的样例列表</param>
        /// <param name="message">自定义成功消息</param>
        public GetIoExampleOfProblemResponse(List<IoExampleDto> ioExamples, string message = "带解释的样例获取成功") : base(200, message)
        {
            IoExamples = ioExamples;
        }

        /// <summary>
        /// 自定义状态码响应构造函数（如题目不存在、无样例数据等场景）
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">错误/提示消息</param>
        public GetIoExampleOfProblemResponse(int statusCode, string message) : base(statusCode, message) { }
    }
    #endregion

    #region 获取题目输入输出样例相关模型
    /// <summary>
    /// 获取题目输入输出样例请求模型（接收客户端传递的题目uuid）
    /// </summary>
    public class GetIoFileOfProblemRequest
    {
        /// <summary>
        /// 目标题目的业务唯一标识（uuid）
        /// </summary>
        [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
        [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetIoFileOfProblemRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化题目uuid）
        /// </summary>
        /// <param name="uuid">题目uuid</param>
        public GetIoFileOfProblemRequest(string uuid)
        {
            Uuid = uuid;
        }
    }

    /// <summary>
    /// 获取题目输入输出样例响应模型（返回样例列表给客户端）
    /// </summary>
    public class GetIoFileOfProblemResponse : BaseResponse
    {
        /// <summary>
        /// 题目输入输出样例列表
        /// </summary>
        [JsonPropertyName("ioFiles")]
        public List<IoFileDto> IoFiles { get; set; } = new List<IoFileDto>();

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetIoFileOfProblemResponse() { }

        /// <summary>
        /// 成功响应构造函数（带样例列表数据）
        /// </summary>
        /// <param name="ioFiles">样例列表</param>
        /// <param name="message">自定义成功消息</param>
        public GetIoFileOfProblemResponse(List<IoFileDto> ioFiles, string message = "样例获取成功") : base(200, message)
        {
            IoFiles = ioFiles;
        }

        /// <summary>
        /// 自定义状态码响应构造函数（如题目不存在、无样例数据等场景）
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">错误/提示消息</param>
        public GetIoFileOfProblemResponse(int statusCode, string message) : base(statusCode, message) { }
    }
    #endregion
    
    /// <summary>
    /// 带解释的题目输入输出样例数据传输对象（用于封装单组带解释的输入输出数据）
    /// </summary>
    public class IoExampleDto
    {
        /// <summary>
        /// 样例序号
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; set; } = 1;

        /// <summary>
        /// 输入内容
        /// </summary>
        [JsonPropertyName("inExample")]
        public string InExample { get; set; } = string.Empty;

        /// <summary>
        /// 输出内容
        /// </summary>
        [JsonPropertyName("outExample")]
        public string OutExample { get; set; } = string.Empty;

        /// <summary>
        /// 样例解释
        /// </summary>
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public IoExampleDto() { }

        /// <summary>
        /// 带参构造函数（快速初始化带解释的样例数据）
        /// </summary>
        /// <param name="index">样例序号</param>
        /// <param name="inExample">输入内容</param>
        /// <param name="outExample">输出内容</param>
        /// <param name="explanation">样例解释</param>
        public IoExampleDto(int index, string inExample, string outExample, string explanation)
        {
            Index = index;
            InExample = inExample;
            OutExample = outExample;
            Explanation = explanation;
        }
    }

    #region 添加带解释的题目输入输出样例相关模型
    /// <summary>
    /// 添加带解释的题目输入输出样例请求模型（接收客户端传递的样例信息）
    /// </summary>
    public class AddIoExampleOfProblemRequest
    {
        /// <summary>
        /// 关联题目的业务唯一标识（uuid）
        /// </summary>
        [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
        [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 样例序号（用于区分同一题目的多组示例）
        /// </summary>
        [Required(ErrorMessage = "样例序号（index）不能为空")]
        [JsonPropertyName("index")]
        public int Index { get; set; } = 1;

        /// <summary>
        /// 输入内容（字符串形式）
        /// </summary>
        [Required(ErrorMessage = "输入内容（inExample）不能为空")]
        [JsonPropertyName("inExample")]
        public string InExample { get; set; } = string.Empty;

        /// <summary>
        /// 输出内容
        /// </summary>
        [Required(ErrorMessage = "输出内容（outExample）不能为空")]
        [JsonPropertyName("outExample")]
        public string OutExample { get; set; } = string.Empty;

        /// <summary>
        /// 样例解释
        /// </summary>
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AddIoExampleOfProblemRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化带解释的样例信息）
        /// </summary>
        /// <param name="uuid">题目uuid</param>
        /// <param name="index">样例序号</param>
        /// <param name="inExample">输入内容</param>
        /// <param name="outExample">输出内容</param>
        /// <param name="explanation">样例解释</param>
        public AddIoExampleOfProblemRequest(string uuid, int index, string inExample, string outExample, string explanation = "")
        {
            Uuid = uuid;
            Index = index;
            InExample = inExample;
            OutExample = outExample;
            Explanation = explanation;
        }
    }

    /// <summary>
    /// 添加带解释的题目输入输出样例响应模型（返回添加结果给客户端）
    /// </summary>
    public class AddIoExampleOfProblemResponse : BaseResponse
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AddIoExampleOfProblemResponse() { }

        /// <summary>
        /// 成功响应构造函数（默认状态码200，消息“带解释的输入输出样例添加成功”）
        /// </summary>
        /// <param name="message">自定义成功消息</param>
        public AddIoExampleOfProblemResponse(string message = "带解释的输入输出样例添加成功") : base(200, message) { }

        /// <summary>
        /// 自定义状态码响应构造函数（如题目不存在、样例重复等场景）
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">错误/提示消息</param>
        public AddIoExampleOfProblemResponse(int statusCode, string message) : base(statusCode, message) { }
    }
    #endregion


    #region 获取题目详情相关模型
/// <summary>
/// 获取题目详情请求模型（通过题目uuid查询单个题目完整信息）
/// </summary>
public class GetProblemRequest
{
    /// <summary>
    /// 目标题目的业务唯一标识（如 P1972）
    /// </summary>
    [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
    [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public GetProblemRequest() { }

    /// <summary>
    /// 带参构造函数（快速初始化题目uuid）
    /// </summary>
    /// <param name="uuid">题目业务唯一标识</param>
    public GetProblemRequest(string uuid)
    {
        Uuid = uuid;
    }
}

/// <summary>
/// 获取题目详情响应模型（返回单个题目的完整信息）
/// </summary>
public class GetProblemResponse : BaseResponse
{
    /// <summary>
    /// 题目业务唯一标识（如 P1972）
    /// </summary>
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// 题目标题
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 题目描述
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 输入格式说明
    /// </summary>
    [JsonPropertyName("inputFormat")]
    public string InputFormat { get; set; } = string.Empty;

    /// <summary>
    /// 输出格式说明
    /// </summary>
    [JsonPropertyName("outputFormat")]
    public string OutputFormat { get; set; } = string.Empty;

    /// <summary>
    /// 题目输入输出样例（二维数组：[输入内容, 输出内容, 样例解释]）
    /// </summary>
    [JsonPropertyName("ioExample")]
    public List<List<string>> IoExample { get; set; } = new List<List<string>>();

    /// <summary>
    /// 题目标签列表（如 主席树、莫队）
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// 难度等级（如 提高+/省选-）
    /// </summary>
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// 时间限制（如 2000(默认毫秒)）
    /// </summary>
    [JsonPropertyName("timeLimit")]
    public string TimeLimit { get; set; } = string.Empty;

    /// <summary>
    /// 内存限制（如 512(默认MB)）
    /// </summary>
    [JsonPropertyName("memoryLimit")]
    public string MemoryLimit { get; set; } = string.Empty;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public GetProblemResponse() { }

    /// <summary>
    /// 成功响应构造函数（带完整题目信息）
    /// </summary>
    /// <param name="uuid">题目业务唯一标识（如 P1972）</param>
    /// <param name="title">题目标题</param>
    /// <param name="description">题目描述</param>
    /// <param name="inputFormat">输入格式说明</param>
    /// <param name="outputFormat">输出格式说明</param>
    /// <param name="ioExample">题目输入输出样例（二维数组：[输入内容, 输出内容, 样例解释]）</param>
    /// <param name="tags">题目标签列表（如 主席树、莫队）</param>
    /// <param name="difficulty">难度等级（如 提高+/省选-）</param>
    /// <param name="timeLimit">时间限制（如 2000(默认毫秒)）</param>
    /// <param name="memoryLimit">内存限制（如 512(默认MB)）</param>
    /// <param name="message">自定义成功消息</param>
    public GetProblemResponse(
        string uuid,
        string title,
        string description,
        string inputFormat,
        string outputFormat,
        List<List<string>> ioExample,
        List<string> tags,
        string difficulty,
        string timeLimit,
        string memoryLimit,
        string message = "题目详情获取成功"
    ) : base(200, message)
    {
        Uuid = uuid;
        Title = title;
        Description = description;
        InputFormat = inputFormat;
        OutputFormat = outputFormat;
        IoExample = ioExample;
        Tags = tags;
        Difficulty = difficulty;
        TimeLimit = timeLimit;
        MemoryLimit = memoryLimit;
    }

    /// <summary>
    /// 自定义状态码响应构造函数（如题目不存在、数据异常等场景）
    /// </summary>
    /// <param name="statusCode">状态码</param>
    /// <param name="message">错误/提示消息</param>
    public GetProblemResponse(int statusCode, string message) : base(statusCode, message) { }
}
#endregion

#region 添加题目标签相关模型
/// <summary>
/// 添加题目标签请求模型（接收客户端传递的题目uuid和标签）
/// </summary>
public class AddTagOfProblemRequest
{
    /// <summary>
    /// 关联题目的业务唯一标识（uuid）
    /// </summary>
    [Required(ErrorMessage = "题目业务唯一标识（uuid）不能为空")]
    [StringLength(50, ErrorMessage = "题目业务唯一标识（uuid）长度不能超过50个字符")]
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    /// <summary>
    /// 标签名称（如 主席树、莫队）
    /// </summary>
    [Required(ErrorMessage = "标签名称（tag）不能为空")]
    [StringLength(50, ErrorMessage = "标签名称长度不能超过50个字符")]
    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public AddTagOfProblemRequest() { }

    /// <summary>
    /// 带参构造函数（快速初始化标签信息）
    /// </summary>
    /// <param name="uuid">题目uuid</param>
    /// <param name="tag">标签名称</param>
    public AddTagOfProblemRequest(string uuid, string tag)
    {
        Uuid = uuid;
        Tag = tag;
    }
}

/// <summary>
/// 添加题目标签响应模型（返回添加结果给客户端）
/// </summary>
public class AddTagOfProblemResponse : BaseResponse
{
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public AddTagOfProblemResponse() { }

    /// <summary>
    /// 成功响应构造函数（默认状态码200，消息“标签添加成功”）
    /// </summary>
    /// <param name="message">自定义成功消息</param>
    public AddTagOfProblemResponse(string message = "标签添加成功") : base(200, message) { }

    /// <summary>
    /// 自定义状态码响应构造函数（如题目不存在、标签重复等场景）
    /// </summary>
    /// <param name="statusCode">状态码</param>
    /// <param name="message">错误/提示消息</param>
    public AddTagOfProblemResponse(int statusCode, string message) : base(statusCode, message) { }
}
#endregion
}