# EF.DbContextFactory
With EF.DbContextFactory you can resolve easily your DbContext dependencies in a safe way injecting a factory instead of an instance itself, enabling you to work in [multi-thread contexts](https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx?f=255&mspperror=-2147217396#Anchor_3) with Entity Framework or just work safest with DbContext following the Microsoft recommendations about the [DbContext lifecycle](https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx?f=255&mspperror=-2147217396#Anchor_1) but keeping your code clean and testable using dependeny injection pattern.

## Build Status ##
[![Visual Studio Team services](https://ef-dbcontextfactory.visualstudio.com/_apis/public/build/definitions/052e796b-6387-4c07-9c1e-2b471f0ea629/1/badge)](https://ef-dbcontextfactory.visualstudio.com)

## Packages Status ##

* EFCore.DbContextFactory
[![](https://img.shields.io/nuget/v/EFCore.DbContextFactory.svg)](https://www.nuget.org/packages/EFCore.DbContextFactory/)
* EF.DbContextFactory.Unity
[![](https://img.shields.io/nuget/v/EF.DbContextFactory.Unity.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Unity/)
* EF.DbContextFactory.Ninject
[![](https://img.shields.io/nuget/v/EF.DbContextFactory.Ninject.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Ninject/)
* EF.DbContextFactory.StructureMap
[![](https://img.shields.io/nuget/v/EF.DbContextFactory.StructureMap.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap/)
* EF.DbContextFactory.StructureMap.WebApi
[![](https://img.shields.io/nuget/v/EF.DbContextFactory.StructureMap.WebApi.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap.WebApi/)

## Getting Started

EF.DbContextFactory provides you integration with most popular dependency injection frameworks such as [Unity](https://github.com/unitycontainer/unity), [Ninject](http://www.ninject.org/), [Structuremap](http://structuremap.github.io/) and [.Net Core](https://dotnet.github.io/).
