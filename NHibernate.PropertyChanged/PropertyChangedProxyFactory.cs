namespace NHibernate.PropertyChanged
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Iesi.Collections.Generic;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Intercept;
    using NHibernate.Proxy;
    using NHibernate.Proxy.DynamicProxy;

    public class PropertyChangedProxyFactory : AbstractProxyFactory
    {
        protected static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(PropertyChangedProxyFactory));
        private readonly bool _entitiesHandlePropertyChanged;
        private readonly ProxyFactory _factory = new ProxyFactory();
        private Type[] _interfaces;

        public PropertyChangedProxyFactory(bool entitiesHandlePropertyChanged)
        {
            _entitiesHandlePropertyChanged = entitiesHandlePropertyChanged;
        }

        public override void PostInstantiate(string entityName, Type persistentClass, ISet<Type> interfaces, System.Reflection.MethodInfo getIdentifierMethod, System.Reflection.MethodInfo setIdentifierMethod, NHibernate.Type.IAbstractComponentType componentIdType)
        {
            base.PostInstantiate(entityName, persistentClass, interfaces, getIdentifierMethod, setIdentifierMethod, componentIdType);
            _interfaces = Interfaces;
            if (!_entitiesHandlePropertyChanged)
            {
                _interfaces = Interfaces.Concat(new[] { typeof(INotifyPropertyChanged) }).ToArray();
            }
        }

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var initializer = CreatePropertyChangedLazyInitializer(id, session);

                object proxyInstance = IsClassProxy
                    ? _factory.CreateProxy(PersistentClass, initializer, _interfaces)
                    : _factory.CreateProxy(_interfaces[0], initializer, _interfaces);

                return (INHibernateProxy)proxyInstance;
            }
            catch (Exception ex)
            {
                _log.Error("Creating a proxy instance failed", ex);
                throw new HibernateException("Creating a proxy instance failed", ex);
            }
        }

        public override object GetFieldInterceptionProxy(object instanceToWrap)
        {
            var interceptor = new DefaultDynamicLazyFieldInterceptor(instanceToWrap);
            return _factory.CreateProxy(PersistentClass, interceptor, new[] { typeof(IFieldInterceptorAccessor) });
        }

        protected virtual PropertyChangedLazyInitializer CreatePropertyChangedLazyInitializer(object id, ISessionImplementor session)
        {
            return new PropertyChangedLazyInitializer(
                EntityName,
                PersistentClass,
                id,
                GetIdentifierMethod,
                SetIdentifierMethod,
                ComponentIdType,
                session,
                _entitiesHandlePropertyChanged);
        }
    }
}