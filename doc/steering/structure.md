# Project Structure

## 版本歷史

| 日期       | 版本 | 變更摘要 | 作者 |
| ---------- | ---- | -------- | ---- |
| 2025-10-31 | 1.0  | 初始建立 | AI   |

## Directory Organization

```
IpLocations/
├── src/                                  # 應用程式原始碼
│   └── IpLocations.Api/                 # Web API 專案
│       ├── Controllers/                 # API 控制器
│       │   └── IpLocationsController.cs # IP 查詢端點
│       ├── Infra/                       # 基礎設施層
│       │   └── IpRangeStore.cs          # IP 範圍查詢服務
│       ├── Misc/                        # 工具類別
│       │   └── IpConverter.cs           # IP 轉換工具
│       ├── Constants/                   # 常數定義
│       │   └── RedisKeyConst.cs         # Redis Key 格式
│       ├── Models/                      # 資料模型（目前為空）
│       ├── Properties/                  # 專案屬性
│       │   └── launchSettings.json      # 啟動設定
│       ├── Program.cs                   # 應用程式進入點
│       ├── appsettings.json             # 通用組態
│       ├── appsettings.Development.json # 開發環境組態
│       ├── Dockerfile                   # Docker 映像定義
│       └── IpLocations.Api.csproj       # 專案檔
│
├── tests/                                # 測試專案
│   ├── IpLocations.UnitTests/           # 單元測試
│   │   ├── IpConverterTests.cs          # IpConverter 單元測試
│   │   └── IpLocations.UnitTests.csproj # 測試專案檔
│   │
│   ├── IpLocations.IntegrationTests/    # 整合測試
│   │   ├── Abstractions/                # 測試基礎設施
│   │   │   ├── BaseIntegrationTest.cs   # 測試基底類別
│   │   │   └── IntegrationTestWebAppFactory.cs # 測試伺服器工廠
│   │   ├── IpLocations/                 # 功能測試
│   │   │   └── GetIpLocationsTests.cs   # IP 查詢整合測試
│   │   └── IpLocations.IntegrationTests.csproj
│   │
│   └── Benchmark/                        # 效能基準測試
│       ├── IpToIntCalculationBenchmark.cs # IP 轉換效能測試
│       ├── Program.cs                    # 測試進入點
│       └── Benchmark.csproj              # 測試專案檔
│
├── doc/                                  # 文件
│   └── steering/                         # 專案指導文件
│       ├── product.md                    # 產品概述
│       ├── tech.md                       # 技術堆疊
│       ├── architecture.md               # 應用程式架構
│       └── structure.md                  # 專案結構
│
├── .github/                              # GitHub 組態
│   └── prompts/                          # AI Agent 指令
│
├── IpLocations.sln                       # 解決方案檔
├── README.md                             # 專案說明
├── AGENTS.md                             # AI Agent 指南
└── .dockerignore                         # Docker 忽略檔案
```

## Naming Conventions

### 目錄
- **主要目錄**：src（原始碼）、tests（測試）、doc（文件）
- **子目錄**：Controllers、Infra、Misc、Constants、Models、Abstractions

### 檔案
- **類別檔案**：PascalCase（如 `IpLocationsController.cs`、`IpRangeStore.cs`）
- **組態檔案**：camelCase 或小寫（如 `appsettings.json`、`launchSettings.json`）
- **測試檔案**：ClassNameTests（如 `IpConverterTests.cs`、`GetIpLocationsTests.cs`）
- **專案檔案**：ProjectName.csproj（如 `IpLocations.Api.csproj`）

### 程式碼
- **類別/型別**：PascalCase（如 `IpLocationsController`、`IpRangeStore`）
- **介面**：IPascalCase（如 `IConnectionMultiplexer`、`IDatabase`）
- **方法/屬性**：PascalCase（如 `GetCountryAsync`、`IpToInt`）
- **參數/區域變數**：camelCase（如 `ip`、`ipNum`、`startIp`）
- **私有欄位**：_camelCase（如 `_db`、`_ipRangeStore`）
- **常數**：PascalCase（如 `IpToCountryRange`）

