@ECHO OFF

ECHO Stopping IIS
net stop w3svc 

REM ECHO Stopping AppPool WHLAdmin
REM %SYSTEMROOT%\System32\inetsrv\appcmd stop apppool /apppool.name:"WHLAdmin"

ECHO Deleting folder contents of E:\wwwroot\WHLAdmin
rmdir E:\wwwroot\WHLAdmin /s /q

ECHO Deploying application
dotnet publish .\WHLAdmin\WHLAdmin.csproj -c Release -o E:\wwwroot\WHLAdmin

REM ECHO Starting AppPool WHLAdmin
REM %SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:"WHLAdmin"

ECHO Starting IIS
net start w3svc