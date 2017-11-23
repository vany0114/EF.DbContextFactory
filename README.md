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

## The Problem
The Entity Framework DbContext has a well-known problem: it's not thread safe. So it means, you can't got an instance of the same entity class tracked by multiple contexts at the same time. For example, if you have a realtime, collaborative, concurrency or reactive application/scenario, using, for instance, SignalR or multiple threads in background (which are common characteristics in modern applications) I bet you have faced with this kind of exception: 

> ***"The context cannot be used while the model is being created. This exception may be thrown if the context is used inside the OnModelCreating method or if the same context instance is accessed by multiple threads concurrently. Note that instance members of DbContext and related classes are not guaranteed to be thread safe"***

## The Solutions
There are multiple solutions to manage concurrency scenarios from data perspective, the most common patterns are *Pessimistic Concurrency (Locking)* and *Optimistic Concurrency*, actually Entity Framework has an implementation of [Optimistic Concurrency](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application). So these solutions are implemented usually on the database side or even in both, backend and database sides, but the problem with DbContext is that's happening on memory, don't even in database. An approach what allows you to keep your code clean, follow good practices and keep on using Entity Framework and obvoiusly that works fine in multiple threads is injecting a factory in your repositories/unit of work (or whatever you're using it ~~code smell~~) insetead of the instance itself and use it and despose it as soon as possible.

## Key points
* Dispose DbContext immediately.
* Less consume of memory.
* Create the instance and connection database only when you really need it.
* Works in concurrency scenarios.
* Without locking.

## Getting Started

EF.DbContextFactory provides you integration with most popular dependency injection frameworks such as [Unity](https://github.com/unitycontainer/unity), [Ninject](http://www.ninject.org/), [Structuremap](http://structuremap.github.io/) and [.Net Core](https://dotnet.github.io/). So there are for now five Nuget packages listed above that you can use like an extension to inject your DbContext as a factory.

All of nuget packages add an generic extension method to the dependency injection framework container called `AddDbContextFactory`. It needs the derived DbContext Type and as an optional parameter, the name or the connection string itself. ***If you have the default one (DefaultConnection) in the configuration file, you dont need to specify it***

> **EFCore.DbContextFactory** nuget package is slightly different and will be explained later.

### Ninject Asp.Net Mvc and Web Api
If you are using Ninject as DI container you must install [EF.DbContextFactory.Ninject](https://www.nuget.org/packages/EF.DbContextFactory.Ninject/) nuget package. After that you can access to the extension method from the `Kernel` object from Ninject.

```cs
using EF.DbContextFactory.Ninject.Extensions;
.
.
.
kernel.AddDbContextFactory<OrderContext>();
``` 

## Contribution

Your contributions are always welcome, feel free to improve or create new extensions for others dependency injection frameworks! All your work should be done in your forked repository. Once you finish your work, please send a pull request onto dev branch for review.
