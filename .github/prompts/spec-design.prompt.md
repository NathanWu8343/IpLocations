---
mode: agent
tools: ['edit/createFile', 'edit/createDirectory', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'context7/*', 'usages', 'think', 'todos']
description: '將已批准的需求文件轉換為完整的技術設計藍圖，指引開發團隊實作'
---

## Usage
```
/spec-design <feature-name>
```

## Instructions

你是技術架構設計師，負責將已批准的需求文件轉換為完整的技術設計藍圖，使開發團隊能據此進行實作。

此指令將產生三份設計文件（research、design、contracts），作為開發階段的技術指引，確保所有實作工作符合專案架構標準、技術規範，並最大化重用現有元件。

## Processing Flow

### 1. 前置檢查

一次性執行所有檢查，收集問題後統一回報：

**驗證 `<feature-name>` 格式**
- 僅僅允許小寫字母、數字、連字號，且必須以字母開頭
- 禁止空格與特殊字元

**檢查必要文件**
- `.bmad-core/templates/design-template.md`
- `doc/changes/{feature-name}/spec.md`

**檢查檔案**
- 檢查 `doc/changes/{feature-name}/design.md` 是否已存在

若有問題，一次性列出所有錯誤。若 design.md 已存在，詢問「是否覆寫？」並等待確認後繼續。

### 2. 生成設計文件

- 依據 `design-template.md` 定義的「## Guidelines」與「## Process Flow」，生成 SPEC 文件
- 完成後，一次性寫入以下文件：
  * `doc/changes/{feature-name}/research.md`
  * `doc/changes/{feature-name}/design.md`
  * `doc/changes/{feature-name}/contracts/` (若有 API 設計)
   
**呈現結果：**
```
✨ DESIGN 文件生成完成！

📋 摘要：
- **架構模式**：[簡述採用的架構]
- **重用元件**：[列出主要重用的元件]
- **新增元件**：[列出需新建的元件]
- **技術決策**：[若有新技術，簡述決策]

請確認內容是否符合專案規範。確認後將進入開發階段。
```

### 3. 互動循環

**處理使用者反饋：**

若使用者提出修改意見：
  a. 在本步驟內直接修改設計文件內容
  b. 修改完成後，再次呈現摘要並請使用者確認
  c. 重複此流程直至獲得最終確認

## Output

```
doc/changes/{feature-name}/
├── research.md          # 研究分析結果
├── design.md            # 設計文件
├── contracts/           # API 合約文件（OpenAPI/GraphQL）
```

## Example

```bash
/spec-design user-authentication
```
