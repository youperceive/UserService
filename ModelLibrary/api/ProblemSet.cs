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
        /// 数据库内部主键（自增，客户端可传空，服务端自动生成）
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

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
}