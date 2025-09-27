# Spec List Command

列出目前專案中的所有規格。

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
    📋 Project Specs Overview

    1. user-authentication (Complete)
       Phase: Implementation (7/8 tasks)
       Last updated: 2025-01-15

    2. data-export (In Progress)
       Phase: Design
       Last updated: 2025-01-14

    3. notification-system (Planning)
       Phase: Requirements
       Last updated: 2025-01-13
    ```

4.  **其他動作**
    -   顯示規格總數
    -   突顯需要注意的規格
    -   建議每個規格的下一步動作
