using System;
using System.Collections.Generic;

public class DependencyContainer
{
    private readonly Dictionary<int, object> _data;

    public DependencyContainer(int dataCapacity)
    {
        _data = new Dictionary<int, object>(dataCapacity, new FastComparable());
    }

    public bool Contains<T>()
    {
        return _data.ContainsKey(typeof(T).GetHashCode());
    }

    public object Get(Type t)
    {
        object resolve;
        _data.TryGetValue(t.GetHashCode(), out resolve);
        return resolve;
    }

    public void Add(object obj)
    {
        _data.Add(obj.GetType().GetHashCode(), obj);
    }

    private void Add(BindLink bindLink)
    {
        _data.Add(bindLink.HashCodeKey, bindLink.Instance);
    }

    public void Remove(object obj)
    {
        _data.Remove(obj.GetType().GetHashCode());
    }

    public T Get<T>()
    {
        object resolve;
        var hasValue = _data.TryGetValue(typeof(T).GetHashCode(), out resolve);
        return hasValue ? (T)resolve : default(T);
    }

    public BindBase Bind<T>() where T: class
    {
        return new BindLink(typeof(T), this);
    }

    private class BindLink : BindBase
    {
        private DependencyContainer _dependencyContainer;

        public object Instance => _instance;
        public int HashCodeKey => _type.GetHashCode();

        public BindLink(Type type, DependencyContainer dependencyContainer) : base(type)
        {
            _dependencyContainer = dependencyContainer;
        }

        public override BindBase FromInstance(object instance)
        {
            base.FromInstance(instance);
            AddToDependencyContainer();
            return this;
        }

        public override BindBase FromNewInstance<T>()
        {
            base.FromNewInstance<T>();
            AddToDependencyContainer();
            return this;
        }

        private void AddToDependencyContainer()
        {
            _dependencyContainer.Add(this);
        }
    }
}

public abstract class BindBase
{
    protected Type _type;
    protected object _instance;

    public BindBase(Type type)
    {
        _type = type;
    }

    public virtual BindBase FromInstance(object instance)
    {
        _instance = instance;
        return this;
    }

    public virtual BindBase FromNewInstance<T>() where T: new()
    {
        _instance = new T();
        return this;
    }
}
