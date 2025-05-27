# Introduction 
Administration module for the Westchester Housing Lottery application - HomeSeeker.

# Getting Started
1.	Installation process
- Download and install [git](https://git-scm.com/download/win)
- Download and install [dotnet Core SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Download and install [VSCode](https://code.visualstudio.com/)
- Download and install VSCode extension [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
2.	Software dependencies
- ASP.Net Core 8.x
- Dapper
- SeriLog
3.	Latest releases
- v0.1
4.	API references
- None

# Build and Test
- To BUILD in VSCode, please press Ctrl+Shift+B
- To DEBUG in VSCode, please press F5
- To RUN in VSCode, please press Ctrl+F5
- To RUN from command prompt, please open command prompt to the working directory (e.g. C:\WHLAdmin), and execute the command "dotnet run"

# Deploying to IIS
- Setup the application pool - WHLAdmin
- Setup a website or virtual directory - WHLAdmin (e.g. E:\Releases\WHLAdmin)
- Open command prompt as administrator
- Navigate to the folder (e.g. E:\Sandbox\WHLAdmin)
- Run the file deploy.cmd
