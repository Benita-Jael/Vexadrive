# VexaDrive - Quick Start Guide

## 🚀 Get Started in 5 Minutes

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- SQL Server running

---

## Backend Setup (2 minutes)

```powershell
# 1. Navigate to backend
cd "C:\Users\2416649\Downloads\VexaDrive\VexaDrive.Api\VexaDrive.Api"

# 2. Apply migrations (first time only)
dotnet ef database update

# 3. Run backend
dotnet run
```

✅ Backend ready at: **http://localhost:5066**

### Default Admin Login
- **Email**: admin@vexadrive.com
- **Password**: Admin@123456

---

## Frontend Setup (2 minutes)

```powershell
# 1. Open new terminal
cd "C:\Users\2416649\Downloads\VexaDrive\VexaDrive-ui"

# 2. Install dependencies (first time only)
npm install

# 3. Start dev server
npm start
```

✅ Frontend ready at: **http://localhost:4200**

---

## Access the Application

### 1. Open Browser
Navigate to: **http://localhost:4200**

### 2. Sign In with Admin Account
- Email: `admin@vexadrive.com`
- Password: `Admin@123456`

### 3. Or Create New Account
Click "Register now" to create a customer account

---

## Key Features to Try

### As Admin
1. **Dashboard**: View overview of all service requests
2. **Manage Requests**: Update service status and ETA
3. **Upload Bills**: Add PDF bills to service requests
4. **Analytics**: View request statistics
5. **Customer Management**: See all registered customers

### As Customer
1. **Create Service Request**: Submit maintenance request
2. **Track Requests**: Monitor service progress in real-time
3. **View Notifications**: Get status updates
4. **Download Bills**: Access digital bills

---

## API Documentation

Visit Swagger UI: **http://localhost:5066**

All 14+ endpoints are documented with:
- Request/response examples
- Authentication requirements
- Status codes

---

## Project Structure

```
VexaDrive/
├── VexaDrive.Api/              # .NET 9 Backend
│   ├── Program.cs              # Startup
│   ├── Controllers/             # API endpoints
│   ├── Models/                 # Database entities
│   ├── DTOs/                   # Data transfer objects
│   ├── Repositories/           # Data access layer
│   └── Services/               # Business logic
│
└── VexaDrive-ui/               # Angular 20 Frontend
    ├── src/
    │   ├── app/
    │   │   ├── components/     # UI components
    │   │   ├── services/       # API services
    │   │   ├── guards/         # Route guards
    │   │   └── models/         # TypeScript models
    │   ├── styles.css          # Global styles
    │   └── environments/        # Configuration
    └── angular.json            # Angular config
```

---

## Common Commands

### Backend
```powershell
# Run
dotnet run

# Build
dotnet build

# Run tests
dotnet test

# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### Frontend
```powershell
# Start dev server
npm start

# Build for production
ng build --configuration production

# Run tests
ng test

# Lint code
ng lint
```

---

## Troubleshooting

### Backend won't start
- Check SQL Server is running
- Verify port 5066 is free
- Check connection string in appsettings.json

### Frontend shows CORS error
- Ensure backend is running on 5066
- Check CORS policy in Program.cs

### Can't log in
- Clear browser cache
- Try default admin credentials
- Check browser console for errors

---

## Environment Configuration

### Development
- Backend: http://localhost:5066
- Frontend: http://localhost:4200
- Database: Local SQL Server

### Production
Update `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiBaseUrl: 'https://your-api-domain.com'
};
```

---

## Need Help?

1. **Swagger API Docs**: http://localhost:5066
2. **Full Documentation**: See `DEPLOYMENT_GUIDE.md`
3. **Code Comments**: Check source files for inline documentation

---

## What's Included

✅ Professional responsive UI
✅ Complete backend API
✅ User authentication & authorization
✅ Role-based access control
✅ Real-time notifications
✅ File upload support
✅ Error handling
✅ Swagger documentation
✅ Mobile-friendly design
✅ Production-ready code

---

**Ready to use? Start the backend and frontend servers and navigate to http://localhost:4200!** 🎉
