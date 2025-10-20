---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'todos']
description: '建立新的功能規格 (SPEC)'
---

## Usage
```
/spec-create <feature-name> <description>
```

## Purpose

將高階的產品需求（PRD）轉化為具體、無歧義且可執行的功能規格（SPEC），作為開發團隊實作與測試的清晰指南。

## Processing Flow

### 1. 前置檢查

一次性執行所有檢查，收集問題後統一回報：

**驗證 `<feature-name>` 格式**
- 僅僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元

**檢查必要文件**
- `.bmad-core/templates/spec-template.md`
- `.bmad-core/steering/product.md`
- `.bmad-core/changes/{feature-name}/prd.md`

**檢查檔案**
- 檢查 `.bmad-core/changes/{feature-name}/spec.md` 是否已存在

若有問題，一次性列出所有錯誤。若 spec.md 已存在，詢問「是否覆寫？」並等待確認後繼續。

### 2. 環境設置

建立 `.bmad-core/changes/{feature-name}/` 目錄 （若不存在）

### 3. 生成需求文件

- 依據 `spec-template.md` 定義的「## Guidelines」與「## Process Flow」，生成 SPEC 文件
- 完成後，一次性寫入 `.bmad-core/changes/{feature-name}/spec.md`

**[若無 [NEEDS CLARIFICATION] 標記]**
```
✨ SPEC 文件生成完成！

📋 摘要：
- 功能範圍：[簡述核心功能]
- 需求數量：[X 個主要需求]
- 驗收標準：[Y 個驗收標準]

請確認內容是否符合專案規範。確認後將進入設計階段。
```

**[若有 [NEEDS CLARIFICATION] 標記]**
```
⚠️ SPEC 文件已生成，但以下資訊需要補充：

1. [具體問題 1]（章節：[章節名稱]）
   💡 建議：[基於分析的推測與建議]

2. [具體問題 2]（章節：[章節名稱]）
   💡 建議：[基於分析的推測與建議]

請提供補充資訊，或確認建議方向是否可行。
```

### 4. 互動循環

**處理使用者反饋：**

若使用者提出修改意見或補充待澄清資訊：
  a. 在本步驟內直接修改需求文件內容
  b. **若補充了待澄清資訊，必須：**
    - 移除對應的 `[NEEDS CLARIFICATION]` 標記
    - 更新「待補充資訊」章節：
      * 若所有問題已解決 → 刪除整個「## 待補充資訊」章節
      * 若仍有未解決問題 → 僅移除已解決項目
  c. **若回應「跳過」：**
    - **指定項目**（如「1. 跳過」）→ 僅移除該項標記及對應條目
    - **未指定項目**（僅回應「跳過」）→ 移除所有標記並刪除整個「## 待補充資訊」章節
  d. 修改完成後，再次呈現摘要並請使用者確認
  e. 重複此流程直至獲得最終確認

## Output

`.bmad-core/changes/{feature-name}/sepc.md`

## Example

```bash
/spec-create user-authentication
```