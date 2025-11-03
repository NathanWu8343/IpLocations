# Research Document - User Authentication

## 版本歷史

| 日期       | 版本 | 變更摘要 | 作者 |
| ---------- | ---- | -------- | ---- |
| 2025-11-03 | 1.0  | 初始建立 | AI   |

## 研究目的

針對使用者身分驗證功能進行技術研究與架構設計決策，確保設計符合專案標準、安全性需求及效能要求。

## 現有架構分析

### 目前系統狀態

- **架構風格**：單體式 Web API（ASP.NET Core 8.0）
- **分層架構**：Controllers → Services（IpRangeStore）→ Infrastructure（Redis）
- **依賴注入**：使用 ASP.NET Core 內建 DI 容器
- **資料儲存**：Redis（Sorted Set 用於 IP 範圍查詢）
- **身分驗證**：目前無驗證機制，為開放式 API
- **錯誤處理**：Controller 層處理，回傳標準 HTTP 狀態碼

### 可重用資源

#### 直接重用
- **Redis 基礎設施**：已配置 `IConnectionMultiplexer`，可擴展用於會話儲存與登入失敗記錄
- **ASP.NET Core DI**：可註冊新的 Service 與 Repository
- **Controller 基底**：`[ApiController]` 與 `[Route]` 模式可沿用
- **Docker 容器化**：已有 Dockerfile 與容器化經驗，可擴展至資料庫服務

#### 需擴展
- **錯誤處理機制**：需新增全域 Exception Middleware 以統一錯誤訊息格式與語言（繁體中文）
- **組態管理**：需新增資料庫連線字串與 JWT 設定

#### 需新建
- **資料庫層**：使用者資料需持久化儲存（Redis 僅用於會話與暫存資料）
- **身分驗證中介軟體**：驗證 JWT Token 並注入使用者身分
- **業務邏輯層**：UserService、AuthService 等
- **資料存取層**：UserRepository（使用 EF Core）
- **驗證器**：EmailValidator、PasswordValidator

## 技術決策

### 決策 1：使用者資料持久化儲存

**決策**：採用 **SQL Server** + **Entity Framework Core** 作為使用者資料的持久化方案。

**理由**：
1. **.NET 生態系統原生支援**：SQL Server 與 EF Core 整合良好，ORM 成熟穩定
2. **關聯式資料模型**：使用者、會話、登入記錄等實體間存在關聯，關聯式資料庫更適合
3. **事務支援**：註冊流程需要原子性操作（檢查電子郵件唯一性 + 建立使用者）
4. **Docker 部署便利**：專案已有容器化基礎，可使用 `mcr.microsoft.com/mssql/server` 映像
5. **查詢效能**：使用者資料量相對較小，SQL Server 足以應對（可透過索引優化電子郵件查詢）

**影響範圍**：
- 需新增 NuGet 套件：`Microsoft.EntityFrameworkCore.SqlServer`
- 需建立 DbContext、Entity Models、Migrations
- `appsettings.json` 需新增資料庫連線字串

**替代方案評估**：
- **PostgreSQL**：
  - 優點：開源免費、跨平台
  - 缺點：.NET 社群中 SQL Server 更為主流，團隊熟悉度可能較低
  - 結論：若無成本考量，SQL Server 更符合 .NET 專案慣例
  
- **僅使用 Redis**：
  - 優點：高效能、已配置
  - 缺點：無關聯查詢、無事務支援、持久化機制不如關聯式資料庫可靠
  - 結論：不適合作為使用者資料的主要儲存

### 決策 2：會話管理策略

**決策**：採用 **JWT Token（儲存於 HTTP-Only Cookie）+ Redis 會話記錄** 的混合架構。

**理由**：
1. **JWT 無狀態特性**：減少伺服器端狀態管理負擔，提升水平擴展能力
2. **Redis 會話元資料**：
   - 儲存會話 ID、使用者 ID、建立時間、過期時間
   - 支援主動登出功能（刪除 Redis 記錄即可失效 Token）
   - 利用 Redis TTL 自動清理過期會話（7 天）
