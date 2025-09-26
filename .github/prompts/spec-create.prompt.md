---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo']
---

# Spec Create Command

建立一個新的功能規格，遵循規格驅動的工作流程。

## Instructions
您正在協助建立一個新的功能規格。請遵循以下步驟：

**WORKFLOW SEQUENCE**: Requirements → Design → Tasks → Generate Commands
**請勿**在任務階段完成並獲得批准之前執行任務指令生成。

1. **建立目錄結構**
   - 建立 `.bmad-core\specs\{feature-name}\` 目錄
   - 初始化空的 requirements.md、design.md 和 tasks.md 檔案

2. **載入指導文件**
   - 載入 `.bmad-core\steering\product.md` (產品願景與目標)
   - 載入 `.bmad-core\steering\tech.md` (技術標準與模式)
   - 載入 `.bmad-core\steering\structure.md` (專案結構慣例)

3. **解析功能描述**
   - 取得功能名稱和可選的描述
   - 立即開始需求收集階段
   - 不要提出連續性問題 - 直接產生初始需求

4. **分析現有程式碼庫** (在撰寫需求之前)
   - **搜尋相似功能**: 尋找現有的身份驗證、資料處理、UI 模式等。
   - **識別可重複使用的元件**: 尋找可以利用的工具程式、服務、hooks 或模組。
   - **與指導文件交叉參照**: 確保發現與 tech.md 的模式和 structure.md 的慣例一致。
   - **尋找整合點**: 定位新功能將與現有系統連接的位置。
   - **記錄發現**: 註記哪些可以重複使用，哪些需要從頭開始建立。

5. **產生初始需求**
   - 使用 `.bmad-core\templates\requirements-template.md` 的需求範本
   - **與 product.md 對齊**: 確保需求支持產品願景與目標。
   - 以「身為 [角色]，我想要 [功能]，以便 [效益]」的格式建立使用者故事。
   - 以 EARS 格式 (WHEN/IF/THEN 陳述) 撰寫驗收標準。
   - 考量邊界案例與技術限制。
   - **參考指導文件**: 註記需求如何與產品願景對齊。

6. **請求使用者批准**
   - 呈現需求文件。
   - **包含程式碼庫分析摘要**: 簡要註記哪些現有程式碼可以被利用。
   - 詢問：「需求看起來正確嗎？如果正確，我們就可以進入設計階段。」
   - 在繼續之前等待明確的批准。

7. **完成需求階段**
   - 呈現需求文件，並突顯可重複使用的機會。
   - 等待明確的批准。
   - **請勿**執行任務指令生成。
   - **NEXT STEP**: 進入 `/spec-design` 階段。

8. **規則**
   - 一次只建立一個規格。
   - 功能名稱一律使用 kebab-case。
   - **強制**: 在撰寫需求之前，務必分析現有的程式碼庫。
   - 驗收標準需遵循確切的 EARS 格式。
   - 未經使用者明確批准，不得繼續。
   - **請勿**在 `/spec-create` 期間執行任務指令生成 - 只建立需求。

## Example

/spec-create user-authentication "允許使用者安全地註冊和登入"

## Next Steps
在使用者批准後，進入 `/spec-design` 階段。
