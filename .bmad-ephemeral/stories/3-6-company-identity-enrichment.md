# Story 3.6: Company Identity Enrichment

Status: in-progress

## Description
As a **System Administrator** and **Company Manager**, I want to manage rich company profiles including logos, contact details, and subscription tiers, so that the platform feels like a branded, premium experience for each client.

## Acceptance Criteria
- [ ] **Data Model:** Update `Company` entity to include:
    - `LogoUrl` (string, optional)
    - `Website` (string, optional)
    - `PrimaryContactEmail` (string, required)
    - `BillingAddress` (string, optional)
    - `SubscriptionTier` (Enum: Free, Basic, Premium, Enterprise)
- [ ] **API:** Update `CompanyController` and Services to handle these new fields in CRUD operations.
- [ ] **UI - Management:** Update `CompanyManagement` page to display companies with their Logos and Tiers (possibly upgrading from a simple table to a Card/Grid view or an enhanced table).
- [ ] **UI - Editing:** Update `Add/Edit Company` dialogs to support input for these new fields.
- [ ] **UI - Branding:** Display the logged-in user's Company Logo in the application header/sidebar (if they belong to a company).

## Technical Notes
-   Need to create an EF Core migration for the schema change.
-   For `LogoUrl`, currently we can just use a text URL (pointing to a placeholder or external image). Full file upload implementation might be a separate story, but we can add the field now.
-   `SubscriptionTier` should be an Enum in the Core project.

## Dependencies
-   Story 3.3 (Company Data Isolation) - Completed.
