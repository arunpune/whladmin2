@ECHO OFF
REM dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
REM reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info /p:ExcludeByFile=\"**\*.cshtml,**\*program.cs,**\*RegexGenerator.g.cs\"
reportgenerator -reports:"lcov.info" -targetdir:"coveragereport" -reporttypes:Html