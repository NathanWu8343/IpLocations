# 技術指導文件：IpLocations API

本文檔概述了 IpLocations API 專案的技術堆疊、架構決策、開發實踐與函式庫使用指南，旨在確保技術選型的一致性與程式碼品質。

---

## 1. 技術堆疊 (Technology Stack)

-   **語言與執行環境**:
    -   **.NET 8.0** (`<TargetFramework>net8.0</TargetFramework>`)
    -   **C# 12**
-   **專案框架**:
    -   **ASP.NET Core 8.0**: 用於建立 Web API。
-   **資料儲存**:
    -   **Redis**: 用於儲存 IP 範圍資料並提供高效查詢。
-   **API 文件**:
    -   **Swashbuckle (Swagger)**: 用於自動產生 OpenAPI 文件。
-   **測試框架**:
    -   **xUnit**: 單元測試與整合測試的主要框架。
    -   **FluentAssertions**: 提供流暢風格的斷言 API。
    -   **Testcontainers**: 在整合測試中用於管理 Docker 容器（特別是 Redis）。

## 2. 架構與設計原則 (Architecture & Design Principles)

-   **分層架構 (Layered Architecture)**：
    -   **Controllers**: API 層，負責處理 HTTP 請求與回應。
    -   **Infrastructure (`Infra`)**: 基礎設施層，封裝與外部系統（如 Redis）的互動邏輯。
    -   **Misc/Constants**: 包含輔助工具與共用常數。
-   **相依性注入 (Dependency Injection)**：
    -   嚴格使用**建構函式注入**模式。
    -   所有服務的生命週期均在 `Program.cs` 中統一設定。
-   **非同步程式設計**:
    -   所有 I/O 密集型操作（特別是 Redis 查詢）都必須使用 `async/await` 模式，以確保系統的延展性。

## 3. 函式庫最佳實踐 (Library Best Practices)

### 3.1. StackExchange.Redis

`StackExchange.Redis` 是與 Redis 互動的首選函式庫。

-   **連線管理**:
    -   `IConnectionMultiplexer` 應被註冊為**單例 (Singleton)**，以在整個應用程式生命週期中重複使用同一個連線物件，避免連線風暴。
-   **Sorted Set 使用**:
    -   本專案使用 Sorted Set 進行 IP 範圍查詢，這是其核心效能的關鍵。
    -   應優先使用 `SortedSetRangeByScoreAsync` 方法，它能高效地根據分數（IP 數字表示）來檢索成員。
    -   **程式碼範例**:
        ```csharp
        // 根據分數 (ipNum) 查詢，並回傳最接近且分數較低的一個成員
        var result = await _db.SortedSetRangeByScoreAsync(
            key,
            double.NegativeInfinity, // 最小分數
            ipNum,                   // 最大分數 (即當前 IP)
            Exclude.None,
            Order.Descending,        // 降序排列，以取得最接近的範圍起點
            0,                       // 從第一個結果開始
            1);                      // 只取一個結果
        ```

### 3.2. xUnit

`xUnit` 是標準的測試框架。

-   **參數化測試**:
    -   對於需要測試多組輸入與輸出的情境，應優先使用 `[Theory]` 搭配 `[InlineData]`，以避免撰寫重複的測試方法。
    -   **程式碼範例**:
        ```csharp
        [Theory]
        [InlineData("1.160.0.0", 30915840)]
        [InlineData("1.0.1.0", 16777472)]
        [InlineData("0.0.0.0", 0)]
        [InlineData("255.255.255.255", 4294967295)]
        public void IpToInt_Should_Return_Correct_Value_When_IpIsValid(string ip, long expected)
        {
            // Arrange & Act
            var result = IpConverter.IpToInt(ip);

            // Assert
            result.Should().Be(expected);
        }
        ```
-   **測試命名**: 測試方法名稱應清晰描述其意圖，遵循 `MethodName_Should_ExpectedBehavior_When_Condition` 的模式。

### 3.3. FluentAssertions

`FluentAssertions` 提供了更具可讀性的斷言語法。

-   **鏈式語法**:
    -   充分利用其鏈式語法 (`.And`) 來組合多個斷言，使測試程式碼更流暢。
-   **語意化**:
    -   選擇最符合語意的斷言方法，例如使用 `.BeNull()` 而非 `.Be(null)`，使用 `.BeEmpty()` 而非 `.HaveCount(0)`。
    -   **程式碼範例**:
        ```csharp
        // 基本斷言
        result.Should().Be(expected);
        
        // 集合斷言
        collection.Should().NotBeEmpty().And.HaveCount(4);

        // 字串斷言
        text.Should().StartWith("prefix").And.EndWith("suffix");
        ```

### 3.4. Testcontainers

`Testcontainers` 用於在整合測試中提供隔離的外部依賴。

-   **容器生命週期**:
    -   利用 `IClassFixture` 或 `IAsyncLifetime` (xUnit) 來管理容器的生命週期，確保容器在測試類別開始時啟動，並在結束時被清理。
-   **動態連線字串**:
    -   絕不在測試程式碼中硬編碼連線資訊。應在容器啟動後，使用 `_redisContainer.GetConnectionString()` 這類方法動態獲取連線字串，並將其注入到 `WebApplicationFactory` 中。
    -   **程式碼範例**:
        ```csharp
        // 在測試的 WebApplicationFactory 中覆寫設定
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 使用 Testcontainer 提供的動態連線字串
            builder.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
        }
        ```
---

## 4. 效能考量 (Performance Considerations)

-   **IP 轉換**: IP 字串到數字的轉換是一個頻繁操作，其演算法效率至關重要。應確保 `IpConverter` 的實作是高效的。
-   **Redis 查詢**: Redis 查詢本身非常快，但應避免在單次請求中執行不必要的多次查詢。

## 5. 錯誤處理 (Error Handling)

-   **API 層**: 對於無效輸入（如格式錯誤的 IP）或未找到的資源，應回傳標準的 HTTP 狀態碼（如 `400 Bad Request` 或 `404 Not Found`）。
-   **服務層**: 內部方法應避免拋出例外，而是回傳可識別的失敗狀態（如 `IpConverter` 對無效 IP 回傳 `-1`）。
