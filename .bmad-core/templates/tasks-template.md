# Tasks Plan Template

## Guidelines

本模板用於將功能規格與技術設計轉換為可執行的任務計畫。

**核心要求**：
- 規劃**任務的拆解與執行順序**，而非**程式碼層級的技術實作**
- 任務依 **User Story** 組織，每個 Story 完成後可獨立驗證與部署
- 測試為**可選**：僅在 `spec.md` 明確要求或使用者要求 TDD 時產生，且必須測試先行

### Phase Structure（階段結構）

生成的任務計畫必須遵循以下階段架構：

**Phase 1: Setup（設定）**
- 目的：專案初始化
- 特性：無阻塞性，可立即開始
- 內容：專案結構、相依性安裝、工具設定

**Phase 2: Foundational（基礎）⚠️**
- 目的：阻擋性先決條件
- 特性：**必須**在任何 User Story 前完成
- 內容：所有 User Story 共用的基礎設施（資料庫、認證、路由框架、錯誤處理等）

**Phase 3+: User Stories（使用者故事）**
- 目的：依優先順序實作功能
- 特性：全部相依 Foundational，彼此間可並行
- 結構：每個 User Story（P1、P2、P3...）擁有獨立的 Phase
- User Story 內部順序：測試 (若有要求) → 模型 → 服務 → 端點 → 整合
- 檢查點：每個 Phase 完成後，該 User Story 應可獨立測試

**Phase N: Polish（優化與完善）**
- 目的：橫切面關注點
- 特性：相依所有 User Story 完成
- 內容：文件更新、程式碼清理、效能最佳化、安全性強化

### Task Scope（任務範圍）

**單一任務原則**：
- 一個任務對應一個明確的交付物（單一檔案、單一功能、單一整合點）
- 任務完成後可獨立驗證結果

**顆粒度判斷**：
- ✅ 適當：`在 src/services/auth.py 中實作 authenticate() 方法`
- ❌ 過大：`實作使用者認證功能`（涵蓋多個檔案與職責）
- ❌ 過小：`在檔案開頭新增 import 語句`（無獨立價值）

**必要條件**：
- 任務描述包含具體檔案路徑
- 任務動作明確（建立/實作/整合/設定）
- 有可驗證的完成標準

**測試任務合併規則**：

同一測試檔案的多個場景合併為單一任務，使用項目列表。

格式：
```markdown
- [ ] T031 [US-001] 在 `tests/integration/RegisterTests.cs` 中撰寫完整的註冊測試套件
  - 成功註冊：有效輸入應回傳 201
  - 欄位驗證：空白欄位應回傳 400
  - 格式驗證：無效格式應回傳 400
```


### Task Format（任務格式）

任務採用以下格式：

```
[TaskID] [Story] [描述]
```

**欄位定義**：

- **[TaskID]**：執行順序中的序號（T001、T002、T003...），全局唯一且遞增

- **[Story]**：格式為 `[US-001]`、`[US-002]`、`[US-003]`，對應至 `spec.md` 的 User Story 編號
  - **僅在 User Story Phase（Phase 3+）中使用**
  - Setup、Foundational、Polish Phase 不使用 Story 標籤

- **[描述]**：必須包含清晰的動作 + 確切的檔案路徑

### Task Organization（任務組織邏輯）

將輸入文件的元件映射到 Phase 結構，遵循以下規則：

1. **識別 User Stories（從 spec.md）**
   - 提取所有 User Stories 及其優先順序（P1、P2、P3...）
   - 為每個 User Story 建立獨立的 Phase（Phase 3+）
   - P1 User Story 標註為 MVP

2. **分配資料模型（從 design.md）**
   - 單一 User Story 使用的模型 → 歸入該 User Story 的 Phase
   - 多個 User Story 共用的模型 → 歸入最早的 User Story 或 Foundational Phase
   - 模型關聯 → 在適當的 User Story Phase 中的服務層任務

3. **分配服務邏輯（從 design.md）**
   - 每個服務 → 對應至使用該服務的 User Story Phase
   - 服務間的相依性 → 確保在同一 User Story 或更早的 Phase

4. **分配 API 端點（從 design.md 或 contracts/）**
   - 每個 endpoint/contract → 對應至其服務的 User Story Phase
   - 若有要求測試：Contract 測試任務 → 在實作任務之前

5. **分配基礎設施（從 design.md）**
   - 共享基礎設施 → Setup Phase（Phase 1）
   - 阻擋性基礎任務 → Foundational Phase（Phase 2）
   - User Story 特定的設定 → 在該 User Story 的 Phase 內

## Process Flow

### 1. 載入指導文件與需求來源

**依序讀取以下文件**：

