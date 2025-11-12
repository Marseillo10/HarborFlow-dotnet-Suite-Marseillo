# Product Requirements Document (PRD) - HarborFlow Suite

## 1. Discovery - Project, Domain, and Vision

### Vision Alignment
The HarborFlow Suite aims to transform port operations and maritime workflows by digitizing manual processes into an intuitive, unified digital platform. It will offer real-time vessel tracking, data-driven analytics, streamlined service request management, and curated maritime news.

### Project Classification
This is a production-grade, multi-part system comprising an ASP.NET Core 9 Web API, a Blazor WebAssembly PWA client, a PostgreSQL database, and Firebase Authentication. It's designed to replace fragmented manual processes.

### Project Type
Primarily a web application, featuring a Blazor PWA frontend and an ASP.NET Core API backend.

### Domain Type
Maritime industry, specifically focusing on port operations.

### Complexity Level
Moderate. While it includes real-time data, authentication, authorization, and offline capabilities, it's structured as a modular monolith, suitable for a small development team.

### Product Magic Essence
The core value lies in delivering measurable operational efficiency and enhanced decision-making through a seamless, unified digital experience, reliable real-time data synchronization, and a user-friendly interface across web and mobile devices.

## 2. Success Definition

### Measurable Objectives
- **User Adoption:** 80% of target users active within 6 months (measured via analytics tracking).
- **Performance:** Page load times under 2 seconds (measured via application monitoring).
- **Availability:** 99.5% uptime (measured via health check monitoring).
- **User Satisfaction:** Greater than 4.0/5.0 rating (measured via user feedback surveys).

### Critical Success Factors
- Delivering a seamless user experience across web and mobile devices.
- Ensuring reliable real-time data synchronization.
- Maintaining robust security and compliance with maritime regulations.
- Providing a scalable architecture that supports organizational growth.
- Offering comprehensive offline capabilities for field operations.

### Key Performance Indicators (KPIs)
- Daily/Monthly Active Users (DAU/MAU).
- Average session duration and user engagement.
- Reduction in service request processing time.
- System response times and error rates.
- Mobile/PWA installation and usage rates.

## 3. Scope Definition

### In-Scope (MVP)
- Real-time vessel position tracking with interactive mapping.
- Role-based access control (RBAC) system with four distinct user roles.
- Service request workflow with digital forms and approval processes.
- Analytics dashboard with operational insights and reporting.
- Maritime news feed aggregation with filtering capabilities.
- Map bookmarking and personalized navigation features.
- Progressive Web App with improved offline capabilities, background synchronization, and push notifications.

### Out-of-Scope (Growth/Vision)
- Native mobile applications (iOS/Android apps).
- Integration with existing port management systems (Phase 2).
- Advanced geofencing and alerting capabilities (Future roadmap).
- Vessel history playback functionality (Future roadmap).
- Multi-language localization support.
- Advanced reporting and business intelligence tools.
- Desktop WPF application development.
- Integration with AIS (Automatic Information System) data feeds.
- Advanced analytics and machine learning capabilities.
- Third-party maritime system integrations.
- Enhanced notification and alerting systems.

## 4. Domain-Specific Exploration

### Regulatory Requirements
- Compliance with general maritime regulations.
- Adherence to data privacy regulations (e.g., GDPR) and secure credential storage.
- Implementation of access audit logging and the principle of least privilege.
- Enforcement of analytics data retention policies.
- Documentation of approval processes and data retention.
- Compliance with maritime data handling standards.

### Industry Standards
- Compatibility with maritime industry standards and protocols.
- Adherence to OWASP guidelines for input validation.
- Alignment with NIST Cybersecurity framework.
- Consideration of ISO 27001 for information security management.

### Safety/Risk Factors
- Ensuring robust security to mitigate risks like data tampering, vessel data exposure, authentication bypass, and privilege escalation.

### Required Validations
- Strict validation for coordinates, vessel IDs, email formats, and password strength.
- Validation of role existence and permission scope.
- Accurate date range validation and data aggregation.
- Enforcement of required fields and business rules in service requests.

## 5. Innovation Discovery

