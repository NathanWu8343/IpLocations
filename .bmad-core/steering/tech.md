## 專案類型
ASP.NET Core 8.0 單體式 REST API 服務，提供後端系統使用的 IPv4 → 國家/地區查詢能力。

## 版本歷史

**CRITICAL**：每次更新請於表格下方新增一列，**不得覆蓋舊有紀錄**

| 日期         | 版本號   | 描述             | 作者             |
| ------------ | -------- | ---------------- | ---------------- |
| 2025-09-30   | 1.2      | 大幅增強套件最佳實踐章節，整合 Context7 MCP 查詢結果，新增 7 個主要套件的詳細使用指引與程式碼範例 | GitHub Copilot   |
| 2025-09-30   | 1.1      | 建立完整技術指引 | GitHub Copilot   |
| 2025-09-29   | 1.0      | 初版建立         | GPT-5            |

## 核心技術

### 主要語言
- **語言**: C# 12
- **執行環境**: .NET 8.0 (SDK ≥ 8.0.302)
- **語言特定工具**: `dotnet` CLI（建置、還原、測試）、隱含 usings 與可為 Null 參考型別已啟用

### 主要依賴/函式庫
- **ASP.NET Core 8.0**: Minimal Hosting Web API 框架。
- **StackExchange.Redis 2.8.0**: Redis 連線與 Sorted Set 範圍查詢。
- **Swashbuckle.AspNetCore 6.4.0**: 產生 OpenAPI/Swagger 文件。
- **xUnit 2.5.3 / 2.8.1**: 單元與整合測試框架。
- **FluentAssertions 6.12.0**: Fluent 斷言工具。
- **Microsoft.AspNetCore.Mvc.Testing 8.0.6**: 建立 `WebApplicationFactory` 供整合測試。
- **Testcontainers.Redis 3.8.0**: 測試中啟動 Docker 化 Redis。
- **BenchmarkDotNet 0.13.12**: 執行 `IpToInt` 效能基準。
- **coverlet.collector 6.x**: 產生測試覆蓋率。
- **Microsoft.NET.Test.Sdk 17.x**: 驅動 .NET 測試執行。
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.20.1**: Visual Studio 容器化支援。

### 應用程式架構
- 單體式 REST API，採 ASP.NET Core 最小主機模型。
- 依照分層概念區分 Controller、基礎設施 (`Infra`)、工具 (`Misc`)、常數 (`Constants`)。
- DI 以建構函式注入，使用 Singleton Redis multiplexer + Singleton `IpRangeStore`。

### 資料儲存
- **主要儲存**: Redis（以 Sorted Set 儲存 IP 範圍與國碼）。
- **快取**: Redis 即為主要資料來源；應用程式本身不維護額外快取。
- **資料格式**: 儲存為 `startIp-endIp:countryCode` 字串。

### 外部整合
- **Redis**: 透過 `StackExchange.Redis` 連線，支援本地或遠端 Redis 實例。
- **協定**: HTTP/REST 對外提供服務；Redis 使用 TCP 端口。
- **驗證**: API 尚未實作驗證；Redis 連線透過組態字串管理。
- **関連檔案**: `Program.cs`, `Infra/IpRangeStore.cs`, `Constants/RedisKeyConst.cs`。

### 監控與儀表板技術
- 目前無專屬儀表板。建議整合 ASP.NET Core 日誌與 Redis 監控（INFO、慢查詢）。
- 可與現有 APM（如 Application Insights、Prometheus）或集中式日誌平台接合。

## 開發環境

### 建置與開發工具
- **建置系統**: `dotnet build`
- **套件管理**: `dotnet restore` (NuGet)
- **開發流程**: 透過 Visual Studio / VS Code / Rider；支援 `dotnet watch` 進行熱重載。

### 程式碼品質工具
- **測試框架**: xUnit + FluentAssertions
- **覆蓋率**: coverlet.collector
- **靜態分析/格式化**: 依賴 .NET SDK 預設分析器；未指定額外格式化工具（建議使用 `dotnet format`）。
- **文件**: Swashbuckle 生成 API 說明；建議後續補充 XML comments。

### 版本控制與協作
- **VCS**: Git（儲存在 GitHub）
- **分支策略**: 目前以功能分支開發（如 `feature/*`, `newx`）；建議遵循 GitHub Flow。
- **程式碼審查**: 透過 Pull Request 進行；需確保測試通過後合併。

### 儀表板開發 (如適用)
- 此專案無前端儀表板；若後續導入，建議獨立模組存放以維持 API 純淨。

