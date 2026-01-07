namespace Orc
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.CsvTextEditor;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcCsvTextEditorModule
    {
        public static IServiceCollection AddOrcCsvTextEditor(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICsvTextEditorInitializer, CsvTextEditorInitializer>();

            serviceCollection.TryAddTransient<ICsvTextEditorInstanceManager, CsvTextEditorInstanceManager>();
            serviceCollection.TryAddTransient<ICsvTextSynchronizationService, CsvTextSynchronizationService>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.CsvTextEditor", "Orc.CsvTextEditor.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.CsvTextEditor", "https://github.com/wildgums/orc.csv"));
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new ResourceBasedThirdPartyNotice("AvalonEdit", "https://github.com/icsharpcode/AvalonEdit", "Orc.CsvTextEditor", "Orc.CsvTextEditor", "Resources.ThirdPartyNotices.csvhelper.txt"));

            return serviceCollection;
        }
    }
}