### Novelty Signals
- The vision of a 'transformative digital platform' to modernize port operations.
- The use of 'cutting-edge web technologies' to replace fragmented manual processes.
- The creation of an 'intuitive, unified digital experience'.
- The adoption of an 'API-first, microservices-ready architecture'.
- The implementation of 'SignalR with Native AOT compilation support' for enhanced real-time performance.
- The use of a 'Progressive Web App architecture' to provide offline functionality and a native app-like experience.

### Unique Aspects
- The combination of real-time vessel tracking, service request management, and analytics in a single, unified platform for the maritime industry is a key innovation.
- The use of a Blazor WebAssembly PWA to deliver a rich, offline-capable user experience without the need for native mobile apps is a modern and efficient approach.
- The application of the latest .NET 9 features, such as Native AOT for SignalR, demonstrates a commitment to high performance and cutting-edge technology.

### Validation Approach
The success of these innovations will be validated through the project's success criteria, including:
- **User Adoption:** Achieving 80% of target users active within 6 months.
- **User Satisfaction:** Attaining a user satisfaction rating of over 4.0/5.0.
- **Efficiency Gains:** Reducing manual processing time by 60-80%.

## 6. Project-Specific Deep Dive

### API/Backend Requirements
- **Endpoints, Methods, and Parameters:**
  - The API will provide RESTful endpoints for managing Users, Companies, Vessels, Service Requests, and Map Bookmarks, using standard HTTP methods (GET, POST, PUT, DELETE).
  - SignalR hubs will be used for real-time communication, including vessel tracking and notifications.
- **Authentication and Authorization:**
  - Authentication will be managed by Firebase Authentication using JWTs.
  - A four-tier Role-Based Access Control (RBAC) system (System Administrator, Port Authority Officer, Vessel Agent, Guest) will be implemented, enforcing company-based data isolation.
- **Error Codes and Rate Limits:**
  - The API will use standard HTTP error codes (e.g., 400, 401, 403, 404, 500) and implement rate limiting to prevent abuse.
- **Data Schemas:**
  - The data schemas are well-defined in the 'Database Design' section of the technical specifications, covering all necessary entities.

### Web Client (Blazor PWA) Requirements
- **Platform Requirements:**
  - The application will be a Progressive Web App (PWA), accessible through modern web browsers on desktop, tablet, and mobile devices.
- **Device Features:**
  - The PWA will support offline capabilities through service workers and IndexedDB.
  - It will also feature background synchronization and push notifications for a rich, native-like user experience.

## 7. UX Principles

### Visual Personality
- A modern and responsive user interface.
- The application will feature both Light and Dark themes, with a high-contrast option to ensure accessibility.
- The overall 'vibe' will be clean, professional, and intuitive.

### Key Interaction Patterns
- Real-time data updates will provide a live and dynamic user experience.
- A Global Command Palette (accessible via Cmd+K or Ctrl+K) will enable quick search and navigation.
- A guided onboarding tour will be available to help new users get acquainted with the platform.
- The application will be fully responsive, providing an optimal experience on desktop, tablet, and mobile devices.
- Touch-friendly interactions will be implemented for a seamless mobile experience.
- Full keyboard navigation will be supported to ensure accessibility.

### Critical User Flows
- **Real-time Vessel Monitoring:** A smooth and intuitive experience for tracking vessels on the interactive map.
- **Service Request Management:** A streamlined and efficient workflow for creating, managing, and approving service requests.
- **Analytics Dashboard:** An interactive and insightful dashboard for visualizing operational data.

## 8. Functional Requirements Synthesis

### User Management
- **FR-1: User Authentication & Authorization:**
  - **FR-1.1:** Users must be able to register and log in using email/password and social providers (via Firebase).
  - **FR-1.2:** All API requests must be authenticated with a valid JWT token.
  - **FR-1.3:** User sessions should persist across browser sessions with automatic token refresh.
  - **FR-1.4:** Users must be able to view and update their basic profile information.
  - **FR-1.5:** A password reset functionality via email must be available.
- **FR-2: Role-Based Access Control (RBAC):**
  - **FR-2.1:** The system must support four distinct user roles: System Administrator, Port Authority Officer, Vessel Agent, and Guest.
  - **FR-2.2:** Granular permissions must be enforced at the API level for all resources.
  - **FR-2.3:** Vessel Agents must only be able to access data associated with their own company.
  - **FR-2.4:** System Administrators must have the ability to manage user roles and permissions.

