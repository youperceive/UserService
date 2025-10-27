# 🚀 Rider 启动指南

## ✅ 项目已配置为 x86 架构

项目现在使用 x86 平台目标，可以在您的系统上运行（使用 .NET 8.0.16 x86 运行时）。

---

## 📝 在 Rider 中启动项目

### **第一步：重新加载项目**

1. 在 Rider 中，点击菜单：**File** → **Invalidate Caches**
2. 选择 **Invalidate and Restart**
3. 等待 Rider 重新启动并重新索引项目

或者简单地：
1. 在 **Solution Explorer** 中右键点击解决方案
2. 选择 **Reload Projects**

### **第二步：启动 Docker Desktop**

在启动后端服务之前，必须先启动 Docker Desktop：
1. 在 Windows 开始菜单搜索 "Docker Desktop"
2. 启动并等待完全运行（托盘图标不再转圈）

### **第三步：启动数据库服务**

打开 **PowerShell** 或 Rider 内置终端：

```powershell
cd D:\SourceCode\CSharpProject\WebAPI\UserService
docker-compose up -d
```

### **第四步：配置 Rider 运行配置**

#### **方式 A：使用现有的运行配置**

1. 在 Rider 顶部工具栏，找到运行配置下拉菜单
2. 选择 **ProblemSetService** 或 **UserService**
3. 点击绿色运行按钮 ▶️

#### **方式 B：创建新的运行配置**

1. 点击 **Run** → **Edit Configurations...**
2. 点击 **+** → **Launch Settings Profile**
3. 配置如下：
   - **Project**: `ProblemSetService`
   - **Launch Profile**: `http`
   - **Platform**: `x86`（如果可选）
4. 点击 **OK**
5. 运行配置

### **第五步：验证启动**

启动成功后，在浏览器访问：
- **ProblemSetService**: http://localhost:5272/swagger
- **UserService**: http://localhost:5170/swagger

---

## ⚙️ Rider 平台配置

如果仍然遇到问题，请检查 Rider 的 .NET SDK 配置：

1. 打开 **File** → **Settings** (Windows/Linux) 或 **Preferences** (Mac)
2. 导航到 **Build, Execution, Deployment** → **.NET Core**
3. 确认 **.NET SDK** 路径指向：
   - `C:\Program Files (x86)\dotnet\` （x86 版本）
   
4. 或者在项目属性中明确指定：
   - 右键项目 → **Properties**
   - 查看 **Platform Target** 是否为 **x86**

---

## 🛠️ 使用命令行启动（推荐测试）

如果 Rider 仍有问题，可以先用命令行测试：

```powershell
cd D:\SourceCode\CSharpProject\WebAPI\UserService\ProblemSetService
dotnet run
```

如果命令行成功运行，说明项目配置正确，可能需要重新配置 Rider。

---

## 📊 架构信息

当前配置：
- ✅ 目标框架: `.NET 8.0`
- ✅ 平台目标: `x86`
- ✅ 运行时: `.NET 8.0.16 (x86)`
- ✅ 位置: `C:\Program Files (x86)\dotnet\`

---

## ❗ 故障排除

### 问题 1: Rider 仍提示缺少 x64 运行时

**解决方案：**
1. 关闭 Rider
2. 删除 `.idea` 文件夹（项目缓存）
3. 删除所有 `bin` 和 `obj` 文件夹
4. 重新打开 Rider

### 问题 2: 无法启动调试

**解决方案：**
- 在 Rider 中禁用 "Use the 64-bit version for debugging"
- 路径：**Run** → **Edit Configurations** → 取消勾选相关选项

### 问题 3: Docker 连接失败

**检查：**
```powershell
docker ps  # 查看容器状态
```

**启动容器：**
```powershell
docker-compose up -d
```

---

## 💡 方案二：安装 x64 .NET 8.0 运行时（可选）

如果您想使用 x64 而不是 x86，可以下载安装：

**下载链接：**
https://dotnet.microsoft.com/download/dotnet/8.0

选择：**Runtime 8.0.x** → **Windows x64**

安装后，可以将项目改回 x64：
- 从所有 `.csproj` 文件中删除 `<PlatformTarget>x86</PlatformTarget>`
- 重新构建项目

---

## 📞 需要帮助？

如果遇到其他问题：
1. 检查 Rider 输出窗口的错误信息
2. 查看 `docker-compose logs` 数据库日志
3. 使用命令行 `dotnet run` 测试是否能正常启动

