# Implementation Plan - User Authentication

## Phase 1: Setup (共享基礎設施)

**目的**: 專案初始化和基本結構建立

- [ ] T001 根據 `doc/changes/user-authentication/design.md` 的 File Structure 建立目錄結構（Controllers/, Services/, Repositories/, Data/, Middleware/, DTOs/, Validators/, Extensions/, Exceptions/）
- [ ] T002 在 `src/IpLocations.Api/IpLocations.Api.csproj` 中新增 NuGet 套件參考：
  - Microsoft.EntityFrameworkCore.SqlServer（EF Core 資料庫存取）
  - Microsoft.EntityFrameworkCore.Tools（資料庫遷移工具）
  - BCrypt.Net-Next（密碼雜湊）
  - System.IdentityModel.Tokens.Jwt（JWT Token 生成與驗證）
  - Microsoft.AspNetCore.Authentication.JwtBearer（JWT 身分驗證中介軟體）
- [ ] T003 在 `src/IpLocations.Api/appsettings.json` 中新增 JWT 組態設定（SecretKey、Issuer、Audience、ExpiryDays）與資料庫連線字串（SQL Server）
- [ ] T004 在 `src/IpLocations.Api/appsettings.Development.json` 中新增本地開發環境的資料庫連線字串與 JWT 設定

---

## Phase 2: Foundational (阻塞性先決條件)

**目的**: 建立所有 User Story 共用的基礎設施，包括資料模型、工具類別、驗證器、異常處理、中介軟體等

**⚠️ 關鍵**: 在此階段完成之前，任何 User Story 的工作都不能開始

### 資料層基礎建設

- [ ] T005 在 `src/IpLocations.Api/Data/Entities/User.cs` 中建立 User 實體（包含 Id, Email, PasswordHash, CreatedAt, UpdatedAt 屬性）
- [ ] T006 在 `src/IpLocations.Api/Data/ApplicationDbContext.cs` 中建立 EF Core DbContext，配置 Users DbSet 與資料庫對應關係（索引、約束、預設值）
- [ ] T007 使用 EF Core Migrations 產生初始資料庫遷移檔案於 `src/IpLocations.Api/Data/Migrations/`（建立 Users 資料表與 IX_Users_Email 唯一索引）
- [ ] T008 在 `src/IpLocations.Api/Repositories/IUserRepository.cs` 中定義 IUserRepository 介面（GetByIdAsync、GetByEmailAsync、EmailExistsAsync、CreateAsync 方法）
- [ ] T009 在 `src/IpLocations.Api/Repositories/UserRepository.cs` 中實作 UserRepository，使用 ApplicationDbContext 操作 Users 資料表

### 工具類別與驗證器

- [ ] T010 在 `src/IpLocations.Api/Misc/PasswordHasher.cs` 中實作密碼雜湊工具類別，使用 BCrypt.Net-Next 提供 HashPassword 與 VerifyPassword 靜態方法（Work Factor = 12）
- [ ] T011 在 `src/IpLocations.Api/Misc/JwtTokenGenerator.cs` 中實作 JWT Token 生成與驗證工具類別，提供 GenerateToken 與 ValidateToken 靜態方法
- [ ] T012 在 `src/IpLocations.Api/Validators/EmailValidator.cs` 中實作電子郵件格式驗證器（使用 Regex 驗證包含 @ 符號與網域名稱）
- [ ] T013 在 `src/IpLocations.Api/Validators/PasswordValidator.cs` 中實作密碼強度驗證器（至少 8 字元、包含大小寫字母、數字、特殊符號）

### 常數與異常定義

- [ ] T014 在 `src/IpLocations.Api/Constants/AuthRedisKeyConst.cs` 中定義身分驗證相關的 Redis Key 常數（Session、LoginAttempts、AccountLockout 前綴）
- [ ] T015 在 `src/IpLocations.Api/Constants/ErrorCodeConst.cs` 中定義錯誤碼常數（VALIDATION_ERROR、EMAIL_ALREADY_EXISTS、INVALID_CREDENTIALS、ACCOUNT_LOCKED、SESSION_EXPIRED、UNAUTHORIZED、INTERNAL_ERROR）
- [ ] T016 在 `src/IpLocations.Api/Exceptions/ValidationException.cs` 中建立自訂驗證異常類別
- [ ] T017 在 `src/IpLocations.Api/Exceptions/AuthenticationException.cs` 中建立自訂身分驗證異常類別
- [ ] T018 在 `src/IpLocations.Api/Exceptions/AccountLockedException.cs` 中建立自訂帳號鎖定異常類別

