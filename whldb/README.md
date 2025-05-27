# Introduction 
Database schema and scripts for the Westchester Housing Lottery application - HomeSeeker.

# Getting Started
1.	Installation process
- Download and install [SQL Server Developer Edition 2022 or higher](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Download and install [SQL Server Management Studio 2022 or higher](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16#download-ssms)
- Download and install [VSCode](https://code.visualstudio.com/)
- Download and install VSCode extension [SQL Database Projects](https://marketplace.visualstudio.com/items?itemName=ms-mssql.sql-database-projects-vscode)
- Download and install VSCode extension [DataWorkspace](https://marketplace.visualstudio.com/items?itemName=ms-mssql.data-workspace-vscode)
- Download and install VSCode extension [SQL Bindings](https://marketplace.visualstudio.com/items?itemName=ms-mssql.sql-bindings-vscode)
2.	Software dependencies
- SQL Server 2022 or higher
3.	Latest releases
- v0.5
4.	API references
- None

# Deploy the Database
- Launch command prompt within the working directory (e.g. C:\WHLDB)
- Run "deployscript.bat localhost WHL (uid) (pwd) 0_1_0 (env)"

Note: Version number should be in the format {Major}_{Minor}_{Revision} (e.g. 0_0_1)
Note: Environment should one of DEV, TEST, PROD
