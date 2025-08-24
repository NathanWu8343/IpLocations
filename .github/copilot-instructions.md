# GitHub Copilot 指南

**語言**：一律使用**繁體中文**回答

本文件為 GitHub Copilot 在本專案中的最佳化使用規範，確保生成程式碼與既有專案風格一致，並符合可維護性、效能、安全性與測試性需求

---

## 優先準則

在為此存放庫生成程式碼時：

1. **版本相容性**：一律偵測並遵循此專案中使用的語言、框架與函式庫的確切版本  
2. **程式庫模式**：掃描程式庫以找出既有模式  
3. **架構一致性**：維持我們的分層架構風格與既定邊界  
4. **程式碼品質**：在所有生成的程式碼中優先考慮可維護性、效能、安全性與可測試性

---

## 技術版本偵測

在生成程式碼之前，請掃描程式庫以識別：

1. **語言版本**：  
   - .NET 8.0 (net8.0) —— 必須嚴格遵循  
   - 啟用 C# 可為 Null 參考型別（`<Nullable>enable</Nullable>`）  
   - 啟用隱含 using（`<ImplicitUsings>enable</ImplicitUsings>`）  
   - 切勿使用超過 C# 12/.NET 8.0 的語言功能  

2. **框架版本**：  
   - ASP.NET Core 8.0 用於 Web API  
   - Microsoft.NET.Sdk.Web 用於 API 專案  
   - Microsoft.NET.Sdk 用於函式庫與測試專案  
   - 生成程式碼必須與這些框架版本相容  

3. **函式庫版本**：  
   - StackExchange.Redis v2.8.0 —— 用於 Redis 操作  
   - Swashbuckle.AspNetCore v6.4.0 —— 用於 API 文件  
   - xUnit v2.5.3+ —— 用於單元測試  
   - FluentAssertions v6.12.0 —— 用於測試斷言  
   - Microsoft.AspNetCore.Mvc.Testing v8.0.6 —— 用於整合測試  
   - Testcontainers.Redis v3.8.0 —— 用於整合測試 Redis 容器  
   - BenchmarkDotNet v0.13.12 —— 用於效能基準測試  

---

## 程式庫掃描指引

1. 找出與正在修改或建立的檔案相似的檔案  
2. 分析模式，包括：  
   - 命名慣例（類別、方法、屬性用 PascalCase；參數與區域變數用 camelCase）  
   - 程式碼組織（Controllers、Infra、Constants、Misc 資料夾）  
   - 錯誤處理（無效輸入回傳空字串，遺失資料回傳 NotFound()）  
   - 非同步模式（async/await 與 Task<T> 回傳型別）  
   - 文件風格（使用 /// XML 文件註解）  
3. 遵循程式庫中最一致的模式  
4. 當存在衝突模式時，優先考慮較新的檔案或測試覆蓋率較高的檔案  
5. 切勿引入程式庫中不存在的模式  

---

## 程式碼品質標準

### 可維護性
- 撰寫自我說明的程式碼並遵循既有命名模式  
- 公開成員、類別、命名空間使用 PascalCase（例如：`IpLocationsController`, `IpRangeStore`）
- 私有欄位與區域變數使用 camelCase 並加底線前綴（例如：`_db`, `_ipRangeStore`）
- 介面名稱以 "I" 為前綴（例如：IUserService）
- 參數與區域變數使用 camelCase（例如：`ipNum`, `startIp`）  
- 程式碼依邏輯分組於 Controllers、Infra、Constants、Misc  
- 類別專注於單一職責，例如 `IpConverter` 處理 IP 轉換  
- 依既有模式在建構函式中實作相依性注入  
- 在參考成員名稱時，使用 `nameof` 而非字串常值
- 為每個函式撰寫清晰且簡潔的註解

### 效能
- 使用有效率的資料結構，例如 `IpRangeStore` 中的 SortedSet  
- 使用位元運算進行 IP 轉換（`IpConverter.IpToInt` 示範）  
- 非同步 I/O 操作一律使用 async/await（與既有 Redis 呼叫一致）  
- 實作快取策略，遵循既有 Redis 使用模式  
- 依既有模式進行資源管理與釋放  

