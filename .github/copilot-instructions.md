# GitHub Copilot 指南

本文件旨在為 GitHub Copilot 提供清晰、一致的程式碼生成指南，確保所有產出皆符合本專案的架構、風格與品質標準。

**語言**：一律使用**繁體中文**回答（程式碼註解、提交訊息、PR、文件）。

---

## 1. 核心原則 (Core Principles)

這是最高指導原則，在任何情況下都應優先遵循。

1.  **版本相容性**：嚴格遵循專案指定的語言、框架與函式庫版本，絕不使用任何超出範圍的功能。
2.  **維持一致性**：深入分析現有程式碼庫，主動學習並複製既有的設計模式、命名慣例與架構風格。
3.  **架構邊界**：尊重既有的分層架構（Layered Architecture），確保新程式碼不會破壞模組之間的界線。
4.  **程式碼品質**：將可維護性、效能、安全性與可測試性視為首要目標，產出的程式碼必須是穩定且高品質的。

---

## 2. 專案設定與技術棧 (Project Configuration & Tech Stack)

在生成任何程式碼之前，必須掃描並識別以下專案設定，並以此為基礎。

### 2.1. 主要技術與版本

* **語言與執行環境**:
    * `.NET 8.0` (`<TargetFramework>net8.0</TargetFramework>`)
    * `C# 12` (切勿使用超過此版本的語言功能)
    * 啟用 C# 可為 Null 參考型別 (`<Nullable>enable</Nullable>`)
    * 啟用隱含 using (`<ImplicitUsings>enable</ImplicitUsings>`)
* **專案框架**:
    * `ASP.NET Core 8.0`: 用於 Web API 專案。
    * `Microsoft.NET.Sdk.Web`: 用於 API 專案類型。
    * `Microsoft.NET.Sdk`: 用於類別庫與測試專案類型。
* **關鍵函式庫**:
    * `StackExchange.Redis v2.8.0`: Redis 操作。
    * `Swashbuckle.AspNetCore v6.4.0`: API 文件 (Swagger)。
    * `xUnit v2.5.3+`: 單元測試框架。
    * `FluentAssertions v6.12.0`: 測試斷言 (Assertions)。
    * `Microsoft.AspNetCore.Mvc.Testing v8.0.6`: 整合測試。
    * `Testcontainers.Redis v3.8.0`: 用於整合測試的 Redis 容器。
    * `BenchmarkDotNet v0.13.12`: 效能基準測試。

### 2.2. 專案結構

嚴格遵循現有的目錄結構來組織檔案。

* `src/IpLocations.Api/`: 主 API 專案
    * `Controllers/`: API 控制器。
    * `Infra/`: 基礎設施層 (資料存取、外部服務整合)。
    * `Constants/`: 應用程式範圍的常數。
    * `Misc/`: 通用工具類別。
    * `Properties/`: 專案設定 (例如 `launchSettings.json`)。
* `tests/`: 測試專案
    * `IpLocations.UnitTests/`: 單元測試。
    * `IpLocations.IntegrationTests/`: 整合測試 (包含 `Abstractions` 子目錄)。
    * `Benchmark/`: 效能基準測試。

---

## 3. 程式碼撰寫規範 (Coding Standards & Conventions)

此部分定義了程式碼的具體風格與撰寫方式。

### 3.1. 命名慣例 (Naming Conventions)

* **類別、介面、公開成員 (PascalCase)**:
    * 控制器: `{Entity}Controller` (例如: `IpLocationsController`)
    * 介面: `I{ServiceName}` (例如: `IUserService`)
    * 服務/儲存庫: `{Entity}Store` 或 `{Entity}Service` (例如: `IpRangeStore`)
    * 工具類別: `{Purpose}` (例如: `IpConverter`)
    * 常數類別: `{Purpose}Const` (例如: `RedisKeyConst`)
* **私有欄位 (camelCase with underscore prefix)**:
    * 使用底線 `_` 作為前綴 (例如: `_db`, `_ipRangeStore`)
* **方法參數與區域變數 (camelCase)**:
    * 使用描述性名稱 (例如: `ipNum`, `startIp`)
* **檔案與目錄**:
    * 檔案名稱必須與其包含的公開類別名稱完全相符。
    * 目錄名稱使用 PascalCase。

### 3.2. C#/.NET 特定指南