### Vessel Tracking & Management
- **FR-3: Real-time Vessel Tracking:**
  - **FR-3.1:** The application must display an interactive map with live vessel positions.
  - **FR-3.2:** Vessel positions must update in real-time for all connected clients (within 1 second).
  - **FR-3.3:** Clicking on a vessel must display an information panel with its details.
  - **FR-3.4:** Users must be able to switch between different map layers (e.g., Street, Satellite).
  - **FR-3.5:** The map interface must be responsive and function correctly on desktop, tablet, and mobile devices.
- **FR-4: Vessel Information:**
  - **FR-4.1:** The system must store and manage essential vessel information, including name, IMO number, type, length, and width.
  - **FR-4.2:** All vessel data must be associated with a specific company.

### Service Request Management
- **FR-5: Service Request Creation:**
  - **FR-5.1:** Users must be able to submit digital service request forms with all required fields and appropriate validation.
- **FR-6: Approval Workflow:**
  - **FR-6.1:** Port Authority Officers must be able to approve or reject service requests, with the option to add comments.
  - **FR-6.2:** The system must accurately track the status of each request (e.g., Pending, Approved, Rejected, In Progress, Completed, Cancelled).
  - **FR-6.3:** Users must be able to view the current status and a complete approval history for their submitted requests.
- **FR-7: Filtering and Notifications:**
  - **FR-7.1:** Users must only be able to view service requests that are relevant to their assigned company and role.
  - **FR-7.2:** Users should receive timely notifications regarding status changes to their service requests.

### Analytics & Reporting
- **FR-8: Dashboard Visualization:**
  - **FR-8.1:** The system must display a data-driven dashboard providing comprehensive operational insights.
  - **FR-8.2:** It must visualize service request status, including counts for pending, approved, and rejected requests.
  - **FR-8.3:** The dashboard must also show vessel distribution by type or category.
- **FR-9: Role-Based Filtering:**
  - **FR-9.1:** Users must only see analytics data that is relevant to their assigned permissions and company.
- **FR-10: Real-time Updates:**
  - **FR-10.1:** Charts and metrics on the dashboard should update automatically in real-time as underlying data changes.
- **FR-11: Export Functionality:**
  - **FR-11.1:** Users should have the ability to export dashboard chart data in common formats such as CSV and PDF.

### Information & News
- **FR-12: Maritime News Aggregation:**
  - **FR-12.1:** The system must provide a curated news feed from the maritime industry.
  - **FR-12.2:** Users must be able to filter news articles by category and keywords directly on the client-side.
  - **FR-12.3:** The news feed should support caching to allow for offline access.

### User Experience & Navigation
- **FR-13: Map Bookmarking:**
  - **FR-13.1:** Users must be able to save and quickly return to specific map locations.
- **FR-14: Progressive Web App (PWA) Features:**
  - **FR-14.1:** The application must function offline, providing a reliable experience even without network connectivity.
  - **FR-14.2:** It must support background synchronization of data, ensuring data consistency when online.
  - **FR-14.3:** The application must be capable of delivering push notifications to users.
  - **FR-14.4:** It should be installable as a native-like app on various devices.
- **FR-15: Global Command Palette:**
  - **FR-15.1:** A centralized search and navigation interface, accessible via keyboard shortcuts (Cmd+K/Ctrl+K), must be provided for enhanced user productivity.

## 9. Non-Functional Requirements Discovery

### Performance Requirements
- **Page Load Times:** The initial page load time must be less than 2 seconds.
- **API Response Time:**
  - Authentication responses must be under 1 second.
  - The 95th percentile API response time for all other requests must be less than 2 seconds.
- **Real-time Updates:**
  - Vessel position updates must propagate to all connected clients within 1 second.
  - SignalR message delivery latency must be less than 1 second.
- **Dashboard Load Time:** The analytics dashboard must load within 3 seconds.
- **Form Submission:** Service request form submissions must complete within 2 seconds.
- **Permission Checks:** Permission check response times must be under 100ms.
- **Database Queries:** Standard database operations must have a response time of less than 100ms.

