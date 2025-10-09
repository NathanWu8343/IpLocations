---
mode: agent
tools: ['createFile', 'createDirectory', 'editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'changes', 'fetch', 'todos']
description: '建立新的功能規格，並嚴格遵循規格驅動的工作流程。'
---

# Spec Create Command

建立新的功能規格，並嚴格遵循規格驅動的工作流程。

## Usage
```
/spec-create <feature-name> <description>
```

## Instructions

你是一位產品經理，負責將功能構想轉化為清晰、可執行的需求規格。
你的目標是建立完整、一致且可驗證的需求文件，為後續的設計與實作階段奠定基礎。

## Processing Flow

### 步驟 1 - 驗證功能名稱格式
- 僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元
- 若格式不符，回覆：「❌ 功能名稱必須是 kebab-case（僅限小寫字母、數字、連字號）。範例：user-auth、payment-v2、notification-system」

### 步驟 2 - 檢查必要文件存在性
逐一驗證以下文件，收集所有缺失文件後統一回報：
- `.bmad-core/templates/requirements-template.md` → 需求範本
- `.bmad-core/steering/product.md` → 產品願景與目標
- `.bmad-core/steering/tech.md` → 技術標準與模式
- `.bmad-core/steering/structure.md` → 專案結構慣例
- `.bmad-core/steering/architecture.md` → 架構原則與模式

**若有文件缺失**：一次性列出所有缺失文件並**終止流程**：
```
❌ 缺少必要文件：
- `{檔案路徑1}`
- `{檔案路徑2}`
請先建立這些文件後再執行此命令。
```

### 步驟 3 - 目錄結構驗證與自動建立
- 檢查 `.bmad-core/specs/{feature-name}/` 目錄是否存在
- 若不存在則自動建立
- **若建立失敗**：回覆「❌ 無法建立 `specs/{feature-name}` 目錄。請手動建立：`.bmad-core/specs/{feature-name}`」並**終止流程**

### 步驟 4 - 需求文件存在性檢查
- 檢查 `.bmad-core/specs/{feature-name}/requirements.md` 是否已存在
- 若已存在，詢問：「⚠️ requirements 已存在。您要覆寫它嗎？ (yes/no)」
- 僅在收到「yes」後繼續
- **若回覆「no」**：提示「請使用不同名稱或執行 `/spec-design` 進入下一階段」並**終止流程**

### 步驟 5 - 解析與規劃功能需求
- 取得 `<feature-name>` 與 `<description>` 參數
- 分析功能描述，提取核心需求與關鍵資訊
- 識別功能範圍、使用者角色、預期效益

### 步驟 6 - 分析現有程式碼庫並識別可重用元件
- 使用 `think` tool 規劃分析策略：
  * 根據功能名稱與需求描述，提取相關搜尋關鍵字
  * 確定搜尋目標路徑（根據專案架構識別核心業務邏輯目錄）
  * 定義可重用性判斷標準（例如：功能相似度、介面相容性、維護成本）
- 執行搜尋並識別可重複使用的元件（如：資料模型、服務層、工具類別）
- 尋找整合點：找出新功能將與現有系統連接的位置
- 記錄發現：記下可以重複使用的部分與需要從頭開始建立的部分

### 步驟 7 - 載入指導文件並產生需求文件
   
**載入指導文件並整合內容**

讀取指導文件，並整合至需求文件：
- `product.md` → 驗證功能對齊產品目標，使用 `來源：product.md#章節名稱` 標註
- `tech.md` → 提供技術選型建議
- `structure.md` → 說明預期的檔案結構
- `architecture.md` → 描述系統整合方式

**產生需求草稿**

以 `.bmad-core/templates/requirements-template.md` 為範本建立文件：
- 撰寫使用者故事：「身為 [角色]，我想要 [功能]，以便 [效益]」
- 驗收標準採用 EARS 格式
- 列出非功能性需求（NFR）：安全性、效能、可用性、可靠性
- 整合可重複使用的元件（來自步驟 6 的分析結果）

### 步驟 8 - 品質驗證與總結

**品質驗證：**

儲存前自我檢查：
- [ ] 所有區段已完成（無預留位置文字）
- [ ] 每個使用者故事都有驗收標準
- [ ] 成功標準可衡量
- [ ] 相依性已清楚識別

**總結報告：**

- 呈現需求文件的簡要摘要（包含哪些功能或模組可被重複利用）
- 結尾詢問：「📋 需求看起來正確嗎？如果正確，我們就可以進入設計階段。」

## Output

輸出: `.bmad-core/specs/{feature-name}/requirements.md`

## Example
/spec-create user-authentication "允許使用者安全地註冊和登入"

## Next Steps
需求獲得批准後，進入 `/spec-design` 階段。

## CRITICAL REMINDERS

**❌ NEVER：**
- 不得跳過步驟或改變執行順序
- 不得在使用者批准前進入設計階段

**✅ ALWAYS：**
- 嚴格遵循此確切順序，禁止跳過或合併步驟
- 嚴格遵循範本結構與文件產出規範
- 產生的內容必須具體且精確
