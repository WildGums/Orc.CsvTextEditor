// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Orc.CsvTextEditor;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    #region Methods
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterTypeIfNotYetRegistered<ICsvTextEditorServiceInitializer, CsvTextEditorServiceInitializer>();
        serviceLocator.RegisterType<ICsvTextEditorService, CsvTextEditorService>();
        serviceLocator.RegisterType<ICsvTextSynchronizationService, CsvTextSynchronizationService>();

        var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
        viewModelLocator.Register<CsvTextEditorControl, CsvTextEditorControlViewModel>();
    }
    #endregion
}