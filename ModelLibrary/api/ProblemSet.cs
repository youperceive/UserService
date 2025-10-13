// Models/ProblemSetModels.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Globalization;
using ModelLibrary.repo;

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

/// <summary>
/// 提交记录数据传输对象（用于前后端交互）
/// </summary>
/// <summary>
    /// 提交记录数据传输对象（用于前后端交互）
    /// </summary>
    public class SubmissionDto
    {
        /// <summary>
        /// 提交对外唯一标识（哈希值）
        /// </summary>
        [JsonPropertyName("id")]
        public string Uuid { get; set; } = string.Empty;

        /// <summary>
        /// 关联用户的对外标识（用户UUID）
        /// </summary>
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 关联题目的对外标识（题目UUID）
        /// </summary>
        [JsonPropertyName("problemId")]
        public string ProblemUuid { get; set; } = string.Empty;

        /// <summary>
        /// 提交时间（格式：Oct/13/2025 11:36）
        /// </summary>
        [JsonPropertyName("when")]
        public string SubmissionTime { get; set; } = string.Empty;

        /// <summary>
        /// 编程语言（如 c++23）
        /// </summary>
        [JsonPropertyName("lang")]
        public string Lang { get; set; } = string.Empty;

        /// <summary>
        /// 提交状态（如 TLE、AC、WA）
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// 运行时间（毫秒）
        /// </summary>
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// 内存使用（MB）
        /// </summary>
        [JsonPropertyName("memory")]
        public string Memory { get; set; } = string.Empty;

        // 新增：实体转DTO的静态方法（统一转换逻辑，方便复用）
        public static SubmissionDto FromEntity(Submission entity)
        {
            if (entity == null)
                return new SubmissionDto();

            return new SubmissionDto
            {
                Uuid = entity.Uuid,
                // 从关联实体取对外UUID（空值防护，避免空引用）
                UserId = entity.User?.Uuid ?? string.Empty,
                ProblemUuid = entity.Problem?.Uuid ?? string.Empty,
                // 时间格式转换：匹配 "Oct/13/2025 11:36" 格式
                SubmissionTime = entity.SubmissionTime.ToString(
                    "MMM/dd/yyyy HH:mm", 
                    CultureInfo.GetCultureInfo("en-US")
                ),
                Lang = entity.Lang,
                State = entity.State,
                // 数值转字符串（匹配前端JSON格式）
                Time = entity.Time.ToString(),
                Memory = entity.Memory.ToString()
            };
        }
    }

    #region 添加提交记录相关模型（完善转换逻辑）
    /// <summary>
    /// 添加提交记录请求模型（保持不变）
    /// </summary>
    public class CreateSubmissionRequest
    {
        /// <summary>
        /// 用户对外标识（用户UUID）
        /// </summary>
        [Required(ErrorMessage = "用户标识（userId）不能为空")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 题目对外标识（题目UUID）
        /// </summary>
        [Required(ErrorMessage = "题目标识（problemId）不能为空")]
        [JsonPropertyName("problemId")]
        public string ProblemUuid { get; set; } = string.Empty;

        /// <summary>
        /// 编程语言（如 c++23）
        /// </summary>
        [Required(ErrorMessage = "编程语言（lang）不能为空")]
        [StringLength(50, ErrorMessage = "编程语言名称长度不能超过50个字符")]
        [JsonPropertyName("lang")]
        public string Lang { get; set; } = string.Empty;

        /// <summary>
        /// 提交的代码内容
        /// </summary>
        [Required(ErrorMessage = "提交代码不能为空")]
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CreateSubmissionRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化提交信息）
        /// </summary>
        public CreateSubmissionRequest(string userId, string problemUuid, string lang, string code)
        {
            UserId = userId;
            ProblemUuid = problemUuid;
            Lang = lang;
            Code = code;
        }
    }

    /// <summary>
    /// 添加提交记录响应模型（完善实体转DTO逻辑）
    /// </summary>
    public class CreateSubmissionResponse : BaseResponse
    {
        /// <summary>
        /// 提交记录信息（仅包含对外展示的字段）
        /// </summary>
        [JsonPropertyName("Submission")]
        public SubmissionDto? Submission { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CreateSubmissionResponse() { }

        /// <summary>
        /// 成功响应构造函数（直接接收DTO）
        /// </summary>
        public CreateSubmissionResponse(SubmissionDto submission, string message = "提交成功") 
            : base(200, message)
        {
            Submission = submission;
        }

        /// <summary>
        /// 自定义状态码响应构造函数（无数据场景）
        /// </summary>
        public CreateSubmissionResponse(int statusCode, string message) 
            : base(statusCode, message) { }
        
        /// <summary>
        /// 完善：接收数据库实体，自动转为DTO（Service层调用）
        /// </summary>
        public CreateSubmissionResponse(int statusCode, string message, Submission submissionEntity) 
            : base(statusCode, message)
        {
            // 调用静态转换方法，确保字段映射完整
            this.Submission = SubmissionDto.FromEntity(submissionEntity);
        }
    }
    #endregion

    #region 获取提交记录相关模型（补充转换复用）
    /// <summary>
    /// 获取单个提交记录请求模型（保持不变）
    /// </summary>
    public class GetSubmissionRequest
    {
        /// <summary>
        /// 提交对外唯一标识（哈希值）
        /// </summary>
        [Required(ErrorMessage = "提交标识（id）不能为空")]
        [JsonPropertyName("id")]
        public string SubmissionUuid { get; set; } = string.Empty;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetSubmissionRequest() { }

        /// <summary>
        /// 带参构造函数（快速初始化提交UUID）
        /// </summary>
        public GetSubmissionRequest(string submissionUuid)
        {
            SubmissionUuid = submissionUuid;
        }
    }

    /// <summary>
    /// 获取提交记录列表请求模型（保持不变）
    /// </summary>
    public class GetSubmissionListRequest
    {
        /// <summary>
        /// 用户对外标识（可选，为空时查询所有用户提交）
        /// </summary>
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        /// <summary>
        /// 题目对外标识（可选，为空时查询所有题目提交）
        /// </summary>
        [JsonPropertyName("problemId")]
        public string? ProblemUuid { get; set; }

        /// <summary>
        /// 分页页码（默认第1页）
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页条数（默认10条）
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// 获取提交记录响应模型（补充实体转DTO构造）
    /// </summary>
    public class GetSubmissionResponse : BaseResponse
    {
        /// <summary>
        /// 提交记录详情
        /// </summary>
        [JsonPropertyName("Submission")]
        public SubmissionDto? Submission { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetSubmissionResponse() { }

        /// <summary>
        /// 成功响应构造函数（接收DTO）
        /// </summary>
        public GetSubmissionResponse(SubmissionDto submission, string message = "查询成功") 
            : base(200, message)
        {
            Submission = submission;
        }

        /// <summary>
        /// 新增：接收实体自动转DTO（简化Service层调用）
        /// </summary>
        public GetSubmissionResponse(Submission submissionEntity, string message = "查询成功") 
            : base(200, message)
        {
            this.Submission = SubmissionDto.FromEntity(submissionEntity);
        }

        /// <summary>
        /// 自定义状态码响应构造函数
        /// </summary>
        public GetSubmissionResponse(int statusCode, string message) 
            : base(statusCode, message) { }
    }

    /// <summary>
    /// 获取提交记录列表响应模型（补充批量转换）
    /// </summary>
    public class GetSubmissionListResponse : BaseResponse
    {
        /// <summary>
        /// 提交记录列表
        /// </summary>
        [JsonPropertyName("submissions")]
        public List<SubmissionDto> Submissions { get; set; } = new List<SubmissionDto>();

        /// <summary>
        /// 总记录数
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; } = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GetSubmissionListResponse() { }

        /// <summary>
        /// 成功响应构造函数（接收DTO列表）
        /// </summary>
        public GetSubmissionListResponse(List<SubmissionDto> submissions, int total, int pageSize, 
                                        string message = "查询成功") : base(200, message)
        {
            Submissions = submissions;
            Total = total;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        }

        /// <summary>
        /// 新增：接收实体列表，批量转为DTO（简化Service层调用）
        /// </summary>
        public GetSubmissionListResponse(List<Submission> submissionEntities, int total, int pageSize, 
                                        string message = "查询成功") : base(200, message)
        {
            // 批量转换：调用静态方法，避免重复代码
            this.Submissions = submissionEntities.Select(SubmissionDto.FromEntity).ToList();
            this.Total = total;
            this.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        }

        /// <summary>
        /// 自定义状态码响应构造函数
        /// </summary>
        public GetSubmissionListResponse(int statusCode, string message) 
            : base(statusCode, message) { }
    }
    #endregion
}

