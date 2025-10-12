---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'context7/*', 'usages', 'think', 'changes', 'fetch', 'extensions', 'todos']
description: '建立或更新專案指導文件，提供 AI agents 持續性背景脈絡'
---

# Document Steering Setup Command

建立或更新專案指導文件，協助 AI agents 理解專案背景、技術慣例與架構模式。

## Usage
```
/document-steering
```

## Instructions

你是整體系統架構師與全端技術領導者，負責建立或更新專案指導文件，使 AI agents 能有效理解並貢獻於程式碼庫。

## Goal

建立四份專案指導文件，作為 AI agents 的持續性上下文，確保所有自動化開發工作符合專案慣例與架構決策。

### Division of Responsibilities for Steering Documents

- **product.md**: 產品目標、使用者需求、核心功能、商業價值
- **tech.md**: 技術選型、依賴版本、函式庫使用要點、已知限制
- **architecture.md**: 系統架構、模組互動、設計模式、架構決策
- **structure.md**: 目錄組織、命名慣例、程式碼結構、檔案組織

這些文件將在後續開發指令中自動作為上下文參考。

## Core Principles

- **專案驅動:** 以專案實況為中心，範本為輔助工具，不強制填充所有章節
- **客觀可驗證:** 內容必須基於可驗證的程式碼事實，避免主觀或模糊描述
- **明確標註不確定性:** 無法推斷的資訊標記 `[NEEDS CLARIFICATION: 具體問題]` 並提問
- **職責分離:** 嚴格區分四個文件範疇（產品、技術、架構、結構），內容不得重疊
- **簡潔優先:** 避免過度複雜的判斷邏輯，不產生無意義的「不適用」對話

## Processing Flow

### 1. 檢查必要檔案

**檢查範本檔案：**
- 確認 `.bmad-core/templates/` 目錄下存在以下四個範本：
  * `product-template.md`
  * `architecture-template.md`
  * `structure-template.md`
  * `tech-template.md`
- 若任一範本缺失則終止執行並回報錯誤

**檢查現有文件：**
- 檢查 `.bmad-core/steering/` 目錄是否存在
- 若存在現有指導文件（`product.md`、`tech.md`、`architecture.md`、`structure.md`），載入其內容
- 現有文件將作為分析的參考基準，新分析結果將與其比對並更新

### 2. 載入範本與專案分析

**載入範本檔案：**
- 讀取 `.bmad-core/templates/` 下四個範本檔案，解析章節與條件佔位符，建立結構化資料：
  - `product-template.md`
  - `architecture-template.md`
  - `structure-template.md`
  - `tech-template.md`

**規劃分析策略（使用 `think` tool）：**
根據範本需求，優先檢視關鍵檔案（如 `.sln`、`.csproj`、`Program.cs`、`README`），規劃探索順序。

**分析程式碼庫：**
- 依照規劃檢閱程式碼庫並記錄以下資訊：
  * 專案類型、目的、主要功能
  * 核心技術堆疊、應用程式架構模式
  * 目錄結構、設計模式、編碼慣例
  * 建置、部署、測試配置

**深度技術分析（使用 `Context7` MCP）**
- 盤點所有框架、函式庫、工具及其版本號
- 使用 Context7 MCP 查詢並彙整套件資訊，聚焦於：
  * 版本限制、相依性衝突、已知問題
  * 官方最佳實踐、效能警告、安全性注意事項
- 分析結果將作為產生 `tech.md` 的依據

**呈現分析推斷：**
基於程式碼分析，結構化呈現專案推斷，每項標註信心度（高/中/低）。
輸出格式如下：

```
根據分析，推斷如下：

**產品詳細資訊：**
- [推斷細節 1] (信心度: 高/中/低)

**技術堆疊：**
- [技術細節 1] (信心度: 高/中/低)

**應用程式架構：**
- [架構細節 1] (信心度: 高/中/低)

**專案結構：**
- [結構細節 1] (信心度: 高/中/低)

**Context7 使用情況：**
- [觀察結果 1] (信心度: 高/中/低)
```

