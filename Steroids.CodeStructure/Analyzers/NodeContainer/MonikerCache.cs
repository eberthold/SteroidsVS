namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;

    public static class MonikerCache
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static Dictionary<string, ImageMoniker> _cache = new Dictionary<string, ImageMoniker>();

        public static ImageMoniker GetMoniker(string name)
        {
            try
            {
                _semaphore.Wait();

                if (_cache.ContainsKey(name))
                {
                    return _cache[name];
                }

                var moniker = (ImageMoniker)typeof(KnownMonikers).GetProperty(name).GetValue(null);
                _cache.Add(name, moniker);
                return moniker;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