* **Null 檢查**:
    * 一律使用 `is null` 或 `is not null` 進行比較，而非 `== null` 或 `!= null`。
    * 信任 C# 的 null 狀態分析，當型別系統已保證值不為 null 時，無需進行多餘的檢查。
* **型別選擇**:
    * 優先使用不可變 (immutable) 型別，除非有明確的可變性需求。
    * 對於資料傳輸物件 (DTOs) 或簡單的不可變資料結構，優先使用 `record` 而非 `class`。
* **非同步程式設計**:
    * 所有 I/O 密集型操作（如資料庫、網路請求）都必須使用 `async/await` 與 `Task<T>`。
* **其他**:
    * 當型別可由右手邊推斷時，使用 `var` 宣告區域變數。
    * 在參考成員名稱時，使用 `nameof` 運算子而非字串常值。
    * 優先使用模式比對 (Pattern Matching) 與 switch 運算式，以取代傳統的 if-else if-else 鏈。
    * 使用 `string.IsNullOrEmpty()` 或 `string.IsNullOrWhiteSpace()` 來驗證字串。
    * 高效地使用 LINQ，例如 `.Any()`, `.FirstOrDefault()`, `.Select()`, `.ToArray()`。

### 3.3. 文件與註解 (Documentation & Comments)

* **XML 文件註解**:
    * 所有 `public` 的 API、類別與方法都必須使用 `///` XML 文件註解。
    * 註解內容需包含 `<summary>`，並視情況加入 `<param>`, `<returns>`, `<exception>`。
    * 描述必須清晰簡潔，風格與 `IpLocationsController` 保持一致。
* **實作註解**:
    * 若程式碼中已存在中文實作註解（例如 `// TODO: ...`），必須予以保留。

---

## 4. 架構與設計模式 (Architecture & Design Patterns)

此部分描述了專案中採用的高階設計決策與模式。

### 4.1. 架構一致性掃描

在修改或建立檔案時，必須執行以下步驟：

1.  **尋找相似檔案**: 在專案中找出功能或職責相似的既有檔案。
2.  **分析既有模式**: 深入分析找到的檔案，識別並學習以下模式：
    * **程式碼組織**: 檔案在專案結構中的位置 (例如: `Controllers`, `Infra`)。
    * **相依性注入**: 依賴項是如何在建構函式中被注入的。
    * **非同步流程**: `async/await` 的使用方式與 `Task<T>` 的回傳型別。
    * **錯誤處理**: 如何處理無效輸入或找不到資料的情況。
3.  **遵循主流模式**: 嚴格遵循在程式碼庫中最常見且最一致的模式。
4.  **解決衝突**: 當存在多種衝突模式時，優先參考較新的檔案或測試覆蓋率較高的實作。
5.  **禁止引入新模式**: 切勿在未經定義的情況下引入專案中不存在的全新設計模式。

### 4.2. 錯誤處理 (Error Handling)

* **API 回應**:
    * 資源不存在時，回傳 `NotFound()` (`404 Not Found`)。
    * 對於其他標準 HTTP 狀態，回傳適當的 `ActionResult` (例如: `Ok()`, `BadRequest()`)。
* **方法回傳值**:
    * 對於無效輸入，回傳一個代表失敗的特定值 (例如，`IpConverter.IpToInt` 對無效 IP 回傳 `-1`)，而非拋出例外。
* **例外處理**:
    * 僅在程式遇到無法恢復的關鍵錯誤時才拋出例外（例如：啟動時缺少必要組態）。
    * 避免在公開 API 中洩露內部實作細節。
* **輸入驗證**:
    * 優先使用正規表示式 (Regex) 或 .NET 內建的驗證機制來檢查輸入格式。

### 4.3. 相依性注入 (Dependency Injection)

* **實作方式**: 嚴格使用建構函式注入 (Constructor Injection)。
* **服務註冊**:
    * 所有服務都在 `Program.cs` 中使用 `IServiceCollection` 擴充方法進行註冊。
    * 遵循最小化主機模型 (Minimal Hosting Model)。
* **生命週期**:
    * `IConnectionMultiplexer` (Redis) 註冊為 `Singleton`。
    * 其他服務的生命週期需參考 `Program.cs` 中的既有設定。

### 4.4. 組態管理 (Configuration Management)

