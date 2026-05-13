# Maphunziro Blackboard 🎓

A comprehensive ERP and Learning Management System (LMS) designed specifically for secondary/high schools in Malawi. Built with modern ASP.NET Core 8, this platform combines the best features of Blackboard, Moodle, and PowerSchool into a single, unified solution.

## 🌟 Features

### 🎓 Learning Management System (Blackboard-Style)
- **Course Management**: Create and manage courses with modules, content, and resources
- **Assignments**: Create assignments with rubrics, submissions, and grading
- **Quizzes & Exams**: Online assessments with automatic grading
- **Student Progress**: Track learning progress and analytics
- **Interactive Content**: Support for videos, PDFs, presentations, and more

### 👥 User Management
- **Multi-Role System**: Super Admin, School Admin, Teacher, Student, Parent, Accountant, Librarian, Registrar
- **Secure Authentication**: ASP.NET Identity with OAuth (Google, Microsoft)
- **Profile Management**: Comprehensive user profiles with photos and details
- **Role-Based Access**: Granular permissions and access control

### 📊 Academic Management
- **Student Information**: Complete student records with admissions, classes, and streams
- **Teacher Management**: Faculty profiles, assignments, and schedules
- **Class & Stream Management**: Organize students by classes and streams
- **Attendance Tracking**: Real-time attendance monitoring and reporting
- **Grading System**: GPA calculation, report cards, and grade distribution

### 💰 Finance Module
- **Fee Structures**: Configurable fee types and amounts
- **Invoicing**: Automated invoice generation and tracking
- **Payment Processing**: Multiple payment methods and receipt generation
- **Financial Reports**: Revenue tracking and financial analytics

### 📚 Library Management
- **Book Catalog**: Digital library catalog with search and categorization
- **Circulation**: Book borrowing, returns, and due date tracking
- **Reservations**: Book reservation system for popular titles
- **Digital Resources**: Support for e-books and digital media

### 🗓️ Calendar & Scheduling
- **Academic Calendar**: Term dates, holidays, and events
- **Timetables**: Class schedules and teacher assignments
- **Event Management**: School events, meetings, and activities
- **Reminders**: Automated notifications for important dates

### 💬 Communication System
- **Messaging**: Internal messaging between users
- **Announcements**: School-wide and course-specific announcements
- **Notifications**: Real-time notifications with SignalR
- **Parent Communication**: Dedicated parent portal for updates

### 📈 Analytics & Reporting
- **Dashboard Analytics**: Comprehensive dashboards for all user types
- **Performance Reports**: Student performance and progress tracking
- **Attendance Reports**: Detailed attendance analytics
- **Financial Reports**: Revenue and payment tracking
- **Export Capabilities**: Export reports to PDF and Excel

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8 MVC
- **Database**: Microsoft SQL Server
- **Authentication**: ASP.NET Identity with OAuth
- **Frontend**: Bootstrap 5, jQuery, Chart.js
- **Real-time**: SignalR
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Architecture**: Clean Architecture with Repository Pattern

## 📋 Prerequisites

- **.NET 8 SDK** or later
- **SQL Server** (LocalDB or SQL Server Express minimum)
- **Visual Studio 2022** or VS Code
- **Git** for version control

## 🚀 Installation

### 1. Clone the Repository
```bash
git clone https://github.com/your-repo/maphunziro-blackboard.git
cd maphunziro-blackboard
```

### 2. Database Setup

#### Option A: SQL Server LocalDB (Recommended for Development)
1. Ensure SQL Server LocalDB is installed with Visual Studio
2. The connection string in `appsettings.json` is already configured for LocalDB

#### Option B: SQL Server Express/Standard
1. Install SQL Server Express or Standard
2. Create a new database named `MaphunziroBlackboardDB`
3. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MaphunziroBlackboardDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Install EF Core Tools
```bash
dotnet tool install --global dotnet-ef
```

### 4. Restore NuGet Packages
```bash
cd src/MaphunziroBlackboard.Web
dotnet restore
```

### 5. Apply Database Migrations
```bash
dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:7123`

## 🔧 Configuration

### Database Connection
Update the connection string in `src/MaphunziroBlackboard.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MaphunziroBlackboardDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### OAuth Setup (Optional)
To enable Google/Microsoft login:

1. **Google OAuth**
   - Create a project in Google Cloud Console
   - Enable Google+ API
   - Create OAuth 2.0 credentials
   - Update `appsettings.json`:
   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "YOUR_GOOGLE_CLIENT_ID",
         "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
       }
     }
   }
   ```

