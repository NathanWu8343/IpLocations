## 版本歷史

**CRITICAL**：每次更新請於表格下方新增一列，**不得覆蓋舊有紀錄**

| 日期         | 版本號   | 描述             | 作者             |
| ------------ | -------- | ---------------- | ---------------- |
| 2025-09-30   | 1.2      | 驗證並確認結構指引內容正確性（配合 tech.md v1.2 更新） | GitHub Copilot   |
| 2025-09-30   | 1.1      | 建立完整結構指引 | GitHub Copilot   |
| 2025-09-29   | 1.0      | 初版建立         | GPT-5            |

## 目錄組織

```
IpLocations/
├── IpLocations.sln
├── README.md
├── src/
│   └── IpLocations.Api/
│       ├── Controllers/
│       │   └── IpLocationsController.cs
│       ├── Infra/
│       │   └── IpRangeStore.cs
│       ├── Misc/
│       │   └── IpConverter.cs
│       ├── Constants/
│       │   └── RedisKeyConst.cs
│       ├── Models/                     # 預留資料契約位置（目前空）
│       ├── Program.cs
│       ├── appsettings*.json
│       └── Dockerfile
├── tests/
│   ├── IpLocations.UnitTests/
│   │   └── IpConverterTests.cs
│   ├── IpLocations.IntegrationTests/
│   │   ├── Abstractions/
│   │   │   ├── BaseIntegrationTest.cs
│   │   │   └── IntegrationTestWebAppFactory.cs
│   │   └── IpLocations/
│   │       └── GetIpLocationsTests.cs
│   ├── Benchmark/
│   │   └── IpToIntCalculationBenchmark.cs
│   └── DebugTest/                       # 開發期間的暫存專案
└── .bmad-core/
    └── steering/                       # Steering 文件 (本次新增)
```

## 命名慣例

### 檔案
- **類別/服務檔案**: 與公開類別同名，使用 PascalCase（例如 `IpRangeStore.cs`）。
- **測試**: 使用 `{Subject}Tests.cs` 或 `{Feature}Tests.cs`。
- **設定**: `appsettings.{Environment}.json`。

### 程式碼
- **命名空間與類別**: PascalCase（`IpLocations.Api.Controllers`）。
- **方法/函式**: PascalCase（C# 慣例）。
- **參數與區域變數**: camelCase。
- **私有欄位**: `_camelCase`。
- **常數**: PascalCase 或 `const` 字首後使用 PascalCase（如 `IpToCountryRange`）。

## 匯入模式

### 匯入順序
1. 系統/框架命名空間 (`System`, `Microsoft.*`)
2. 第三方套件 (`StackExchange.Redis` 等)
3. 專案內命名空間 (`IpLocations.Api.*`)

### 模組/套件組織
- 使用 .NET 隱含 usings；額外相依以 `using` 宣告於檔案頂部。
- 不使用相對匯入；命名空間以專案根命名前綴（`IpLocations.Api`）。

## 程式碼結構模式

### 模組/類別組織
```
1. using 與命名空間宣告
2. 類別或 record 定義
3. 常數與欄位
4. 建構函式
5. 公開方法
6. 私有協助方法
```

### 函式/方法組織
- 先進行輸入驗證與邊界檢查。
- 核心邏輯保持簡潔，善用早期返回。
- 非同步操作使用 `async`/`await`，避免同步呼叫。
- 以明確的 `return` 作為終止點，維持可讀性。

### 檔案組織原則
- 每個檔案維持單一主要類別/record。
- 相關實作（例如 Redis 存取）集中於對應資料夾。
- 公開 API 置於類別開頭，私有協助方法置於結尾。

## 程式碼組織原則
1. **單一職責**：Controller 僅負責 HTTP 轉換；資料操作交由 Store/Service。
2. **模組化**：`Controllers`、`Infra`、`Misc` 分離關注點。
3. **可測試性**：核心邏輯抽離至可單獨測試的類別（例如 `IpConverter`）。
4. **一致性**：遵循現有命名與 DI 模式，避免引入新架構。

## 模組邊界
- **API 層 (`Controllers`)**：負責路由、輸入驗證、回應格式。
- **基礎設施層 (`Infra`)**：封裝 Redis 讀寫與 Warm-up 行為。
- **工具層 (`Misc`)**：提供純函式或靜態工具（如 IP 轉換）。
- **常數層 (`Constants`)**：集中維護 Redis Key 模板與共用常數。
- **測試專案**：`UnitTests` 單獨測試工具/邏輯；`IntegrationTests` 以 `WebApplicationFactory` + `Testcontainers` 驗證端對端；`Benchmark` 用於性能追蹤。

## 程式碼大小指南
- **檔案**: 建議 ≤ 300 行；超出時考慮拆分模組。
- **方法**: 建議 ≤ 40 行；若邏輯複雜，抽離為私有方法。
- **巢狀深度**: 儘量控制在 3 層以內；使用早期返回減少巢狀。
- **測試案例**: 使用 `[Theory]` 與多組資料取代重複測試方法。

## 儀表板/監控結構 (如適用)
- 目前無儀表板程式碼；如未來加入，建議於 `src/IpLocations.Dashboard/` 等獨立專案維護，透過 API 存取資料。

## 文件標準
- 公開 Controller 與服務方法需加入 XML `<summary>`，描述用途與回傳行為。
- 保留現有中文註解（例如 TODO），更新時同步調整。
- 測試方法名稱採 `MethodName_Should_ExpectedBehavior_When_Condition`。
- README 紀錄資料來源與效能基準；若新增功能需追加章節。