* **存取方式**: 使用 `IConfiguration` 介面來存取 `appsettings.json` 中的設定。
* **必要組態**:
    * 對於應用程式啟動所必需的組態值，使用空合併運算子 (`??`) 進行檢查。
    * 若缺少必要組態，應拋出 `ArgumentNullException`，模式如下：
        ```csharp
        _redisConnectionString = configuration["Redis"] ?? throw new ArgumentNullException("Redis configuration is missing.");
        ```

### 4.5. Redis 使用模式

* **客戶端**: 僅使用 `StackExchange.Redis` 函式庫。
* **操作介面**: Redis 操作必須透過 `IDatabase` 介面進行。
* **資料結構**: 範圍查詢使用 `SortedSet`，並透過 `SortedSetRangeByScoreAsync` 方法執行。
* **Key 命名**: Redis Key 必須統一定義在 `RedisKeyConst` 類別中。
* **非同步**: 所有與 Redis 的互動都必須是非同步的 (`async/await`)。

---

## 5. 測試策略 (Testing Strategy)

所有新功能或變更都必須伴隨對應的測試。

### 5.1. 通用原則

* **測試結構**: 嚴格遵循 `Arrange-Act-Assert` 格式，並可選擇性地加上註解標示各區塊。
* **斷言函式庫**: 一律使用 `FluentAssertions` (例如: `result.Should().Be(expected);`)。
* **測試覆蓋**: 必須同時包含正向情境（有效輸入）與負向情境（無效輸入、邊界條件）的測試案例。

### 5.2. 單元測試 (Unit Tests)

* **框架**: `xUnit`
* **命名慣例**: `MethodName_Should_ExpectedBehavior_When_Condition`
* **參數化測試**: 優先使用 `[Theory]` 搭配 `[InlineData]` 來測試多組輸入與預期輸出。

### 5.3. 整合測試 (Integration Tests)

* **框架**: `Microsoft.AspNetCore.Mvc.Testing` 搭配 `xUnit`。
* **基礎設施**:
    * 測試類別必須繼承自專案的 `BaseIntegrationTest`。
    * 使用 `IClassFixture<IntegrationTestWebAppFactory>` 來管理應用程式生命週期。
* **外部依賴**:
    * 使用 `Testcontainers.Redis` 來建立用於測試的 Redis 容器。
* **驗證**: 測試應驗證 HTTP 回應的狀態碼 (Status Code) 與回應內容 (Response Body)。
* **非同步**: 測試方法必須回傳 `async Task`。

### 5.4. 效能測試 (Performance Tests)

* **框架**: `BenchmarkDotNet`
* **設定**:
    * 必須套用 `[MemoryDiagnoser]` 屬性以分析記憶體分配。
    * 使用 `[SimpleJob(RuntimeMoniker.Net80)]` 指定執行的 .NET 版本。
* **實作**: 建立多個 `[Benchmark]` 方法來比較不同實作方案的效能。

---

## 6. 開發流程 (Development Workflow)

此部分定義了版本控制與協作的規範。

### 6.1. Git Commit 訊息 (Conventional Commits)

* **格式**:
    ```text
    <type>: <subject>
    
    <body>
    ```
* **Type**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `revert`。
* **Subject**: 祈使句、簡短描述、結尾不加句點。
* **Body** (選填): 以條列式說明「變更內容 / 原因 / 影響」。
* **範例**:
    ```text
    fix: 修正時間格式轉換的時區偏移問題
    
    - 調整 TimeUtils 的時區計算邏輯，確保能正確處理 UTC 時間。
    - 新增單元測試案例以涵蓋夏令時間轉換。
    ```

### 6.2. 分支與 Pull Request (PR) 流程

* **分支命名**: `feature/<short-description>`, `fix/<short-description>`, `chore/<short-description>`。
* **PR 標題**: 需與 Commit `type` 對齊 (例如: `feat: 新增訂單建立 API`)。
* **PR Checklist**: 提交 PR 前必須完成以下檢查項目：
    * `[ ]` 單元測試與整合測試皆通過。
    * `[ ]` 已更新相關文件 (README, API 文件等)。
    * `[ ]` 程式碼中無效能陷阱 (如 N+1 查詢)。
    * `[ ]` 錯誤訊息清晰且與專案風格一致。
* **Merge 策略**: 採用 **Squash and Merge** 以保持 Git 歷史的線性與整潔。