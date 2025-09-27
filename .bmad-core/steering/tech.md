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
    -   **Swashbuckle.AspNetCore**: 用於自動產生 OpenAPI (Swagger) 文件。
-   **測試框架**:
    -   **xUnit**: 單元測試與整合測試的主要框架。
    -   **FluentAssertions**: 提供流暢風格的斷言 API。
    -   **Testcontainers**: 在整合測試中用於管理 Docker 容器（特別是 Redis）。
-   **效能測試**:
    -   **BenchmarkDotNet**: 用於評估與比較關鍵演算法的效能。

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
    -   應優先使用 `SortedSetRangeByScoreAsync` 方法，它能高效地根據分數（IP 數字表示）來檢索成員。官方文件證實，此方法是處理範圍查詢的最佳選擇。
    -   **關鍵參數解析**:
        -   `score` (分數): IP 位址的數字表示。
        -   `start` / `stop`: 定義查詢的分數範圍。使用 `double.NegativeInfinity` 到 `ipNum` 可找到所有小於等於目標 IP 的範圍。
        -   `order`: 設定為 `Order.Descending`，確保第一個回傳的結果是分數最接近（即最大）的項目。
        -   `take`: 設定為 `1`，因為我們只需要最接近的那一個 IP 範圍起點。
    -   **程式碼範例**:
        ```csharp
        // 根據分數 (ipNum) 查詢，並回傳最接近且分數較低的一個成員
        var result = await _db.SortedSetRangeByScoreAsync(
            key,
            double.NegativeInfinity, // 最小分數
            ipNum,                   // 最大分數 (即當前 IP)
            # 技術堆疊

            ## 專案類型
            IpLocations 是一個以 **ASP.NET Core 8** 打造的雲端原生 **HTTP API 服務**，提供 IP-to-country 查詢能力，部署目標涵蓋容器化環境與雲端平台。

            ## 核心技術

            ### 主要語言
            - **語言**: C# 12
            - **執行環境**: .NET 8.0 (net8.0)
            - **語言特定工具**: `dotnet` CLI、`implicit usings` 啟用、可為 Null 參考型別 (`<Nullable>enable</Nullable>`)

            ### 主要依賴/函式庫
            - **ASP.NET Core 8.0**: 建立 RESTful API 與中介軟體管線。
            - **StackExchange.Redis 2.8.0**: 與 Redis 快取伺服器進行高效非同步互動。
            - **Swashbuckle.AspNetCore 6.4.0**: 產生 Swagger/OpenAPI 文件與測試介面。
            - **xUnit 2.5.3+ / FluentAssertions 6.12.0**: 單元與整合測試框架與斷言工具。
            - **Testcontainers.Redis 3.8.0**: 在整合測試中動態啟動 Redis 容器。
            - **BenchmarkDotNet 0.13.12**: 評估 IP 轉換等熱點程式碼的效能。

            ### 應用程式架構
            - 採 **分層架構**：Controller → Infrastructure → Redis。
            - 透過 **Minimal Hosting model** 在 `Program.cs` 配置服務、管線與健康度設定。
            - 使用 **Record/不可變物件** 傳遞查詢結果，並以工具類別 (`IpConverter`) 處理純運算邏輯。

            ### 資料儲存
            - **主要儲存**: Redis Sorted Set，用以儲存 IP 區段起點與國家代碼。
            - **快取策略**: Redis 本身即為快取層；資料由離線批次載入並常駐記憶體。
            - **資料格式**: REST 回應採 JSON，Redis 成員值為 `countryCode|endIp` 形式的字串。

            ### 外部整合
            - **協定**: HTTP/REST (JSON)。
            - **驗證**: 目前內部使用，不強制驗證；未來可透過 API Key 或 Gateway 控制。

            ### 監控與儀表板技術
            - 尚未內建前端儀表板；建議整合 Application Insights、Prometheus + Grafana，收集 ASP.NET Core Metrics 與 Redis 連線儀表。

            ## 開發環境

            ### 建置與開發工具
            - **建置系統**: `dotnet build`，CI 中建議啟用 `dotnet test` 與 `dotnet format`。
            - **套件管理**: NuGet (`dotnet restore`)。
            - **開發工作流程**: 使用 `dotnet watch` 進行本機熱重載；整合測試透過 Testcontainers 自動啟動 Redis。

            ### 程式碼品質工具
            - **靜態分析**: C# 編譯器警告、可攜式分析器 (`xUnit` Analyzer)。
            - **格式化**: 遵循 .NET 既定格式；可選擇 `dotnet format` 維護一致性。
            - **測試框架**: `xUnit` (單元/整合)、`BenchmarkDotNet` (效能)。
            - **文件**: 產出 XML 文件並由 Swagger 引用；README 提供整體說明。

            ### 版本控制與協作
            - **VCS**: Git；儲存庫採用 GitHub Flow，分支命名 `feature/*`, `fix/*`, `chore/*`。
            - **程式碼審查**: 透過 Pull Request 審查，遵循 Conventional Commits 與 squash merge。

            ### 儀表板開發
            - 尚無獨立儀表板；若後續新增，可額外建立前端專案並以 Reverse Proxy 裝載。

            ## 部署與分發
            - **目標平台**: Docker 容器、Kubernetes、Azure App Service 或其他支援 .NET 的雲端平台。
            - **分發方式**: 透過容器映像或 `dotnet publish` 產出自包含套件。
            - **安裝需求**: .NET 8 執行環境、Redis 7.x 以上服務。
            - **更新機制**: CI/CD 推送新映像；需同步資料匯入腳本以維護 Redis 內容新鮮度。

            ## 技術需求與限制

            ### 效能需求
            - 單次查詢目標延遲：平均 < 50 ms，P99 < 100 ms。
            - 可擴展至每秒 2k 請求，並透過水平擴充與共用 Redis multiplexer 維持效率。

            ### 相容性需求
            - **平台支援**: Windows、Linux x64 容器 (glibc)；建議在 Linux 上部署以獲最佳效能。
            - **依賴版本**: 必須使用 StackExchange.Redis 2.8.0、Swashbuckle 6.4.0，以維持 API 介面一致性。
            - **標準合規**: 遵循 HTTP/1.1 與 JSON API 慣例，回應採 UTF-8。

            ### 安全性與合規性
            - 建議在閘道層採用 HTTPS 與 API Key 驗證。
            - 避免在日誌中記錄完整 IP 查詢歷史；若需保留資料，遵守 GDPR/CCPA 去識別化原則。

            ### 可擴展性與可靠性
            - 預期服務須長期處理高讀取、低寫入工作負載。
            - Redis 連線 multiplexer 設為 Singleton，並配置 `AbortOnConnectFail = false` 以提升恢復力。
            - 應在部署中配置健康檢查與啟動探針，確保資料載入完成後才接流量。

            ## 技術決策與理由

            ### 決策日誌
            1. **StackExchange.Redis + Sorted Set**: 以分數範圍查詢快速對照 IP 區間；曾評估關聯式資料庫但無法滿足延遲需求。
            2. **ASP.NET Core Web API**: 與組織既有 .NET 技術堆疊一致，支援原生 DI 與中介軟體；替代方案如 Node.js 因生態與既有專業度較低而未採用。
            3. **Testcontainers.Redis**: 自動提供測試 Redis 實例，避免本機環境差異；相較手動 mock 或共用實體 Redis 更具隔離性。
            4. **BenchmarkDotNet**: 針對 IP 轉換與 Redis 查詢流程進行量測，確保效能改善具備量化依據。

            ## 已知限制
            - **IPv4 限制**: 目前僅支援 IPv4；若要支援 IPv6，需要擴充資料模型與查詢範圍邏輯。
            - **資料更新流程**: 尚未內建自動資料匯入或版本控管；需外部批次腳本維護。
            - **監控缺口**: 仍依賴外部平台整合監控指標，尚無內建儀表板或警示設定。
-   **Redis 查詢**: Redis 查詢本身非常快，但應避免在單次請求中執行不必要的多次查詢。
