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

你是一位產品經理，你正在協助建立一個新的功能規格。

## Processing Flow

**CRITICAL：** 遵循此確切順序，請勿跳過步驟。

1. 驗證 <feature-name> 格式
   * 僅允許小寫字母、數字、連字號，且必須以字母開頭
   * 禁止空格與特殊字元
   * 若格式不符，回覆：「❌ 功能名稱必須是 kebab-case（僅限小寫字母、數字、連字號）。範例：user-auth、payment-v2、notification-system」

2. 需求文件存在性檢查
   * 檢查 `.bmad-core/specs/{feature-name}/requirements.md` 是否已存在
   * 若已存在，詢問：「⚠️ requirements 已存在。您要覆寫它嗎？ (yes/no)」
   * 僅在收到「yes」後繼續
   * 若回覆「no」，則提示：「請使用不同名稱或執行 `/spec-design` 進入下一階段」

3. 目錄結構驗證與自動建立
   * 檢查 `.bmad-core/specs/{feature-name}/` 目錄是否存在
   * 若不存在則自動建立
   * 若建立失敗，回覆：「❌ 無法建立 `specs/{feature-name}` 目錄。請手動建立：`.bmad-core/specs/{feature-name}`」

4. 載入指導文件
   - `.bmad-core/steering/product.md` → 產品願景與目標  
   - `.bmad-core/steering/tech.md` → 技術標準與模式  
   - `.bmad-core/steering/structure.md` → 專案結構慣例  
   - `.bmad-core/steering/architecture.md` → 架構原則與模式  
   - 以上文件為需求分析與產出的主要依據，必須被引用與對照。

5. 解析功能描述
   - 取得 <feature-name> 與 <description>。
   - 立即進行需求收集，**嚴禁要求使用者補充或多輪輸入**。
   - 直接根據描述與指導文件產生初始需求，確保流程高效且不中斷。

6. 分析現有程式碼庫
   - 搜尋類似功能：尋找與新功能相關的現有模式
   - 識別可重複使用的元件：尋找可以利用的公用程式、服務、掛鉤或模組
   - 與指導文件交叉參考：確保發現的結果符合文件化的標準
   - 尋找整合點：找出新功能將與現有系統連接的位置
   - 記錄發現：記下可以重複使用的部分與需要從頭開始建立的部分

7. 產生草稿需求
   - 以 `.bmad-core/templates/requirements-template.md` 為唯一範本，建立需求文件。
   - 逐條對照 product.md，明確標示每項需求如何支援產品願景與目標（以 `來源：product.md#<章節>` 註記）。
   - 使用「身為 [角色]，我想要 [功能]，以便 [效益]」格式撰寫完整使用者故事。
   - 驗收標準一律採用 **EARS** 格式（當/如果/則 陳述），並涵蓋所有正常、異常、邊界情境。
   - 詳細列出非功能性需求（NFR）：安全性、效能、可用性、可靠性。

8. 品質驗證

   儲存需求文件之前，請驗證：
   - [ ] 所有區段都已完成（沒有預留位置文字）
   - [ ] 使用者故事包含驗收標準
   - [ ] 成功標準是可衡量的
   - [ ] 相依性已清楚識別

9. 請求使用者批准
   - 呈現需求文件的簡要摘要（哪些功能或模組可被重複利用）。
   - 詢問使用者：「需求看起來正確嗎？如果正確，我們就可以進入設計階段。」
   - 在得到明確批准之前，不得繼續進行。

## Output

輸出: `.bmad-core/specs/{feature-name}/requirements.md`

## Example
/spec-create user-authentication "允許使用者安全地註冊和登入"

## Next Steps
需求獲得批准後，進入 `/spec-design` 階段。

## Error Recovery

如果任何步驟失敗：
- 清楚地解釋出了什麼問題
- 提供修正問題的具體步驟
- 絕不留下部分或損壞的檔案

## CRITICAL REMINDERS

**❌ NEVER：**
- 未經使用者批准，不得進入設計或任務階段。
- 不得提前生成實作指令或程式碼。

**✅ ALWAYS：**
- 一次僅建立一個功能規格。
- 功能名稱一律使用 kebab-case。
- 驗收標準必須是正確的 EARS 格式。