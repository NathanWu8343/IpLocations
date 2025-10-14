---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'todos']
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

## Processing Flow

### 1. 前置檢查

一次性執行所有檢查，收集問題後統一回報：

**驗證 `<feature-name>` 格式**
- 僅僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元

**檢查必要文件**
- `.bmad-core/templates/requirements-template.md`
- `.bmad-core/steering/product.md`

**檢查檔案**
- 檢查 `.bmad-core/specs/{feature-name}/requirements.md` 是否已存在

若有問題，一次性列出所有錯誤。若 requirements.md 已存在，詢問「是否覆寫？」並等待確認後繼續。

### 2. 環境設置與資料載入

建立 `.bmad-core/specs/{feature-name}/` 目錄 （若不存在）

**載入資料**
- 讀取 `.bmad-core/steering/product.md`（提取產品目標、使用者需求）
- 讀取 `.bmad-core/templates/requirements-template.md`（取得生成規則）

### 3. 生成需求文件

- 依據 `requirements-template.md` 定義的執行流程與撰寫規範，生成需求文件
- 完成後，一次性寫入 `.bmad-core/specs/{feature-name}/requirements.md`

**[若無 [NEEDS CLARIFICATION] 標記]**
```
✨ 需求文件生成完成！

📋 摘要：
- 功能範圍：[簡述核心功能]
- 需求數量：[X 個主要需求]
- 成功標準：[Y 個可衡量指標]

請確認內容是否符合專案規範。確認後將進入設計階段。
```

**[若有 [NEEDS CLARIFICATION] 標記]**
```
⚠️ 需求文件已生成，但以下資訊需要補充：

1. [具體問題 1]（位於：[章節名稱]）
   💡 建議：[基於分析的推測與建議]

2. [具體問題 2]（位於：[章節名稱]）
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
      * 若所有問題已解決 → 刪除整個「待補充資訊」章節
      * 若仍有未解決問題 → 僅移除已解決項目
  c. 修改完成後，再次呈現摘要並請使用者確認
  d. 重複此流程直至獲得最終確認

## Output

`.bmad-core/specs/{feature-name}/requirements.md`

## Example

```bash
/spec-create user-authentication "允許使用者安全地註冊和登入"
```