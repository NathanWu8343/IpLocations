# 專案結構指導文件：IpLocations API

本文檔定義了 IpLocations API 的專案結構、檔案組織、命名慣例與編碼風格，旨在確保程式碼庫的一致性與可維護性。

---

## 1. 目錄結構 (Directory Structure)

專案遵循功能導向的分層結構，將不同職責的程式碼分離到各自的目錄中。

-   `src/IpLocations.Api/`: 主 API 專案
    -   `Controllers/`: 存放 ASP.NET Core API 控制器。每個控制器對應一組相關的 API 端點。
    -   `Infra/`: 基礎設施層。包含與外部服務（如 Redis）或資料來源互動的類別。
    -   `Constants/`: 存放應用程式範圍的常數，例如 Redis Key 的定義。
    -   `Misc/`: 存放通用的工具或輔助類別，例如 `IpConverter`。
    -   `Program.cs`: 應用程式的進入點，負責設定服務、相依性注入與中介軟體管線。
-   `tests/`: 測試專案
    -   `IpLocations.UnitTests/`: 單元測試專案，專注於測試單一類別或方法的邏輯。
    -   `IpLocations.IntegrationTests/`: 整合測試專案，用於測試多個元件協同工作的場景，特別是 API 端點與資料庫的互動。

## 2. 命名慣例 (Naming Conventions)

-   **類別與介面 (PascalCase)**:
    -   控制器: `{Entity}Controller` (例如: `IpLocationsController`)
    -   服務/儲存庫: `{Purpose}Store` (例如: `IpRangeStore`)
    -   常數類別: `{Purpose}Const` (例如: `RedisKeyConst`)
-   **私有欄位 (camelCase with underscore prefix)**:
    -   一律使用底線 `_` 作為前綴 (例如: `_db`, `_ipRangeStore`)。
-   **方法與屬性 (PascalCase)**:
    -   公開的方法與屬性使用 `PascalCase` (例如: `GetCountryAsync`, `IpAddress`)。
-   **方法參數與區域變數 (camelCase)**:
    -   使用具描述性的 `camelCase` 名稱 (例如: `ipNum`, `startIp`)。
-   **測試方法**:
    -   遵循 `MethodName_Should_ExpectedBehavior_When_Condition` 格式，以清晰表達測試的意圖。

## 3. 程式碼範例與模式 (Code Examples & Patterns)

以下範例展示了在專案中應遵循的常見模式。

### 3.1. API 控制器 (Controller)

控制器應保持精簡，其主要職責是接收請求、呼叫業務邏輯服務，並回傳適當的 HTTP 回應。

-   **依賴注入**: 透過建構函式注入所需的服務 (`IpRangeStore`)。
-   **非同步**: `Action` 方法應為 `async Task<IActionResult>`。
-   **結果處理**: 根據服務層的回傳結果，回傳 `Ok()` 或 `NotFound()`。

```csharp
// 位於: src/IpLocations.Api/Controllers/IpLocationsController.cs

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

### 3.2. 基礎設施服務 (Infrastructure Service)

服務類別封裝了與外部系統的互動細節，例如 Redis 查詢。

-   **介面導向**: 透過 `IDatabase` 介面與 Redis 互動。
-   **錯誤處理**: 對於無效輸入，應回傳一個代表失敗的特定值（例如 `string.Empty` 或 `-1`），而非拋出例外。
-   **非同步**: 所有 Redis 操作都應使用非同步方法（例如 `SortedSetRangeByScoreAsync`）。

```csharp
// 位於: src/IpLocations.Api/Infra/IpRangeStore.cs

public class IpRangeStore
{
    private readonly IDatabase _db;

    public IpRangeStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task<string> GetCountryAsync(string ip)
    {
        var ipNum = IpConverter.IpToInt(ip);
        if (ipNum == -1) return string.Empty;

        // 使用 SortedSet 進行高效查詢
        var result = await _db.SortedSetRangeByScoreAsync(GetRedisKey(ipNum), 
            double.NegativeInfinity, ipNum, 
            Exclude.None, Order.Descending, 0, 1);

        if (result.Any())
        {
            // ... 處理查詢結果 ...
        }

        return string.Empty;
    }
}
```

### 3.3. 單元測試 (Unit Test)

單元測試應專注於單一元件的邏輯，並使用參數化測試來涵蓋多種情境。

-   **Arrange-Act-Assert**: 遵循此模式組織測試程式碼。
-   **`[Theory]` 與 `[InlineData]`**: 用於提供多組測試資料。
-   **`FluentAssertions`**: 使用 `Should()` 語法進行斷言。

```csharp
// 位於: tests/IpLocations.UnitTests/IpConverterTests.cs

public class IpConverterTests
{
    [Theory]
    [InlineData("1.160.0.0", 30915840)]
    [InlineData("1.0.1.0", 16777472)]
    public void IpToInt_Should_Return_Correct_Value_When_IpIsValid(string ip, long expected)
    {
        // Arrange & Act
        var result = IpConverter.IpToInt(ip);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("invalid-ip")]
    [InlineData("256.0.0.0")]
    public void IpToInt_Should_Return_NegativeOne_When_IpIsInvalid(string ip)
    {
        // Arrange & Act
        var result = IpConverter.IpToInt(ip);

        // Assert
        result.Should().Be(-1);
    }
}
```

### 3.4. 整合測試 (Integration Test)

整合測試用於驗證 API 端點是否能與後端服務（如 Redis）正確協作。

-   **`WebApplicationFactory`**: 用於在測試環境中啟動應用程式。
-   **`Testcontainers`**: 用於建立一個隔離的 Redis 容器供測試使用。
-   **動態設定**: 在測試啟動時，將 Redis 容器的動態連線字串注入到應用程式設定中。

```csharp
// 位於: tests/IpLocations.IntegrationTests/Abstractions/BaseIntegrationTest.cs

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    protected HttpClient HttpClient { get; private set; }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // 覆寫應用程式設定，使用測試容器的 Redis
                builder.ConfigureAppConfiguration((_, conf) =>
                {
                    conf.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Redis"] = _redisContainer.GetConnectionString()
                    });
                });
            });

        HttpClient = application.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
    }
}
```
