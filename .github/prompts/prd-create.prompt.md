---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'todos']
description: '建立新的產品需求文件 (PRD)'
---

## Usage
```
/prd-create <feature-name> <description>
```

## Purpose

將功能構想轉化為結構化的產品需求文件 (PRD)，明確定義使用者需求、驗收標準與成功指標。

## Processing Flow

### 1. 前置檢查

一次性執行所有檢查，收集問題後統一回報：

**驗證 `<feature-name>` 格式**
- 僅僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元

**檢查必要文件**
- `.bmad-core/templates/prd-template.md`
- `.bmad-core/steering/product.md`

**檢查檔案**
- 檢查 `.bmad-core/changes/{feature-name}/prd.md` 是否已存在

若有問題，一次性列出所有錯誤。若 prd.md 已存在，詢問「是否覆寫？」並等待確認後繼續。

### 2. 環境設置

建立 `.bmad-core/changes/{feature-name}/` 目錄 （若不存在）

### 3. 生成文件

- 依據 `prd-template.md` 定義的「## Guidelines」與「## Process Flow」，生成 PRD 文件
- 完成後，一次性寫入 `.bmad-core/changes/{feature-name}/prd.md`

**[若無 [NEEDS CLARIFICATION] 標記]**
```
✨ 產品需求文件 (PRD) 已成功生成！

📋 生成摘要：
- 主要目的：[簡述功能的核心目標]
- 故事數量：[總計 X 個使用者故事]
- 成功指標：[Y 項可衡量的關鍵指標]

請檢視內容是否符合專案規範。若確認無誤，將自動進入下一階段 (SPEC)。
```

**[若有 [NEEDS CLARIFICATION] 標記]**
```
⚠️ 需求文件已生成，但以下資訊需要補充：

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

`.bmad-core/changes/{feature-name}/prd.md`

## Example

```bash
/prd-create user-authentication "允許使用者安全地註冊和登入"
```