### 命名空間
- 遵循專案結構：`IpLocations.Api.{FolderName}`
- 範例：
  - `IpLocations.Api.Controllers`
  - `IpLocations.Api.Infra`
  - `IpLocations.Api.Misc`
  - `IpLocations.Api.Constants`

## Import Patterns

### 匯入順序

1. System 命名空間
2. 第三方套件（如 Microsoft.*, StackExchange.*）
3. 專案內部命名空間

範例：
```csharp
using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using IpLocations.Api.Infra;
using IpLocations.Api.Misc;
```

### 模組組織
- 使用 `using` 語句匯入命名空間
- 避免使用 `using static`，除非有明確需求
- 專案啟用 `ImplicitUsings`，常用命名空間自動匯入

## Code Structure Patterns

### 類別組織

1. 常數與靜態欄位
2. 私有欄位
3. 建構函式
4. 公開方法
5. 私有輔助方法

範例（IpRangeStore）：
```csharp
public class IpRangeStore
{
    // 1. 私有欄位
    private readonly IDatabase _db;

    // 2. 建構函式
    public IpRangeStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    // 3. 公開方法
    public async Task<string> GetCountryAsync(string ip)
    {
        // ...
    }

    public void WarmUp()
    {
        // ...
    }

    // 4. 私有輔助方法
    private void AddIpRange(string startIp, string endIp, string country)
    {
        // ...
    }

    private string GetRedisKey(long value)
    {
        // ...
    }
}
```

### 方法組織

1. 參數驗證與輸入處理
2. 核心邏輯
3. 回傳結果

範例：
```csharp
public async Task<string> GetCountryAsync(string ip)
{
    // 1. 輸入處理
    var ipNum = IpConverter.IpToInt(ip);
    if (ipNum == -1) return string.Empty;

    // 2. 核心邏輯
    var result = await _db.SortedSetRangeByScoreAsync(...);
    
    if (result.Any())
    {
        var rangeInfo = result.First().ToString();
        // 驗證與解析邏輯
    }

    // 3. 回傳結果
    return string.Empty;
}
```

### 測試組織

#### 單元測試
- 測試類別命名：ClassNameTests
- 測試方法命名：MethodName_Should_ExpectedBehavior_When_Condition
- 使用 `[Theory]` 與 `[InlineData]` 進行資料驅動測試

範例：
```csharp
public class IpConverterTests
{
    [Theory]
    [InlineData("192.168.0.1", 3232235521)]
    [InlineData("127.0.0.1", 2130706433)]
    public void IpToInt_Should_ReturnExpectedValue_WhenIpIsValid(string ip, long expected)
    {
        // Arrange (if needed)
        
        // Act
        var result = IpConverter.IpToInt(ip);

        // Assert
        result.Should().Be(expected);
    }
}
```

#### 整合測試
- 測試類別繼承 `BaseIntegrationTest`
- 實作 `IClassFixture<IntegrationTestWebAppFactory>` 共享測試環境
- 使用 Testcontainers 管理測試用 Redis 容器

範例：
```csharp
public class GetIpLocationsTests : BaseIntegrationTest
{
    public GetIpLocationsTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData("1.160.0.0", "TW")]
    public async Task Create_Should_ReturnOk_WhenRequesIsValid(string ip, string expected)
    {
        // Arrange

        // Act
        HttpResponseMessage response = await HttpClientStub.GetAsync($"api/IpLocations?ip={ip}");

        // Assert (HTTP)
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert (Content)
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Be(expected);
    }
}
```

## Design Patterns

### Dependency Injection
- 使用 ASP.NET Core 內建 DI 容器
- Service 類別透過建構函式注入依賴
- 單例服務：`IConnectionMultiplexer`、`IpRangeStore`

### Repository Pattern (簡化版)
- `IpRangeStore` 封裝 Redis 資料存取邏輯
- Controller 不直接操作 Redis

### Factory Pattern
- `IntegrationTestWebAppFactory` 用於建立測試用 Web 應用程式

### Strategy Pattern (效能測試)
- 使用 BenchmarkDotNet 比較不同 IP 轉換策略的效能
