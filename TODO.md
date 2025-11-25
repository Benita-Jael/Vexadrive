Step 2 - Roles, registration and login implementation plan:

Backend:
- [ ] Modify JwtTokenGenerator to dynamically add user roles to JWT claims.
- [ ] Seed roles "Admin" and "Customer" and a default Admin user during app startup.
- [ ] Modify Register endpoint to assign "Customer" role by default.
- [ ] Add endpoint for Admin to promote Customer user to Admin.
- [ ] Use [Authorize(Roles="Admin")] and [Authorize(Roles="Customer")] attributes on controllers/routes.

Frontend:
- [ ] Create RoleGuard enforcing role-based route access.
- [ ] Update AuthService to parse JWT token to extract roles, expose roles getter method.
- [ ] Update routes to use RoleGuard with role info.
- [ ] Update header navigation to show role-specific links, preserving CSS/layout.
