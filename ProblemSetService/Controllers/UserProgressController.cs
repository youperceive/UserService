using Microsoft.AspNetCore.Mvc;
using ModelLibrary.api;
using System.Text.Json;

namespace ProblemSetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProgressController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    
    public UserProgressController(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    /// <summary>
    /// 获取用户进度数据（热力图数据）
    /// </summary>
    [HttpGet("{userId}")]
    public ActionResult<ProgressDataResponse> GetProgressData(string userId)
    {
        try
        {
            // 方法1：尝试读取文件
            var jsonPath = Path.Combine(_env.ContentRootPath, "Data", "progressData.json");
            
            ProgressData? progressData = null;
            
            if (System.IO.File.Exists(jsonPath))
            {
                try
                {
                    var jsonContent = System.IO.File.ReadAllText(jsonPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    progressData = JsonSerializer.Deserialize<ProgressData>(jsonContent, options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取进度文件失败，使用假数据: {ex.Message}");
                }
            }
            
            // 方法2：如果文件不存在或读取失败，使用简化的假数据
            if (progressData == null)
            {
                progressData = GenerateFakeProgressData();
            }
            
            var response = new ProgressDataResponse
            {
                StatusCode = 200,
                Message = "获取成功",
                Data = progressData
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取进度数据异常: {ex.Message}");
            Console.WriteLine($"堆栈: {ex.StackTrace}");
            return StatusCode(500, new BaseResponse(500, $"服务器错误: {ex.Message}"));
        }
    }
    
    /// <summary>
    /// 生成假的进度数据
    /// </summary>
    private ProgressData GenerateFakeProgressData()
    {
        var yearData = new List<DayRecord>();
        var random = new Random();
        var startDate = new DateTime(2025, 1, 1);
        
        // 生成365天的数据
        for (int i = 0; i < 365; i++)
        {
            var date = startDate.AddDays(i);
            yearData.Add(new DayRecord
            {
                Date = date.ToString("yyyy-MM-dd"),
                Solved = random.Next(0, 10) // 随机0-9题
            });
        }
        
        return new ProgressData
        {
            Year = 2025,
            Month = null,
            Day = null,
            YearData = yearData,
            MonthData = null,
            DayData = null
        };
    }
}

#region Response Models

public class ProgressDataResponse : BaseResponse
{
    public ProgressData? Data { get; set; }
}

public class ProgressData
{
    public int Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }
    public List<DayRecord>? YearData { get; set; }
    public Dictionary<string, List<MonthRecord>>? MonthData { get; set; }
    public Dictionary<string, List<HourRecord>>? DayData { get; set; }
}

public class DayRecord
{
    public string Date { get; set; } = string.Empty;
    public int Solved { get; set; }
}

public class MonthRecord
{
    public string Day { get; set; } = string.Empty;
    public int Solved { get; set; }
}

public class HourRecord
{
    public string Hour { get; set; } = string.Empty;
    public int Solved { get; set; }
}

#endregion
