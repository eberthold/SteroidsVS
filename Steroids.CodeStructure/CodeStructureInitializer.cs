namespace Steroids.CodeStructure
{
    using System.Windows;
    using Steroids.CodeStructure.Resources;
    using Steroids.SharedUI;

    public static class CodeStructureInitializer
    {
        public static void Initialize()
        {
            SharedUiInitializer.Initialize();
            Application.Current.Resources.MergedDictionaries.Add(new ModuleResourceDictionary());
        }
    }
}