### DTOs 資料傳輸物件

- [ ] T019 在 `src/IpLocations.Api/DTOs/Common/ErrorResponse.cs` 中建立錯誤回應 DTO（包含 Code 與 Message 屬性）
- [ ] T020 在 `src/IpLocations.Api/DTOs/Auth/RegisterRequest.cs` 中建立註冊請求 DTO（Email、Password）
- [ ] T021 在 `src/IpLocations.Api/DTOs/Auth/RegisterResponse.cs` 中建立註冊回應 DTO（UserId、Email、CreatedAt、Message）
- [ ] T022 在 `src/IpLocations.Api/DTOs/Auth/LoginRequest.cs` 中建立登入請求 DTO（Email、Password）
- [ ] T023 在 `src/IpLocations.Api/DTOs/Auth/LoginResponse.cs` 中建立登入回應 DTO（Token、ExpiresAt）
- [ ] T024 在 `src/IpLocations.Api/DTOs/Auth/UserProfileResponse.cs` 中建立個人資料回應 DTO（Email、CreatedAt）

### 中介軟體與擴展方法

- [ ] T025 在 `src/IpLocations.Api/Middleware/ExceptionHandlingMiddleware.cs` 中實作全域例外處理中介軟體，統一攔截自訂異常並回傳繁體中文錯誤訊息（根據 ErrorCodeConst 對照表）
- [ ] T026 在 `src/IpLocations.Api/Middleware/JwtAuthenticationMiddleware.cs` 中實作 JWT 驗證中介軟體，從 Cookie 提取 Token、驗證簽章與有效期、檢查 Redis 會話、注入 HttpContext.User
- [ ] T027 在 `src/IpLocations.Api/Extensions/ClaimsPrincipalExtensions.cs` 中實作 ClaimsPrincipal 擴展方法（GetUserId、GetEmail），用於從 JWT Claims 提取使用者資訊
- [ ] T028 在 `src/IpLocations.Api/Extensions/ServiceCollectionExtensions.cs` 中實作 DI 註冊擴展方法（AddAuthServices、AddRepositories、AddDatabase），統一管理服務註冊

### 基礎服務註冊與中介軟體配置

- [ ] T029 在 `src/IpLocations.Api/Program.cs` 中註冊 ApplicationDbContext（使用 SQL Server 連線字串）、IUserRepository、JwtTokenGenerator 組態
- [ ] T030 在 `src/IpLocations.Api/Program.cs` 中配置中介軟體管線，新增 ExceptionHandlingMiddleware（置於最前方）與 JwtAuthenticationMiddleware（置於 UseAuthorization 之前）

**✅ 檢查點**: 基礎已就緒 - 資料庫結構建立、工具類別可用、驗證器就緒、中介軟體配置完成，User Story 實作現在可以並行開始

---

## Phase 3: US-001 - 使用者註冊帳號 (優先順序: P1) 🎯 MVP

**目標**: 建立完整的使用者註冊功能，包含電子郵件與密碼驗證、重複檢查、密碼加密儲存

**獨立測試**: 透過 POST /api/auth/register 端點提交有效的註冊請求，應回傳 201 狀態碼與使用者資訊，資料庫中應建立對應的 User 記錄且密碼為加密格式

### US-001 的測試

**注意：請先撰寫這些測試，確保它們在實作前會失敗**

