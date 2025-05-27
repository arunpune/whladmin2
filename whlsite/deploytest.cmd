@ECHO OFF

ECHO Stopping IIS
net stop w3svc

REM ECHO Stopping AppPool WHLSite
REM %SYSTEMROOT%\System32\inetsrv\appcmd stop apppool /apppool.name:"WHLSite"

ECHO Deleting folder contents of E:\wwwroot\WHLSite
rmdir E:\wwwroot\WHLSite /s /q

ECHO Deploying application
dotnet publish .\WHLSite\WHLSite.csproj -c Release -o E:\wwwroot\WHLSite

REM ECHO Starting AppPool WHLSite
REM %SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:"WHLSite"

ECHO Starting IIS
net start w3svc