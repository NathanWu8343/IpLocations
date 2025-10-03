# Technology Stack

## 專案類型
**Web API 服務** - 基於 ASP.NET Core 的 RESTful API 應用程式，提供 IP 地理位置查詢服務

## 版本歷史

| 日期         | 版本號   | 描述             | 作者             |
| ------------ | -------- | ---------------- | ---------------- |
| 2025-10-03   | 1.0      | 初版建立         | Claude 3.5 Sonnet |

## 核心技術

### 主要技術堆疊
- **程式語言**: C# 12 / .NET 8.0
- **程式框架**: ASP.NET Core 8.0 (Web API)
- **執行環境**: .NET 8.0 Runtime
- **語言特定工具**: 
  - NuGet (套件管理器)
  - dotnet CLI (建置與開發工具)

### 主要依賴/函式庫
- **StackExchange.Redis 2.8.0**: Redis 用戶端函式庫，用於高效能資料存取
- **Swashbuckle.AspNetCore 6.4.0**: Swagger/OpenAPI 文件產生工具
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets 1.20.1**: Docker 容器支援

### 資料儲存
- **主要儲存**: Redis 7.x (記憶體內資料庫)
  - 資料結構: Sorted Set (用於 IP 範圍查詢)
  - 分區策略: 使用 IP 數值前 3 位作為分區鍵
- **快取**: 無額外快取層 (Redis 本身即為快取)
- **資料格式**: 
  - Redis Key: 字串格式 `ip-to-country:range-{partition}`
  - Redis Value: 字串格式 `{startIp}-{endIp}:{countryCode}`

