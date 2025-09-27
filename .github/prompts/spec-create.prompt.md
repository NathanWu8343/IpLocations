---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo']
---

# Spec Create Command

建立新的功能規格，並嚴格遵循規格驅動的工作流程。

## Usage
```
/spec-create [feature-name] [description]
```

## Instructions
您正在協助建立一個新的功能規格。請精確執行以下步驟，不得跳過或合併。

## Processing Flow

1. **建立目錄結構**
   - 建立 `.bmad-core\specs\{feature-name}\` 目錄。
   - 初始化三個檔案：`requirements.md`、`design.md`、`tasks.md`，檔案內容先保持為空白。

2. **載入指導文件**
   - `.bmad-core\steering\product.md` → 產品願景與目標  
   - `.bmad-core\steering\tech.md` → 技術標準與模式  
   - `.bmad-core\steering\structure.md` → 專案結構慣例  
   - 以上文件為需求分析與產出的主要依據，必須被引用與對照。

3. **解析功能描述**
   - 取得 **功能名稱**（強制使用 kebab-case 命名）與 **可選描述**。
   - 立即開始需求收集階段，**不得要求使用者進一步輸入多輪資訊**。
   - 直接產生初始需求，避免延宕。

4. **分析現有程式碼庫**（需求前置強制步驟）
   - **搜尋相似功能**：檢視是否已有身份驗證、資料處理、UI 模式等相關程式碼。
   - **識別可重複使用的元件**：如工具程式、服務、hooks、模組。
   - **與指導文件交叉檢查**：比對 tech.md 與 structure.md，確保一致性。
   - **尋找整合點**：標記新功能可能與現有系統交會的模組或 API。
   - **記錄發現**：明確區分可重複利用的部分與必須新建的部分。

5. **產生初始需求**
   - 使用 `.bmad-core\templates\requirements-template.md` 作為需求文件範本。
   - **與 product.md 對齊**：逐項確認需求如何支持產品願景與目標。
   - 以「身為 [角色]，我想要 [功能]，以便 [效益]」格式撰寫使用者故事。
   - 驗收標準必須使用 **EARS 格式 (WHEN/IF/THEN 陳述)**。
   - 同時考量：邊界案例、失敗情境、技術限制。
   - 在需求文件中，註記需求與產品願景、技術標準、結構慣例的對應關係。

6. **請求使用者批准**
   - 呈現需求文件，並附上程式碼庫分析摘要（哪些功能或模組可被重複利用）。
   - 詢問使用者：「需求看起來正確嗎？如果正確，我們就可以進入設計階段。」
   - 在得到明確批准之前，不得繼續進行。

## Example
/spec-create user-authentication "允許使用者安全地註冊和登入"

## Next Steps
需求獲得批准後，進入 `/spec-design` 階段。

## CRITICAL REMINDERS

**❌ 絕對禁止：**
- 未經使用者批准，不得進入設計或任務階段。
- 不得提前生成實作指令或程式碼。

**✅ 必須遵守：**
- 一次僅建立一個功能規格。
- 功能名稱一律使用 kebab-case。
- 驗收標準必須是正確的 EARS 格式。
- **強制步驟**：在產生需求前，必須完成程式碼庫分析並記錄。