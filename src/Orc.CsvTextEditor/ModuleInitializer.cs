using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Orc.CsvTextEditor;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterTypeIfNotYetRegistered<ICsvTextEditorInitializer, CsvTextEditorInitializer>();
        // TODO: it should not be registered here
        serviceLocator.RegisterType<ICsvTextEditorInstance, CsvTextEditorInstance>();
        serviceLocator.RegisterType<ICsvTextSynchronizationService, CsvTextSynchronizationService>();
    }
}