- [ ] T031 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫註冊成功的整合測試（SCN-015：有效電子郵件與密碼，應回傳 201 與使用者資訊）
- [ ] T032 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫欄位驗證的整合測試（SCN-001 ~ SCN-003：空白電子郵件/密碼，應回傳 400 與錯誤訊息）
- [ ] T033 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫電子郵件格式驗證測試（SCN-004 ~ SCN-006：無效格式應回傳 400）
- [ ] T034 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫密碼長度驗證測試（SCN-007 ~ SCN-008：少於 8 字元應回傳 400）
- [ ] T035 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫密碼強度驗證測試（SCN-009 ~ SCN-013：缺少大小寫/數字/特殊符號應回傳 400）
- [ ] T036 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫電子郵件重複檢查測試（SCN-014：重複電子郵件應回傳 409 與錯誤訊息）
- [ ] T037 [US-001] 在 `tests/IpLocations.IntegrationTests/Auth/RegisterTests.cs` 中撰寫密碼加密儲存驗證測試（SCN-016：檢查資料庫中密碼欄位為加密格式且長度 > 原始密碼）
- [ ] T038 [US-001] 在 `tests/IpLocations.UnitTests/Validators/EmailValidatorTests.cs` 中撰寫電子郵件驗證器單元測試（有效/無效格式）
- [ ] T039 [US-001] 在 `tests/IpLocations.UnitTests/Validators/PasswordValidatorTests.cs` 中撰寫密碼驗證器單元測試（長度與強度規則）
- [ ] T040 [US-001] 在 `tests/IpLocations.UnitTests/Misc/PasswordHasherTests.cs` 中撰寫密碼雜湊工具單元測試（雜湊不可逆、相同密碼每次雜湊結果不同、驗證正確/錯誤密碼）

### US-001 的實作

- [ ] T041 [US-001] 在 `src/IpLocations.Api/Services/IUserService.cs` 中定義 IUserService 介面（RegisterAsync、GetProfileAsync 方法）
- [ ] T042 [US-001] 在 `src/IpLocations.Api/Services/UserService.cs` 中實作 UserService.RegisterAsync 方法：
  - 使用 EmailValidator 與 PasswordValidator 驗證輸入
  - 呼叫 UserRepository.EmailExistsAsync 檢查電子郵件是否已註冊
  - 使用 PasswordHasher.HashPassword 加密密碼
  - 呼叫 UserRepository.CreateAsync 建立使用者記錄
  - 回傳使用者 ID
- [ ] T043 [US-001] 在 `src/IpLocations.Api/Controllers/AuthController.cs` 中建立 AuthController 並實作 POST /api/auth/register 端點：
  - 接收 RegisterRequest
  - 呼叫 IUserService.RegisterAsync
  - 回傳 RegisterResponse（201 Created）
  - 處理 ValidationException（400）與 EmailAlreadyExists（409）
- [ ] T044 [US-001] 在 `src/IpLocations.Api/Program.cs` 中註冊 IUserService 與 UserService 至 DI 容器（使用 Scoped 生命週期）
- [ ] T045 [US-001] 執行所有 US-001 相關測試，確認測試通過且程式碼覆蓋率達標

**✅ 檢查點**: 此時，US-001 應完全正常運作並可獨立測試 - 使用者可成功註冊帳號，電子郵件與密碼驗證運作正常，密碼以加密格式儲存

---

## Phase 4: US-002 - 使用者安全登入 (優先順序: P1) 🎯 MVP

**目標**: 實作使用者登入功能，包含憑證驗證、會話建立、Token 生成

**獨立測試**: 透過 POST /api/auth/login 端點提交正確的帳號密碼，應回傳 200 狀態碼與 JWT Token（儲存於 HTTP-Only Cookie），使用該 Token 應可通過身分驗證

### US-002 的測試

- [ ] T046 [US-002] 在 `tests/IpLocations.IntegrationTests/Auth/LoginTests.cs` 中撰寫登入成功測試（SCN-017：正確憑證應回傳 200 與 Token，Cookie 中包含 JWT）
- [ ] T047 [US-002] 在 `tests/IpLocations.IntegrationTests/Auth/LoginTests.cs` 中撰寫帳號不存在測試（SCN-018：不存在的帳號應回傳 401 與「帳號或密碼錯誤」訊息）
- [ ] T048 [US-002] 在 `tests/IpLocations.IntegrationTests/Auth/LoginTests.cs` 中撰寫密碼錯誤測試（SCN-019：錯誤密碼應回傳 401 與「帳號或密碼錯誤」訊息）
- [ ] T049 [US-002] 在 `tests/IpLocations.IntegrationTests/Auth/LoginTests.cs` 中撰寫登入失敗記錄測試（SCN-020：登入失敗應記錄失敗次數與時間戳記於 Redis）
- [ ] T050 [US-002] 在 `tests/IpLocations.IntegrationTests/Auth/LoginTests.cs` 中撰寫錯誤訊息一致性測試（SCN-021：帳號不存在與密碼錯誤的錯誤訊息應完全一致）
- [ ] T051 [US-002] 在 `tests/IpLocations.UnitTests/Services/AuthServiceTests.cs` 中撰寫 AuthService.LoginAsync 單元測試（Mock IUserRepository 與 IDatabase，驗證登入流程）

