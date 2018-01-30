using System.Linq;
using System.Windows;
using Steroids.SharedUI.Resources;

namespace Steroids.SharedUI
{
    public static class SharedUiInitializer
    {
        public static void Initialize()
        {
            if (!Application.Current.Resources.MergedDictionaries.OfType<ModuleResourceDictionary>().Any())
            {
                Application.Current.Resources.MergedDictionaries.Add(new ModuleResourceDictionary());
            }
        }
    }
}
