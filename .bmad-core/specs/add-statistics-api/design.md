### **設計文件: `add-statistics-api`**

#### **1. 整體架構與流程**

新功能將整合至現有 API 架構中。下方圖表展示了兩個主要流程：記錄統計與查詢統計。

```mermaid
sequenceDiagram
    participant User
    participant IpLocationsController
    participant StatisticsStore
    participant Redis

    %% Flow 1: Increment Statistics %%
    User->>+IpLocationsController: GET /api/IpLocations?ip={ip}
    IpLocationsController->>IpLocationsController: (Existing logic to find country)
    alt Found
        IpLocationsController->>+StatisticsStore: IncrementCountAsync("TW")
        StatisticsStore->>+Redis: HINCRBY statistics:countries "TW" 1
        Redis-->>-StatisticsStore: (New count)
        StatisticsStore-->>-IpLocationsController: 
        IpLocationsController->>-User: 200 OK ("TW")
    else Not Found
        IpLocationsController->>-User: 404 Not Found
    end

    %% Flow 2: Get Statistics %%
    participant StatisticsController
    User->>+StatisticsController: GET /api/statistics
    StatisticsController->>+StatisticsStore: GetAllCountsAsync()
    StatisticsStore->>+Redis: HGETALL statistics:countries
    Redis-->>-StatisticsStore: (Hash entries)
    StatisticsStore-->>-StatisticsController: Dictionary<string, long>
    StatisticsController->>-User: 200 OK (JSON Response)
```

#### **2. 檔案變更**

*   **新建**:
    *   `src/IpLocations.Api/Controllers/StatisticsController.cs`: 處理統計查詢的 API 端點。
    *   `src/IpLocations.Api/Infra/StatisticsStore.cs`: 封裝與 Redis 統計資料互動的邏輯。
*   **修改**:
    *   `src/IpLocations.Api/Controllers/IpLocationsController.cs`: 注入 `StatisticsStore` 並在成功查詢後呼叫計數方法。
    *   `src/IpLocations.Api/Constants/RedisKeyConst.cs`: 新增統計用的 Redis Key。
    *   `src/IpLocations.Api/Program.cs`: 註冊 `StatisticsStore` 服務。

#### **3. API 介面定義**

*   **`GET /api/statistics`**
    *   **描述**: 取得所有國家代碼的查詢次數統計。
    *   **成功回應 (200 OK)**:
        ```json
        {
          "TW": 1024,
          "CA": 512,
          "CN": 256
        }
        ```
    *   **無資料時的回應 (200 OK)**:
        ```json
        {}
        ```

#### **4. 資料模型 (Redis)**

*   **資料結構**: `Hash`
*   **Key**: `statistics:countries` (將定義於 `RedisKeyConst`)
*   **Field**: 國家代碼 (例如: `"TW"`)
*   **Value**: 查詢次數 (例如: `1024`)

#### **5. 元件設計**

**a. `RedisKeyConst.cs`**

```csharp
namespace IpLocations.Api.Constants
{
    public static class RedisKeyConst
    {
        // ... existing keys ...
        public const string CountryStatistics = "statistics:countries";
    }
}
```

**b. `StatisticsStore.cs`**

```csharp
using StackExchange.Redis;
using System.Threading.Tasks;

namespace IpLocations.Api.Infra
{
    public class StatisticsStore
    {
        private readonly IDatabase _db;

        public StatisticsStore(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task IncrementCountAsync(string countryCode)
        {
            // 使用 HINCRBY 原子性地增加計數
            await _db.HashIncrementAsync(RedisKeyConst.CountryStatistics, countryCode, 1);
        }

        public async Task<Dictionary<string, long>> GetAllCountsAsync()
        {
            var entries = await _db.HashGetAllAsync(RedisKeyConst.CountryStatistics);
            var result = new Dictionary<string, long>();
            foreach (var entry in entries)
            {
                result[entry.Name] = (long)entry.Value;
            }
            return result;
        }
    }
}
```

**c. `StatisticsController.cs`**

```csharp
using IpLocations.Api.Infra;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IpLocations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsStore _statisticsStore;

        public StatisticsController(StatisticsStore statisticsStore)
        {
            _statisticsStore = statisticsStore;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _statisticsStore.GetAllCountsAsync();
            return Ok(result);
        }
    }
}
```

**d. `IpLocationsController.cs` (修改)**

```csharp
// ... using statements ...
public class IpLocationsController : ControllerBase
{
    private readonly IpRangeStore _ipRangeStore;
    private readonly StatisticsStore _statisticsStore; // 新增

    // 修改建構函式
    public IpLocationsController(IpRangeStore ipRangeStore, StatisticsStore statisticsStore)
    {
        _ipRangeStore = ipRangeStore;
        _statisticsStore = statisticsStore; // 新增
    }

    [HttpGet]
    public async Task<IActionResult> Get(string ip)
    {
        var result = await _ipRangeStore.GetCountryAsync(ip);
        if (string.IsNullOrEmpty(result))
        {
            return NotFound();
        }

        // 新增：記錄統計
        await _statisticsStore.IncrementCountAsync(result);

        return Ok(result);
    }
}
```

**e. `Program.cs` (修改)**

```csharp
// ... existing code ...
builder.Services.AddSingleton<IpRangeStore>();
builder.Services.AddSingleton<StatisticsStore>(); // 新增服務註冊
// ... existing code ...
```

#### **6. 測試策略**

*   **單元測試 (`IpLocations.UnitTests`)**:
    *   為 `StatisticsStore` 建立測試，使用 `Moq` 或類似框架模擬 `IDatabase` 介面，驗證 `HashIncrementAsync` 與 `HashGetAllAsync` 是否被正確呼叫。
*   **整合測試 (`IpLocations.IntegrationTests`)**:
    *   建立 `StatisticsControllerTests`，繼承自 `BaseIntegrationTest`。
    *   測試 `GET /api/statistics` 端點，驗證其回傳正確的狀態碼與 JSON 結構。
    *   修改 `GetIpLocationsTests`，在成功呼叫 `GET /api/IpLocations` 後，再呼叫 `GET /api/statistics`，驗證對應國家的計數是否已增加。
