---
mode: agent
tools: ['createFile', 'createDirectory', 'editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'think', 'changes', 'fetch', 'githubRepo', 'todos', 'context7']
---

# Spec Tasks Command

根據已批准的設計文件，產生完整且可執行的實作任務列表。

## Usage
```
/spec-tasks <feature-name>
```

## Instructions
您正在處理規格工作流程的 **任務階段**，請嚴格依照以下步驟執行。

## Processing Flow

1. **先決條件檢查**
   - 確認 `.bmad-core/specs/{feature-name}/design.md` 檔案存在且已獲得使用者批准。
   - 若檔案不存在，立即中止並提示使用者：  
     「請先執行 `/spec-design` 或提供 design.md，才能進入任務階段。」
   - 載入 `requirements.md` 與 `design.md`，確保任務能完整追溯至需求與設計。

2. **載入指導文件**
   - `.bmad-core/steering/product.md` → 產品願景與目標  
   - `.bmad-core/steering/tech.md` → 技術標準與模式  
   - `.bmad-core/steering/structure.md` → 專案結構慣例  
   - `.bmad-core/steering/architecture.md` → 架構原則與模式
   - 任務必須明確符合這些文件的規範，並在必要時標示引用。

3. **生成任務列表**  
   - 使用 `.bmad-core/templates/tasks-template.md` 為唯一範本，建立任務文件。
   - 強制遵循 **程式碼重用優先** 與 **專案慣例**
   - 任務必須拆解為 **原子級、可執行的編碼步驟**。

   **原子化任務規範：**
   - **檔案範圍**：單一任務影響的檔案數 ≤ 3。  
   - **完成時間**：有經驗的工程師可於 15–30 分鐘內完成。  
   - **單一目的**：每個任務只產出一個明確、可測試的結果。  
   - **指定檔案**：必須明確列出要建立或修改的檔案名稱。  
   - **AI Agent 友善**：明確定義輸入/輸出，避免跨領域切換。  

   **任務格式指南**
   - 使用核取方塊格式：`- [ ] 任務編號. 任務描述`
   - **指定檔案**: 務必包含要建立/修改的確切檔案路徑
   - **包含實施細節** 作為項目符號
   - 使用以下方式參考需求：`_需求: X.Y, Z.A_`
   - 使用以下方式參考要利用的現有程式碼：`_利用: path/to/file.ts, path/to/component.tsx_`
   - 僅專注於編碼任務（無部署、使用者測試等）
   - **避免使用廣泛術語**: 任務標題中不要出現「系統」、「整合」、「完成」

   **好壞任務範例**
   ❌ **壞範例（過於廣泛）**:
   - 「實施驗證系統」（影響許多檔案，多個目的）
   - 「新增使用者管理功能」（範圍模糊，未指定檔案）
   - 「建立完整的儀表板」（規模太大，多個元件）

   ✅ **好範例（原子性）**:
   - 「在 models/user.py 中建立包含 email/password 欄位的 User 模型」
   - 「在 utils/auth.py 中使用 bcrypt 新增密碼雜湊工具」
   - 「在 components/LoginForm.tsx 中建立包含 email/password 輸入的 LoginForm 元件」 

   **實作指引：**
   - **遵循 structure.md**：任務必須符合專案目錄與檔案命名慣例。  
   - **優先擴充/調整既有程式碼**，僅在必要時新增檔案或模組。  
   - 僅包含 **編碼相關工作**。以下不得出現：  
     * 使用者驗收測試 (UAT)  
     * 部署/CI/CD 流程  
     * 效能監控與度量  
     * 使用者訓練或文件撰寫  
     * 業務流程或管理變更  

4. **品質驗證**

在最終確定任務之前，請驗證：
- [ ] 所有任務都有明確的驗收標準
- [ ] 任務規模合理（每個 1-3 天）
- [ ] 相依性合乎邏輯且可實現
- [ ] 合併的任務涵蓋所有史詩需求

5. **批准流程**
   - 呈現完整的任務列表給使用者審閱。
   - 詢問使用者確認：「這些任務看起來可以嗎？」
   - 收集回饋並進行修訂，直到獲得明確批准。

## Output

輸出: `.bmad-core/specs/{feature-name}/tasks.md`

## Example
/spec-tasks user-authentication

## Next Phase
在任務列表獲得批准後，您可以選擇：  
   - 使用 `/spec-execute 1 {feature-name}` 執行任務  
   - 使用 `/spec-status {feature-name}` 檢查進度


## CRITICAL REMINDERS

**❌ NEVER：**
- 未經使用者批准，不得進入後續階段。
- 任務階段完成前，不得自行產生實作程式碼或部署腳本。

**✅ ALWAYS：**
- 任務需完整追溯至需求與設計。
- 嚴格遵循 product.md、tech.md、structure.md 的規範。

