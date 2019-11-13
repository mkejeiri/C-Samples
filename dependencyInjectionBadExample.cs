namespace ServiceLocatorBuilder
{
    public class ServiceProviderBuilder
    {
        public static IServiceContainer Build()
        {
            var provider = new ServiceContainer();

            provider.Register<ILifeInsuranceService>(() => new LifeInsuranceService());               
            provider.Register<IConfigProvider>(() => new ConfigProvider());
            provider.Register<ILoggerFactory>(() => new NLogLoggerFactory());
            return provider;
        }
    }
}

namespace ServiceLocator
{   
    public interface IServiceContainer
    {
        T Get<T>();
        T SafeGet<T>() where T : class;
    }

    public class ServiceContainer : IServiceContainer
    {
        private ConcurrentDictionary<Type,Func<object>> factories = new ConcurrentDictionary<Type, Func<object>>();
        public void Register<T>(Func<T> functionFactory)
        {
            if (!factories.TryAdd(typeof(T), () => functionFactory()))
            {
                throw new InvalidOperationException("Cannot add service for "+ typeof(T).FullName);
            }
        }
        public T Get<T>()
        {
            Func<object> factory;
            if (!factories.TryGetValue(typeof(T), out factory))
            {
                throw new InvalidOperationException("Unable to get service " + typeof(T).FullName);
            }

            return (T)factory.Invoke();
        }

        public T SafeGet<T>() where T:class
        {
            Func<object> factory;
            if (!factories.TryGetValue(typeof(T), out factory))
            {
                return null;
            }

            return (T)factory.Invoke();
        }
    }

  
public static class ServiceContainerInstance
    {
        private static readonly object Padlock = new object();
        private static IServiceContainer _instance = new ServiceContainer();

        public static T Get<T>()
        {
            return _instance.Get<T>();
        }

        public static T SafeGet<T>() where T:class
        {
            return _instance.SafeGet<T>();
        }

        public static void InitializeIfNotYetSet(Func<IServiceContainer> builder)
        {
            lock (Padlock)
            {
                _instance = builder.Invoke();
            }
        }
    }
}



//app entry point
static App() => ServiceContainerInstance.InitializeIfNotYetSet(ServiceProviderBuilder.Build);


//Used inside Program
{
//Retrieve an instance through 	
var lifeInsuranceService = ServiceContainerInstance.Get<ILifeInsuranceService>();
}