## 部署與分發
- **目標平台**: Linux 容器或 Windows 伺服器上的 .NET 8 Runtime。
- **分發方式**: Docker（`src/IpLocations.Api/Dockerfile`）或直接使用 `dotnet publish` 部署。
- **安裝需求**: .NET 8 Runtime、Redis 服務、設定連線字串。
- **更新機制**: 透過 CI/CD 管線重新部署容器或應用程式。

## 技術需求與限制

### 效能需求
- API P95 回應時間 ≤ 30 ms；Warm-up 完成後查詢應為 O(log n)。
- Redis 查詢需保持非阻塞，避免同步 API。

### 相容性需求
- **平台支援**: .NET 8 (Windows/Linux x64)。
- **依賴版本**: 需維持與目前 NuGet 版本相容；升級前需檢驗 context7 建議。
- **標準**: RESTful API, JSON 回應, ISO Alpha-2 國碼。

### 安全性與合規性
- 目前未實作驗證；部署時建議置於受保護網路或經由 API Gateway。
- 傳輸層建議強制 HTTPS；Redis 建議使用 TLS 與密碼。

### 可擴展性與可靠性
- **預期負載**: 每秒數百次查詢（內部應用）。
- **可用性需求**: 99.9% SLA。
- **擴充**: 橫向擴充 API 節點，共享外部 Redis；需考慮資料載入同步。

## 技術決策與理由
1. **使用 Redis Sorted Set 儲存 IP 範圍**：支援範圍查詢且可快速 warm-up；替代方案（SQL、Trie）成本較高。
2. **Minimal Hosting + Controller**：保持 ASP.NET Core 8 既有模式，可與 Dependency Injection 無縫整合。
3. **Testcontainers.Redis 用於整合測試**：確保測試環境與實際部署一致，避免手動安裝 Redis。

## 已知限制
- **IPv6 未支援**：僅處理 IPv4；未來需調整資料模型與 `IpConverter` 演算法。
- **資料載入流程手動**：目前 warm-up 寫死在程式碼內，缺乏自動化與外部資料來源。
- **監控不足**：缺少正式的 APM/儀表板與告警機制。

## 套件最佳實踐

本節整合 Context7 MCP 查詢結果，提供各套件在本專案中的具體使用指引與程式碼模式。

### ASP.NET Core 8.0
**Context7 來源**：`/dotnet/aspnetcore.docs`

#### Minimal Hosting 模式
專案採用 Minimal Hosting 模型，所有配置集中在 `Program.cs`：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 註冊服務
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnection));
builder.Services.AddSingleton<IpRangeStore>();

var app = builder.Build();

// 配置中間件管線
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 啟動時初始化
using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IpRangeStore>().WarmUp();

app.Run();
```

**關鍵實踐**：
- 使用 `builder.Services` 註冊所有依賴項
- 透過 `CreateScope()` 在 `app.Build()` 後但 `app.Run()` 前執行初始化
- 避免在 `WebApplication.CreateBuilder()` 前進行複雜邏輯

#### Controller 設計模式
```csharp
[Route("api/[controller]")]
[ApiController]
public class IpLocationsController : ControllerBase
{
    private readonly IpRangeStore _ipRangeStore;

    public IpLocationsController(IpRangeStore ipRangeStore)
    {
        _ipRangeStore = ipRangeStore;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string ip)
    {
        var result = await _ipRangeStore.GetCountryAsync(ip);
        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }
}
```

**關鍵實踐**：
- 使用 `[ApiController]` 屬性啟用自動模型驗證與問題詳細資訊
- 建構函式注入所有依賴項
- 非同步方法回傳 `Task<IActionResult>`
- 使用 `NotFound()`、`Ok()` 等方法回傳標準 HTTP 狀態

#### 整合測試配置
```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 替換 Redis 連線為測試容器
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddSingleton<IConnectionMultiplexer>(_testRedisConnection);
        });
    }
}
```

### StackExchange.Redis 2.8.0
**Context7 來源**：`/stackexchange/stackexchange.redis`

#### 連線管理
```csharp
// 註冊為 Singleton（重要：整個應用程式生命週期共享）
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(configuration["Redis"] 
        ?? throw new ArgumentNullException("Redis configuration is missing.")));

// 使用時透過 DI 注入
public class IpRangeStore
{
    private readonly IDatabase _db;

