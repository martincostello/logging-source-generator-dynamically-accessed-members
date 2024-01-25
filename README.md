# Logging Source Generator with [DynamicallyAccessedMembers]

This project reproduces [dotnet/runtime#97498](https://github.com/dotnet/runtime/issues/97498).

To reprodue the issue run `dotnet build` in the root of the repository.

The build will fail with the following error:

```console
❯ dotnet build
MSBuild version 17.8.3+195e7f5a3 for .NET
  Determining projects to restore...
  Restored C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj (in 294 ms).
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,30): error CS0246: The type or namespace name 'DynamicallyAccessedMembersAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,30): error CS0246: The type or namespace name 'DynamicallyAccessedMembers' could not be found (are you missing a using directive or an assembly reference?) [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,57): error CS0103: The name 'DynamicallyAccessedMemberTypes' does not exist in the current context [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]

Build FAILED.

C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,30): error CS0246: The type or namespace name 'DynamicallyAccessedMembersAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,30): error CS0246: The type or namespace name 'DynamicallyAccessedMembers' could not be found (are you missing a using directive or an assembly reference?) [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Microsoft.Extensions.Logging.Generators\Microsoft.Extensions.Logging.Generators.LoggerMessageGenerator\LoggerMessage.g.cs(4,57): error CS0103: The name 'DynamicallyAccessedMemberTypes' does not exist in the current context [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
    0 Warning(s)
    3 Error(s)

Time Elapsed 00:00:01.55
```

The intended behaviour can be produced by removing the `[DynamicallyAccessedMembers]` attribute
but which will produce the following warning from `dotnet publish`:

```console
❯ dotnet publish
MSBuild version 17.8.3+195e7f5a3 for .NET
  Determining projects to restore...
  Restored C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj (in 288 ms).
  Repro -> C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\bin\Release\net8.0\win-x64\Repro.dll
  Generating native code
C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Program.cs(68): Trim analysis warning IL2075: Describer`1.Describe(!0): 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicProperties' in call to 'System.Type.GetProperties()'. The return value of method 'System.Object.GetType()' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to. [C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\Repro.csproj]
  Repro -> C:\Coding\martincostello\logging-source-generator-dynamically-accessed-members\bin\Release\net8.0\win-x64\publish\
```
