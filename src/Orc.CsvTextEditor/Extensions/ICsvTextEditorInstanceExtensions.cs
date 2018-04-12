// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvTextEditorInstanceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Linq;
    using Catel;

    public static class ICsvTextEditorInstanceExtensions
    {
        public static void ShowTool<T>(this ICsvTextEditorInstance csvTextEditorInstance)
            where T : ICsvTextEditorTool
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tool = csvTextEditorInstance.Tools.OfType<T>().FirstOrDefault();
            tool?.Open();
        }

        public static void ShowTool(this ICsvTextEditorInstance csvTextEditorInstance, string toolName)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tool = csvTextEditorInstance.GetToolByName(toolName);

            tool?.Open();
        }

        public static ICsvTextEditorTool GetToolByName(this ICsvTextEditorInstance csvTextEditorInstance, string toolName)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);

            var tools = csvTextEditorInstance.Tools;
            return tools.FirstOrDefault(x => x.Name == toolName);
        }
    }
}