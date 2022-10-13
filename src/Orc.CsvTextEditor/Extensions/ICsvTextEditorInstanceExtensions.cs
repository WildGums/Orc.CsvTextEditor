namespace Orc.CsvTextEditor
{
    using System;
    using System.Linq;
    using Controls;

    public static class ICsvTextEditorInstanceExtensions
    {
        public static void ShowTool<T>(this ICsvTextEditorInstance csvTextEditorInstance, object? parameter = null)
            where T : class, IControlTool
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var toolManager = csvTextEditorInstance.ToolManager;
            if (toolManager is null)
            {
                return;
            }

            var tool = csvTextEditorInstance.Tools.OfType<T>().FirstOrDefault();
            if (tool is null)
            {
                tool = toolManager.AttachTool(typeof(T)) as T;
            }

            tool?.Open(parameter);
        }

        public static void ShowTool(this ICsvTextEditorInstance csvTextEditorInstance, string toolName, object? parameter = null)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var tool = csvTextEditorInstance.GetToolByName(toolName);

            tool?.Open(parameter);
        }

        public static IControlTool? GetToolByName(this ICsvTextEditorInstance csvTextEditorInstance, string toolName)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var tools = csvTextEditorInstance.Tools;
            return tools.FirstOrDefault(x => x.Name == toolName);
        }
    }
}