### US-002 的實作

- [ ] T052 [US-002] 在 `src/IpLocations.Api/Services/IAuthService.cs` 中定義 IAuthService 介面（LoginAsync、LogoutAsync、ValidateSessionAsync 方法）
- [ ] T053 [US-002] 在 `src/IpLocations.Api/Services/AuthService.cs` 中實作 AuthService.LoginAsync 方法：
  - 呼叫 IUserRepository.GetByEmailAsync 查詢使用者
  - 使用 PasswordHasher.VerifyPassword 驗證密碼
  - 若密碼錯誤，拋出 AuthenticationException（訊息統一為「帳號或密碼錯誤」）
  - 若密碼正確，使用 JwtTokenGenerator.GenerateToken 生成 JWT Token
  - 建立會話資料（JSON 格式）並儲存至 Redis（Key: `auth:session:{sessionId}`, TTL: 604800 秒）
  - 回傳 LoginResponse（Token、ExpiresAt）
- [ ] T054 [US-002] 在 `src/IpLocations.Api/Controllers/AuthController.cs` 中實作 POST /api/auth/login 端點：
  - 接收 LoginRequest
  - 呼叫 IAuthService.LoginAsync
  - 將 JWT Token 設定至 HTTP-Only Cookie（Secure、SameSite=Strict、Max-Age=604800）
  - 回傳 LoginResponse（200 OK）
  - 處理 AuthenticationException（401）
- [ ] T055 [US-002] 在 `src/IpLocations.Api/Program.cs` 中註冊 IAuthService 與 AuthService 至 DI 容器
- [ ] T056 [US-002] 執行所有 US-002 相關測試，確認測試通過

**✅ 檢查點**: 此時，US-001 和 US-002 都應能獨立運作 - 使用者可註冊帳號並成功登入，登入後獲得有效的 JWT Token

---

## Phase 5: US-003 - 會話管理與自動登出 (優先順序: P1) 🎯 MVP

**目標**: 實作會話驗證、過期檢查與登出功能，確保使用者在會話有效期間內可存取 API，過期後需重新登入

**獨立測試**: 登入後使用 Token 存取需驗證的端點（如 /api/auth/profile），在會話有效期（7 天內）應可正常存取，超過 7 天或登出後應回傳 401 錯誤

### US-003 的測試

- [ ] T057 [US-003] 在 `tests/IpLocations.IntegrationTests/Auth/SessionTests.cs` 中撰寫會話建立測試（SCN-022：登入成功後 Redis 應存在會話資料，包含建立時間、使用者 ID、過期時間）
- [ ] T058 [US-003] 在 `tests/IpLocations.IntegrationTests/Auth/SessionTests.cs` 中撰寫有效會話存取測試（SCN-023 ~ SCN-024：會話有效期間內存取 API 應允許且無需重新登入）
- [ ] T059 [US-003] 在 `tests/IpLocations.IntegrationTests/Auth/SessionTests.cs` 中撰寫會話過期測試（SCN-025 ~ SCN-026：會話超過 7 天應回傳 401 與「會話已過期，請重新登入」訊息）
- [ ] T060 [US-003] 在 `tests/IpLocations.IntegrationTests/Auth/SessionTests.cs` 中撰寫登出測試（SCN-027 ~ SCN-028：登出後 Redis 會話應被刪除，使用原 Token 存取應回傳 401）
- [ ] T061 [US-003] 在 `tests/IpLocations.UnitTests/Middleware/JwtAuthenticationMiddlewareTests.cs` 中撰寫 JWT 中介軟體單元測試（Token 驗證、Claims 提取、會話檢查）

### US-003 的實作

- [ ] T062 [US-003] 在 `src/IpLocations.Api/Services/AuthService.cs` 中實作 AuthService.ValidateSessionAsync 方法：檢查 Redis 中會話是否存在且未過期
- [ ] T063 [US-003] 在 `src/IpLocations.Api/Services/AuthService.cs` 中實作 AuthService.LogoutAsync 方法：從 Redis 刪除會話記錄（DEL auth:session:{sessionId}）
- [ ] T064 [US-003] 在 `src/IpLocations.Api/Middleware/JwtAuthenticationMiddleware.cs` 中完善 JWT 驗證邏輯：
  - 從 Cookie 提取 JWT Token
  - 使用 JwtTokenGenerator.ValidateToken 驗證簽章與有效期
  - 從 Token Payload 提取 SessionId
  - 呼叫 IAuthService.ValidateSessionAsync 檢查會話是否存在於 Redis
  - 若驗證成功，從 Claims 提取 UserId 與 Email 並設定 HttpContext.User
  - 若驗證失敗，設定 HTTP 401 狀態碼