### Security Requirements
- **Authentication & Authorization:**
  - The system must ensure robust security and compliance with maritime regulations.
  - JWT token validation must be performed on every API request.
  - Role-based access control (RBAC) with granular permissions must be enforced.
  - Company-based data isolation must be strictly maintained.
  - The system must be protected against authentication bypass and privilege escalation.
- **Data Protection:**
  - Data at rest must be encrypted using AES-256, and data in transit must be protected with TLS 1.3.
  - Secure credential storage is mandatory.
  - The system must protect against data tampering.
- **Input Validation:**
  - Comprehensive input validation and sanitization must be implemented to prevent injection vulnerabilities (e.g., SQL Injection, Cross-Site Scripting (XSS), Cross-Site Request Forgery (CSRF)).
- **Audit Logging:**
  - Comprehensive audit logging of all security-related events and data modifications must be maintained.
- **Compliance:**
  - The system must adhere to data privacy regulations (e.g., GDPR).
  - Compliance with maritime industry standards and protocols is required.
  - Alignment with the NIST Cybersecurity framework and OWASP guidelines is essential.

### Scalability Requirements
- **Architecture:**
  - The system must have a scalable architecture that supports organizational growth.
  - The modular monolithic architecture should allow for a future migration to microservices if the need arises.
- **Concurrent Users:**
  - The system must be able to support up to 50,000 Monthly Active Users (MAUs) on the free tier of Firebase Authentication.
- **Real-time Connections:**
  - The system must be able to handle a high number of concurrent WebSocket connections to support its real-time features.
- **Database:**
  - The database must be scalable, with options for read replicas and connection pooling to handle increased load.
- **API:**
  - The API must be stateless to allow for horizontal scaling across multiple instances.

### Maintenance Requirements
- **Firebase Token Refresh:** The system must automatically handle Firebase Authentication token refreshes.
- **RSS Feed Updates:** Maritime news feeds must be updated daily.
- **Database Backups:** Daily automated backups for the PostgreSQL database are required.
- **Service Worker Updates:** The PWA service worker must be updated as needed to ensure optimal functionality and security.
- **Code Maintainability:** The codebase must adhere to Clean Architecture principles, clear service boundaries, and modular design patterns to ensure long-term maintainability.
- **Monitoring & Logging:** Structured logging and health checks must be implemented to facilitate easier debugging, troubleshooting, and provide operational insights.

## 10. Tenant Model and Permission Matrix

### Tenant Model
The system will employ a simple, single-database multi-tenancy model where each tenant is a "Company". Data will be isolated at the application level through a mandatory `CompanyId` foreign key on all relevant tables (e.g., `Vessels`, `ServiceRequests`, `Users`). All database queries will be filtered by the `CompanyId` of the authenticated user's session to ensure data segregation.

### Permission Matrix

| Feature/Action | System Administrator | Port Authority Officer | Vessel Agent | Guest |
| :--- | :---: | :---: | :---: | :---: |
| **User Management** | | | | |
| View All Users | ✅ | ✅ | ❌ | ❌ |
| Manage User Roles | ✅ | ❌ | ❌ | ❌ |
| View Own Profile | ✅ | ✅ | ✅ | ✅ |
| Edit Own Profile | ✅ | ✅ | ✅ | ✅ |
| **Vessel Tracking** | | | | |
| View All Vessels | ✅ | ✅ | ✅ (Own Company) | ✅ |
| View Vessel Details | ✅ | ✅ | ✅ (Own Company) | ✅ |
| **Service Requests** | | | | |
| Create Service Request | ✅ | ✅ | ✅ | ❌ |
| View All Service Requests | ✅ | ✅ | ❌ | ❌ |
| View Own Company's Requests | ✅ | ✅ | ✅ | ❌ |
| Approve/Reject Requests | ✅ | ✅ | ❌ | ❌ |
| **Analytics** | | | | |
| View Full Dashboard | ✅ | ✅ | ❌ | ❌ |
| View Company Dashboard | ✅ | ✅ | ✅ | ❌ |
| Export Data | ✅ | ✅ | ✅ | ❌ |
| **News** | | | | |
| View News Feed | ✅ | ✅ | ✅ | ✅ |
| **Map Bookmarks** | | | | |
| Create/Delete Bookmarks | ✅ | ✅ | ✅ | ✅ |

## 11. References
- Product Brief
- Market Research Document
- UX Design Specifications