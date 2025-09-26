---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'context7']
---

# Spec Tasks Command

根據批准的設計生成實作任務列表。


## Instructions
您正在處理 spec 工作流程的任務階段。

**WORKFLOW**: 這是命令生成之前的最後一個步驟。
**SEQUENCE**: Create Tasks → Get Approval → Ask User → Generate Commands
**DO NOT** 在任務被批准前運行任務命令生成。

1. **先決條件**
   - 確保 design.md 存在且已被批准
   - 加載 requirements.md 和 design.md 以獲取上下文
   - **加載 structure.md**: 檢查專案結構慣例
   - 理解完整的功能範圍

2. **生成任務列表** (優先考慮程式碼重用並遵循慣例)
   - 將設計分解為原子級、可執行的編碼任務
   - **遵循 structure.md**: 確保任務尊重專案文件組織
   - **優先擴展/修改現有程式碼**，而不是從頭開始構建
   - 使用帶有編號層級的複選框格式
   - 每個任務都應引用具體的需求和要利用的現有程式碼
   - 僅專注於編碼任務 (不包括部署、使用者測試等)

3. **任務指南**
   - 任務應該是具體且可操作的
   - **引用現有程式碼以供重用**: 包括要擴展或修改的具體文件/組件
   - 包括具體的文件名和組件
   - 漸進式構建 (每個任務都建立在前一個任務之上)
   - 使用 _Requirements: X.Y_ 格式引用需求
   - 採用測試驅動開發方法，利用現有的測試模式

4. **Task Format**
   ```markdown
   - [ ] 1. Task description
     - Sub-bullet with details
     - Specific files to create/modify
     - _Leverage: existing-component.ts, utils/helpers.js_
     - _Requirements: 1.1, 2.3_
   ```

5. **排除的任務**
   - 使用者驗收測試
   - 部署到生產環境
   - 效能指標收集
   - 使用者培訓或文件
   - 業務流程變更

6. **批准流程**
   - 提交完整的任務列表
   - 詢問：「這些任務看起來可以嗎？」
   - 根據回饋進行修訂
   - 繼續直到獲得明確批准

7. **生成任務命令** (僅在任務批准後)
   - **等待**: 在使用者明確批准任務之前，不要運行命令生成
   - **詢問使用者**: 「您希望我為每個任務生成單獨的命令以便於執行嗎？ (yes/no)」
   - **如果是**: 執行 `npx @pimzino/claude-code-spec-workflow@latest generate-task-commands {feature-name}`
   - **如果否**: 繼續使用傳統的 `/spec-execute` 方法
   - **目的**: 在 `.claude/commands/{feature-name}/` 中創建單獨的任務命令
   - **結果**: 每個任務都有自己的命令： `/{feature-name}-task-{task-id}`
   - **範例**: 創建 `/{feature-name}-task-1`, `/{feature-name}-task-2.1` 等。
   - **跨平台**: 自動適用於 Windows、macOS 和 Linux
   - **需要重啟**: 通知使用者重啟 Claude Code 以使新命令可見

## Task Structure
```markdown
# Implementation Plan

- [ ] 1. 設置專案結構
  - 遵循現有模式創建目錄結構
  - 擴展現有基底類別來定義核心介面
  - _Leverage: src/types/base.ts, src/models/BaseModel.ts_
  - _Requirements: 1.1_

- [ ] 2. 實作資料模型
- [ ] 2.1 創建基底模型類別
  - 擴展現有的驗證工具
  - 使用現有的測試輔助工具編寫單元測試
  - _Leverage: src/utils/validation.ts, tests/helpers/testUtils.ts_
  - _Requirements: 2.1, 2.2_
```

## Next Phase
在批准和命令生成之後：
1. **重啟 Claude Code** 以使新命令可見
2. 然後您可以：
   - 使用 `/spec-execute` 來實作任務
   - 使用單獨的任務命令： `/{feature-name}-task-1`, `/{feature-name}-task-2`, 等。
   - 使用 `/spec-status {feature-name}` 檢查進度
