# Project Structure

## 版本歷史

[請依下表格式記錄每次重大版本的更新資訊，每次更新請新增一行，勿覆蓋原有紀錄]

| 日期         | 版本號            | 描述          | 作者          |
| ------------ | ----------------- | ------------- | ------------- |
| [YYYY-MM-DD] | [版本號 (如 1.0)] | [5~10 字簡述] | [AI模型/姓名] |

## 目錄組織
[以樹狀結構呈現專案的目錄結構]

**範例結構（根據專案類型選擇）：**

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

**你的專案結構：**
```
[填寫你的目錄結構]
```

## 命名慣例

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

**建議：遵循所選語言的主流慣例**
- 例如：C# 使用 PascalCase，Python 使用 snake_case，JavaScript 使用 camelCase

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

1. **單一職責**: [每個檔案應有單一明確目的]
2. **模組化**: [程式碼應組織成可重用模組]
3. **可測試性**: [建構易於測試的程式碼]
4. **一致性**: [遵循程式碼庫中已建立的模式]

## 測試結構模式

[定義在測試檔案中組織測試程式碼的常見模式。以下為範例 - 請選擇適用於您專案的模式]

### 測試命名慣例

**常見格式（選擇一種）：**

1. **行為導向格式：**
   ```
   MethodName_Should_ExpectedBehavior_When_Condition
   ```
   範例：
   - `ConvertIp_Should_ReturnNegativeOne_When_IpIsInvalid`
   - `GetUser_Should_ThrowException_When_UserNotFound`

2. **句子格式：**
   ```
   method_name_should_expected_behavior_when_condition
   ```
   範例：
   - `convert_ip_should_return_negative_one_when_ip_is_invalid`
   - `get_user_should_throw_exception_when_user_not_found`

3. **BDD 格式：**
   ```
   Given_Precondition_When_Action_Then_Outcome
   ```
   範例：
   - `Given_InvalidIp_When_Convert_Then_ReturnsNegativeOne`
   - `Given_UserNotFound_When_GetUser_Then_ThrowsException`

**你的專案格式：** [填寫或選用上述格式]

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

1. **單一職責**: [每個測試案例應專注於驗證單一行為或情境]
2. **獨立性**: [測試案例之間應相互獨立，不依賴執行順序]
3. **可重複性**: [測試結果應可重複且一致，不受外部狀態影響]
4. **可維護性**: [測試程式碼應清晰易讀，易於維護與更新]
5. **快速回饋**: [優先撰寫快速執行的測試，提供即時回饋]
6. **完整覆蓋**: [涵蓋正常情境、邊界條件與錯誤處理]

## 程式碼大小指南

### 建議標準

| 項目 | 建議值 | 說明 |
|------|--------|------|
| **檔案大小** | ≤ 300 行 | 超過建議拆分為多個檔案 |
| **類別/模組方法數** | ≤ 10 個 | 超過建議拆分職責 |
| **函式/方法行數** | ≤ 50 行 | 超過建議重構為多個小函式 |
| **函式參數數量** | ≤ 5 個 | 超過建議使用物件封裝參數 |
| **巢狀深度** | ≤ 3 層 | 超過建議提取子函式 |
| **圈複雜度** | ≤ 10 | 超過建議簡化邏輯分支 |

**注意：** 以上為通用建議，實際標準應依據專案特性與團隊共識調整。

### 你的專案標準
- **檔案大小**: [自訂標準]
- **函式大小**: [自訂標準]
- **其他**: [自訂標準]

## 儀表板/監控結構 (*若功能涉及資料時納入*)
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