    public IpRangeStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
}
```

#### Sorted Set 範圍查詢（專案核心功能）
```csharp
public async Task<string?> GetCountryAsync(string ip)
{
    var ipNum = IpConverter.IpToInt(ip);
    if (ipNum == -1) return null;

    // 使用 SortedSetRangeByScoreAsync 進行範圍查詢
    var ranges = await _db.SortedSetRangeByScoreAsync(
        key: RedisKeyConst.IpToCountryRange,
        start: ipNum,
        stop: ipNum,
        take: 1,
        flags: CommandFlags.None
    );

    if (ranges.Length == 0) return null;

    var parts = ranges[0].ToString().Split(':');
    if (parts.Length != 2) return null;

    var rangeParts = parts[0].Split('-');
    if (rangeParts.Length != 2) return null;

    var startIp = long.Parse(rangeParts[0]);
    var endIp = long.Parse(rangeParts[1]);

    return (ipNum >= startIp && ipNum <= endIp) ? parts[1] : null;
}
```

**關鍵實踐**：
- 所有 Redis 操作必須使用 `async/await`（如 `SortedSetRangeByScoreAsync`）
- 使用 `CommandFlags.None` 為預設行為；需要發後不理時使用 `CommandFlags.FireAndForget`
- Sorted Set 的 score 用於範圍查詢，value 儲存複合資料（`startIp-endIp:countryCode`）
- 避免同步呼叫（不使用 `.Wait()` 或 `.Result`）

#### 資料寫入與批次操作
```csharp
public void WarmUp()
{
    // 批次寫入範例資料
    var entries = new[]
    {
        new SortedSetEntry($"{IpConverter.IpToInt("1.0.0.0")}-{IpConverter.IpToInt("1.0.0.255")}:US", 
                           IpConverter.IpToInt("1.0.0.0")),
        // ... 更多項目
    };

    _db.SortedSetAdd(RedisKeyConst.IpToCountryRange, entries, CommandFlags.FireAndForget);
}
```

**最佳化建議**：
- 批次操作優於逐筆操作（使用陣列版本的 `SortedSetAdd`）
- Warm-up 可使用 `FireAndForget` 加速啟動（除非需要確認成功）
- 考慮使用 `IAsyncDisposable` 釋放連線（雖然 Singleton 通常不需要）

### Swashbuckle.AspNetCore 6.4.0
**Context7 來源**：`/domaindrivendev/swashbuckle.aspnetcore`

#### 基本配置
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "IpLocations API", 
        Version = "v1",
        Description = "IPv4 位址對應國家/地區查詢服務"
    });

    // 整合 XML 註解
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // 啟用註解支援
    c.EnableAnnotations();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IpLocations API V1");
    c.RoutePrefix = string.Empty; // Swagger UI 作為根路徑
});
```

#### Controller XML 註解範例
```csharp
/// <summary>
/// IP to Country
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IpLocationsController : ControllerBase
{
    /// <summary>
    /// 查詢 IPv4 位址對應的國家代碼
    /// </summary>
    /// <param name="ip">IPv4 位址（例如：1.0.0.1）</param>
    /// <returns>ISO Alpha-2 國家代碼</returns>
    /// <response code="200">成功回傳國家代碼</response>
    /// <response code="404">找不到對應的國家資訊</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string ip)
    {
        var result = await _ipRangeStore.GetCountryAsync(ip);
        return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
    }
}
```

**關鍵實踐**：
- `.csproj` 中啟用 `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- 使用 `<summary>`、`<param>`、`<returns>`、`<response>` 標籤
- `ProducesResponseType` 屬性補充型別資訊

### xUnit 2.5.3 / 2.8.1
**Context7 來源**：`/xunit/xunit`

#### 參數化測試
```csharp
public class IpConverterTests
{
    [Theory]
    [InlineData("192.168.1.1", 3232235777)]
    [InlineData("10.0.0.1", 167772161)]
    [InlineData("0.0.0.0", 0)]
    [InlineData("255.255.255.255", 4294967295)]
    [InlineData("invalid", -1)]
    public void IpToInt_Should_ConvertCorrectly_When_GivenValidInput(string ip, long expected)
    {
        // Act
        var result = IpConverter.IpToInt(ip);

        // Assert
        result.Should().Be(expected);
    }
}
```

#### 非同步測試
```csharp
[Fact]
public async Task GetCountryAsync_Should_ReturnCountryCode_When_IpInRange()
{
    // Arrange
    var store = new IpRangeStore(_redis);
    store.WarmUp();

    // Act
    var result = await store.GetCountryAsync("1.0.0.1");

    // Assert
    result.Should().Be("US");
}
```

**關鍵實踐**：
- 使用 `[Theory]` 搭配 `[InlineData]` 測試多組輸入
- 非同步測試方法回傳 `async Task`
- 遵循 Arrange-Act-Assert 模式

### FluentAssertions 6.12.0
**Context7 來源**：`/fluentassertions/fluentassertions`

#### 基本斷言
```csharp
// 值比較
result.Should().Be(expected);
result.Should().NotBeNull();

