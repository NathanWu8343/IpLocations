
# Specification Template

## Guidelines

### 指導原則

- 定義系統**如何做 (How)** 以及**做到什麼品質 (How Well)**
- 描述系統邏輯、資料設計、行為規則與邊界條件
- 使用開發團隊能理解的語言，但**禁止指定實作技術**（API、框架、程式碼）
- 每個需求必須獨立、可測試、可驗證

### BDD Scenarios 格式規範

採用 Gherkin 語法：

```gherkin
Scenario: [場景描述]
  Given [前置條件]
  When [觸發動作]
  Then [預期結果]
```

**撰寫原則**：
- 每個 Scenario 對應 PRD 的一個 AC，並標註 `[對應：AC-XXX]`
- 一個 AC 應建立多個 Scenarios，涵蓋：正常流程、邊界情境、錯誤處理
- 使用具體數值和明確驗證點，避免模糊描述
- Given 描述前置狀態，When 描述操作，Then 描述可驗證結果
  
## Process Flow

### 1. 載入指導文件

**依序讀取並提取關鍵資訊：**

a. **產品脈絡**
   - 讀取 `.bmad-core/steering/product.md`
   - 提取：產品目的、核心功能、目標使用者

b. **需求來源**
   - 讀取 `.bmad-core/changes/{feature-name}/prd.md`
   - 提取：所有 User Stories（US-XXX）與 Acceptance Criteria（AC-XXX）
   - **注意**：必須載入完整的 PRD，不可遺漏任何 US

c. **輸出規範**
   - 讀取本文件的 `## Guidelines` 與 `## Template` 章節
   - 確認需產出的章節結構：FR、NFR、BDD Scenarios、Key Entities、Edge Cases 等
   - 理解 BDD Scenarios 的格式要求與撰寫原則

### 2. 需求轉換與分析

**從 PRD 轉換為 SPEC（內部思考）：**

a. **拆解使用者故事**
   - 將每個 User Story 拆解為多個 Functional Requirements（FR）
   - 每個 Acceptance Criteria 至少對應一個 FR
   - 識別隱含的系統行為（如資料驗證、狀態管理）
   - 標記來源：`[來源：prd.md#US-001-AC-001]`

b. **轉換為 BDD Scenarios**
   - 將每個 Acceptance Criteria 轉換為一個或多個 BDD Scenario
   - 每個 Scenario 只對應一個 AC，標註 `[對應：AC-XXX]`
   - 確保每個場景包含 Given-When-Then 結構
   - 補充邊界情境的測試場景（正常、異常、邊界）

c. **提取非功能性需求**
   - 從 PRD 的 Success Metrics 識別效能要求（NFR）
   - 從邊界情境識別安全性、可用性要求
   - 補充業界標準的非功能性需求

d. **識別關鍵元素**
   - 提取核心 **Key Entities**（資料物件、角色）

e. **處理不明確資訊**
   - 優先依據 PRD、產品脈絡、業界標準推斷
   - 僅在無法推斷時標記 `[NEEDS CLARIFICATION: 具體問題]`
   - 即使標記待釐清，仍需基於合理假設繼續完成

### 3. 填寫範本內容

依照 Template 結構逐步完整填寫每個章節。

**如內容含 `[NEEDS CLARIFICATION]` 標記時：**
- 請於文件指定佔位符新增「## 待補充資訊」章節
- 條列所有需澄清問題：`[問題描述]（章節：[章節名稱]）`

## Template

以下為最終需求文件的 Markdown 範本：

```markdown
# Feature Specification

[如有需澄清資訊，插入以下章節]
## 待補充資訊

- [問題 1]（章節：[章節名稱]）
- [問題 2]（章節：[章節名稱]）

## Goals
[一句話描述此功能成功後的結果]

- [達成 PRD 中的成功指標]
- [支援核心使用者情境]

## Background
[1-2 個簡短段落，總結背景，例如我們在 PRD 中學到的內容，避免與目標重複，這解決了什麼問題以及為什麼，目前的概況或需求是什麼]

## Functional Requirements

- **FR-001**: 系統必須 [具體能力] `[來源：prd.md#US-XXX-AC-XXX]`
- **FR-002**: 系統必須 [具體能力] `[來源：prd.md#US-XXX-AC-XXX]`

*不明確需求標註範例：*

- **FR-006**: 系統必須透過 [NEEDS CLARIFICATION： 未指定驗證方法 - 電子郵件/密碼, SSO, OAuth?] 來驗證使用者 `[來源：prd.md#US-XXX-AC-XXX]`
- **FR-007**: 系統必須將使用者資料保留 [NEEDS CLARIFICATION： 未指定保留期限] `[來源：prd.md#US-XXX-AC-XXX]`

## Non-Functional Requirements
- **NFR-001**: 系統應在 [NEEDS CLARIFICATION： 未指定負載量] 的同時支援至少 [NEEDS CLARIFICATION： 未指定使用者數量] 名使用者
- **NFR-002**: 系統應在 [NEEDS CLARIFICATION： 未指定時間] 內回應 95% 的請求
- **NFR-003**: 系統應符合 [NEEDS CLARIFICATION： 未指定標準] 的安全性標準
- **NFR-004**: 系統應具備 [NEEDS CLARIFICATION： 未指定備援需求] 的高可用性

## BDD Scenarios

[針對 PRD 中的每個 User Story，依序撰寫對應的 BDD Scenarios。每個 User Story 至少包含 3-5 個場景：正常流程、邊界情境、錯誤處理]

### US-001: [User Story 名稱] `[來源：prd.md#US-001]`

[為此 User Story 建立多個 Scenario，涵蓋正常流程、邊界情境、錯誤處理]

- **SCN-001 - [場景描述]** `[對應：AC-XXX]`
```gherkin
[使用 gherkin 程式碼區塊撰寫完整場景]
```

- **SCN-002 - [場景描述]** `[對應：AC-XXX]`
```gherkin
[使用 gherkin 程式碼區塊撰寫完整場景]
```

### US-002: [User Story 名稱] `[來源：prd.md#US-002]`

- **SCN-003 - [場景描述]** `[對應：AC-XXX]`
```gherkin
[使用 gherkin 程式碼區塊撰寫完整場景]
```

## Key Entities *(若功能涉及資料則包含)*

- **[實體 1]**: [代表什麼、主要屬性（不含實作）]
- **[實體 2]**: [代表什麼、與其他實體的關係]

## Dependencies
- 外部服務相依性
- 內部團隊相依性
- 先決工作

## Definition of Done (DoD)

- [ ] 所有需求均已實作並通過驗收標準
- [ ] 相關文件已更新且完整
- [ ] 程式碼已通過測試 (單元測試、整合測試、回歸測試)
- [ ] 無重大缺陷或阻礙問題
- [ ] 已完成程式碼審查並獲得批准
- [ ] 部署流程已驗證且可順利執行
- [ ] 使用者手冊或操作指引已提供 (如適用)
- [ ] 非功能性需求已驗證達標

```