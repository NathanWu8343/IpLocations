---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'context7']
---

# Spec Status Command

顯示所有規格或特定規格的目前狀態。

## Usage
```
/spec-status <feature-name>
```

## Instructions
顯示規格工作流程的目前狀態。

1.  **若未提供 feature-name：**
    -   列出 `.bmad-core/specs/` 目錄中的所有規格
    -   顯示每個規格的目前階段
    -   顯示完成狀態

2.  **若提供 feature-name：**
    -   顯示該規格的詳細狀態
    -   顯示目前工作流程階段
    -   顯示已完成與待處理的任務
    -   列出下一個建議的動作

3.  **狀態資訊：**
    -   需求：[已完成/進行中/待處理]
    -   設計：[已完成/進行中/待處理]
    -   任務：[已完成/進行中/待處理]
    -   實作：[X/Y 個任務已完成]

4.  **輸出格式：**
    ```
    Spec: user-authentication
    Phase: Implementation
    Progress: Requirements ✅ | Design ✅ | Tasks ✅
    Implementation: 3/8 tasks complete
    Next: Execute task 4 - "Implement password validation"
    ```

## Workflow Phases
-   **需求**：收集和記錄需求
-   **設計**：建立技術設計和架構
-   **任務**：分解為實作任務
-   **實作**：執行個別任務
-   **完成**：所有任務已完成並整合
