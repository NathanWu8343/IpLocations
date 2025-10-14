---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'context7/*', 'usages', 'think', 'todos']
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

## Purpose

建立四份專案指導文件（product、tech、architecture、structure），作為 AI agents 的持續性上下文，確保所有自動化開發工作符合專案慣例與架構決策。

## Core Principles

- **專案驅動：** 以專案實況為中心，範本為輔助工具，不強制填充所有章節
- **客觀可驗證：** 內容必須基於可驗證的程式碼事實，避免主觀或模糊描述
- **職責分離：** 嚴格區分四個文件範疇，內容不得重疊
- **簡潔精確**：內容具體明確，無模糊描述或推測

## 文件職責範疇

- **product.md**：目的、核心功能、目標使用者
- **tech.md**：主要框架、函式庫、工具及其版本
- **architecture.md**：應用程式架構模式、模組邊界與依賴方向
- **structure.md**：目錄組織、命名慣例、設計模式

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

### 2. 專案分析

**載入範本檔案：**
讀取 `.bmad-core/templates/` 下四個範本檔案，解析章節結構

**規劃分析策略（內部思考）：**
根據範本需求，優先檢視關鍵檔案（如 `.sln`、`.csproj`、`Program.cs`、`README`），規劃探索順序

**分析程式碼庫：**
依照規劃檢閱程式碼庫並記錄以下資訊：
- 專案類型、目的、主要功能
- 核心技術堆疊、應用程式架構模式
- 目錄結構、設計模式、編碼慣例
- 建置、部署、測試配置

**依賴套件分析（使用 `Context7` MCP）：**
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
- [推斷細節 1] （信心度： 高/中/低）

**技術堆疊：**
- [技術細節 1] （信心度： 高/中/低）

**應用程式架構：**
- [架構細節 1] （信心度： 高/中/低）

**專案結構：**
- [結構細節 1] （信心度： 高/中/低）

**Context7 使用情況：**
- [觀察結果 1] （信心度： 高/中/低）
```

結尾詢問：「請檢視以上推斷內容。是否有需要修正、補充或進一步說明之處？」

### 3. 產生文件

**篩選與填充（內部思考）：**

逐一處理範本每個章節：
- **專案有相關內容** → 保留章節，填充已知資訊，缺失部分標記 `[NEEDS CLARIFICATION： 具體問題]`
- **專案無相關內容** → 移除整個章節
- **條件章節 `（*如適用*）`** → 專案不涉及則直接移除

**建立目錄與生成文件：**
- 若 `.bmad-core/steering/` 目錄不存在則自動建立
- 產生四個指導文件：
  * `product.md` ← `product-template.md`
  * `architecture.md` ← `architecture-template.md`
  * `structure.md` ← `structure-template.md`
  * `tech.md` ← `tech-template.md`（含 Context7 技術要點）

**如文件中出現 `[NEEDS CLARIFICATION]` 標記：**
- 於文件最前方新增「## 待補充資訊」章節
- 以清單方式列出所有需澄清問題：
  - `[問題描述]（章節：[章節名稱]）`
- 每個問題需明確標示所屬章節

**版本歷史規則：**
- **首次建立**： 版本 `1.0`，變更摘要填 `初始建立`，作者填 `AI`
- **文件更新**： 版本號 +0.1 （如 1.0 → 1.1 → 1.2），變更摘要簡述主要變更（5~10 字內）
- **新版本記錄添加在表格最下方**，保留所有歷史版本

**呈現完成狀態：**

✅ 已更新指導文件於 `.bmad-core/steering/`

**[若無 [NEEDS CLARIFICATION] 標記]**
```
文件已基於程式碼分析完成，無需額外補充。
請檢視文件內容，如需調整請告知。
```

**[若有 [NEEDS CLARIFICATION] 標記]**
```
⚠️ 以下資訊待補充：

**產品** （product.md）
- [具體問題 1] （位於：[章節名]） → 建議：[基於分析的推測與建議]

**技術** （tech.md）
- [具體問題 1] （位於：[章節名]） → 建議：[基於分析的推測與建議]

**架構** （architecture.md）
- [具體問題 1] （位於：[章節名]） → 建議：[基於分析的推測與建議]

**結構** （structure.md）
- [具體問題 1] （位於：[章節名]） → 建議：[基於分析的推測與建議]

請補充上述資訊，或標示「跳過」。
```

### 步驟 4：互動循環

**處理使用者反饋：**

若使用者提出修改意見或補充待澄清資訊：
  a. 在本步驟內直接修改需求文件內容
  b. **若補充了待澄清資訊，必須：**
    - 移除對應的 `[NEEDS CLARIFICATION]` 標記
    - 更新「待補充資訊」章節：
      * 若所有問題已解決 → 刪除整個「待補充資訊」章節
      * 若仍有未解決問題 → 僅移除已解決項目
  c. 修改完成後，再次呈現摘要並請使用者確認
  d. 重複此流程直至獲得最終確認

## Output

四個指導文件（若已存在則更新）：
- `.bmad-core/steering/product.md`
- `.bmad-core/steering/architecture.md`
- `.bmad-core/steering/structure.md`
- `.bmad-core/steering/tech.md`
