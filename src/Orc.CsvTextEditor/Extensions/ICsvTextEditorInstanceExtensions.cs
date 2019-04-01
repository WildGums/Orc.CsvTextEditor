// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorInstanceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Linq;
    using Catel;
    using Controls;

    public static class ICsvTextEditorInstanceExtensions
    {
        #region Methods
        public static void ShowTool<T>(this ICsvTextEditorInstance csvTextEditorInstance, object parameter = null)
            where T : IControlTool
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tool = csvTextEditorInstance.Tools.OfType<T>().FirstOrDefault();
            tool?.Open(parameter);
        }

        public static void ShowTool(this ICsvTextEditorInstance csvTextEditorInstance, string toolName, object parameter = null)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tool = csvTextEditorInstance.GetToolByName(toolName);

            tool?.Open(parameter);
        }

        public static IControlTool GetToolByName(this ICsvTextEditorInstance csvTextEditorInstance, string toolName)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tools = csvTextEditorInstance.Tools;
            return tools.FirstOrDefault(x => x.Name == toolName);
        }
        #endregion
    }
}
