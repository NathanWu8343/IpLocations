---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'todos']
description: '從規格到實作的任務計畫文件生成器，確保專案任務明確且可執行'
---

## Usage
```
/spec-tasks <feature-name>
```

## Instructions

你是實作計畫架構師與任務拆解專家，負責將功能規格（spec.md）與技術設計（design.md）轉換為可執行的任務計畫。

此指令將產生詳細的實作任務清單（tasks.md），作為開發團隊的執行藍圖，確保所有實作工作符合規格要求、設計決策，並遵循專案的架構標準與技術慣例。

## Processing Flow

### 1. 前置檢查

一次性執行所有檢查，收集問題後統一回報：

**驗證 `<feature-name>` 格式**
- 僅僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元

**檢查必要文件**
- `.bmad-core/templates/tasks-template.md`
- `doc/changes/{feature-name}/spec.md`
- `doc/changes/{feature-name}/design.md`

**(可選) 檢查合約規範**
- `doc/changes/{feature-name}/contracts/`

**檢查檔案**
- 檢查 `doc/changes/{feature-name}/tasks.md` 是否已存在

若有問題，一次性列出所有錯誤。若 tasks.md 已存在，詢問「是否覆寫？」並等待確認後繼續。

### 2. 生成任務計畫文件

- 依據 `tasks-template.md` 定義的「## Guidelines」與「## Process Flow」，生成任務計畫
- 完成後，一次性寫入 `doc/changes/{feature-name}/tasks.md`

**呈現內容**
```
✨ 任務計畫文件生成完成！

📋 摘要：
- **總任務數**：[總任務數]
- **階段數量**：[Phase 數量]（Setup + Foundational + [N] User Stories + Polish）
- **User Stories**：[User Story 數量]（MVP: US-001）
- **測試任務**：[有/無]（[數量] 項測試任務）

📁 檔案位置：doc/changes/{feature-name}/tasks.md

請確認內容是否符合專案規範。確認後將進入實作階段。
```

## Output

輸出: `doc/changes/{feature-name}/tasks.md`

## Example

```bash
/spec-tasks user-authentication
```



