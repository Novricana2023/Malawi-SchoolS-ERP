Azure App Service Deployment Checklist
-----------------------------------

1. Create Azure resources
   - App Service (Linux or Windows) with .NET 8 stack
   - Azure SQL Database (server, database, admin user)

2. App Service configuration
   - Configuration -> Connection strings: add DefaultConnection (Type: SQLAzure)
	 Example value:
	 Server=tcp:<your-server>.database.windows.net,1433;Initial Catalog=<your-db>;User ID=<your-user>;Password=<your-password>;MultipleActiveResultSets=true;TrustServerCertificate=False;Encrypt=True;
   - Application settings: set Authentication client ids/secrets and any keys

3. Publish options
   - In Visual Studio: Right-click project -> Publish -> Select App Service or Folder
   - Using CLI: dotnet publish src/MaphunziroBlackboard.Web -c Release -o <publish-folder>

4. Troubleshooting
   - Enable filesystem logging temporarily and check Kudu (https://<app>.scm.azurewebsites.net)
   - Enable stdout by editing the generated web.config in the publish folder and set stdoutLogEnabled="true" and stdoutLogFile=".\logs\stdout"

5. Security
   - Do not store production secrets in source control; use App Service configuration or Key Vault
