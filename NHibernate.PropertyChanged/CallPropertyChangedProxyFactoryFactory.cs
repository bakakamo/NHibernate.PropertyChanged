namespace NHibernate.PropertyChanged
{
    using NHibernate.Bytecode;
    using NHibernate.Proxy;

    public class CallPropertyChangedProxyFactoryFactory : IProxyFactoryFactory
    {
        #region IProxyFactoryFactory Members

        public IProxyFactory BuildProxyFactory()
        {
            return new PropertyChangedProxyFactory(false);
        }

        public IProxyValidator ProxyValidator
        {
            get { return new PropertyChangedDynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}
