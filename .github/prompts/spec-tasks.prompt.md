---
mode: agent
tools: ['editFiles', 'search', 'runCommands', 'runTasks', 'usages', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'context7']
---

# Spec Tasks Command

根據已批准的設計文件，產生完整且可執行的實作任務列表。

## Instructions
您正在處理規格工作流程的 **任務階段**，請嚴格依照以下步驟執行。

## Processing Flow

1. **先決條件檢查**
   - 確認 `.bmad-core\specs\{feature-name}\design.md` 檔案存在且已獲得使用者批准。
   - 若檔案不存在或未經批准，立即中止並提示使用者：  
     「請先執行 `/spec-design` 或提供 design.md，才能進入任務階段。」
   - 載入 `requirements.md` 與 `design.md`，確保任務能完整追溯至需求與設計。

2. **載入指導文件**
   - `.bmad-core\steering\product.md` → 產品願景與目標  
   - `.bmad-core\steering\tech.md` → 技術標準與模式  
   - `.bmad-core\steering\structure.md` → 專案結構慣例  
   - 任務必須明確符合這些文件的規範，並在必要時標示引用。

3. **生成任務列表**  
   （強制遵循 **程式碼重用優先** 與 **專案慣例**）
   - 使用 `.bmad-core\templates\tasks-template.md` 作為格式範本。
   - 任務必須拆解為 **原子級、可執行的編碼步驟**。

   **原子化任務規範：**
   - **檔案範圍**：單一任務影響的檔案數 ≤ 3。  
   - **完成時間**：有經驗的工程師可於 15–30 分鐘內完成。  
   - **單一目的**：每個任務只產出一個明確、可測試的結果。  
   - **指定檔案**：必須明確列出要建立或修改的檔案名稱。  
   - **代理程式友善**：明確定義輸入/輸出，避免跨領域切換。  

   **範例：**
   - ❌ 不佳：「實作驗證系統」  
   - ✅ 佳：「在 `models/user.py` 中建立具備 email 與 password 欄位的 `User` 模型」  
   - ❌ 不佳：「新增使用者管理功能」  
   - ✅ 佳：「在 `utils/auth.py` 中使用 bcrypt 新增密碼雜湊工具函式」  

   **實作指引：**
   - **遵循 structure.md**：任務必須符合專案目錄與檔案命名慣例。  
   - **優先擴充/調整既有程式碼**，僅在必要時新增檔案或模組。  
   - 僅包含 **編碼相關工作**。以下不得出現：  
     * 使用者驗收測試 (UAT)  
     * 部署/CI/CD 流程  
     * 效能監控與度量  
     * 使用者訓練或文件撰寫  
     * 業務流程或管理變更  

4. **批准流程**
   - 呈現完整的任務列表給使用者審閱。
   - 詢問使用者確認：「這些任務看起來可以嗎？」
   - 收集回饋並進行修訂，直到獲得明確批准。

## CRITICAL REMINDERS

**❌ 絕對禁止：**
- 未經使用者批准，不得進入後續階段。
- 任務階段完成前，不得自行產生實作程式碼或部署腳本。

**✅ 必須遵守：**
- 任務需完整追溯至需求與設計。
- 嚴格遵循 product.md、tech.md、structure.md 的規範。

## Next Phase
在任務列表獲得批准後，您可以選擇：  
   - 使用 `/spec-execute 1 {feature-name}` 執行任務  
   - 使用 `/spec-status {feature-name}` 檢查進度
