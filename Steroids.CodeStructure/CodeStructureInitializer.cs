namespace Steroids.CodeStructure
{
    using System.Windows;
    using Steroids.CodeStructure.Resources;

    public static class CodeStructureInitializer
    {
        public static void Initialize()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ModuleResourceDictionary());
        }
    }
}