### 外部整合
- **資料來源**: IP2Location Lite (https://lite.ip2location.com/ip2location-lite)
  - 協定: 手動下載 CSV 檔案
  - 資料格式: IP 範圍與國家代碼對應
  - 更新頻率: 手動更新 (未來可自動化)

### 監控與儀表板技術
- **日誌框架**: ASP.NET Core 內建日誌系統
- **建議監控工具**: 
  - Application Insights (Azure 環境)
  - Prometheus + Grafana (開源方案)
  - ELK Stack (日誌聚合)

## 開發環境

### 建置與開發工具
- **建置系統**: .NET SDK 8.0 (dotnet build)
- **套件管理**: NuGet
- **IDE/編輯器**: 
  - Visual Studio 2022 (建議)
  - Visual Studio Code + C# Extension
  - JetBrains Rider

### 程式碼品質工具
- **靜態分析**: 
  - Roslyn Analyzers (內建於 .NET SDK)
  - 啟用 Nullable Reference Types
- **格式化**: 
  - .editorconfig (程式碼風格設定)
  - 建議使用 Visual Studio 內建格式化工具
- **測試框架**: 
  - **單元測試**: xUnit 2.5.3 / 2.8.1
  - **整合測試**: xUnit + Microsoft.AspNetCore.Mvc.Testing 8.0.6
  - **斷言函式庫**: FluentAssertions 6.12.0
  - **測試容器**: Testcontainers.Redis 3.8.0
  - **效能測試**: BenchmarkDotNet 0.13.12
  - **程式碼覆蓋率**: coverlet.collector 6.0.0 / 6.0.2

### 版本控制與協作
- **VCS**: Git
- **分支策略**: Feature Branch Workflow (建議)
- **程式碼審查流程**: Pull Request 審查

## 部署與分發

### 目標平台
- **容器環境**: Docker (Linux 容器)
- **基礎映像**: mcr.microsoft.com/dotnet/aspnet:8.0
- **建置映像**: mcr.microsoft.com/dotnet/sdk:8.0

### 分發方式
- **容器化部署**: Docker Image
- **雲端部署**: 
  - Azure Container Apps
  - Azure Kubernetes Service (AKS)
  - AWS ECS/EKS
  - 自建 Kubernetes 叢集

### 安裝需求
- **執行環境**: 
  - .NET 8.0 Runtime (非容器部署)
  - Docker Runtime (容器部署)
- **外部依賴**: Redis 7.x 或更新版本
- **網路**: HTTP/HTTPS 連接埠 (預設 8080/8081)

### 更新機制
- **容器滾動更新**: 透過容器編排工具實現零停機部署
- **版本標籤**: 使用 Docker Image Tags 管理版本

## 套件指南

### StackExchange.Redis 使用要點

**最佳實踐:**
- ✅ 使用單例模式註冊 `IConnectionMultiplexer`，避免重複建立連線
- ✅ 使用 `ConfigurationOptions` 設定連線參數，而非連線字串
- ✅ 啟用詳細日誌 (`IncludeDetailInExceptions = true`) 以便除錯
- ✅ 使用 `GetDatabase()` 取得資料庫實例，會自動重用低編號資料庫實例

**常見錯誤與避免方式:**
- ❌ **避免**: 在每次操作時建立新的 `ConnectionMultiplexer` (會導致連線池耗盡)
- ❌ **避免**: 未處理 Redis 連線逾時異常 (`RedisTimeoutException`)
- ❌ **避免**: 在生產環境關閉逾時設定或設定過長
- ⚠️ **注意**: 大型 payload (>2GiB) 可能導致整數溢位錯誤
- ⚠️ **注意**: `Subscription` 操作需確保執行緒安全

**效能最佳化:**
- 使用 Pipeline 批次操作減少網路往返
- 避免在熱路徑中進行不必要的記憶體配置
- 對於頻繁存取的資料庫，重用 `IDatabase` 實例
- 考慮使用 `async/await` 避免阻塞執行緒

**限制與注意事項:**
- Redis Sorted Set 的 Score 使用 double 類型，極大數值可能損失精度
- 單一 Redis 實例的併發能力受限於單執行緒特性，需透過分區或叢集擴展
- 連線失敗時會自動重試，但需實作應用層重試策略以處理暫時性錯誤

### ASP.NET Core 使用要點

**最佳實踐:**
- ✅ 使用依賴注入 (DI) 管理服務生命週期
- ✅ 使用非同步方法 (`async/await`) 處理 I/O 操作
- ✅ 實作集中式錯誤處理中介軟體
- ✅ 使用 `Problem Details` (RFC 7807) 格式回傳錯誤訊息

**常見錯誤與避免方式:**
- ❌ **避免**: 錯誤的 DI 生命週期設定 (如 Singleton 依賴 Scoped 服務)
- ❌ **避免**: 中介軟體註冊順序錯誤 (如在 Routing 之前註冊 Authorization)
- ❌ **避免**: 在同步方法中阻塞非同步操作 (`.Result` 或 `.Wait()`)

**效能最佳化:**
- 使用回應壓縮中介軟體
- 啟用回應快取 (適用於靜態或低變化內容)
- 使用適當的 DI 生命週期 (Singleton > Scoped > Transient)

**限制與注意事項:**
- 需注意 .NET SDK 版本相容性
- Visual Studio 需啟用「使用 .NET Core SDK 預覽版」選項

### xUnit 與 FluentAssertions 使用要點

**最佳實踐:**
- ✅ 使用 `[Theory]` 與 `[InlineData]` 進行參數化測試
- ✅ 遵循 AAA (Arrange-Act-Assert) 模式組織測試
- ✅ 使用 FluentAssertions 的流暢語法提高可讀性
- ✅ 測試命名清楚描述行為: `Method_Should_ExpectedBehavior_When_Condition`

**常見錯誤與避免方式:**
- ❌ **避免**: 在 `And` 之後呼叫 `.Should()` (已標記為過時)
- ❌ **避免**: 一個測試案例測試多個行為
- ❌ **避免**: 測試案例之間有相依性或順序要求

**FluentAssertions 斷言範例:**
```csharp
result.Should().Be(expected);
result.Should().NotBeNullOrEmpty();
collection.Should().HaveCount(3).And.Contain(item);
response.StatusCode.Should().Be(HttpStatusCode.OK);
```

### Testcontainers 使用要點

**最佳實踐:**
- ✅ 使用 `IAsyncLifetime` 管理容器生命週期
- ✅ 在測試類別層級啟動容器 (使用 `IClassFixture`)
- ✅ 確保容器在測試結束後正確清理

**注意事項:**
- 需要本機安裝 Docker Desktop 或 Docker Engine
- 容器啟動需要時間，可能影響測試執行速度
- CI/CD 環境需支援 Docker-in-Docker 或類似機制

## 技術需求與限制

### 效能需求
```
- 響應時間: P95 < 100ms, P99 < 200ms
- 吞吐量: > 1000 請求/秒 (單一實例)
- 資源使用: 
  - 記憶體: < 512MB (正常運作)
  - CPU: < 50% (正常負載)
- 啟動時間: < 5 秒 (包含 WarmUp)
- Redis 查詢延遲: < 5ms (P95)
```

### 測試覆蓋目標
```
- 最低行覆蓋率: 80%
- 關鍵路徑覆蓋: 100% (IP 轉換、查詢邏輯)
- 分支覆蓋: 75%
- 邊界條件: 應涵蓋所有邊界情境 (有效/無效 IP、邊界 IP 範圍)
```

### 相容性需求
- **平台支援**: 
  - Windows Server 2019+
  - Linux (Ubuntu 20.04+, Debian 11+, RHEL 8+)
  - Docker 20.10+
- **依賴版本**: 
  - .NET 8.0 或更新版本
  - Redis 7.0 或更新版本
- **標準合規性**: 
  - HTTP/1.1, HTTP/2
  - OpenAPI 3.0
  - IPv4 格式 (RFC 791)

### 安全性與合規性
- **安全性需求**: 
  - API 速率限制: 每人每秒 10 次查詢 (未來實作)
  - HTTPS 加密傳輸 (生產環境強制)
  - 輸入驗證: 嚴格驗證 IP 格式 (使用 Regex)
  - 錯誤訊息: 避免洩露敏感系統資訊
- **威脅模型**: 
  - DDoS 攻擊: 透過速率限制與負載平衡器防護
  - IP 注入攻擊: 透過 Regex 驗證防護

### 可擴展性與可靠性
- **預期負載**: 
  - 並發使用者: 1000+
  - 每日查詢量: 10M+
- **可用性需求**: 
  - 正常執行時間目標: 99.9%
  - 災難復原: Redis 持久化 (RDB/AOF) 與備份
- **成長預測**: 
  - 水平擴展: 透過負載平衡器增加實例數
  - Redis 擴展: Sentinel (高可用) 或 Cluster (分散式)

## 技術決策與理由

### 決策日誌

1. **選擇 Redis Sorted Set 而非關聯式資料庫**
   - **理由**: 記憶體內資料結構提供極低延遲查詢 (<5ms)
   - **替代方案**: PostgreSQL (考慮過但查詢效能不足)
   - **取捨**: 需額外管理 Redis 基礎設施，但效能優勢明顯

2. **使用位元左移運算子進行 IP 轉換**
   - **理由**: 避免記憶體配置，效能優於 LINQ 聚合 (快 50 倍)
   - **證據**: BenchmarkDotNet 測試顯示 0.2ns vs 10ns
   - **替代方案**: `BitConverter.ToInt32()` (較慢且需處理位元組順序)

3. **IP 範圍分區策略 (前 3 位數字)**
   - **理由**: 平衡查詢效能與記憶體使用，避免單一 Sorted Set 過大
   - **取捨**: 增加 Key 管理複雜度，但提升查詢效能與可擴展性

4. **選擇 xUnit 而非 MSTest 或 NUnit**
   - **理由**: 現代化設計、平行執行支援、社群活躍
   - **FluentAssertions**: 提供更易讀的斷言語法

5. **採用 Testcontainers 進行整合測試**
   - **理由**: 確保測試環境一致性，避免 Mock 與實際行為差異
   - **取捨**: 測試執行速度較慢，但提高信心度

## 已知限制

- **IPv4 限制**: 目前僅支援 IPv4，IPv6 支援已列入未來規劃
  - **影響**: 無法查詢 IPv6 位址來源
  - **潛在解決方案**: 擴展資料模型與查詢邏輯以支援 IPv6 (預計 2025 Q4)

- **資料更新機制**: 目前需手動更新 IP 資料庫
  - **影響**: 新增或變更的 IP 範圍無法即時反映
  - **潛在解決方案**: 實作自動下載與更新排程 (未來版本)

- **單一資料來源**: 僅依賴 IP2Location Lite 資料
  - **影響**: 資料準確度受限於單一來源
  - **潛在解決方案**: 考慮整合多個資料來源並取交集

- **無速率限制**: 目前未實作 API 速率限制
  - **影響**: 可能遭受濫用或 DDoS 攻擊
  - **潛在解決方案**: 實作基於 IP 的速率限制中介軟體 (每人每秒 10 次)

- **記憶體限制**: Redis 記憶體內儲存受限於伺服器記憶體大小
  - **影響**: 無法無限擴展資料量
  - **潛在解決方案**: Redis Cluster 或持久化分層儲存