### 安全性
- 使用 regex 驗證輸入（如 `IpConverter.IpToInt` 所示）  
- 無效輸入回傳 -1 或空字串，而非拋出例外  
- Redis 操作使用參數化方式（如既有程式碼）  
- 連線字串管理遵循既有模式  
- 依既有設定方式處理敏感資料  

### 可測試性
- 遵循相依性注入模式以保持低耦合  
- 建構函式注入（如 `IpLocationsController` 與 `IpRangeStore`）  
- 整合測試繼承 `BaseIntegrationTest`  
- 外部相依性使用 Testcontainers 模擬  
- 測試遵循 Arrange-Act-Assert 格式  

---

## 文件需求

- 公開 API 使用 `///` XML 文件註解  
- 參數、回傳值、例外狀況依既有風格記錄  
- 描述清楚簡潔，如 `IpLocationsController` 中所示  
- 若已有中文實作註解，需保留（例如：`//TODO: 可以調整為檔案載入方式`）  
- 文件完整度需與既有程式碼一致  

---

## 測試方法

### 單元測試
- 使用 xUnit `[Theory]` 與 `[InlineData]`  
- 命名模式：`MethodName_Should_ExpectedBehavior_WhenCondition`  
- 使用 FluentAssertions（`.Should().Be()`）  
- 測試結構包含 Arrange-Act-Assert 並加上註解  
- 測試有效與無效輸入（如 `IpConverterTests`）  

### 整合測試
- 繼承 `BaseIntegrationTest`  
- 使用 `IntegrationTestWebAppFactory`  
- 套用 `IClassFixture<IntegrationTestWebAppFactory>`  
- 使用 Testcontainers.Redis 管理容器  
- 測試採用 async Task 回傳型別  
- 驗證 HTTP 狀態碼與回應內容  

### 效能測試
- 使用 BenchmarkDotNet  
- 套用 `[MemoryDiagnoser]` 與 `[SimpleJob(RuntimeMoniker.Net80)]`  
- 建立多個基準方法比較不同實作  
- 遵循既有基準測試模式  

---

## 技術專屬指南

### .NET 指南
- 僅針對 .NET 8.0（`<TargetFramework>net8.0</TargetFramework>`）  
- 啟用可為 Null 參考型別與隱含 using  
- 宣告變數為不可為 null，並在進入點檢查 `null`
- 一律使用 `is null` 或 `is not null`，而非 `== null` 或 `!= null`
- 信任 C# 的 null 註解，當型別系統保證值不可為 null 時，不要額外新增檢查
- 除非有可變性需求，否則偏好使用不可變類型。
- 對於不可變類型，偏好使用 `record` 而非 `class`  
- 非同步操作使用 async/await 與 Task<T>  
- 相依性注入遵循 Program.cs 的既有模式  
- 使用最小化主機模型（minimal hosting model）
- 儘可能使用模式比對與 switch 運算式


### ASP.NET Core 指南
- 使用 `[Route("api/[controller]")]` 路由模式  
- 控制器繼承 `ControllerBase`  
- 回傳適當的 ActionResult（`Ok()`, `NotFound()` 等）  
- 使用 `[ApiController]` 啟用自動模型驗證  
- 服務註冊遵循 Program.cs 模式  
- 相依性註冊使用 `IServiceCollection` 擴充方法  

### Redis 指南
- 僅使用 StackExchange.Redis v2.8.0 模式  
- `IConnectionMultiplexer` 註冊為 Singleton  
- Redis 操作使用 `IDatabase`（如 `IpRangeStore`）  
- 範圍查詢使用 SortedSet（`SortedSetRangeByScoreAsync`）  
- Redis Key 命名遵循 `RedisKeyConst`  
- 所有 Redis 操作均使用 async  

### 測試指南
- 使用 xUnit v2.5.3+  
- 斷言使用 FluentAssertions  
- API 整合測試使用 Microsoft.AspNetCore.Mvc.Testing  
- Redis 測試使用 Testcontainers.Redis  
- 測試專案結構：UnitTests 與 IntegrationTests  

---

## 專案結構指南

遵循既有資料夾結構：  