- [ ] T065 [US-003] 在 `src/IpLocations.Api/Controllers/AuthController.cs` 中實作 POST /api/auth/logout 端點：
  - 從 HttpContext.User 取得 SessionId（使用 ClaimsPrincipalExtensions）
  - 呼叫 IAuthService.LogoutAsync
  - 清除 Cookie（Set-Cookie: jwt_token=; Max-Age=0）
  - 回傳 200 OK
- [ ] T066 [US-003] 執行所有 US-003 相關測試，確認測試通過

**✅ 檢查點**: 此時，US-001、US-002、US-003 都應能獨立運作 - 使用者可註冊、登入、使用 Token 存取 API、主動登出，會話管理機制完整運作

---

## Phase 6: US-004 - 防護暴力破解攻擊 (優先順序: P1) 🎯 MVP

**目標**: 實作登入失敗計數與帳號鎖定機制，連續失敗 3 次自動鎖定 30 分鐘，防止暴力破解攻擊

**獨立測試**: 連續 3 次輸入錯誤密碼登入，第 3 次應回傳「帳號已被鎖定，請於 30 分鐘後再試」錯誤，鎖定期間內使用正確密碼也無法登入，30 分鐘後可正常登入

### US-004 的測試

- [ ] T067 [US-004] 在 `tests/IpLocations.IntegrationTests/Auth/BruteForceProtectionTests.cs` 中撰寫連續失敗 3 次鎖定測試（SCN-029：連續 3 次錯誤密碼應鎖定帳號並記錄鎖定時間於 Redis）
- [ ] T068 [US-004] 在 `tests/IpLocations.IntegrationTests/Auth/BruteForceProtectionTests.cs` 中撰寫失敗 2 次未鎖定測試（SCN-030：僅失敗 2 次應記錄失敗次數但不鎖定）
- [ ] T069 [US-004] 在 `tests/IpLocations.IntegrationTests/Auth/BruteForceProtectionTests.cs` 中撰寫鎖定期間登入測試（SCN-031 ~ SCN-032：鎖定期間內使用正確密碼登入應回傳 403 與鎖定訊息）
- [ ] T070 [US-004] 在 `tests/IpLocations.IntegrationTests/Auth/BruteForceProtectionTests.cs` 中撰寫自動解鎖測試（SCN-033 ~ SCN-034：鎖定 30 分鐘後應自動解鎖並允許登入）
- [ ] T071 [US-004] 在 `tests/IpLocations.IntegrationTests/Auth/BruteForceProtectionTests.cs` 中撰寫登入成功重置失敗次數測試（SCN-035 ~ SCN-036：成功登入應重置失敗計數為 0 並清除 Redis 記錄）
- [ ] T072 [US-004] 在 `tests/IpLocations.UnitTests/Services/BruteForceProtectionServiceTests.cs` 中撰寫暴力破解防護服務單元測試（Mock IDatabase，測試失敗計數累積、鎖定狀態檢查、重置邏輯）

### US-004 的實作

- [ ] T073 [US-004] 在 `src/IpLocations.Api/Services/IBruteForceProtectionService.cs` 中定義 IBruteForceProtectionService 介面（IsAccountLockedAsync、RecordFailedAttemptAsync、ResetFailedAttemptsAsync 方法）
- [ ] T074 [US-004] 在 `src/IpLocations.Api/Services/BruteForceProtectionService.cs` 中實作 BruteForceProtectionService：
  - IsAccountLockedAsync：檢查 Redis 中是否存在 `auth:account-lockout:{email}` Key
  - RecordFailedAttemptAsync：使用 INCR 遞增 `auth:login-attempts:{email}`，若達 3 次則使用 SETEX 建立鎖定記錄（TTL 1800 秒）
  - ResetFailedAttemptsAsync：使用 DEL 刪除 `auth:login-attempts:{email}` 與 `auth:account-lockout:{email}`
