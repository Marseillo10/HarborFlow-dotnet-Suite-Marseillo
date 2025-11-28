# Story 2.2: JWT Token Validation

Status: done

## Description
As a system, all API requests must validate Firebase JWT tokens to ensure secure access.

## Acceptance Criteria
- All API requests successfully validate Firebase JWT tokens.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Configured JwtBearer authentication in Program.cs.
- Applied [Authorize] attribute to controllers (e.g., CompanyController) to enforce token validation.

### File List
- HarborFlowSuite/HarborFlowSuite.Server/Program.cs
- HarborFlowSuite/HarborFlowSuite.Server/Controllers/CompanyController.cs