2. **Microsoft OAuth**
   - Register an app in Azure Portal
   - Configure authentication
   - Update `appsettings.json`:
   ```json
   {
     "Authentication": {
       "Microsoft": {
         "ClientId": "YOUR_MICROSOFT_CLIENT_ID",
         "ClientSecret": "YOUR_MICROSOFT_CLIENT_SECRET"
       }
     }
   }
   ```

### Email Configuration
Configure SMTP settings for email notifications:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@maphunziro.com",
    "FromName": "Maphunziro Blackboard"
  }
}
```

## 👤 Default User Accounts

After running the application for the first time, the following default accounts are created:

- **Super Admin**: `admin@maphunziro.com` / `Admin@123`

You can use this account to:
- Set up your school information
- Create other admin accounts
- Configure system settings

## 🏫 Getting Started Guide

### 1. School Setup
1. Log in as Super Admin
2. Navigate to Settings → School Information
3. Update your school details, logo, and contact information

### 2. User Management
1. Create administrator accounts for your school
2. Add teachers and students
3. Set up parent accounts
4. Assign appropriate roles and permissions

### 3. Academic Structure
1. Create classes and streams
2. Set up subjects and departments
3. Define academic years and terms
4. Create course catalog

### 4. Course Creation
1. Teachers can create courses
2. Add modules and content
3. Create assignments and quizzes
4. Publish courses for student enrollment

### 5. Student Enrollment
1. Students can browse and enroll in courses
2. Track progress and submit assignments
3. Access grades and feedback

## 📱 Mobile Compatibility

The system is fully responsive and works on:
- **Desktop** (Chrome, Firefox, Edge, Safari)
- **Tablets** (iPad, Android tablets)
- **Smartphones** (iPhone, Android phones)

## 🔒 Security Features

- **Secure Authentication**: ASP.NET Identity with password policies
- **Role-Based Authorization**: Granular access control
- **Data Protection**: Input validation and SQL injection prevention
- **Session Management**: Secure session handling
- **Audit Logging**: Comprehensive activity tracking

## 📊 Performance Features

- **Caching**: Response caching for improved performance
- **Database Optimization**: Efficient queries and indexing
- **Lazy Loading**: Optimized data loading
- **Async Operations**: Non-blocking database operations

## 🔄 Backup & Recovery

### Database Backup
```sql
-- Full backup
BACKUP DATABASE MaphunziroBlackboardDB 
TO DISK = 'C:\Backup\MaphunziroBlackboardDB.bak'
WITH FORMAT, INIT;

-- Differential backup
BACKUP DATABASE MaphunziroBlackboardDB 
TO DISK = 'C:\Backup\MaphunziroBlackboardDB_diff.bak'
WITH DIFFERENTIAL;
```

### File Backup
Regularly backup the following:
- Database files
- User-uploaded content (images, documents)
- Configuration files
- Log files

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Verify SQL Server is running
   - Check connection string in appsettings.json
   - Ensure database exists

2. **Migration Issues**
   - Delete the database and recreate
   - Run `dotnet ef database update` again

3. **Login Issues**
   - Clear browser cookies
   - Check user account status
   - Verify email confirmation

4. **Performance Issues**
   - Check database indexes
   - Review query performance
   - Monitor server resources

### Log Files
Check the following log files for debugging:
- Application logs: `logs/log-.txt`
- Database logs: SQL Server logs
- System logs: Windows Event Viewer

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For support and questions:
- **Email**: support@maphunziro.com
- **Documentation**: Check the `/docs` folder
- **Issues**: Report issues on GitHub

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- **ASP.NET Core Team** for the excellent framework
- **Bootstrap Team** for the responsive UI framework
- **Chart.js** for data visualization
- **SignalR Team** for real-time communication
- **Malawi Education Community** for feedback and requirements

## 🗺️ Roadmap

### Version 1.1 (Planned)
- [ ] Mobile app (React Native)
- [ ] Advanced analytics dashboard
- [ ] Bulk data import/export
- [ ] API documentation
- [ ] Multi-language support

### Version 1.2 (Future)
- [ ] Video conferencing integration
- [ ] AI-powered learning recommendations
- [ ] Advanced reporting features
- [ ] Cloud deployment options
- [ ] Integration with other systems

---

**Maphunziro Blackboard** - Empowering Education in Malawi 🇲🇼

Built with ❤️ for Malawian schools