**(1-2) 功能層級來源**
- `doc/changes/{feature-name}/spec.md` → User Story、成功指標、功能需求
- `doc/changes/{feature-name}/design.md` → 設計決策、元件結構、技術選型
  - ⚠️ 必須載入完整內容，不可遺漏任何設計細節

**(可選) 合約規範**
- `doc/changes/{feature-name}/contracts/` → OpenAPI/GraphQL 結構描述（若存在）

### 2. 生成文件草稿

**執行順序**：
a. 為每個 User Story 建立獨立的 Phase（依 Task Organization 規則分配元件）
b. 填寫 Setup 與 Foundational Phase（識別共享與阻塞性基礎）
c. 填寫 Polish Phase（橫切面關注點）
d. 補充 Dependencies 說明

### 3. 草稿反思與精煉

**審視任務計畫品質：**
從執行者視角重新審視整份內容，檢查並修正：

a. **任務拆解品質**
   - 任務顆粒度符合單一職責原則
   - 無過度細分或過於籠統的描述
   - 每個任務有明確的交付物

b. **執行順序合理性**
   - Phase 間依賴清晰（Setup → Foundational → Stories → Polish）
   - Phase 內任務順序遵循 Phase Structure 定義

c. **階段完整性**
   - Foundational 包含所有關鍵基礎任務
   - 所有 User Story 有對應階段，無遺漏

d. **可執行性**
   - 任務描述包含具體檔案路徑與明確動作
   - [Story] 標籤正確標註
   - 每個 Story 檢查點可獨立驗證

若發現問題 → 直接修正 → 繼續後續步驟

## Template

⚠️ **重要提醒**：
- 以下所有 T001-T028+ 的範例任務**僅為格式示意**
- 實際生成時，**必須**根據 `spec.md` 和 `design.md` 重新撰寫所有任務內容
- **禁止直接使用或稍作修改範例任務**
- 範例中的檔案路徑、框架名稱、功能描述都必須替換為實際專案的內容

以下為最終任務文件的 Markdown 範本：

