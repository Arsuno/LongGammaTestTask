using System;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure
{
    public interface IResourceLoader
    {
        T[] LoadAll<T>(string path) where T : Object;
        Object Load(string path, Type type);
    }
}