- `src/IpLocations.Api/` —— 主 API 專案  
  - `Controllers/` —— API 控制器  
  - `Infra/` —— 基礎設施（資料存取、外部服務）  
  - `Constants/` —— 應用程式常數  
  - `Misc/` —— 工具類別  
  - `Properties/` —— 啟動設定與組態  
- `tests/` —— 測試專案  
  - `IpLocations.UnitTests/` —— 單元測試  
  - `IpLocations.IntegrationTests/` —— 整合測試（含 Abstractions 資料夾）  
  - `Benchmark/` —— 效能基準測試  

---

## 命名慣例

### 類別與方法
- 控制器：`{Entity}Controller`（例如：`IpLocationsController`）  
- 服務：`{Entity}Store` 或 `{Entity}Service`（例如：`IpRangeStore`）  
- 工具類別：`{Purpose}`（例如：`IpConverter`）  
- 常數：`{Purpose}Const`（例如：`RedisKeyConst`）  

### 變數與參數
- 使用描述性名稱（如：`ipNum`, `startIp`, `endIp`, `redisConnection`）  
- 私有欄位加底線前綴（例如：`_db`, `_ipRangeStore`）  
- 當型別明顯時使用 `var`  

### 檔案與資料夾
- 檔名需與類別名稱完全一致  
- 資料夾使用 PascalCase  
- 功能需依邏輯分組  

---

## 錯誤處理模式

- 無效輸入回傳適當值（例如：無效 IP 回傳 `-1`，未找到則回傳空字串）  
- 必要組態值使用 `ArgumentNullException`  
- 資源不存在時回傳 `NotFound()`  
- 使用 regex 驗證輸入（如 `IpConverter`）  
- 例外處理需避免洩露內部實作細節  

## 組態模式

- 使用 `IConfiguration` 存取連線字串與設定  
- Redis 連線字串儲存於組態鍵 `"Redis"`  
- 必要組態使用空合併運算子拋出例外  
- 模式：`?? throw new ArgumentNullException("ConfigKey")`  

## 一般最佳實踐

- 使用 `string.IsNullOrEmpty()` 驗證字串  
- 高效使用 LINQ（`.Any()`, `.First()`, `.Select()`, `.ToArray()`）  
- 所有 I/O 操作一致使用 async/await  
- 相依性生命週期依既有模式（Redis 使用 Singleton）  
- 正確實作資源釋放  
- 使用有意義的變數名稱  
- 類別中邏輯需依功能分組  
- 方法需保持簡潔與專注  
- 避免硬編碼，使用常數  

---

## Git Commit（Conventional Commits）

### 格式

```text
<type>: <subject>

<body>
````
* **type**：
  * `feat` 新功能
  * `fix` 修錯
  * `docs` 文件
  * `style` 格式（不影響行為）
  * `refactor` 重構（不改變商業邏輯）
  * `perf` 效能
  * `test` 測試
  * `chore` 建置/工具/維運
  * `revert` 回復（如：`revert: feat(auth): support OAuth2 login`）
* **subject**：祈使句、簡短、末尾不加句點
* **body**（選填)：條列「變更內容 / 原因 / 影響」

### 範例

```text
fix: 修正員工管理的時間格式轉換

1. 調整 TimeUtils 的時區偏移計算，改為自 timestamp 扣除偏移量。
```

## 分支與 PR 流程

* **分支命名**：`feature/<短描述>`、`fix/<短描述>`、`chore/<短描述>`
* **PR 標題**：與 commit type 對齊（如 `feat: 新增 Orders 建立 API`）
* **PR Checklist（至少）**：

  * [ ] 單元測試通過（含 Positive/Negative）
  * [ ] 有必要的文件更新（README/OpenAPI/變更說明）
  * [ ] 觀測性端點可用（`/actuator/health`、`/actuator/prometheus`）
  * [ ] 無明顯 N+1 / 多餘查詢；必要索引已建立
  * [ ] 錯誤訊息清晰且一致
* **Merge 策略**：建議 **squash & merge**（保持線性歷史）
* **Changelog**：版本釋出時依 Conventional Commits 產生（建議 `CHANGELOG.md`）

---