3. **HTTP-Only Cookie**：
   - 防止 XSS 攻擊（JavaScript 無法讀取）
   - 瀏覽器自動附帶於請求，無需前端手動管理
4. **安全性增強**：
   - JWT Payload 僅包含必要資訊（UserId、Email、過期時間）
   - 敏感資料（如密碼雜湊）不存於 Token
   - Token 簽章防止篡改

**影響範圍**：
- 需新增 NuGet 套件：`System.IdentityModel.Tokens.Jwt`、`Microsoft.AspNetCore.Authentication.JwtBearer`
- 需擴展 Redis 使用：新增會話相關 Key（`auth:session:{sessionId}`）
- 需新增 JWT Middleware 驗證 Token 並注入 `HttpContext.User`
- `appsettings.json` 需新增 JWT 設定（Issuer、Audience、SecretKey、ExpiryInDays）

**替代方案評估**：
- **純 Cookie Session（Server-Side Session）**：
  - 優點：簡單直觀
  - 缺點：每次請求需查詢 Redis，增加延遲；水平擴展需共享 Session 儲存
  - 結論：效能不如 JWT + Redis 混合架構
  
- **純 JWT（無 Redis）**：
  - 優點：完全無狀態，易於擴展
  - 缺點：無法實現主動登出（Token 在有效期內無法撤銷）；無法追蹤活躍會話
  - 結論：不符合需求（FR-021 要求支援登出功能）

**Redis Key 設計**：
```
# 會話記錄（TTL = 7 天 = 604800 秒）
Key: auth:session:{sessionId}
Value: JSON { "userId": "{guid}", "email": "{email}", "createdAt": "{timestamp}", "expiresAt": "{timestamp}" }

# 會話索引（支援按使用者查詢所有會話）
Key: auth:user-sessions:{userId}
Value: Set [ "{sessionId1}", "{sessionId2}", ... ]
```

### 決策 3：密碼加密演算法

**決策**：使用 **BCrypt.Net-Next** 進行密碼雜湊與驗證。

**理由**：
1. **需求明確指定**：FR-007 要求使用 bcrypt 演算法
2. **業界標準**：bcrypt 是密碼儲存的推薦演算法（OWASP、NIST 認可）
3. **抗暴力破解**：
   - 內建 Salt 機制（每次雜湊自動生成隨機 Salt）
   - 可調整 Work Factor（增加計算成本，延緩破解速度）
4. **.NET 生態支援**：`BCrypt.Net-Next` 是成熟且活躍維護的函式庫
5. **時序攻擊防護**：bcrypt 驗證時間恆定，符合 NFR-002 要求

**影響範圍**：
- 需新增 NuGet 套件：`BCrypt.Net-Next`
- 密碼驗證邏輯封裝於 `PasswordHasher` 工具類別（放置於 `Misc/` 目錄）

**替代方案評估**：
- **ASP.NET Core Identity PasswordHasher**：
  - 優點：ASP.NET Core 內建，無需額外套件
  - 缺點：預設使用 PBKDF2（雖然安全，但需求明確要求 bcrypt）
  - 結論：不符合需求指定
  
- **Argon2**：
  - 優點：2015 年密碼雜湊競賽優勝者，效能與安全性優於 bcrypt
  - 缺點：需求已指定 bcrypt，變更需與利害關係人確認
  - 結論：可作為未來優化方向

**使用範例**：
```csharp
// 註冊時
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);

// 登入時
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
```

### 決策 4：架構分層調整

**決策**：在現有架構基礎上新增以下層次，形成完整的分層架構：

