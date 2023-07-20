namespace Orc.CsvTextEditor
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Controls;

    public static class ICsvTextEditorInstanceExtensions
    {
        [ObsoleteEx(TreatAsErrorFromVersion = "5.1", RemoveInVersion = "6.0", Message = "Use ShowToolAsync instead")]
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

            Task.Run(async () =>
            {
                if (tool is null)
                {
                    tool = await toolManager.AttachToolAsync(typeof(T)) as T;
                }

                if (tool is not null)
                {
                    await tool.OpenAsync(parameter);
                };
            });
        }

        public static async Task ShowToolAsync<T>(this ICsvTextEditorInstance csvTextEditorInstance, object? parameter = null)
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
                tool = await toolManager.AttachToolAsync(typeof(T)) as T;
            }

            if (tool is not null)
            {
                await tool.OpenAsync(parameter);
            };
        }

        [ObsoleteEx(TreatAsErrorFromVersion = "5.1", RemoveInVersion = "6.0", Message = "Use ShowToolAsync instead")]
        public static void ShowTool(this ICsvTextEditorInstance csvTextEditorInstance, string toolName, object? parameter = null)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var tool = csvTextEditorInstance.GetToolByName(toolName);

            Task.Run(async () =>
            {
                tool?.OpenAsync(parameter);
            });
        }

        public static async Task ShowToolAsync(this ICsvTextEditorInstance csvTextEditorInstance, string toolName, object? parameter = null)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var tool = csvTextEditorInstance.GetToolByName(toolName);

            if (tool is not null)
            {
                await tool.OpenAsync(parameter);
            };
        }

        public static IControlTool? GetToolByName(this ICsvTextEditorInstance csvTextEditorInstance, string toolName)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);

            var tools = csvTextEditorInstance.Tools;
            return tools.FirstOrDefault(x => x.Name == toolName);
        }
    }
}
