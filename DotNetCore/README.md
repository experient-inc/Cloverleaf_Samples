# Cloverleaf - .NET Core feed example
This project is a .net core 3.1 implementation of a Cloverleaf pull, using REST calls, BSON, and the generated Entity schema objects provided at [https://api.experientdata.com/].
Please see Program.cs for implementation details and nuances.

### Dependencies
This console application uses .NET Core 3.1, which is their current LTS version.  It is available for download at [https://dotnet.microsoft.com/download/dotnet-core/3.1]
It also uses the NuGet package MongoDB.BSON, as we are consuming the Cloverleaf feed with the BSON flag set high for better compression than standard JSON.

### Setup
Once you've cloned the project locally, open a console window within the directory of the .csproj file, and do the following to confirm it builds successfully on your machine:

```
dotnet restore
dotnet build
```

### Execution
To run the project as-is to test it, you can do the following :

`dotnet run -- -EventCode <eventCode> -FeedType <feedtype> -CLUserName <yourUserName> [-CLPassword <yourPassword>] [-Since <sinceValue] [-OutFile <explicitFilePath>]`

Conversely, if you want to run a precompiled instead, you can run the following from the build output directory:

`./CLFeedPull.NetCore.exe -EventCode <eventCode> -FeedType <feedtype> -CLUserName <yourUserName> [-CLPassword <yourPassword>] [-Since <sinceValue] [-OutFile <explicitFilePath>]`

### Output
Results of execution are written as .json files to a sub-folder in the user's home/documents directory named `CLFeedRes`.  This can be overridden via the `-outfile` parameter.

