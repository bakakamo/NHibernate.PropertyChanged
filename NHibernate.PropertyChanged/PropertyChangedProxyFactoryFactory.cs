namespace NHibernate.PropertyChanged
{
    using NHibernate.Bytecode;
    using NHibernate.Proxy;

    public class PropertyChangedProxyFactoryFactory : IProxyFactoryFactory
    {
        #region IProxyFactoryFactory Members

        public virtual IProxyFactory BuildProxyFactory()
        {
            return new PropertyChangedProxyFactory(true);
        }

        public virtual IProxyValidator ProxyValidator
        {
            get { return new PropertyChangedDynProxyTypeValidator(); }
        }

        public virtual bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public virtual bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}