// 字串斷言
result.Should().StartWith("US");
result.Should().MatchRegex(@"^[A-Z]{2}$");

// 集合斷言
ranges.Should().HaveCount(1);
ranges.Should().ContainSingle(r => r.ToString().Contains("US"));
```

#### 非同步異常斷言
```csharp
[Fact]
public async Task GetCountryAsync_Should_NotThrow_When_ValidIp()
{
    // Arrange
    var store = new IpRangeStore(_redis);
    Func<Task> act = async () => await store.GetCountryAsync("1.0.0.1");

    // Assert
    await act.Should().NotThrowAsync();
}

[Fact]
public async Task GetCountryAsync_Should_ReturnNull_When_InvalidIp()
{
    // Arrange
    var store = new IpRangeStore(_redis);

    // Act
    var result = await store.GetCountryAsync("invalid");

    // Assert
    result.Should().BeNull();
}
```

**關鍵實踐**：
- 使用 `.Should()` 擴充方法提升可讀性
- 非同步操作使用 `NotThrowAsync()`、`ThrowAsync<T>()`
- 避免使用 v5 的同步斷言方式（會導致死鎖）

### Testcontainers for .NET 3.8.0
**Context7 來源**：`/testcontainers/testcontainers-dotnet`

#### Redis 容器配置
```csharp
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.2")  // ⚠️ 固定版本確保可重現
        .Build();

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString()
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
```

#### 動態埠映射
```csharp
// ❌ 錯誤：靜態埠綁定（會導致衝突）
// .WithPortBinding(6379, 6379)

// ✅ 正確：動態埠映射
var container = new ContainerBuilder()
    .WithImage("redis:7.2")
    .WithPortBinding(6379, true)  // true 表示動態分配主機埠
    .Build();

await container.StartAsync();

// 取得實際映射的埠
var port = container.GetMappedPublicPort(6379);
var connectionString = $"{container.Hostname}:{port}";
```

**關鍵實踐**：
- **必須**固定映像版本（`.WithImage("redis:7.2")`）確保測試可重現
- 使用動態埠避免衝突（`WithPortBinding(port, true)` + `GetMappedPublicPort(port)`）
- 實作 `IAsyncLifetime` 管理容器生命週期
- 從容器連線時使用 `container.Hostname` 而非 `localhost`

### BenchmarkDotNet 0.13.12
**Context7 來源**：`/dotnet/benchmarkdotnet`

#### 基準測試配置
```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class IpToIntCalculationBenchmark
{
    private string _testIp = "192.168.1.100";

    [GlobalSetup]
    public void GlobalSetup()
    {
        // 一次性初始化，在所有 benchmark 前執行
        _testIp = "192.168.1.100";
    }

    [Benchmark(Baseline = true)]
    public long Default()
    {
        return IpConverter.IpToInt(_testIp);
    }

    [Benchmark]
    public long Alternative()
    {
        // 替代實作
        return AlternativeIpConverter.IpToInt(_testIp);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        // 所有 benchmark 後執行清理
    }
}
```

**關鍵實踐**：
- `[MemoryDiagnoser]`：分析記憶體分配（Gen0/Gen1/Gen2、Allocated）
- `[SimpleJob(RuntimeMoniker.Net80)]`：指定執行的 .NET 版本
- `[GlobalSetup]`/`[GlobalCleanup]`：管理共享資源初始化與釋放
- `[Benchmark(Baseline = true)]`：設定基準線以顯示相對效能
- 避免在 benchmark 方法內進行初始化（會影響測量準確性）

### 無 Context7 資源的套件

以下套件無 Context7 文件，遵循官方文件與專案既有慣例：

- **Microsoft.NET.Test.Sdk 17.x**：測試執行引擎，無需特殊配置
- **coverlet.collector 6.x**：測試覆蓋率收集，透過 `dotnet test --collect:"XPlat Code Coverage"` 使用
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.20.1**：Visual Studio 容器化支援，自動整合
- **FluentAssertions.Analyzers 0.32.0**：靜態分析器，提供編譯時建議
- **Microsoft.AspNetCore.Mvc.Testing 8.0.6**：已透過上述 ASP.NET Core 章節涵蓋

升級這些套件時，請參考官方 NuGet 頁面與 GitHub 儲存庫的 CHANGELOG。