- [ ] T075 [US-004] 在 `src/IpLocations.Api/Services/AuthService.cs` 中整合暴力破解防護邏輯至 LoginAsync 方法：
  - 登入前呼叫 IBruteForceProtectionService.IsAccountLockedAsync 檢查鎖定狀態
  - 若鎖定，拋出 AccountLockedException
  - 密碼驗證失敗時呼叫 RecordFailedAttemptAsync 記錄失敗
  - 密碼驗證成功時呼叫 ResetFailedAttemptsAsync 重置失敗計數
- [ ] T076 [US-004] 在 `src/IpLocations.Api/Middleware/ExceptionHandlingMiddleware.cs` 中新增 AccountLockedException 的錯誤處理（回傳 403 與「帳號已被鎖定，請於 30 分鐘後再試」訊息）
- [ ] T077 [US-004] 在 `src/IpLocations.Api/Program.cs` 中註冊 IBruteForceProtectionService 與 BruteForceProtectionService 至 DI 容器
- [ ] T078 [US-004] 執行所有 US-004 相關測試，確認測試通過

**✅ 檢查點**: 此時，所有 P1 User Story（US-001 ~ US-004）都應能獨立運作 - 暴力破解防護機制已啟用，連續登入失敗會觸發帳號鎖定，30 分鐘後自動解鎖

---

## Phase 7: US-005 - 明確的錯誤訊息提示 (優先順序: P2)

**目標**: 確保所有錯誤訊息以繁體中文顯示，語氣友善且具建設性，為使用者提供清晰的操作指引

**獨立測試**: 觸發各種錯誤情境（無效電子郵件格式、密碼強度不足、電子郵件重複等），檢查回應中的錯誤訊息是否為繁體中文且語氣友善

### US-005 的測試

- [ ] T079 [US-005] 在 `tests/IpLocations.IntegrationTests/Auth/ErrorMessageTests.cs` 中撰寫電子郵件格式錯誤訊息測試（SCN-037：無效格式應顯示「請輸入有效的電子郵件格式」且為繁體中文）
- [ ] T080 [US-005] 在 `tests/IpLocations.IntegrationTests/Auth/ErrorMessageTests.cs` 中撰寫密碼強度錯誤訊息測試（SCN-038：弱密碼應顯示「密碼必須至少 8 字元，並包含大寫字母、小寫字母、數字與特殊符號」且為繁體中文）
- [ ] T081 [US-005] 在 `tests/IpLocations.IntegrationTests/Auth/ErrorMessageTests.cs` 中撰寫電子郵件重複錯誤訊息測試（SCN-039：重複郵件應顯示「此電子郵件已被註冊，請使用其他電子郵件或直接登入」且語氣友善）
- [ ] T082 [US-005] 在 `tests/IpLocations.IntegrationTests/Auth/ErrorMessageTests.cs` 中撰寫錯誤訊息語言一致性測試（SCN-040：所有錯誤訊息應統一使用繁體中文，無英文或簡體中文）

### US-005 的實作

- [ ] T083 [US-005] 檢查並更新 `src/IpLocations.Api/Validators/EmailValidator.cs` 中的錯誤訊息，確保為繁體中文：「請輸入有效的電子郵件格式」
- [ ] T084 [US-005] 檢查並更新 `src/IpLocations.Api/Validators/PasswordValidator.cs` 中的錯誤訊息，確保為繁體中文：「密碼必須至少 8 字元，並包含大寫字母、小寫字母、數字與特殊符號」
- [ ] T085 [US-005] 檢查並更新 `src/IpLocations.Api/Services/UserService.cs` 中電子郵件重複檢查的錯誤訊息，確保為繁體中文：「此電子郵件已被註冊，請使用其他電子郵件或直接登入」
- [ ] T086 [US-005] 檢查並更新 `src/IpLocations.Api/Services/AuthService.cs` 中登入失敗的錯誤訊息，確保為繁體中文：「帳號或密碼錯誤」
- [ ] T087 [US-005] 檢查並更新 `src/IpLocations.Api/Middleware/ExceptionHandlingMiddleware.cs` 中所有錯誤訊息，確保對照 ErrorCodeConst 的中文訊息正確且語氣友善
- [ ] T088 [US-005] 執行所有 US-005 相關測試，確認所有錯誤訊息為繁體中文且測試通過

