## Database migration instructions

1. Ensure the .NET SDK and EF tools are installed:

   dotnet tool install --global dotnet-ef

2. From the repository root run:

   dotnet ef migrations add AddBlobNameToAssignmentSubmission -s src/MaphunziroBlackboard.Web -p src/MaphunziroBlackboard.Infrastructure
   dotnet ef database update -s src/MaphunziroBlackboard.Web -p src/MaphunziroBlackboard.Infrastructure

3. If you use Visual Studio, open Package Manager Console and run:

   Add-Migration AddBlobNameToAssignmentSubmission -Project MaphunziroBlackboard.Infrastructure -StartupProject MaphunziroBlackboard.Web
   Update-Database -Project MaphunziroBlackboard.Infrastructure -StartupProject MaphunziroBlackboard.Web

Note: Ensure your DefaultConnection in appsettings.Development.json points to a valid dev database before running updates.
