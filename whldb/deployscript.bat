REM Database deployment script

@ECHO OFF

REM Usage
REM deployscript.bat [Server(\Instance)] [Database] [Username] [Password] [Release Version] [Environment]

: Setup parameters
SET ServerName=%1%
SET Database=%2%
SET Username=%3%
SET Password=%4%
SET ReleaseVersion=%5%
SET Environment=%6%

: Usage
IF "%ServerName%"=="" GOTO HELP_USAGE
IF "%Database%"=="" GOTO HELP_USAGE
IF "%Username%"=="" GOTO HELP_USAGE
IF "%Password%"=="" GOTO HELP_USAGE
IF "%ReleaseVersion%"=="" GOTO HELP_USAGE
IF "%Environment%"=="" GOTO HELP_USAGE

: Setup script folders
SET AllReleaseConfig=.\ReleaseConfig\*.sql
SET AllDrops=.\Drops\*.sql
SET AllUDTs=.\UserDefinedTypes\*.sql
SET AllTables=.\Tables\*.sql
SET AllSequences=.\Sequences\*.sql
SET AllViews=.\Views\*.sql
SET AllFunctions=.\Functions\*.sql
SET AllStoredProcedures=.\StoredProcedures\*.sql
SET AllInserts=.\Inserts\*.sql
SET AllUpdates=.\Updates\*.sql
SET AllJobs=.\Jobs\*.sql

: Deploy database schema
FOR %%f IN (%AllReleaseConfig% %AllDrops% %AllUDTs% %AllTables% %AllSequences% %AllViews% %AllFunctions% %AllStoredProcedures% %AllInserts% %AllUpdates% %AllJobs%) DO (
    ECHO Scripting [%%f]...
    REM ECHO sqlcmd  -S %ServerName%  -U %Username%  -P %Password%  -d %Database%  -b  -i "%%f" -v VER="%ReleaseVersion%" ENV="%Environment%"
    REM TIMEOUT /T 10
    sqlcmd  -S %ServerName%  -U %Username%  -P %Password%  -d %Database%  -b  -i "%%f" -v VER="%ReleaseVersion%" ENV="%Environment%"
    IF ERRORLEVEL 1 (
        ECHO "Failed to deploy sql script"
        REM EXIT ERRORLEVEL
    )
)
GOTO END_DEPLOY

:HELP_USAGE
ECHO ================================================================================
ECHO Usage:
ECHO   deployscript.bat [Server(\Instance)] [Database] [Username] [Password] [Release Version] [Release Environment]
ECHO Example:
ECHO   deployscript.bat SQLDB WHL username password 1_0_0 DEV
ECHO ================================================================================

:END_DEPLOY