**✅ 檢查點**: 此時，US-005 應完全正常運作 - 所有錯誤訊息統一使用繁體中文，語氣友善且提供清晰的操作指引

---

## Phase 8: US-006 - 查看註冊使用者資訊 (優先順序: P3)

**目標**: 實作個人資料查詢端點，讓已登入使用者可查看自己的電子郵件與註冊日期，且不顯示密碼資訊

**獨立測試**: 登入後透過 GET /api/auth/profile 端點查詢個人資料，應回傳 200 狀態碼與使用者資訊（Email、CreatedAt），未登入存取應回傳 401 錯誤

### US-006 的測試

- [ ] T089 [US-006] 在 `tests/IpLocations.IntegrationTests/Auth/ProfileTests.cs` 中撰寫已登入使用者存取個人資料測試（SCN-041：有效 Token 應回傳 200 與個人資料）
- [ ] T090 [US-006] 在 `tests/IpLocations.IntegrationTests/Auth/ProfileTests.cs` 中撰寫未登入使用者存取測試（SCN-042：無 Token 應回傳 401 與「未授權，請先登入」訊息）
- [ ] T091 [US-006] 在 `tests/IpLocations.IntegrationTests/Auth/ProfileTests.cs` 中撰寫個人資料內容驗證測試（SCN-043：回應應包含正確的電子郵件與註冊日期）
- [ ] T092 [US-006] 在 `tests/IpLocations.IntegrationTests/Auth/ProfileTests.cs` 中撰寫不顯示密碼測試（SCN-044：回應不應包含密碼欄位，無論明文或加密）
- [ ] T093 [US-006] 在 `tests/IpLocations.UnitTests/Services/UserServiceTests.cs` 中撰寫 GetProfileAsync 單元測試（Mock IUserRepository，驗證資料轉換邏輯）

### US-006 的實作

- [ ] T094 [US-006] 在 `src/IpLocations.Api/Services/UserService.cs` 中實作 UserService.GetProfileAsync 方法：
  - 呼叫 IUserRepository.GetByIdAsync 查詢使用者
  - 將 User 實體轉換為 UserProfileResponse（僅包含 Email 與 CreatedAt，不包含 PasswordHash）
  - 若使用者不存在，拋出 NotFoundException
- [ ] T095 [US-006] 在 `src/IpLocations.Api/Controllers/AuthController.cs` 中實作 GET /api/auth/profile 端點：
  - 新增 `[Authorize]` 屬性（需身分驗證）
  - 從 HttpContext.User 取得 UserId（使用 ClaimsPrincipalExtensions.GetUserId）
  - 呼叫 IUserService.GetProfileAsync
  - 回傳 UserProfileResponse（200 OK）
- [ ] T096 [US-006] 執行所有 US-006 相關測試，確認測試通過

**✅ 檢查點**: 此時，所有 User Story（US-001 ~ US-006）都應能獨立運作 - 使用者可查看個人資料，且系統不會暴露密碼資訊

---

## Phase 9: Polish & Cross-Cutting Concerns

**目的**: 橫切面改進、文件更新、效能與安全性強化

