# Project Structure

## 版本歷史

| 日期         | 版本號   | 描述             | 作者             |
| ------------ | -------- | ---------------- | ---------------- |
| 2025-10-03   | 1.0      | 初版建立         | Claude 3.5 Sonnet |

## 目錄組織

```
IpLocations/
├── .bmad-core/                      # AI 協作專案管理
│   ├── specs/                       # 需求規格文件
│   ├── steering/                    # 指導文件 (本文件所在)
│   └── templates/                   # 文件範本
│
├── .github/                         # GitHub 專案設定
│   └── prompts/                     # AI 提示詞範本
│
├── src/                             # 主要原始碼
│   └── IpLocations.Api/             # Web API 專案
│       ├── Controllers/             # API 控制器
│       │   └── IpLocationsController.cs
│       ├── Infra/                   # 基礎設施層
│       │   └── IpRangeStore.cs
│       ├── Misc/                    # 工具類別
│       │   └── IpConverter.cs
│       ├── Constants/               # 常數定義
│       │   └── RedisKeyConst.cs
│       ├── Models/                  # 資料模型 (待擴充)
│       ├── Properties/              # 專案屬性
│       │   └── launchSettings.json
│       ├── appsettings.json         # 應用程式設定
│       ├── appsettings.Development.json
│       ├── Dockerfile               # 容器化設定
│       ├── IpLocations.Api.http     # HTTP 測試檔案
│       └── Program.cs               # 應用程式進入點
│
├── tests/                           # 測試專案
│   ├── IpLocations.UnitTests/       # 單元測試
│   │   ├── IpConverterTests.cs
│   │   └── IpLocations.UnitTests.csproj
│   │
│   ├── IpLocations.IntegrationTests/ # 整合測試
│   │   ├── Abstractions/            # 測試基礎類別
│   │   │   ├── BaseIntegrationTest.cs
│   │   │   └── IntegrationTestWebAppFactory.cs
│   │   ├── IpLocations/             # 功能測試
│   │   │   └── GetIpLocationsTests.cs
│   │   └── IpLocations.IntegrationTests.csproj
│   │
│   └── Benchmark/                   # 效能基準測試
│       ├── IpToIntCalculationBenchmark.cs
│       ├── Program.cs
│       └── Benchmark.csproj
│
├── IpLocations.sln                  # 解決方案檔案
├── README.md                        # 專案說明文件
├── .gitignore                       # Git 忽略檔案
└── .dockerignore                    # Docker 忽略檔案
```

**目錄組織原則**:
- **按功能分組**: `Controllers`, `Infra`, `Misc` 分別代表不同職責
- **按測試類型分組**: `UnitTests`, `IntegrationTests`, `Benchmark` 區分測試範疇
- **設定檔案分離**: 開發與生產環境設定分開管理

## 命名慣例

### 目錄
- **主要目錄**: 使用 PascalCase (如 `Controllers`, `Infra`)
- **特殊目錄**: 
  - `.bmad-core`: 使用 kebab-case (隱藏目錄)
  - `bin`, `obj`: .NET 標準建置目錄 (小寫)

### 檔案
- **類別檔案**: PascalCase + `.cs` (如 `IpLocationsController.cs`)
- **介面檔案**: `I{Name}.cs` (如 `IIpRangeStore.cs` - 若未來抽象化)
- **測試檔案**: `{ClassName}Tests.cs` (如 `IpConverterTests.cs`)
- **設定檔案**: 
  - `appsettings.json`, `appsettings.{Environment}.json`
  - `launchSettings.json`
- **容器檔案**: `Dockerfile`, `.dockerignore`

### 程式碼命名

**類別/類型**: PascalCase
```csharp
public class IpLocationsController { }
public class IpRangeStore { }
public static class IpConverter { }
```

**函式/方法**: PascalCase
```csharp
public async Task<IActionResult> Get(string ip) { }
public async Task<string> GetCountryAsync(string ip) { }
public static long IpToInt(string ip) { }
```

