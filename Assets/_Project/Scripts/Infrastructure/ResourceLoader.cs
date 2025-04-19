using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure
{
    public class ResourceLoader : IResourceLoader
    {
        public T[] LoadAll<T>(string path) where T : Object
        { 
            return Resources.LoadAll<T>(path);
        }

        public Object Load(string path, Type type)
        {
            return Resources.Load(path, type);
        }
    }
}