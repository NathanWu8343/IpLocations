---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'context7']
---

# Spec Design Command

根據已批准的需求產生設計文件。

## Instructions
您正在處理規格工作流程的設計階段。

1. **先決條件**
   - 確保 requirements.md 存在且已獲批准。
   - 載入需求文件以供參考。
   - **載入指導文件**: 檢查並載入 tech.md 和 structure.md。
   - 研究現有的程式碼庫模式與架構。

2. **生成設計文件**
   - 遵循範本建立全面的設計。
   - 包含所有必要章節：
     - Overview
     - Architecture
     - Components and Interfaces
     - Data Models
     - Error Handling
     - Testing Strategy

3. **程式碼庫研究階段** (強制)
   - **映射現有模式**: 識別符合您需求的資料模型、API 模式、元件結構。
   - **與 tech.md 交叉參照**: 確保模式與記載的技術標準一致。
   - **分類可重複使用的工具**: 尋找可利用的驗證函式、輔助函式、中介軟體、hooks。
   - **記錄架構決策**: 註記要遵循的現有技術堆疊、狀態管理、路由模式。
   - **對照 structure.md 驗證**: 確保檔案組織遵循專案慣例。
   - **識別整合點**: 映射新功能如何連接到現有的身份驗證、資料庫、API。
   - **尋找相似的實作**: 尋找已實作且需求相似的功能。
   - **註記差距**: 記錄需要從頭建立的內容，以及可以重複使用或擴展的內容。

4. **設計內容** (利用程式碼庫研究與指導文件)
   - **重複使用架構**: 基於現有模式進行建構，而非創造新模式。
   - **遵循 tech.md 標準**: 確保設計遵循記載的技術指南。
   - **尊重 structure.md 慣例**: 根據專案結構組織元件。
   - **擴展元件**: 設計時應利用並擴展現有的工具、服務、元件。
   - 使用 Mermaid 圖表進行視覺化表示。
   - 定義與現有系統整合的清晰介面。
   - 指定遵循既定模式的資料模型。
   - 規劃與當前方法一致的錯誤處理。
   - 使用現有測試工具概述測試方法。

5. **批准流程**
   - 呈現完整的設計文件。
   - **突顯程式碼重複使用**: 清楚地展示將利用哪些現有程式碼。
   - **展示與指導文件的一致性**: 註記設計如何遵循 tech.md 和 structure.md。
   - 詢問：「設計看起來正確嗎？如果正確，我們就可以進入實作計畫階段。」
   - 納入回饋與修訂。
   - 持續進行直到獲得明確批准。

## Design Structure
```markdown
# Design Document

## Overview
[高層級描述]

## Code Reuse Analysis
[將利用、擴展或整合哪些現有程式碼]

## Architecture
[基於現有模式的系統架構]

## Components and Interfaces
[詳細的元件規格，包含重複使用的機會]

## Data Models
[遵循既定模式的資料結構]

## Error Handling
[與當前方法一致的錯誤情境]

## Testing Strategy
[使用現有工具與模式的測試方法]
```

## Next Phase
批准後，進入 `/spec-tasks` 階段。