**常數**: PascalCase (C# 慣例)
```csharp
public const string IpToCountryRange = "ip-to-country:range-{0}";
```

**變數**: camelCase
```csharp
var ipNum = IpConverter.IpToInt(ip);
var redisConnection = builder.Configuration.GetConnectionString("Redis");
```

**私有欄位**: 前綴 `_` + camelCase
```csharp
private readonly IDatabase _db;
private readonly IpRangeStore _ipRangeStore;
```

**介面**: 前綴 `I` + PascalCase
```csharp
public interface IConnectionMultiplexer { }
public interface IDatabase { }
```

**命名空間**: PascalCase，對應專案結構
```csharp
namespace IpLocations.Api.Controllers { }
namespace IpLocations.Api.Infra { }
namespace IpLocations.Api.Misc { }
```

**參數**: camelCase
```csharp
public IpLocationsController(IpRangeStore ipRangeStore) { }
public async Task<string> GetCountryAsync(string ip) { }
```

## 匯入模式

### 匯入順序
```csharp
// 1. 系統命名空間 (System.*)
using System;
using System.Net;
using System.Text.RegularExpressions;

// 2. 外部套件命名空間 (依字母順序)
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

// 3. 專案內部命名空間 (依字母順序)
using IpLocations.Api.Constants;
using IpLocations.Api.Infra;
using IpLocations.Api.Misc;
```

### 模組/套件組織
- 使用專案參考 (`<ProjectReference>`) 而非 DLL 參考
- 測試專案參考主專案: `IpLocations.Api.csproj`
- 使用 `ImplicitUsings` 自動匯入常用命名空間:
  ```xml
  <ImplicitUsings>enable</ImplicitUsings>
  ```

## 程式碼結構模式

### 類別組織
```csharp
namespace IpLocations.Api.Controllers
{
    // 1. XML 文件註解 (如適用)
    /// <summary>
    /// IP to Country
    /// </summary>

    // 2. 類別屬性
    [Route("api/[controller]")]
    [ApiController]

    // 3. 類別宣告
    public class IpLocationsController : ControllerBase
    {
        // 4. 私有欄位
        private readonly IpRangeStore _ipRangeStore;

        // 5. 建構函式
        public IpLocationsController(IpRangeStore ipRangeStore)
        {
            _ipRangeStore = ipRangeStore;
        }

        // 6. 公開方法
        [HttpGet]
        public async Task<IActionResult> Get(string ip)
        {
            // 實作
        }

        // 7. 私有方法 (如有需要)
        private void SomeHelperMethod() { }
    }
}
```

### 方法組織
```csharp
public async Task<IActionResult> Get(string ip)
{
    // 1. 輸入驗證 (當前未實作，建議未來加入)
    
    // 2. 核心邏輯
    var result = await _ipRangeStore.GetCountryAsync(ip);
    
    // 3. 結果處理與回應
    return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
}
```

### 靜態工具類別
```csharp
namespace IpLocations.Api.Misc
{
    public static class IpConverter
    {
        // 純函式設計，無狀態
        public static long IpToInt(string ip)
        {
            // 1. 驗證
            if (!Regex.IsMatch(ip, ipv4Pattern)) 
                return -1;
            
            // 2. 轉換邏輯
            var octets = ip.Split('.').Select(long.Parse).ToArray();
            
            // 3. 返回結果
            return octets[0] << 24 | octets[1] << 16 | octets[2] << 8 | octets[3];
        }
    }
}
```

## 程式碼組織原則

1. **單一職責**: 每個類別應有單一明確目的
   - `IpLocationsController`: 僅處理 HTTP 協調
   - `IpRangeStore`: 僅處理 IP 查詢邏輯
   - `IpConverter`: 僅處理 IP 格式轉換

2. **模組化**: 程式碼組織成可重用模組
   - 工具類別 (`Misc`) 設計為靜態方法，可獨立重用
   - 基礎設施層 (`Infra`) 可抽象為介面以支援不同實作

3. **可測試性**: 建構易於測試的程式碼
   - 依賴注入設計，便於替換為 Mock 物件
   - 純函式設計 (`IpConverter`)，易於單元測試

4. **一致性**: 遵循程式碼庫中已建立的模式
   - 所有 Controller 繼承 `ControllerBase`
   - 所有 async 方法使用 `Async` 後綴
   - 所有欄位使用 `_` 前綴

## 測試結構模式

### 測試檔案組織
```csharp
using FluentAssertions;
using IpLocations.Api.Misc;

namespace IpLocations.UnitTests
{
    // 1. 測試類別命名: {ClassName}Tests
    public class IpConverterTests
    {
        // 2. 測試方法使用 [Fact] 或 [Theory]
        // 3. 方法命名: MethodName_Should_ExpectedBehavior_When_Condition
        
        [Theory]
        [InlineData("192.168.0.1", 3232235521)]
        public void IpToInt_Should_ReturnExpectedValue_WhenIpIsValid(string ip, long expected)
        {
            // Arrange (準備)
            // (參數已由 InlineData 提供)
            
            // Act (執行)
            var result = IpConverter.IpToInt(ip);
            
            // Assert (斷言)
            result.Should().Be(expected);
        }
    }
}
```

### 測試案例組織 (AAA 模式)
```csharp
[Fact]
public async Task Create_Should_ReturnOk_WhenRequestIsValid()
{
    // Arrange (準備): 設定測試資料與環境
    // (在此例中透過 BaseIntegrationTest 建構函式完成)
    
    // Act (執行): 執行待測試的操作
    HttpResponseMessage response = await HttpClientStub.GetAsync(
        requestUri: $"api/IpLocations?ip={ip}");
    
    // Assert (斷言): 驗證結果
    // Assert (HTTP)
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    // Assert (HTTP Content Response)
    var result = await response.Content.ReadAsStringAsync();
    result.Should().Be(expected);
}
```

### 測試命名慣例

**採用**: `MethodName_Should_ExpectedBehavior_When_Condition`

**範例**:
```csharp
// 正面案例
IpToInt_Should_ReturnExpectedValue_WhenIpIsValid
Create_Should_ReturnOk_WhenRequestIsValid

// 負面案例  
IpToInt_Should_ReturnNegativeOne_WhenIpFormatIsInvalid
```

### 測試分類與組織

**按類型分類**:
```
tests/
├── IpLocations.UnitTests/           # 單元測試
│   └── IpConverterTests.cs          # 測試純函式邏輯
│
├── IpLocations.IntegrationTests/    # 整合測試
│   ├── Abstractions/                # 測試基礎設施
│   └── IpLocations/                 # 功能測試 (對應 API 端點)
│
└── Benchmark/                       # 效能測試
    └── IpToIntCalculationBenchmark.cs
```

**按功能分類**:
- 整合測試目錄結構對應 API 端點結構
- `IpLocations/` 對應 `/api/IpLocations` 端點

### 測試資料管理

**單元測試**: 使用 `[InlineData]` 提供測試資料
```csharp
[Theory]
[InlineData("192.168.0.1", 3232235521)]
[InlineData("127.0.0.1", 2130706433)]
[InlineData("0.0.0.0", 0)]
public void IpToInt_Should_ReturnExpectedValue_WhenIpIsValid(string ip, long expected)
```

**整合測試**: 使用 Testcontainers 提供真實 Redis 環境
```csharp
private readonly RedisContainer _redisContainer = new RedisBuilder()
    .WithImage("redis:7")
    .Build();
```

**測試夾具**: 使用 `IClassFixture` 共用測試環境
```csharp
public abstract class BaseIntegrationTest 
    : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
```

## 測試組織原則

1. **單一職責**: 每個測試案例專注於驗證單一行為
   - ✅ 好: `IpToInt_Should_ReturnExpectedValue_WhenIpIsValid`
   - ❌ 差: `IpToInt_Should_Work` (不明確)

2. **獨立性**: 測試案例之間相互獨立，不依賴執行順序
   - 使用 `[Theory]` 而非多個 `[Fact]` 避免重複
   - 每個測試自行準備所需資料

3. **可重複性**: 測試結果可重複且一致
   - 使用 Testcontainers 確保環境一致
   - 避免依賴外部資源 (如真實 API)

4. **可維護性**: 測試程式碼清晰易讀
   - 使用 FluentAssertions 提高可讀性
   - AAA 模式使測試結構清晰

5. **快速回饋**: 優先撰寫快速執行的測試
   - 單元測試 (毫秒級) > 整合測試 (秒級) > 端對端測試
   - 使用 `[Trait]` 標記慢速測試，可選擇性執行

6. **完整覆蓋**: 涵蓋正常情境、邊界條件與錯誤處理
   - 正常案例: 有效 IP 查詢
   - 邊界案例: `0.0.0.0`, `255.255.255.255`
   - 錯誤案例: 無效 IP 格式

## 程式碼大小指南

```
建議指南：
- **檔案大小**: 
  - Controller: < 200 行 (保持輕量)
  - Service/Infrastructure: < 300 行
  - Utility: < 150 行

- **方法/函式大小**: 
  - 一般方法: < 30 行
  - 複雜方法: < 50 行 (考慮拆分)
  - 測試方法: < 20 行 (AAA 各佔約 5 行)

- **類別複雜度**: 
  - 單一職責: 每個類別不超過 5 個公開方法
  - Cyclomatic Complexity: < 10 (建議使用 Code Metrics 檢查)

- **巢狀深度**: 
  - 最大巢狀層級: 3 層
  - 超過 3 層應考慮提取方法
```

**範例** (好的程式碼大小):
```csharp
// IpLocationsController.cs - 約 30 行
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

**超過建議大小時的處理方式**:
- 提取私有方法減少複雜度
- 拆分為多個類別 (考慮職責分離)
- 使用設計模式 (如 Strategy, Chain of Responsibility) 簡化邏輯