```markdown
# Implementation Plan

## Phase 1: Setup (共享基礎設施)

**目的**: 專案初始化和基本結構

- [ ] T001 根據實作計畫建立專案結構
- [ ] T002 使用 [framework] 相依性初始化 [language] 專案
- [ ] T003 設定 linting 和格式化工具

---

## Phase 2: Foundational (阻塞性先決條件)

**目的**: 必須在任何 User Story 可以實作之前完成的核心基礎設施

**⚠️ 關鍵**: 在此階段完成之前，任何 User Story 的工作都不能開始

基礎任務範例 (根據您的專案進行調整):

- [ ] T004 設定資料庫結構描述和遷移框架
- [ ] T005 實作驗證/授權框架
- [ ] T006 設定 API 路由和中介軟體結構
- [ ] T007 建立所有 Story 都相依的基礎模型/實體
- [ ] T008 設定錯誤處理和日誌記錄基礎設施
- [ ] T009 設定環境組態管理

**✅ 檢查點**: 基礎已就緒 -  User Story 實作現在可以並行開始

---

## Phase 3: US-001 - [標題] (優先順序: P1) 🎯 MVP

**目標**: [此 User Story 交付內容的簡要說明]

**獨立測試**: [如何自行驗證此 Story 是否正常運作]

### US-001 的測試 (可選 - 僅當要求測試時) ⚠️

**注意：請先撰寫這些測試，確保它們在實作前會失敗**

- [ ] T010 [US-001] 在 tests/contract/test_[name].py 中為 [endpoint] 進行合約測試
  - 成功註冊：有效輸入應回傳 201
  - 欄位驗證：空白欄位應回傳 400
  - 格式驗證：無效格式應回傳 400
- [ ] T011 [US-001] 在 tests/integration/test_[name].py 中為 [user journey] 進行整合測試
  - 有效格式：標準格式應通過驗證
  - 無效格式：缺少 @ 或域名應驗證失敗

### User Story 1 的實作

- [ ] T012 [US-001] 在 src/models/[entity1].py 中建立 [Entity1] 模型
- [ ] T013 [US-001] 在 src/models/[entity2].py 中建立 [Entity2] 模型
- [ ] T014 [US-001] 在 src/services/[service].py 中實作 [Service] (相依於 T012、T013)
- [ ] T015 [US-001] 在 src/[location]/[file].py 中實作 [endpoint/feature]
- [ ] T016 [US-001] 新增驗證和錯誤處理
- [ ] T017 [US-001] 為 US-001 的操作新增日誌記錄

**✅ 檢查點**: 此時，US-001 應完全正常運作並可獨立測試

---

## Phase 4: US-002 - [標題] (優先順序: P2)

**目標**: [此 Story 交付內容的簡要說明]

**獨立測試**: [如何自行驗證此 Story 是否正常運作]

### US-002 的測試 (可選 - 僅當要求測試時) ⚠️

- [ ] T018 [US-002] 在 tests/contract/test_[name].py 中為 [endpoint] 進行合約測試
  - [列出所有測試場景與預期行為]
- [ ] T019 [US-002] 在 tests/integration/test_[name].py 中為 [user journey] 進行整合測試
  - [列出所有測試場景與預期行為]

### US-002 的實作

- [ ] T020 [US-002] 在 src/models/[entity].py 中建立 [Entity] 模型
- [ ] T021 [US-002] 在 src/services/[service].py 中實作 [Service]
- [ ] T022 [US-002] 在 src/[location]/[file].py 中實作 [endpoint/feature]
- [ ] T023 [US-002] 與 US-001 的元件整合 (如果需要)

**✅ 檢查點**: 此時，US-001 和 US-002 都應能獨立運作

---

## Phase 5: US-003 - [標題] (優先順序: P3)

**目標**: [此 Story 交付內容的簡要說明]

**獨立測試**: [如何自行驗證此 Story 是否正常運作]

### US-003  的測試 (可選 - 僅當要求測試時) ⚠️

- [ ] T024 [US-003] 在 tests/contract/test_[name].py 中為 [endpoint] 進行合約測試
  - [列出所有測試場景與預期行為]
- [ ] T025 [US-003] 在 tests/integration/test_[name].py 中為 [user journey] 進行整合測試
  - [列出所有測試場景與預期行為]

### US-003 的實作

- [ ] T026 [US-003] 在 src/models/[entity].py 中建立 [Entity] 模型
- [ ] T027 [US-003] 在 src/services/[service].py 中實作 [Service]
- [ ] T028 [US-003] 在 src/[location]/[file].py 中實作 [endpoint/feature]

**✅ 檢查點**: 所有 User Story 現在都應能獨立運作

---

[視需要新增更多 User Story 階段，遵循相同的模式]

---

## Phase N: Polish & Cross-Cutting Concerns

**目的**: 影響多個 User Story 的改進

- [ ] TXXX docs/ 中的文件更新
- [ ] TXXX 程式碼清理和重構
- [ ] TXXX 所有 Story 的效能最佳化
- [ ] TXXX tests/unit/ 中的額外單元測試 (如果要求)
- [ ] TXXX 安全性強化

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: 無相依性 - 可立即開始
- **Foundational (Phase 2)**: 相依於 Setup 完成 - **阻塞**所有 User Story
- **User Stories (Phase 3+)**: 全部相依於 Foundational 階段完成
  - User Story 接著可以並行進行 (如果人力充足)
  - 或按優先順序循序進行 (P1 → P2 → P3)
- **Polish (Final Phase)**: 相依於所有期望的 User Story 都已完成

### User Story Dependencies

- **US-001 (P1)**: 可在 Foundational (Phase 2) 之後開始 - 不相依於其他 Story
- **US-002 (P2)**: 可在 Foundational (Phase 2) 之後開始 - 可能與 US-001  整合，但應可獨立測試
- **US-003 (P3)**: 可在 Foundational (Phase 2) 之後開始 - 可能與 US-001 / US-002 整合，但應可獨立測試

### Within Each User Story

- 測試 (如果包含) **必須**在實作前撰寫並**失敗**
- 模型先於服務
- 服務先於端點
- 核心實作先於整合
- User Story 完成後再移至下一個優先順序

## Implementation Strategy

### MVP First (僅限 User Story 1)

1. 完成 Phase 1: Setup
2. 完成 Phase 2: Foundational (**關鍵** - 阻塞所有 User Story)
3. 完成 Phase 3: User Story 1
4. **停止並驗證**: 獨立測試 User Story 1
5. 如果準備就緒，則部署/展示

### Incremental Delivery

1. 完成 Setup + Foundational → 基礎就緒
2. 新增 User Story 1 → 獨立測試 → 部署/展示 (MVP!)
3. 新增 User Story 2 → 獨立測試 → 部署/展示
4. 新增 User Story 3 → 獨立測試 → 部署/展示
5. 每個 Story 都增加價值，而不會破壞先前的美好

---

## Notes

- [Story] 標籤將任務對應到特定的 User Story 以實現可追蹤性
- 每個 User Story 都應能獨立完成和測試
- 在實作前驗證測試是否失敗
- 在每個任務或邏輯群組後提交
- 在任何檢查點停止以獨立驗證 Story
- 避免：模糊的任務、相同的檔案衝突、破壞獨立性的跨 Story 相依性

---
```