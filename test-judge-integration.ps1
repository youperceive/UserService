# 评测机集成测试脚本
# 用途：快速测试后端评测接口是否正常工作

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   评测机集成测试脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 检查 Docker 是否运行
Write-Host "[1/5] 检查 Docker 状态..." -ForegroundColor Yellow
$dockerRunning = $false
try {
    $dockerInfo = docker info 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Docker 正在运行" -ForegroundColor Green
        $dockerRunning = $true
    } else {
        Write-Host "❌ Docker 未运行，请启动 Docker Desktop" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ 找不到 Docker 命令，请确认已安装 Docker Desktop" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. 构建项目
Write-Host "[2/5] 构建 ProblemSetService..." -ForegroundColor Yellow
Push-Location ProblemSetService
$buildOutput = dotnet build --verbosity minimal 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ 构建成功" -ForegroundColor Green
} else {
    Write-Host "❌ 构建失败" -ForegroundColor Red
    Write-Host $buildOutput
    Pop-Location
    exit 1
}
Pop-Location

Write-Host ""

# 3. 启动后端服务（后台）
Write-Host "[3/5] 启动后端服务..." -ForegroundColor Yellow
Push-Location ProblemSetService
$backendProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run" -PassThru -WindowStyle Minimized

Write-Host "⏳ 等待服务启动 (15秒)..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# 检查服务是否启动
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5170/swagger/index.html" -Method Get -TimeoutSec 5 -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ 后端服务已启动 (http://localhost:5170)" -ForegroundColor Green
    } else {
        Write-Host "⚠️  后端服务状态异常" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ 无法连接到后端服务" -ForegroundColor Red
    Write-Host "   请手动检查: http://localhost:5170/swagger" -ForegroundColor Yellow
}

Pop-Location
Write-Host ""

# 4. 测试评测接口
Write-Host "[4/5] 测试评测接口..." -ForegroundColor Yellow

$testCode = @"
#include <iostream>
using namespace std;
int main() {
    int a, b;
    cin >> a >> b;
    cout << a + b << endl;
    return 0;
}
"@

$requestBody = @{
    code = $testCode
    lang = "C++"
    input = "1 2"
    timeout = "5"
} | ConvertTo-Json

try {
    Write-Host "📤 发送测试请求: POST http://localhost:5170/api/Judge/run" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri "http://localhost:5170/api/Judge/run" -Method Post -Body $requestBody -ContentType "application/json" -TimeoutSec 60
    
    Write-Host ""
    Write-Host "📥 评测结果:" -ForegroundColor Cyan
    Write-Host "   标准输出: $($response.standardOutput)" -ForegroundColor White
    Write-Host "   错误输出: $($response.errorOutput)" -ForegroundColor White
    Write-Host "   退出码: $($response.exitCode)" -ForegroundColor White
    
    if ($response.exitCode -eq 0 -and $response.standardOutput -eq "3`n") {
        Write-Host ""
        Write-Host "✅ 评测接口工作正常！" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "⚠️  输出不符合预期" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ 评测请求失败" -ForegroundColor Red
    Write-Host "   错误: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "可能的原因:" -ForegroundColor Yellow
    Write-Host "  1. Docker 容器下载中（首次运行需要下载 C++ 镜像）" -ForegroundColor Yellow
    Write-Host "  2. 超时（Docker 镜像较大）" -ForegroundColor Yellow
    Write-Host "  3. 后端服务未完全启动" -ForegroundColor Yellow
}

Write-Host ""

# 5. 总结
Write-Host "[5/5] 测试总结" -ForegroundColor Yellow
Write-Host ""
Write-Host "服务状态:" -ForegroundColor Cyan
Write-Host "  • 后端服务: http://localhost:5170" -ForegroundColor White
Write-Host "  • Swagger UI: http://localhost:5170/swagger" -ForegroundColor White
Write-Host "  • 评测接口: http://localhost:5170/api/Judge/run" -ForegroundColor White
Write-Host ""
Write-Host "下一步:" -ForegroundColor Cyan
Write-Host "  1. 访问 Swagger UI 手动测试其他接口" -ForegroundColor White
Write-Host "  2. 启动前端: cd ..\InitialThinkFrontend && npm run dev" -ForegroundColor White
Write-Host "  3. 在浏览器中测试完整流程" -ForegroundColor White
Write-Host ""
Write-Host "按任意键停止后端服务并退出..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# 停止后端进程
if ($backendProcess -and !$backendProcess.HasExited) {
    Write-Host "正在停止后端服务..." -ForegroundColor Yellow
    Stop-Process -Id $backendProcess.Id -Force
    Write-Host "✅ 后端服务已停止" -ForegroundColor Green
}

Write-Host ""
Write-Host "测试脚本执行完毕！" -ForegroundColor Cyan