- [ ] T097 在 `doc/changes/user-authentication/contracts/auth-api.yaml` 中新增 OpenAPI 文件的 Security Schemes 定義（JWT Bearer Token 與 Cookie Authentication）
- [ ] T098 在 `src/IpLocations.Api/Controllers/AuthController.cs` 中為所有端點新增 XML 註解，提供 Swagger UI 的詳細說明
- [ ] T099 在 `README.md` 中更新專案說明，新增使用者身分驗證功能介紹與使用範例
- [ ] T100 在 `doc/changes/user-authentication/` 中建立部署指南文件（deploy-guide.md），說明資料庫遷移、Redis 設定、環境變數配置步驟
- [ ] T101 執行效能測試：使用 BenchmarkDotNet 測試註冊與登入端點，確認 95% 請求處理時間 < 200ms（NFR-001）
- [ ] T102 執行安全性測試：驗證密碼雜湊不可逆、會話識別碼隨機性（生成 10,000 個 Session ID 檢查重複）、時序攻擊防護（NFR-002、NFR-003）
- [ ] T103 在 `tests/IpLocations.IntegrationTests/` 中執行完整的測試套件，確認所有 Scenarios（SCN-001 ~ SCN-044）測試通過且覆蓋率達 80% 以上
- [ ] T104 程式碼清理：移除未使用的 using、統一命名慣例、格式化程式碼（遵循專案 linting 規則）
- [ ] T105 建立 E2E 測試腳本或 Postman Collection，涵蓋完整註冊 → 登入 → 存取 API → 查看個人資料 → 登出流程
- [ ] T106 在 `src/IpLocations.Api/Program.cs` 中新增日誌記錄配置，記錄所有登入嘗試（成功與失敗）、帳號鎖定/解鎖事件（NFR-004）
- [ ] T107 檢查並更新 `src/IpLocations.Api/Dockerfile`，確保包含 SQL Server 與 Redis 連線配置，支援容器化部署
- [ ] T108 Code Review：提交 Pull Request 並請團隊成員進行程式碼審查，確認符合專案規範與最佳實務

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: 無相依性 - 可立即開始
- **Foundational (Phase 2)**: 相依於 Setup 完成 - **阻塞**所有 User Story
- **User Stories (Phase 3-8)**: 全部相依於 Foundational 階段完成
  - US-001 (Phase 3): 可在 Foundational 後立即開始，無其他相依
  - US-002 (Phase 4): 相依於 US-001（需要使用者資料），但可獨立測試
  - US-003 (Phase 5): 相依於 US-002（需要登入功能），但可獨立測試
  - US-004 (Phase 6): 相依於 US-002（整合至登入流程），但可獨立測試
  - US-005 (Phase 7): 橫切所有前述 User Story（檢查錯誤訊息），可在任意時間點執行
  - US-006 (Phase 8): 相依於 US-001、US-003（需要使用者資料與身分驗證），但可獨立測試
- **Polish (Phase 9)**: 相依於所有期望的 User Story 都已完成

### Within Each User Story

- 測試任務**必須**在實作任務之前撰寫並**失敗**（TDD 方法）
- 資料模型與工具類別先於服務層
- 服務層先於控制器層
- 核心實作先於整合
- User Story 完成後驗證檢查點再移至下一個優先順序

### Critical Path (MVP)

1. Phase 1: Setup → Phase 2: Foundational（**阻塞性**）
2. Phase 3: US-001（註冊）→ Phase 4: US-002（登入）→ Phase 5: US-003（會話管理）→ Phase 6: US-004（暴力破解防護）
3. 驗證所有 P1 User Story 獨立運作
4. 部署/展示 MVP

---

## Implementation Strategy

### MVP First (P1 User Stories: US-001 ~ US-004)

1. 完成 Phase 1: Setup
2. 完成 Phase 2: Foundational（**關鍵** - 阻塞所有 User Story）
3. 依序完成 Phase 3 → Phase 4 → Phase 5 → Phase 6（US-001 ~ US-004）
4. **停止並驗證**: 所有 P1 User Story 可獨立測試且運作正常
5. 部署/展示 MVP

### Incremental Delivery

1. 完成 Setup + Foundational → 基礎就緒
2. 新增 US-001（註冊）→ 獨立測試 → 提交
3. 新增 US-002（登入）→ 獨立測試 → 提交
4. 新增 US-003（會話管理）→ 獨立測試 → 提交
5. 新增 US-004（暴力破解防護）→ 獨立測試 → 部署/展示 MVP
6. 新增 US-005（錯誤訊息）→ 獨立測試 → 提交
7. 新增 US-006（個人資料）→ 獨立測試 → 提交
8. 完成 Polish → Code Review → 最終部署

---

## Notes

- [Story] 標籤將任務對應到特定的 User Story 以實現可追蹤性
- 每個 User Story 都應能獨立完成和測試
- 測試必須在實作前撰寫並驗證失敗（TDD 方法）
- 在每個任務或邏輯群組後提交 Git Commit
- 在每個 Phase 的檢查點停止以獨立驗證 Story 是否正常運作
- 資料庫遷移檔案應納入版本控制（`src/IpLocations.Api/Data/Migrations/`）
- Redis Key 命名遵循 `auth:*` 前綴規範，避免與現有 IP 查詢功能衝突
- 所有密碼必須使用 bcrypt 加密，禁止明文儲存
- JWT Token 必須儲存於 HTTP-Only Cookie，防止 XSS 攻擊
- 錯誤訊息統一使用繁體中文，語氣友善且具建設性
- 避免：模糊的任務描述、相同檔案的衝突修改、破壞獨立性的跨 Story 相依性
