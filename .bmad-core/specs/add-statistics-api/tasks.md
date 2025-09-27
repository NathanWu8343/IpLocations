# 實施計畫: `add-statistics-api`

## 任務總覽
此計畫旨在為 IpLocations API 新增統計功能。我們將建立一個新的 `StatisticsStore` 來處理 Redis 中的計數，並新增一個 `StatisticsController` 來公開這些數據。現有的 `IpLocationsController` 將被修改以在成功查詢後更新計數。所有變更都將伴隨對應的整合測試。

## 指導文件合規性
- **structure.md**: 新檔案將遵循現有的專案結構，例如 `Controllers` 和 `Infra` 目錄。
- **tech.md**: 實作將使用 `StackExchange.Redis` 進行 Redis 操作，並遵循建構函式注入的相依性注入模式。

---

## 任務

- [ ] 1. 在 `RedisKeyConst.cs` 中新增統計用的 Redis Key
  - **檔案**: `src/IpLocations.Api/Constants/RedisKeyConst.cs`
  - 新增一個 `public const string` 用於存儲國家查詢次數的 Hash Key。
  - Key 名稱應為 `statistics:countries`。
  - **目的**: 統一管理 Redis Key，避免在程式碼中出現魔術字串。
  - _需求: 4 (資料模型)_

- [ ] 2. 建立 `StatisticsStore` 類別以封裝 Redis 操作
  - **檔案**: `src/IpLocations.Api/Infra/StatisticsStore.cs`
  - 建立一個新的 `StatisticsStore` 類別。
  - 透過建構函式注入 `IConnectionMultiplexer`。
  - 實作 `IncrementCountAsync(string countryCode)` 方法，使用 `HINCRBY` 命令。
  - 實作 `GetAllCountsAsync()` 方法，使用 `HGETALL` 命令，並將結果轉換為 `Dictionary<string, long>`。
  - **目的**: 建立一個專門處理統計資料存取的服務。
  - _需求: 2.1, 3.3_

- [ ] 3. 在 `Program.cs` 中註冊 `StatisticsStore` 服務
  - **檔案**: `src/IpLocations.Api/Program.cs`
  - 將 `StatisticsStore` 註冊為 Singleton 服務。
  - **目的**: 讓 `StatisticsStore` 可以被注入到其他類別中。
  - _利用: `Program.cs` 中現有的服務註冊模式_
  - _需求: 2.1_

- [ ] 4. 建立 `StatisticsController` 以提供 API 端點
  - **檔案**: `src/IpLocations.Api/Controllers/StatisticsController.cs`
  - 建立一個新的 `StatisticsController`，繼承自 `ControllerBase`。
  - 透過建構函式注入 `StatisticsStore`。
  - 建立 `GET /api/statistics` 端點，呼叫 `_statisticsStore.GetAllCountsAsync()` 並回傳結果。
  - **目的**: 提供一個公開的 API 來查詢統計資料。
  - _需求: 3.3, 3.4_

- [ ] 5. 修改 `IpLocationsController` 以記錄查詢次數
  - **檔案**: `src/IpLocations.Api/Controllers/IpLocationsController.cs`
  - 在建構函式中注入 `StatisticsStore`。
  - 在 `Get` 方法中，當成功查詢到國家代碼後，呼叫 `_statisticsStore.IncrementCountAsync(result)`。
  - **目的**: 在每次成功查詢時自動更新統計計數。
  - _利用: `src/IpLocations.Api/Controllers/IpLocationsController.cs`_
  - _需求: 2.1, 2.2_

- [ ] 6. 為 `StatisticsController` 新增整合測試
  - **檔案**: `tests/IpLocations.IntegrationTests/Statistics/GetStatisticsTests.cs`
  - 建立新的測試類別，繼承自 `BaseIntegrationTest`。
  - 撰寫測試案例，驗證 `GET /api/statistics` 在沒有資料時回傳空的 JSON 物件 `{}`。
  - **目的**: 確保新的 API 端點行為符合預期。
  - _利用: `tests/IpLocations.IntegrationTests/IpLocations/GetIpLocationsTests.cs`_
  - _需求: 3.3, 3.4_

- [ ] 7. 更新 `GetIpLocationsTests` 以驗證統計功能
  - **檔案**: `tests/IpLocations.IntegrationTests/IpLocations/GetIpLocationsTests.cs`
  - 修改現有測試，在成功呼叫 `GET /api/IpLocations` 後，接著呼叫 `GET /api/statistics`。
  - 使用 `FluentAssertions` 驗證回傳的統計資料中，對應國家的計數是否正確增加。
  - **目的**: 確保 IP 查詢與統計計數功能已正確整合。
  - _利用: `tests/IpLocations.IntegrationTests/IpLocations/GetIpLocationsTests.cs`_
  - _需求: 2.1, 3.3_
