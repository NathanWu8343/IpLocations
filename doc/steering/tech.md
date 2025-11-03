# Technology Stack

## 版本歷史

| 日期       | 版本 | 變更摘要 | 作者 |
| ---------- | ---- | -------- | ---- |
| 2025-10-31 | 1.0  | 初始建立 | AI   |

## Project Type

這是一個 **Web API 服務**專案，使用 ASP.NET Core 建構 RESTful API，提供 IP 位址地理位置查詢功能。

## Core Technologies

### 主要技術堆疊
- **程式語言**：C# (.NET 8.0)
- **程式框架**：ASP.NET Core 8.0 Web API
- **執行環境**：.NET 8.0 Runtime
- **語言特定工具**：dotnet CLI、NuGet 套件管理器

### 主要依賴與使用指南

#### 核心依賴

- **StackExchange.Redis**
  - 版本：2.8.0
  - 用途：Redis 客戶端函式庫，用於與 Redis 資料庫進行通訊
  - 關鍵功能：
    - Sorted Set 操作（SortedSetAdd, SortedSetRangeByScore）
    - ConnectionMultiplexer 連線管理
    - 支援非同步操作
  - 使用要點：
    - 使用單例模式註冊 `IConnectionMultiplexer`，避免重複建立連線
    - 透過 `GetDatabase()` 取得 IDatabase 實例進行操作
    - 使用 `SortedSetRangeByScore` 進行範圍查詢時，注意排序方向與分數範圍
  - 注意事項：
    - ConnectionMultiplexer 是執行緒安全且設計為可重用，應作為單例使用
    - 避免在每次操作時建立新的連線，會導致效能問題
    - 連線字串應包含合理的 timeout 設定（connectTimeout、syncTimeout）

- **Swashbuckle.AspNetCore**
  - 版本：6.4.0
  - 用途：產生 Swagger/OpenAPI 文件與互動式 API 測試介面
  - 使用要點：
    - 在 Development 環境啟用 Swagger UI
    - 使用 XML 註解增強 API 文件說明
  - 注意事項：
    - 生產環境應考慮是否需要關閉 Swagger UI

- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets**
  - 版本：1.20.1
  - 用途：提供 Visual Studio 的 Docker 容器工具支援
  - 使用要點：
    - 簡化 Docker 映像建置與偵錯流程

#### 開發依賴（測試、建置工具等）

- **xUnit**
  - 版本：2.5.3（單元測試）/ 2.8.1（整合測試）
  - 用途：單元測試與整合測試框架
  - 使用要點：
    - 使用 `[Fact]` 屬性標記簡單測試
    - 使用 `[Theory]` 與 `[InlineData]` 進行資料驅動測試
    - 整合測試使用 `IClassFixture<T>` 共享測試環境設定

- **FluentAssertions**
  - 版本：6.12.0
  - 用途：提供流暢且易讀的測試斷言語法
  - 使用要點：
    - 使用 `.Should().Be()` 等流暢 API 進行斷言
    - 提供更清楚的錯誤訊息

- **Microsoft.AspNetCore.Mvc.Testing**
  - 版本：8.0.6
  - 用途：提供 ASP.NET Core Web API 整合測試支援
  - 使用要點：
    - 使用 `WebApplicationFactory<T>` 建立測試伺服器
    - 可自訂 `ConfigureTestServices` 替換依賴服務

- **Testcontainers.Redis**
  - 版本：3.8.0
  - 用途：在整合測試中使用 Docker 容器化的 Redis 實例
  - 使用要點：
    - 使用 `RedisBuilder` 建立測試用 Redis 容器
    - 實作 `IAsyncLifetime` 管理容器生命週期
    - 透過 `GetConnectionString()` 取得測試用連線字串
  - 注意事項：
    - 需要本機安裝 Docker Desktop 或 Docker Engine
    - 測試執行時間會因容器啟動而增加

- **BenchmarkDotNet**
  - 版本：0.13.12
  - 用途：效能基準測試框架
  - 使用要點：
    - 用於比較不同 IP 轉換演算法的效能差異
    - 測試結果顯示位移運算子效能最佳

- **coverlet.collector**
  - 版本：6.0.0 / 6.0.2
  - 用途：程式碼覆蓋率收集工具

### 資料儲存

- **主要儲存**
  - 名稱：Redis
  - 版本：7（Docker 映像：redis:7）
  - 用途：儲存 IP 範圍與國家代碼的對應資料，使用 Sorted Set 資料結構

- **資料結構**
  - 名稱：Redis Sorted Set
  - 用途：以 IP 範圍起始值作為分數（score），範圍資訊作為成員（member），支援快速範圍查詢

- **資料格式**
  - 名稱：字串格式（Redis Value）
  - 格式：`{startIp}-{endIp}:{countryCode}`
  - 範例：`1.160.0.0-1.175.255.255:TW`

### 外部整合

- **API**：IP2Location Lite（資料來源，離線圖資）
- **協定**：HTTP/REST
- **驗證**：目前無驗證機制
- **檔案位置**：`src/IpLocations.Api/Controllers/IpLocationsController.cs`

## Development Environment

### 建置與開發工具
- **建置系統**：dotnet CLI、MSBuild
- **套件管理**：NuGet
- **開發工作流程**：支援熱重載（dotnet watch）

### 程式碼品質工具
- **測試框架**：xUnit（單元測試、整合測試）
- **效能測試**：BenchmarkDotNet
- **程式碼覆蓋率**：coverlet.collector

### 版本控制與協作
- **VCS**：Git
- **分支策略**：基於 master 分支進行開發
- **程式碼審查流程**：透過 Pull Request 進行程式碼審查

### 容器化開發
- **容器平台**：Docker
- **基礎映像**：mcr.microsoft.com/dotnet/aspnet:8.0（執行期）、mcr.microsoft.com/dotnet/sdk:8.0（建置期）
- **暴露埠號**：8080（HTTP）、8081（HTTPS）
- **多階段建置**：使用 Dockerfile 進行多階段建置，優化映像大小

## Configuration Management

- **組態檔案**：appsettings.json（通用）、appsettings.Development.json（開發環境）
- **環境變數**：支援透過環境變數覆寫組態
- **機密管理**：開發環境使用 User Secrets（UserSecretsId 已設定）
- **連線字串管理**：Redis 連線字串存放於 `ConnectionStrings:Redis` 設定項