**新增層次**：
- **Services/**：業務邏輯層
  - `UserService`：使用者管理邏輯（註冊、查詢個人資料）
  - `AuthService`：身分驗證邏輯（登入、登出、會話驗證）
  - `BruteForceProtectionService`：暴力破解防護邏輯（失敗計數、帳號鎖定）
  
- **Repositories/**：資料存取層（Repository Pattern）
  - `UserRepository`：使用者資料 CRUD（使用 EF Core）
  - 介面 `IUserRepository`（支援測試替換）
  
- **Data/**：資料庫相關
  - `ApplicationDbContext`：EF Core DbContext
  - `Entities/`：資料庫實體（User、Session 等）
  - `Migrations/`：EF Core Migrations
  
- **Middleware/**：中介軟體
  - `JwtAuthenticationMiddleware`：驗證 JWT Token，注入使用者身分
  - `ExceptionHandlingMiddleware`：全域錯誤處理，統一回應格式
  
- **DTOs/**：資料傳輸物件
  - `Auth/`：註冊、登入請求/回應 DTO
  - `User/`：使用者資料 DTO
  
- **Validators/**：輸入驗證
  - `EmailValidator`：電子郵件格式驗證
  - `PasswordValidator`：密碼強度驗證
  
- **Extensions/**：擴展方法
  - `ServiceCollectionExtensions`：DI 註冊擴展
  - `ClaimsPrincipalExtensions`：從 JWT Claims 提取使用者資訊

**理由**：
1. **符合 Clean Architecture 原則**：依賴方向由外向內（Controllers → Services → Repositories → Data）
2. **關注點分離**：
   - Controllers：僅負責 HTTP 層面（路由、參數綁定、回應格式）
   - Services：封裝業務規則與流程
   - Repositories：封裝資料存取細節
3. **可測試性**：
   - Service 可注入 Mock Repository 進行單元測試
   - Repository 可使用 In-Memory Database 進行整合測試
4. **可維護性**：各層職責明確，修改時影響範圍可控
5. **符合專案慣例**：現有專案已有 `Infra/`、`Misc/`、`Constants/` 目錄，新增目錄延續相同命名風格

**影響範圍**：
- 專案結構需新增以上目錄
- `Program.cs` 需註冊新增的 Services、Repositories、DbContext
- 需定義介面以支援 DI（如 `IUserService`、`IUserRepository`）

**與現有架構整合**：
- **IpLocationsController**：維持不變，未來可新增 `[Authorize]` 屬性以限制存取
- **IpRangeStore**：維持不變，繼續作為 IP 查詢服務
- **Redis 使用**：擴展為兩個用途（IP 範圍 + 身分驗證資料），使用不同 Key 前綴區分

### 決策 5：API 端點設計

**決策**：新增 **AuthController**，遵循 RESTful 設計原則，端點如下：

| HTTP Method | Endpoint                | 功能       | 對應需求            |
| ----------- | ----------------------- | ---------- | ------------------- |
| POST        | /api/auth/register      | 使用者註冊 | FR-001 ~ FR-011     |
| POST        | /api/auth/login         | 使用者登入 | FR-012 ~ FR-016     |
| POST        | /api/auth/logout        | 使用者登出 | FR-021              |
| GET         | /api/auth/profile       | 查看個人資料 | FR-031 ~ FR-033   |

**理由**：
1. **符合 RESTful 慣例**：使用標準 HTTP 方法表達操作語義
2. **資源導向設計**：`/api/auth` 作為身分驗證資源的命名空間
3. **易於擴展**：未來可新增 `/api/auth/refresh-token`、`/api/auth/change-password` 等端點
4. **與現有 API 一致**：延續 `/api/{controller}/{action}` 的路由模式（如 `/api/IpLocations`）

**影響範圍**：
- 需新增 `Controllers/AuthController.cs`
- Swagger 文檔需包含新端點
- 需定義 Request/Response DTO

**端點詳細設計**：
參見 `contracts/auth-api.yaml`（OpenAPI 規格）

### 決策 6：錯誤處理機制

**決策**：實作 **全域 ExceptionHandlingMiddleware**，統一處理錯誤回應格式與語言。

**理由**：
1. **集中管理錯誤訊息**：所有錯誤訊息在 Middleware 層轉換為繁體中文（符合 FR-030）
2. **統一回應格式**：
   ```json
   {
     "error": {
       "code": "VALIDATION_ERROR",
       "message": "密碼必須至少 8 字元，並包含大寫字母、小寫字母、數字與特殊符號"
     }
   }
   ```
3. **避免程式碼重複**：無需在每個 Controller Action 重複 try-catch 邏輯
4. **安全性考量**：
   - 開發環境回傳詳細錯誤（含 Stack Trace）
   - 生產環境回傳通用錯誤訊息（防止敏感資訊洩漏）
5. **符合 ASP.NET Core 中介軟體模式**：在請求管線早期攔截例外

**影響範圍**：
- 需新增 `Middleware/ExceptionHandlingMiddleware.cs`
- `Program.cs` 需註冊 Middleware（置於管線最前方）
- 需定義自訂例外類別（如 `ValidationException`、`AuthenticationException`）

**錯誤碼設計**：
| 錯誤碼               | HTTP 狀態碼 | 說明                 | 對應需求    |
| -------------------- | ----------- | -------------------- | ----------- |
| VALIDATION_ERROR     | 400         | 輸入驗證失敗         | FR-027, FR-028 |
| EMAIL_ALREADY_EXISTS | 409         | 電子郵件已被註冊     | FR-029      |
| INVALID_CREDENTIALS  | 401         | 帳號或密碼錯誤       | FR-015      |
| ACCOUNT_LOCKED       | 403         | 帳號已被鎖定         | FR-024      |
| SESSION_EXPIRED      | 401         | 會話已過期           | FR-020      |
| UNAUTHORIZED         | 401         | 未授權存取           | FR-033      |

### 決策 7：暴力破解防護實作

**決策**：使用 **Redis** 儲存登入失敗記錄與帳號鎖定狀態，利用 TTL 機制自動解除鎖定。

**理由**：
1. **高效能讀寫**：Redis 記憶體儲存，適合高頻率的失敗計數更新與查詢
2. **自動過期機制**：利用 Redis TTL 實現「30 分鐘後自動解鎖」（FR-025），無需排程任務
3. **原子操作**：Redis `INCR` 指令保證計數器原子性，避免競態條件
4. **複用現有基礎設施**：專案已配置 Redis，無需額外引入新服務
5. **簡化邏輯**：無需在資料庫中新增鎖定狀態欄位與清理邏輯

**影響範圍**：
- 擴展 Redis 使用場景（原本僅用於 IP 範圍查詢）
- 需在 `BruteForceProtectionService` 中封裝 Redis 操作邏輯
- Redis Key 需使用明確前綴以區分用途

**Redis Key 設計**：
```
# 登入失敗計數（每次失敗 INCR，成功登入 DEL）
Key: auth:login-attempts:{email}
Value: Integer (失敗次數)
TTL: 無（由應用程式邏輯控制）

# 帳號鎖定狀態（失敗 3 次後建立，TTL = 30 分鐘 = 1800 秒）
Key: auth:account-lockout:{email}
Value: JSON { "lockedAt": "{timestamp}", "unlockAt": "{timestamp}" }
TTL: 1800 秒
```

**實作邏輯**：
1. **登入失敗**：
   - 檢查 `auth:account-lockout:{email}` 是否存在（EXISTS）
   - 若存在 → 回傳「帳號已鎖定」（FR-024）
   - 若不存在 → INCR `auth:login-attempts:{email}`
   - 若失敗次數達 3 → 設定 `auth:account-lockout:{email}`（TTL = 1800 秒）

2. **登入成功**：
   - DEL `auth:login-attempts:{email}`（FR-026）

3. **檢查鎖定狀態**：
   - 使用 TTL 指令檢查 `auth:account-lockout:{email}` 剩餘時間
   - 若 TTL > 0 → 帳號仍鎖定
   - 若 TTL = -2（Key 不存在）→ 帳號未鎖定或已自動解鎖

**替代方案評估**：
- **資料庫儲存失敗記錄**：
  - 優點：持久化、可稽核
  - 缺點：效能較差（每次登入需查詢資料庫）；需排程任務清理過期記錄
  - 結論：效能不如 Redis，且規格未要求持久化失敗記錄
  
- **In-Memory Cache（IMemoryCache）**：
  - 優點：無需外部服務
  - 缺點：無法跨執行個體共享（水平擴展時會失效）
  - 結論：不適合分散式部署

### 決策 8：會話識別碼生成策略

**決策**：使用 **`Guid.NewGuid()`** 生成會話識別碼（Session ID）。

**理由**：
1. **符合安全性需求**：
   - Guid（128 位元）滿足 NFR-003 要求（至少 128 位元）
   - .NET 的 `Guid.NewGuid()` 使用密碼學安全隨機數生成器（CSPRNG）
2. **唯一性保證**：Guid 碰撞機率極低（約 1/2^122），適合作為全域唯一識別碼
3. **實作簡便**：.NET 內建支援，無需額外套件
4. **儲存效率**：Guid 可轉換為字串（36 字元）或二進位（16 位元組），適合 Redis 儲存

**影響範圍**：
- 會話建立邏輯中使用 `Guid.NewGuid().ToString()` 生成 Session ID
- JWT Payload 中包含 `SessionId` Claim

**替代方案評估**：
- **自訂隨機字串（Base64 編碼）**：
  - 優點：可自訂長度與字元集
  - 缺點：需手動實作 CSPRNG，增加出錯風險
  - 結論：Guid 已滿足需求且更可靠
  
- **遞增整數**：
  - 優點：簡單
  - 缺點：不符合 NFR-003（無隨機性），易被預測，安全性不足
  - 結論：不適用

## 套件需求清單

以下為實作身分驗證功能需引入的 NuGet 套件：

| 套件名稱                                      | 版本   | 用途                        |
| --------------------------------------------- | ------ | --------------------------- |
| `BCrypt.Net-Next`                             | 4.0.3+ | 密碼雜湊與驗證              |
| `Microsoft.EntityFrameworkCore.SqlServer`     | 8.0.0+ | SQL Server 資料庫存取       |
| `Microsoft.EntityFrameworkCore.Design`        | 8.0.0+ | EF Core 設計時工具（Migrations） |
| `Microsoft.EntityFrameworkCore.Tools`         | 8.0.0+ | EF Core CLI 工具            |
| `System.IdentityModel.Tokens.Jwt`             | 7.0.0+ | JWT Token 生成與驗證        |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0+ | ASP.NET Core JWT 驗證中介軟體 |

## 安全性考量

### 密碼儲存安全
- ✅ 使用 bcrypt 雜湊（Work Factor = 12）
- ✅ 每次雜湊自動生成隨機 Salt
- ✅ 絕不明文儲存密碼
- ✅ 密碼驗證時間恆定（bcrypt 內建防時序攻擊）

### 會話安全
- ✅ JWT Token 儲存於 HTTP-Only Cookie（防止 XSS）
- ✅ Cookie 設定 `Secure` 標記（僅 HTTPS 傳輸）
- ✅ Cookie 設定 `SameSite=Strict`（防止 CSRF）
- ✅ Session ID 使用 CSPRNG 生成（Guid.NewGuid）
- ✅ Token 有效期限 7 天，過期後需重新登入

### 暴力破解防護
- ✅ 連續 3 次失敗鎖定帳號 30 分鐘
- ✅ 登入錯誤訊息不區分「帳號不存在」與「密碼錯誤」（防止帳號枚舉）
- ✅ 失敗記錄儲存於 Redis（高效能查詢）

### 錯誤訊息安全
- ✅ 生產環境不回傳 Stack Trace
- ✅ 不洩漏內部實作細節（如資料庫結構）
- ✅ 統一錯誤格式與語言（繁體中文）

### 稽核日誌（NFR-004）
- ✅ 記錄所有登入嘗試（成功/失敗）
- ✅ 記錄帳號鎖定/解鎖事件
- ✅ 記錄會話建立/銷毀事件
- ⚠️ 日誌儲存機制需在實作階段確認（可使用 Serilog + File/Database Sink）

## 效能考量

### 目標（NFR-001）
- 註冊與登入請求處理時間 < 200ms（95% 情況）

### 優化策略
1. **Redis 快取會話資料**：避免每次請求查詢資料庫
2. **資料庫索引**：
   - `Users` 表：在 `Email` 欄位建立唯一索引（加速查詢與防重複）
   - `Users` 表：在 `Id` 欄位建立主鍵索引
3. **bcrypt Work Factor**：設定為 12（平衡安全性與效能）
4. **非同步 I/O**：所有資料庫與 Redis 操作使用 `async/await`
5. **連線池**：
   - SQL Server：EF Core 預設啟用連線池
   - Redis：`IConnectionMultiplexer` 單例模式，重用連線

### 效能監控
- 使用 ASP.NET Core 內建日誌記錄請求處理時間
- 可引入 Application Insights 或 Prometheus 進行進階監控

## 相容性考量

### 瀏覽器支援（NFR-006）
- ✅ Chrome、Firefox、Edge、Safari（最新兩個主要版本）
- ✅ 使用標準 HTTP Cookie，無需特殊前端處理

### API 向後相容性
- ✅ 新增的 `/api/auth` 端點不影響現有 `/api/IpLocations`
- ⚠️ 若未來對 `/api/IpLocations` 加上 `[Authorize]` 屬性，需提供遷移指南

## 未來擴展方向

1. **電子郵件驗證**：註冊後發送驗證信，啟用帳號後才能登入
2. **雙因素驗證（2FA）**：使用 TOTP（如 Google Authenticator）
3. **密碼重設功能**：透過電子郵件發送重設連結
4. **社交登入**：整合 OAuth 2.0（Google、GitHub 等）
5. **API Key 管理**：為進階使用者提供 API Key 存取方式
6. **使用配額管理**：基於使用者身分限制 API 呼叫次數
7. **Refresh Token**：延長會話有效期而無需重新登入

## 風險與挑戰

### 高風險項目
1. **密碼雜湊效能**：bcrypt 計算密集，可能影響高併發場景
   - **緩解措施**：Work Factor 設定為 12（業界標準）；若效能不足可考慮非同步佇列處理
   
2. **資料庫遷移**：首次導入 EF Core Migrations，需謹慎處理
   - **緩解措施**：在開發環境充分測試；使用 `dotnet ef migrations script` 預覽 SQL

3. **Redis 單點故障**：若 Redis 不可用，會話驗證失效
   - **緩解措施**：未來可引入 Redis Cluster 或 Sentinel；短期可依賴 Redis 持久化（RDB/AOF）

### 中風險項目
1. **JWT Secret 管理**：Secret Key 洩漏將導致 Token 可偽造
   - **緩解措施**：使用 User Secrets（開發）與 Azure Key Vault（生產）

2. **時區處理**：會話過期時間需統一使用 UTC
   - **緩解措施**：所有時間戳記使用 `DateTime.UtcNow`

## 研究結論

本研究完成以下目標：
- ✅ 分析現有架構並確認可重用資源
- ✅ 評估並選擇技術方案（SQL Server、JWT + Redis、bcrypt）
- ✅ 設計架構分層調整（Services、Repositories、Middleware 等）
- ✅ 定義 API 端點規格
- ✅ 規劃錯誤處理與安全性機制
- ✅ 評估效能與相容性考量

**建議後續步驟**：
1. 審閱本研究文件並確認技術決策
2. 閱讀 `design.md` 瞭解詳細設計
3. 閱讀 `contracts/auth-api.yaml` 瞭解 API 規格
4. 進入開發階段，按 `tasks.md` 執行實作任務
