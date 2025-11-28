# Story 2.5: Account Recovery

Status: done

## Description
As a user, I want to be able to reset my password via email if I forget it.

## Acceptance Criteria
- Password reset functionality via email is available and works.
- SMTP configuration guide provided for reliable email delivery.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented ForgotPassword.razor page.
- Added SendPasswordResetEmail to AuthService.
- Added "Forgot Password?" link to Login page.
- Created `smtp_setup_guide.md` to assist with configuring reliable email delivery via SendGrid to bypass Gmail filtering issues.

### File List
- HarborFlowSuite/HarborFlowSuite.Client/Pages/ForgotPassword.razor
- HarborFlowSuite/HarborFlowSuite.Client/Services/IAuthService.cs
- HarborFlowSuite/HarborFlowSuite.Client/Services/AuthService.cs
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/auth.js
- HarborFlowSuite/HarborFlowSuite.Client/Pages/Login.razor
- smtp_setup_guide.md
