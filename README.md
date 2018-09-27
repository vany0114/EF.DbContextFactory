# EF.DbContextFactory
<!--- [![Visual Studio Team services](https://ef-dbcontextfactory.visualstudio.com/_apis/public/build/definitions/052e796b-6387-4c07-9c1e-2b471f0ea629/1/badge)](https://ef-dbcontextfactory.visualstudio.com) --->
[![Appveyor](https://ci.appveyor.com/api/projects/status/610ww7c83vqvxa6c?svg=true)](https://ci.appveyor.com/project/vany0114/ef-dbcontextfactory)
[![GitHub Issues](https://img.shields.io/github/issues/vany0114/EF.DbContextFactory.svg)](https://github.com/vany0114/EF.DbContextFactory/issues)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/vany0114/EF.DbContextFactory/blob/master/LICENSE)
[![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=With+EF.DbContextFactory+you+can+resolve+easily+your+DbContext+dependencies+in+a+safe+way+injecting+a+factory+instead+of+an+instance+itself%2C+enabling+you+to+work+in+multi-thread+contexts+with+Entity+Framework+or+just+work+safest+with+DbContext+following+the+Microsoft+recommendations+about+the+DbContext+lifetime.&url=https://github.com/vany0114/EF.DbContextFactory&hashtags=EF.DbContextFactory,EntityFramework,DbContext,Thread-Safe)


With EF.DbContextFactory you can resolve easily your DbContext dependencies in a safe way injecting a factory instead of an instance itself, enabling you to work in [multi-thread contexts](https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx?f=255&mspperror=-2147217396#Anchor_3) with Entity Framework or just work safest with DbContext following the Microsoft recommendations about the [DbContext lifecycle](https://msdn.microsoft.com/en-us/library/jj729737(v=vs.113).aspx?f=255&mspperror=-2147217396#Anchor_1) but keeping your code clean and testable using dependency injection pattern.

## Packages Status ##

|<span>   |   |   |
|---|---|---|
|[EFCore.DbContextFactory](#aspnet-core)   |[![](https://img.shields.io/nuget/v/EFCore.DbContextFactory.svg)](https://www.nuget.org/packages/EFCore.DbContextFactory/)   |[![NuGet](https://img.shields.io/nuget/dt/EFCore.DbContextFactory.svg)](https://www.nuget.org/packages/EFCore.DbContextFactory/)   |
|[EF.DbContextFactory.Unity](#unity-aspnet-mvc-and-web-api)   |[![](https://img.shields.io/nuget/v/EF.DbContextFactory.Unity.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Unity/)   |[![NuGet](https://img.shields.io/nuget/dt/EF.DbContextFactory.Unity.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Unity/)   |
|[EF.DbContextFactory.Ninject](#ninject-aspnet-mvc-and-web-api)   |[![](https://img.shields.io/nuget/v/EF.DbContextFactory.Ninject.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Ninject/)   |[![NuGet](https://img.shields.io/nuget/dt/EF.DbContextFactory.Ninject.svg)](https://www.nuget.org/packages/EF.DbContextFactory.Ninject/)   |
|[EF.DbContextFactory.StructureMap](#structuremap-aspnet-mvc-and-web-api)   |[![](https://img.shields.io/nuget/v/EF.DbContextFactory.StructureMap.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap/)   |[![NuGet](https://img.shields.io/nuget/dt/EF.DbContextFactory.StructureMap.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap/)   |
|[EF.DbContextFactory.StructureMap.WebApi](#structuremap-410361-aspnet-mvc-and-web-api-or-webapistructuremap)   |[![](https://img.shields.io/nuget/v/EF.DbContextFactory.StructureMap.WebApi.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap.WebApi/)   |[![NuGet](https://img.shields.io/nuget/dt/EF.DbContextFactory.StructureMap.WebApi.svg)](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap.WebApi/)   |
|[EF.DbContextFactory.SimpleInjector](#simpleinjector-aspnet-mvc-and-web-api)   |[![](https://img.shields.io/nuget/v/EF.DbContextFactory.SimpleInjector.svg)](https://www.nuget.org/packages/EF.DbContextFactory.SimpleInjector/)   |[![NuGet](https://img.shields.io/nuget/dt/EF.DbContextFactory.SimpleInjector.svg)](https://www.nuget.org/packages/EF.DbContextFactory.SimpleInjector/)   |
    
## The Problem
The Entity Framework DbContext has a well-known problem: it’s not thread safe. So it means, you can’t get an instance of the same entity class tracked by multiple contexts at the same time. For example, if you have a realtime, collaborative, concurrency or reactive application/scenario, using, for instance, SignalR or multiple threads in background (which are common characteristics in modern applications). I bet you have faced this kind of exception:

> ***"The context cannot be used while the model is being created. This exception may be thrown if the context is used inside the OnModelCreating method or if the same context instance is accessed by multiple threads concurrently. Note that instance members of DbContext and related classes are not guaranteed to be thread safe"***

## The Solutions
There are multiple solutions to manage concurrency scenarios from data perspective, the most common patterns are *Pessimistic Concurrency (Locking)* and *Optimistic Concurrency*, actually Entity Framework has an implementation of [Optimistic Concurrency](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application). So these solutions are implemented usually on the database side or even in both, backend and database sides, but the problem with DbContext is that's happening on memory, don't even in database. An approach what allows you to keep your code clean, follow good practices and keep on using Entity Framework and obvoiusly that works fine in multiple threads is injecting a factory in your repositories/unit of work (or whatever you're using it ~~code smell~~) instead of the instance itself and use it and dispose it as soon as possible.

## Key points
* Dispose DbContext immediately.
* Less consume of memory.
* Create the instance and connection database only when you really need it.
* Works in concurrency scenarios.
* Without locking.

## Getting Started

EF.DbContextFactory provides you integration with most popular dependency injection frameworks such as [Unity](https://github.com/unitycontainer/unity), [Ninject](http://www.ninject.org/), [Structuremap](http://structuremap.github.io/) and [.Net Core](https://dotnet.github.io/). So there five Nuget packages so far listed above that you can use like an extension to inject your DbContext as a factory.

All of nuget packages add a generic extension method to the dependency injection framework container called `AddDbContextFactory`. It needs the derived DbContext Type and as an optional parameter, the name or the connection string itself. ***If you have the default one (DefaultConnection) in the configuration file, you dont need to specify it***

> **EFCore.DbContextFactory** nuget package is slightly different and will be explained later.

The other thing what you need is to inject your DbContext as a factory instead of the instance itself:

```cs
public class OrderRepositoryWithFactory : IOrderRepository
{
    private readonly Func<OrderContext> _factory;

    public OrderRepositoryWithFactory(Func<OrderContext> factory)
    {
        _factory = factory;
    }
    .
    .
    .
}
``` 

And then just use it when you need it executing the factory, you can do that with the `Invoke` method or implicitly just using the parentheses and that's it!

```cs
public class OrderRepositoryWithFactory : IOrderRepository
{
    .
    .
    .
    public void Add(Order order)
    {
        using (var context = _factory.Invoke())
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }
    }
    
    public void DeleteById(Guid id)
    {
        // implicit way way
        using (var context = _factory())
        {
            var order = context.Orders.FirstOrDefault(x => x.Id == id);
            context.Entry(order).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}
``` 

### Ninject Asp.Net Mvc and Web Api
If you are using Ninject as DI container into your Asp.Net Mvc or Web Api project you must install [EF.DbContextFactory.Ninject](https://www.nuget.org/packages/EF.DbContextFactory.Ninject/) nuget package. After that, you are able to access to the extension method from the `Kernel` object from Ninject.

```cs
using EF.DbContextFactory.Ninject.Extensions;
.
.
.
kernel.AddDbContextFactory<OrderContext>();
``` 

### StructureMap Asp.Net Mvc and Web Api
If you are using StructureMap as DI container into your Asp.Net Mvc or Web Api project you must install [EF.DbContextFactory.StructureMap](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap/) nuget package. After that, you are able to access the extension method from the `Registry` object from StructureMap.

```cs
using EF.DbContextFactory.StructureMap.Extensions;
.
.
.
this.AddDbContextFactory<OrderContext>();
``` 

### StructureMap 4.1.0.361 Asp.Net Mvc and Web Api or WebApi.StructureMap
If you are using StructureMap >= `4.1.0.361` as DI container or or WebApi.StructureMap for Web Api projects you must install [EF.DbContextFactory.StructureMap.WebApi](https://www.nuget.org/packages/EF.DbContextFactory.StructureMap.WebApi/) nuget package. After that, you are able to access the extension method from the `Registry` object from StructureMap. (In my opinion this StructureMap version is cleaner)

```cs
using EF.DbContextFactory.StructureMap.WebApi.Extensions;
.
.
.
this.AddDbContextFactory<OrderContext>();
``` 

### Unity Asp.Net Mvc and Web Api
If you are using Unity as DI container into your Asp.Net Mvc or Web Api project you must install [EF.DbContextFactory.Unity](https://www.nuget.org/packages/EF.DbContextFactory.Unity/) nuget package. After that, you are able to access the extension method from the `UnityContainer` object from Unity.

```cs
using EF.DbContextFactory.Unity.Extensions;
.
.
.
container.AddDbContextFactory<OrderContext>();
``` 

### SimpleInjector Asp.Net Mvc and Web Api
If you are using SimpleInjector as DI container into your Asp.Net Mvc or Web Api project you must install [EF.DbContextFactory.SimpleInjector](https://www.nuget.org/packages/EF.DbContextFactory.SimpleInjector/) nuget package. After that, you are able to access the extension method from the `Container` object from SimpleInjector.

```cs
using EF.DbContextFactory.SimpleInjector.Extensions;
.
.
.
container.AddDbContextFactory<OrderContext>();
``` 

### Asp.Net Core
If you are working with Asp.Net Core you probably know that it brings its own Dependency Injection container, so you don't need to install another package or framework to deal with it. So you only need to install [EFCore.DbContextFactory](https://www.nuget.org/packages/EFCore.DbContextFactory/) nuget package. After that, you are able to access to the extension method from the `ServiceCollection` object from Asp.Net Core. 

>EFCore.DbContextFactory is supported from .Net Core 2.0.

The easiest way to resolve your DbContext factory is using the extension method called `AddSqlServerDbContextFactory`. It automatically configures your DbContext to use SqlServer and you can pass it optionally  the name or the connection string itself ***If you have the default one (DefaultConnection) in the configuration file, you dont need to specify it*** and your `ILoggerFactory`, if you want.

```cs
using EFCore.DbContextFactory.Extensions;
.
.
.
services.AddSqlServerDbContextFactory<OrderContext>();
``` 

Also you can use the known method `AddDbContextFactory` with the difference that it receives the `DbContextOptionsBuilder` object so you’re able to build your DbContext as you need.

```cs
var dbLogger = new LoggerFactory(new[]
{
    new ConsoleLoggerProvider((category, level)
        => category == DbLoggerCategory.Database.Command.Name
           && level == LogLevel.Information, true)
});

// ************************************sql server**********************************************
// this is like if you had called the AddSqlServerDbContextFactory method.
services.AddDbContextFactory<OrderContext>(builder => builder
    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
    .UseLoggerFactory(dbLogger));

services.AddDbContextFactory<OrderContext>((provider, builder) => builder
    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
// ************************************sqlite**************************************************
services.AddDbContextFactory<OrderContext>(builder => builder
    .UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
    .UseLoggerFactory(dbLogger));

// ************************************in memory***********************************************
services.AddDbContextFactory<OrderContext>(builder => builder
    .UseInMemoryDatabase("OrdersExample")
    .UseLoggerFactory(dbLogger));
``` 

## Examples :metal:

You can find the examples in this repository and you can see the examples with Ninject, Structuremap, Structuremap.WebApi, Unity and Asp.Net Core, all you need is to run the migrations and that's it. Every example project has two controllers, one to receive a repository that implements the DbContextFactory and another one that doesn't, and every one creates and deletes orders at the same time in different threads to simulate the concurrency. So you can see how the one that doesn't implement the DbContextFactory throws errors related to concurrency issues.

![](example.gif)

## Contribution :heart: :muscle:

Your contributions are always welcome, feel free to improve it or create new extensions for others dependency injection frameworks! All your work should be done in your forked repository. Once you finish your work, please send a pull request onto dev branch for review.
 
Visit my blog <http://elvanydev.com/EF-DbContextFactory/> to view the whole post and to know the motivation for this project!
 
