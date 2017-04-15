# DC.Build.Tasks
MSBuild task **GetCurrentBuildVersion** to auto generate version number in .NET Core projects (.csproj).

In new .NET Core .csproj files (VS 2017) we can no longer use **AssemblyVersion** attribute (in *AssemblyInfo.cs*) with star (*) to auto-generate version number:

```csharp
[assembly: System.Reflection.AssemblyVersion("1.0.*")]
```
> Error CS7034: The specified version string does not conform to the required format - major[.minor[.build[.revision]]]

I came up with solution that worked almost the same as old AssemblyVersion attribute.

Values for *AssemblyVersion* and *AssemblyFileVersion* is in MSBuild project **.csproj** file (not in *AssemblyInfo.cs*) as property **FileVersion** (generates *AssemblyFileVersionAttribute*) and **AssemblyVersion** (generates *AssemblyVersionAttribute*).
In MSBuild process we use our custom MSBuild task to generate version numbers and then we override values of these **FileVersion** and **AssemblyVersion** properties with new values from task.

## MSBuild task GetCurrentBuildVersion

MSBuild task is in **DC.Build.Tasks.dll** which is **.NET Standard 1.3 class library**.
In this library is Task class named **GetCurrentBuildVersion** which inherit from **Microsoft.Build.Utilities.Task** class from **Microsoft.Build.Utilities.Core** NuGet package.
It takes **BaseVersion** property (optional) on input and returns generated version in **Version** output property.
The logic to get version numbers is same as .NET automatic versioning (**Build number** is days count since 1/1/2000 and **Revision** is half seconds since midnight).

## Usage
Setup MSBuild in your **.csproj** project file to use **GetCurrentBuildVersion** task and set **FileVersion** and **AssemblyVersion** properties.

In .csproj file it looks like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <UsingTask TaskName="GetCurrentBuildVersion" AssemblyFile="$(MSBuildThisFileFullPath)\..\..\DC.Build.Tasks.dll" />
 
  <PropertyGroup>
    ...
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
  </PropertyGroup>
 
  ...
 
  <Target Name="BeforeBuildActionsProject1" BeforeTargets="BeforeBuild">
    <GetCurrentBuildVersion BaseVersion="$(FileVersion)">
      <Output TaskParameter="Version" PropertyName="FileVersion" />
    </GetCurrentBuildVersion>
    <PropertyGroup>
      <AssemblyVersion>$(FileVersion)</AssemblyVersion>
    </PropertyGroup>
  </Target>
 
</Project>
```


**Importtant things here:**

- Mentioned **UsingTask** imports GetCurrentBuildVersion task from **DC.Build.Tasks.dll**. It assumes that this dll file is located on parent directory from your .csproj file.
- Our **BeforeBuildActionsProject1** Target that calls task must have unique name per project in case we have more projects in the solution which calls GetCurrentBuildVersion task.