結尾詢問：「請檢視以上推斷內容。是否有需要修正、補充或進一步說明之處？」

### 3. 範本預填充與章節篩選

**篩選與填充（使用 `think` tool）：**

逐一處理範本每個章節：
- **專案有相關內容** → 保留章節，填充已知資訊，缺失部分標記 `[NEEDS CLARIFICATION: 具體問題]`
- **專案無相關內容** → 移除整個章節
- **條件章節 `(*如適用*)`** → 專案不涉及則直接移除

移除所有佔位符 `(*如適用*)`

### 4. 產生指導文件與呈現完成狀態

若 `.bmad-core/steering/` 目錄不存在則自動建立。

**產生文件時必須：**
1. 內容具體且精確（無模糊描述或推測）
2. 嚴格職責分離（每個檔案專注單一領域，避免內容重疊）
3. 只保留「專案實際涉及」的章節
4. 移除所有佔位符 `(*如適用*)`

**檔案對應範本：**
- `product.md` ← `.bmad-core/templates/product-template.md`
- `architecture.md` ← `.bmad-core/templates/architecture-template.md`
- `structure.md` ← `.bmad-core/templates/structure-template.md`
- `tech.md` ← `.bmad-core/templates/tech-template.md`（含 Context7 技術要點）

**處理原則：** 依「Clarification Handling」處理所有標記

**呈現完成狀態：**

✅ 已更新指導文件於 `.bmad-core/steering/`

**[若無 [NEEDS CLARIFICATION] 標記]**
```
文件已基於程式碼分析完成，無需額外補充。
請檢視文件內容，如需調整請告知。
```

**[若有 [NEEDS CLARIFICATION] 標記，使用極簡格式]**
```
⚠️ 以下資訊待補充：

**產品** (product.md)
- 問題標題 (章節: 章節名)

**技術** (tech.md)
- 問題標題 (章節: 章節名)

**架構** (architecture.md)
- 問題標題 (章節: 章節名)

**結構** (structure.md)
- 問題標題 (章節: 章節名)

請補充上述資訊，或標示「跳過」。
```

**處理用戶反饋：**
- 依用戶意見更新文件，依「Clarification Handling」處理標記
- 修改完成後再次呈現完成狀態並詢問：「修改完成，請確認是否還需要調整？」
- 持續此流程直至用戶明確表示滿意（如「確認無誤」「可以了」「沒問題」等）

## Clarification Handling

統一處理所有 `[NEEDS CLARIFICATION]` 標記的規則。

**三種處理情境:**
1. **資訊已提供:**
   - 替換 `[NEEDS CLARIFICATION: ...]` 所在整行為提供的內容
   - 從開頭「## 待補充資訊」列表移除該項
   - 若列表已空，移除整個章節

2. **明確標示不適用:**
   - 移除該標記所在章節或條目(含標題)
   - 從開頭列表移除該項

3. **資訊仍缺失:**
   - 保留原標記不變
   - 確保開頭存在「## 待補充資訊」章節
   - 列表格式： `- [問題描述] (章節: [章節名])`

## Output

四個指導文件（若已存在則更新）：
- `.bmad-core/steering/product.md`
- `.bmad-core/steering/architecture.md`
- `.bmad-core/steering/structure.md`
- `.bmad-core/steering/tech.md`

## Next Steps

建立指導文件後，它們將在以下指令中自動被參考：
- `/spec-create` - 使需求與產品願景保持一致
- `/spec-design` - 遵循既定的技術與架構模式
- `/spec-tasks` - 使用正確的檔案組織
- `/spec-execute` - 遵循所有慣例進行實作

## Critical Reminders

**❌ NEVER：**
- 強制套用範本所有章節到專案
- 對專案不存在的內容進行詢問
- 產生「不適用」「跳過」的無意義問答
 
**✅ ALWAYS：**
- 以專案實際情況為準，範本僅作為記錄格式參考
- 只保留專案實際涉及的章節
- 只詢問真正需要且缺失的資訊