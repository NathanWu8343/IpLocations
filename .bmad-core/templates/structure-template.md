# Project Structure

[如有待澄清事項，插入以下章節]
## 待補充資訊

- [問題 1]（章節：[章節名稱]）
- [問題 2]（章節：[章節名稱]）

## 版本歷史

| 日期       | 版本 | 變更摘要 | 作者 |
| ---------- | ---- | -------- | ---- |
| 2025-10-13 | 1.0  | 初始建立 | AI   |

## 目錄組織
[以樹狀結構呈現專案的目錄結構]

```
# 應用程式專案
project-root/
├── src/                    # 原始碼
│   ├── api/               # API 層
│   ├── core/              # 核心邏輯
│   └── infra/             # 基礎設施
├── tests/                  # 測試
├── docs/                   # 文件
└── build/                  # 建置輸出

# 函式庫專案
project-root/
├── lib/                    # 函式庫原始碼
├── examples/               # 使用範例
├── tests/                  # 測試
└── docs/                   # 文件
```

## 命名慣例
[定義專案中使用的命名慣例。以下為範例 - 請選擇適用於您專案的模式]

### 目錄
- **主要目錄**: [例如：src, lib, app, core]
- **子目錄**: [例如：components, services, utils, models]

### 檔案
- **模組/類別**: [例如：PascalCase、snake_case、kebab-case]
- **服務/處理器**: [例如：UserService、user_service、user-service]
- **工具/輔助**: [例如：dateUtils、date_utils、date-utils]
- **測試檔案**: [例如：UserService.test、user_service_test、UserServiceTests]

### 程式碼
- **類別/型別**: [例如：PascalCase、CamelCase、snake_case]
- **函式/方法**: [例如：camelCase、snake_case、PascalCase]
- **常數**: [例如：UPPER_SNAKE_CASE、PascalCase、kConstantName]
- **變數**: [例如：camelCase、snake_case、lowercase]
- **介面**: [例如：IUserService、UserServiceInterface、user_service_protocol]
- **命名空間/套件**: [例如：PascalCase、snake_case、kebab-case]

## 匯入模式

### 匯入順序
[定義您的專案的匯入順序慣例。以下為範例 - 請選擇適用於您專案的模式]

1. 外部依賴
2. 內部模組
3. 相對匯入
4. 樣式匯入

### 模組/套件組織
[描述您的專案的匯入/包含模式]
`
範例：
- 從專案根目錄的絕對匯入
- 模組內的相對匯入
- 套件/命名空間組織
- 依賴管理方法
`
## 程式碼結構模式

[定義在檔案中組織程式碼的常見模式。以下為範例 - 請選擇適用於您專案的模式]

### 模組/類別組織

1. 匯入/包含/依賴
2. 常數與設定
3. 類型/介面定義
4. 主要實作
5. 協助/公用程式函式
6. 匯出/公開 API

### 函式/方法組織

**範例模式：**
- 先進行輸入驗證
- 核心邏輯置中
- 全程錯誤處理
- 清晰的返回點

### 檔案組織原則

- 每個檔案一個類別/模組
- 相關功能分組在一起
- 公開 API 置於頂部/底部
- 隱藏實作細節

## 程式碼組織原則
`
選擇適用於您專案的方式：
- 每個檔案一個類別/模組
- 相關功能分組在一起
- 公開 API 置於頂部/底部
- 隱藏實作細節
`

## 測試結構模式

[定義在測試檔案中組織測試程式碼的常見模式。以下為範例 - 請選擇適用於您專案的模式]

### 測試命名慣例
[定義測試案例命名慣例。以下為範例 - 請選擇適用於您專案的模式]

- 方法導向: `MethodName_Should_ExpectedBehavior_When_Condition`
- 行為導向: `Should_ExpectedBehavior_When_Condition`
- 句子形式: `method name should expected behavior when condition`
- 情境描述: `Given_Precondition_When_Action_Then_Outcome`

### 測試檔案組織

1. 匯入/依賴項 (測試框架、待測試模組、工具)
2. 測試夾具/設定 (Fixtures/Setup)
3. 測試類別或群組 (按功能或模組分類)
4. 個別測試案例 (清晰命名、單一職責)
5. 協助/公用函式 (測試資料準備、共用斷言)
6. 清理/拆除 (Teardown/Cleanup)

### 測試案例組織

[定義測試案例內部的組織模式。以下為範例 - 請選擇適用於您專案的模式。]

**範例**
```csharp
[Fact]
public void IpToInt_Should_ReturnNegativeOne_When_IpIsInvalid()
{
    // Arrange
    var ipConverter = new IpConverter();
    var invalidIp = "999.999.999.999";

    // Act
    var result = ipConverter.IpToInt(invalidIp);

    // Assert
    Assert.Equal(-1, result);
}
```    

### 測試分類與組織
[定義如何分類和組織測試檔案。以下為範例 - 請選擇適用於您專案的模式。]

**測試類型：**
- 單元測試 (Unit Tests): 測試個別函式/方法
- 整合測試 (Integration Tests): 測試模組間互動
- 端對端測試 (E2E Tests): 測試完整流程
- 效能測試 (Performance Tests): 測試執行效能與資源使用

**按功能分類：**
- 依照被測試模組的結構分類
- 與源碼目錄結構對應
- 使用測試套件 (Test Suite) 分組相關測試

### 測試資料管理
[定義如何管理測試資料。以下為範例 - 請選擇適用於您專案的模式。]

- 測試資料工廠 (Test Data Factories): 產生標準測試資料
- 測試夾具 (Fixtures): 共用的測試前置條件
- Mock/Stub 物件: 隔離外部依賴
- 測試資料檔案: 儲存大量或複雜的測試資料

## 測試組織原則
`
選擇適用於您專案的方式：
- 每個測試案例聚焦單一功能或情境
- 測試須獨立且可重現
- 覆蓋正常、邊界及異常情境
- 測試命名清楚描述預期行為
- 測試失敗時能快速定位問題
`

## 程式碼大小指南
`
建議指南：
- **檔案大小**: [定義每個檔案的最大行數]
- **函式/方法大小**: [定義每個函式的最大行數]
- **類別/模組複雜度**: [定義複雜度限制]
- **巢狀深度**: [最大巢狀層級]
`

## 儀表板/監控結構 (*如適用*)
[儀表板或監控元件的組織方式]

### 範例結構：
```
src/
└── dashboard/          # 獨立的儀表板子系統
    ├── server/        # 後端伺服器元件
    ├── client/        # 前端資產
    ├── shared/        # 共用類型/公用程式
    └── public/        # 靜態資產
```

### 關注點分離

- 儀表板與核心業務邏輯隔離
- 自有 CLI 進入點以獨立運作
- 對主應用程式的依賴最小化
- 可在不影響核心功能的情況下停用

## 文件與註解

- 所有公開 API 皆須有文件
- 複雜邏輯應包含行內註解
- 主要模組的 README 檔案
- 遵循語言特定的文件慣例