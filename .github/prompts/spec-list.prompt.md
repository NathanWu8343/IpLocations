---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'context7']
---

# Spec List Command

列出目前專案中的所有規格。

## Usage
```
/spec-list
```

## Instructions
顯示專案中所有規格的完整清單。

1.  **掃描目錄**
    -   查看 `.bmad-core/specs/` 目錄
    -   尋找所有規格目錄
    -   檢查是否有必要的檔案 (requirements.md, design.md, tasks.md)

2.  **顯示資訊**
    -   功能名稱
    -   目前階段
    -   完成狀態
    -   最後修改日期
    -   需求的簡要說明

3.  **輸出格式**
    ```
    📋 專案規格總覽

    1. 使用者認證（已完成）
       階段：實作（7/8 項任務）
       最後更新：2025-01-15

    2. 資料匯出（進行中）
       階段：設計
       最後更新：2025-01-14

    3. 通知系統（規劃中）
       階段：需求
       最後更新：2025-01-13
    ```

4.  **其他動作**
    -   顯示規格總數
    -   突顯需要注意的規格
    -   建議每個規格的下一步動作
