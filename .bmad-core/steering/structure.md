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
    -   `Benchmark/`: 效能基準測試專案，用於評估關鍵演算法的效能。

## 2. 命名慣例 (Naming Conventions)

-   **類別與介面 (PascalCase)**:
    -   控制器: `{Entity}Controller` (例如: `IpLocationsController`)
    -   服務/儲存庫: `{Purpose}Store` (例如: `IpRangeStore`)
    -   常數類別: `{Purpose}Const` (例如: `RedisKeyConst`)
-   **私有欄位 (camelCase with underscore prefix)**:
    # 專案結構

    ## 目錄組織

    ```
    IpLocations/
    ├── IpLocations.sln
    ├── README.md
    ├── src/
    │   └── IpLocations.Api/               # 主 Web API 專案
    │       ├── Constants/                 # 全域常數 (例如 Redis Key)
    │       ├── Controllers/               # ASP.NET Core 控制器
    │       ├── Infra/                     # 資料存取與外部整合
    │       ├── Misc/                      # 公用工具 (例如 IpConverter)
    │       ├── Models/                    # (保留) DTO 或 Record 類型
    │       └── Program.cs                 # 應用程式進入點
    └── tests/
        ├── IpLocations.UnitTests/         # 單元測試 (xUnit + FluentAssertions)
        ├── IpLocations.IntegrationTests/  # 整合測試 (Testcontainers + WebApplicationFactory)
        └── Benchmark/                     # BenchmarkDotNet 效能量測
    ```

    ## 命名慣例

    ### 檔案
    - **控制器**: `PascalCase` 名稱與類別同步，例如 `IpLocationsController.cs`。
    - **服務/儲存庫**: `{Purpose}Store.cs`，例如 `IpRangeStore.cs`。
    - **常數**: `{Purpose}Const.cs`，例如 `RedisKeyConst.cs`。
    - **測試檔**: 與被測類別對應，如 `IpConverterTests.cs`、`GetIpLocationsTests.cs`。

    ### 程式碼
    - **類別、介面、紀錄**: `PascalCase`；介面採 `I` 前綴 (例如 `IConnectionMultiplexer`)。
    - **方法與屬性**: `PascalCase`，如 `GetCountryAsync`、`ConnectionString`。
    - **私有欄位**: `_camelCase`，例如 `_database`、`_logger`。
    - **區域變數與參數**: `camelCase`，例如 `ipNum`、`startIp`。
    - **常數**: `PascalCase` 置於 `const` 或 `static readonly` 成員中，集中於 `RedisKeyConst` 類別。
    - **測試方法**: `MethodName_Should_ExpectedBehavior_When_Condition`，如 `GetCountryAsync_Should_Return_NotFound_When_IpIsUnknown`。

    ## 匯入模式

    ### 匯入順序
    1. `System.*` 命名空間
    2. 第三方套件 (例如 `StackExchange.Redis`)
    3. 專案內部命名空間 (例如 `IpLocations.Api.Infra`)

    所有檔案均使用 `implicit usings`，僅在需要額外命名空間時額外引入。禁止使用萬用字元 `*` 匯入。

    ### 模組/套件組織
    - API 專案的命名空間根為 `IpLocations.Api`，依資料夾同步延伸，例如 `IpLocations.Api.Controllers`。
    - 測試專案命名空間為 `IpLocations.UnitTests`、`IpLocations.IntegrationTests`，以被測模組為後綴。

    ## 程式碼結構模式

    ### 模組/類別組織
    1. `using` 列表
    2. 命名空間宣告
    3. 類別或紀錄宣告
    4. 常數與欄位
    5. 建構函式與注入相依性
    6. 公開方法
    7. 私有輔助方法

    ### 函式/方法組織
    - 開頭執行輸入驗證（例如 `string.IsNullOrWhiteSpace` 或 `IpConverter.IpToInt`）。
    - 執行核心邏輯，保持流程平坦，避免深度巢狀。
    - 統一使用 `async`/`await` 與 `Task` 回傳；同步輔助函式使用純函式介面。
    - 錯誤或無效狀況以特定回傳值 (例如 `string.Empty`) 表示，呼叫端判斷後轉換成 HTTP 狀態碼。

    ### 檔案組織原則
    - 每個檔案僅包含單一公開類別/紀錄 (`IpConverter` 為靜態類別例外)。
    - 測試檔與產品碼一一對應，並遵循 AAA (Arrange-Act-Assert) 分段註解。
    - 公開 API (控制器動作) 儘量保持精簡，將邏輯委派給基礎設施或工具層。

    ## 程式碼組織原則

    1. **單一職責**：控制器負責 HTTP 流程；Redis 查詢集中於 `IpRangeStore`；轉換工具集中於 `IpConverter`。
    2. **模組化**：Redis Key 與常數集中管理，避免重複字串；未來額外資料來源以新資料夾擴充。
    3. **可測試性**：純函式 (`IpConverter`) 具備完整單元測試；控制器透過整合測試與 Redis 容器驗證端對端行為。
    4. **一致性**：遵循官方 .NET 命名與非同步慣例，避免在同檔內混用不同風格。

    ## 模組邊界
    - **Controllers ↔ Infra**：控制器僅呼叫 `IpRangeStore` 公開方法，不直接觸及 Redis API。
    - **Infra ↔ Utils**：`IpRangeStore` 依賴 `IpConverter` 進行 IP 轉換，維持 Redis 查詢邏輯專注。
    - **Tests ↔ Production**：測試專案僅參考公開類別與介面；整合測試使用 `WebApplicationFactory` 與 Testcontainers 注入測試設定。
    - **Constants**：所有 Redis Key 透過 `RedisKeyConst` 管理，避免魔術字串散落。

    ## 程式碼大小指南
    - **檔案大小**：控制器、Store 類別控制在 200 行內；工具類保持精簡。
    - **方法大小**：一般不超過 40 行；複雜邏輯拆分為私有方法或純函式。
    - **巢狀深度**：避免超過 3 層 if/loop；以提早返回與模式比對維持可讀性。
    - **測試**：每個測試方法聚焦單一情境，資料驅動測試減少重複程式碼。

    ## 儀表板/監控結構 (如適用)
    - 目前無內建儀表板；建議建立獨立 `monitoring/` 資料夾（若日後需要），隔離後端核心程式碼。
    - 將監控腳本或 Kusto 查詢範本放置於 `docs/monitoring` 以供營運團隊使用。

    ## 文件標準
    - 公開類別與方法須提供 XML `<summary>` 註解，以利 Swagger 與 IntelliSense。
    - 複雜邏輯（例如 Redis 回傳解析）需附上行內中文註解說明處理步驟。
    - 每個測試專案維持 `README` 或頂層註解，描述測試環境需求（如 Docker Desktop）。
    {
