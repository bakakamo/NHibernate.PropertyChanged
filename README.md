NHibernate.PropertyChanged
==========================

Tool to add INotifyPropertyChanged support to NHibernate's proxies, enabling their usage with WPF for example.

Two modes are available depending on whether your entity already implement INotifyPropertyChanged or not.

## How to use ##

### Method 1: For entities that already implement INotifyPropertyChanged ###

1. Reference NHibernate.PropertyChanged.dll
2. Configure your session factories with the proxy supplied in the library, **PropertyChangedProxyFactoryFactory**.

For example when using FluentNHibernate:

```csharp
Fluently.Configure()
    .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
    .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Person>()))
    
    // just add this line
    .ProxyFactoryFactory<PropertyChangedProxyFactoryFactory>()
    
    .BuildSessionFactory();
```


### Method 2: For entities that don't implement INotifyPropertyChanged ###

1. Reference NHibernate.PropertyChanged.dll
2. Configure your session factories with the other proxy supplied in the library, **AddPropertyChangedProxyFactoryFactory**.

For example when using FluentNHibernate:

```csharp
Fluently.Configure()
    .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
    .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Person>()))
    
    // just add these two lines
    .ProxyFactoryFactory<AddPropertyChangedProxyFactoryFactory>()
    .ExposeConfiguration(c => c.SetInterceptor(new AddPropertyChangedInterceptor()))
    
    .BuildSessionFactory();
```

Note that with this method, since the entities themselves don't implement INotifyPropertyChanged, when you create a new entity, no PropertyChanged event will be available. To get a new entity that implements it, you will need to use the following factory:
```csharp
var person = AddPropertyChangedInterceptorProxyFactory.Create<Person>();
```

When you save these entities, you will need to specify the entity name:
```csharp
Session.SaveOrUpdate(typeof(Person).FullName, person);
```

Because of these complications, it is recommended to use the first method. If you don't want to implement INotifyPropertyChanged by hand, you can use a tool like [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged "PropertyChanged.Fody").