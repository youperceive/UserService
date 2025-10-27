# InitialThink 后端服务启动脚本

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  InitialThink 后端服务启动脚本" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Docker 是否运行
Write-Host "[1/4] 检查 Docker 状态..." -ForegroundColor Yellow
try {
    docker ps | Out-Null
    Write-Host "✓ Docker 运行正常" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker 未运行，请先启动 Docker Desktop" -ForegroundColor Red
    Write-Host "启动 Docker Desktop 后，请重新运行此脚本" -ForegroundColor Yellow
    pause
    exit 1
}

# 启动数据库服务
Write-Host ""
Write-Host "[2/4] 启动 PostgreSQL 和 Redis..." -ForegroundColor Yellow
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ 数据库服务启动成功" -ForegroundColor Green
    Write-Host "  - PostgreSQL: localhost:5432" -ForegroundColor Gray
    Write-Host "  - Redis: localhost:6379" -ForegroundColor Gray
} else {
    Write-Host "✗ 数据库服务启动失败" -ForegroundColor Red
    pause
    exit 1
}

# 等待数据库就绪
Write-Host ""
Write-Host "[3/4] 等待数据库初始化..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Write-Host "✓ 数据库就绪" -ForegroundColor Green

# 启动 Web 服务
Write-Host ""
Write-Host "[4/4] 可用的服务启动命令：" -ForegroundColor Yellow
Write-Host ""
Write-Host "启动 UserService:" -ForegroundColor Cyan
Write-Host "  cd UserService" -ForegroundColor Gray
Write-Host "  dotnet run --launch-profile http" -ForegroundColor Gray
Write-Host "  访问: http://localhost:5170/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host "启动 ProblemSetService:" -ForegroundColor Cyan
Write-Host "  cd ProblemSetService" -ForegroundColor Gray
Write-Host "  dotnet run --launch-profile http" -ForegroundColor Gray
Write-Host "  访问: http://localhost:5272/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "提示：如需停止数据库服务，运行：" -ForegroundColor Yellow
Write-Host "  docker-compose down" -ForegroundColor Gray
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 询问是否启动 ProblemSetService
$response = Read-Host "是否现在启动 ProblemSetService? (Y/N)"
if ($response -eq 'Y' -or $response -eq 'y') {
    Write-Host ""
    Write-Host "正在启动 ProblemSetService..." -ForegroundColor Yellow
    Set-Location ProblemSetService
    dotnet run --launch-profile http
}

