[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETFramework,Version=v4.7", FrameworkDisplayName=".NET Framework 4.7")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/csvtexteditor", "Orc.CsvTextEditor")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.wildgums.com/orc/csvtexteditor", "orccsvtexteditor")]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.CsvTextEditor
{
    public class CaretTextLocationChangedEventArgs : System.EventArgs
    {
        public CaretTextLocationChangedEventArgs(Orc.CsvTextEditor.Location location) { }
        public Orc.CsvTextEditor.Location Location { get; }
    }
    public class Column
    {
        public Column() { }
        public int Index { get; set; }
        public int Offset { get; set; }
        public int Width { get; set; }
    }
    public class CsvColumnCompletionData : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
    {
        public CsvColumnCompletionData(string text) { }
        public object Content { get; }
        public object Description { get; }
        public System.Windows.Media.ImageSource Image { get; }
        public double Priority { get; }
        public string Text { get; }
        public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, System.EventArgs insertionRequestEventArgs) { }
    }
    public class CsvTextEditorControl : Catel.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
    {
        public static readonly System.Windows.DependencyProperty CsvTextEditorInstanceProperty;
        public static readonly System.Windows.DependencyProperty EditorInstanceTypeProperty;
        public static readonly System.Windows.DependencyProperty TextProperty;
        public CsvTextEditorControl() { }
        [Catel.MVVM.Views.ViewToViewModel("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.ViewToViewModel)]
        public Orc.CsvTextEditor.ICsvTextEditorInstance CsvTextEditorInstance { get; set; }
        public System.Type EditorInstanceType { get; set; }
        [Catel.MVVM.Views.ViewToViewModel("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.TwoWayViewWins)]
        public string Text { get; set; }
        public void InitializeComponent() { }
        protected override void OnInitialized(System.EventArgs e) { }
        protected override void OnViewModelChanged() { }
    }
    public class CsvTextEditorInitializer : Orc.CsvTextEditor.ICsvTextEditorInitializer
    {
        public CsvTextEditorInitializer() { }
        public virtual void Initialize(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
    }
    public class CsvTextEditorInstance : Catel.Disposable, Orc.CsvTextEditor.ICsvTextEditorInstance, System.IDisposable
    {
        public CsvTextEditorInstance(ICSharpCode.AvalonEdit.TextEditor textEditor, Catel.MVVM.ICommandManager commandManager, Orc.CsvTextEditor.ICsvTextEditorInitializer initializer, Catel.Services.IDispatcherService dispatcherService, Catel.IoC.ITypeFactory typeFactory) { }
        public bool CanRedo { get; }
        public bool CanUndo { get; }
        public int ColumnsCount { get; }
        public bool HasSelection { get; }
        public bool IsAutocompleteEnabled { get; set; }
        public bool IsDirty { get; }
        public string LineEnding { get; }
        public int LinesCount { get; }
        public System.Collections.Generic.IEnumerable<Orc.Controls.IControlTool> Tools { get; }
        public event System.EventHandler<Orc.CsvTextEditor.CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        public event System.EventHandler<System.EventArgs> TextChanged;
        public void AddTool(Orc.Controls.IControlTool tool) { }
        public void Copy() { }
        public void Cut() { }
        public void DeleteNextSelectedText() { }
        public void DeletePreviousSelectedText() { }
        protected override void DisposeManaged() { }
        public void ExecuteOperation<TOperation>()
            where TOperation : Orc.CsvTextEditor.Operations.IOperation { }
        public Orc.CsvTextEditor.Location GetLocation() { }
        public string GetSelectedText() { }
        public string GetText() { }
        public void GotoPosition(int lineIndex, int columnIndex) { }
        public void Initialize(string text) { }
        public void InsertAtCaret(char character) { }
        public bool IsCaretWithinQuotedField() { }
        public void Paste() { }
        public void Redo() { }
        public void RefreshView() { }
        public void RemoveTool(Orc.Controls.IControlTool tool) { }
        public void ResetIsDirty() { }
        public void SetText(string text) { }
        public void Undo() { }
    }
    [System.Obsolete("Use ControlToolBase instead. Will be removed in version 4.0.0.", true)]
    public abstract class CsvTextEditorToolBase : Orc.Controls.ControlToolBase, Orc.Controls.IControlTool, Orc.CsvTextEditor.ICsvTextEditorTool
    {
        protected CsvTextEditorToolBase(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        protected Orc.CsvTextEditor.ICsvTextEditorInstance CsvTextEditorInstance { get; }
        protected ICSharpCode.AvalonEdit.TextEditor TextEditor { get; }
    }
    public class CsvTextSynchronizationScope : Catel.Disposable
    {
        public CsvTextSynchronizationScope(Orc.CsvTextEditor.CsvTextSynchronizationService csvTextSynchronizationService) { }
        protected override void DisposeManaged() { }
    }
    public class CsvTextSynchronizationService
    {
        public CsvTextSynchronizationService() { }
        public bool IsSynchronizing { get; set; }
        public System.IDisposable SynchronizeInScope() { }
    }
    public class FindReplaceService : Orc.Controls.Services.IFindReplaceService, Orc.CsvTextEditor.IFindReplaceService
    {
        public FindReplaceService(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance = null) { }
        public bool FindNext(string textToFind, Orc.Controls.FindReplaceSettings settings) { }
        [System.Obsolete("Use FindNext with Orc.Controls.FindReplaceSettings parameter instead. Will be rem" +
            "oved in version 4.0.0.", true)]
        public bool FindNext(string textToFind, Orc.CsvTextEditor.FindReplaceSettings settings) { }
        public string GetInitialFindText() { }
        public bool Replace(string textToFind, string textToReplace, Orc.Controls.FindReplaceSettings settings) { }
        [System.Obsolete("Use FindNext with Orc.Controls.FindReplaceSettings parameter instead. Will be rem" +
            "oved in version 4.0.0.", true)]
        public bool Replace(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings) { }
        public void ReplaceAll(string textToFind, string textToReplace, Orc.Controls.FindReplaceSettings settings) { }
        [System.Obsolete("Use FindNext with Orc.Controls.FindReplaceSettings parameter instead. Will be rem" +
            "oved in version 4.0.0.", true)]
        public void ReplaceAll(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings) { }
    }
    [System.Obsolete("Use `Orc.Controls.FindReplaceSettings` instead. Will be removed in version 4.0.0." +
        "", true)]
    public class FindReplaceSettings : Orc.Controls.FindReplaceSettings
    {
        public FindReplaceSettings() { }
    }
    [System.Obsolete("Use `Use Orc.CsvTextEditor.FindReplaceTool instead` instead. Will be removed in v" +
        "ersion 4.0.0.", true)]
    public class FindReplaceTextEditorTool : Orc.CsvTextEditor.CsvTextEditorToolBase
    {
        public FindReplaceTextEditorTool(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, Catel.Services.IUIVisualizerService uiVisualizerService, Catel.IoC.ITypeFactory typeFactory) { }
        public override string Name { get; }
        public override void Close() { }
        protected override void OnOpen(object parameter = null) { }
    }
    public class FindReplaceTool : Orc.Controls.FindReplaceTool<Orc.CsvTextEditor.FindReplaceService>
    {
        public FindReplaceTool(Catel.Services.IUIVisualizerService uiVisualizerService, Catel.IoC.ITypeFactory typeFactory, Catel.IoC.IServiceLocator serviceLocator) { }
        protected override Orc.CsvTextEditor.FindReplaceService CreateFindReplaceService(object target) { }
    }
    public class FirstLineAlwaysBoldTransformer : ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer
    {
        public FirstLineAlwaysBoldTransformer() { }
        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line) { }
    }
    public class HighlightAllOccurencesOfSelectedWordTransformer : ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer
    {
        public HighlightAllOccurencesOfSelectedWordTransformer() { }
        public string SelectedWord { set; }
        public ICSharpCode.AvalonEdit.Editing.Selection Selection { set; }
        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line) { }
    }
    public interface ICsvTextEditorInitializer
    {
        void Initialize(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance);
    }
    public interface ICsvTextEditorInstance : System.IDisposable
    {
        bool CanRedo { get; }
        bool CanUndo { get; }
        int ColumnsCount { get; }
        bool HasSelection { get; }
        bool IsAutocompleteEnabled { get; set; }
        bool IsDirty { get; }
        string LineEnding { get; }
        int LinesCount { get; }
        System.Collections.Generic.IEnumerable<Orc.Controls.IControlTool> Tools { get; }
        event System.EventHandler<Orc.CsvTextEditor.CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        event System.EventHandler<System.EventArgs> TextChanged;
        void AddTool(Orc.Controls.IControlTool tool);
        void Copy();
        void Cut();
        void DeleteNextSelectedText();
        void DeletePreviousSelectedText();
        void ExecuteOperation<TOperation>()
            where TOperation : Orc.CsvTextEditor.Operations.IOperation;
        Orc.CsvTextEditor.Location GetLocation();
        string GetSelectedText();
        string GetText();
        void GotoPosition(int lineIndex, int columnIndex);
        void Initialize(string text);
        void InsertAtCaret(char character);
        bool IsCaretWithinQuotedField();
        void Paste();
        void Redo();
        void RefreshView();
        void RemoveTool(Orc.Controls.IControlTool tool);
        void ResetIsDirty();
        void SetText(string text);
        void Undo();
    }
    public static class ICsvTextEditorInstanceExtensions
    {
        public static Orc.Controls.IControlTool GetToolByName(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, string toolName) { }
        public static void ShowTool(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, string toolName, object parameter = null) { }
        public static void ShowTool<T>(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, object parameter = null)
            where T : Orc.Controls.IControlTool { }
    }
    [System.Obsolete("Use IControlTool instead. Will be removed in version 4.0.0.", true)]
    public interface ICsvTextEditorTool : Orc.Controls.IControlTool { }
    [System.Obsolete("Use `Orc.CsvTextEditor.IFindReplaceService` instead. Will be removed in version 4" +
        ".0.0.", true)]
    public interface IFindReplaceSerivce : Orc.CsvTextEditor.IFindReplaceService { }
    public interface IFindReplaceService
    {
        bool FindNext(string textToFind, Orc.CsvTextEditor.FindReplaceSettings settings);
        bool Replace(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings);
        void ReplaceAll(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings);
    }
    public static class KeyGestureExtensions
    {
        public static bool IsKeyAndModifierEquals(this System.Windows.Input.KeyGesture left, System.Windows.Input.KeyGesture right) { }
    }
    public class Line
    {
        public Line() { }
        public int Index { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
    }
    public class Location
    {
        public Location() { }
        public Orc.CsvTextEditor.Column Column { get; set; }
        public Orc.CsvTextEditor.Line Line { get; set; }
        public int Offset { get; set; }
    }
    public static class LocationExtensions
    {
        public static int GetOffsetInLine(this Orc.CsvTextEditor.Location location) { }
    }
    public static class StringExtensions
    {
        public static string DuplicateTextInLine(this string text, int startOffset, int endOffset, string newLine) { }
        public static string[] GetLines(this string text, out string newLineSymbol) { }
        public static string GetNewLineSymbol(this string text) { }
        public static string GetWordFromOffset(this string text, int offset) { }
        public static string InsertCommaSeparatedColumn(this string text, int column, int linesCount, int columnsCount, string newLine) { }
        public static string InsertLineWithTextTransfer(this string text, int insertLineIndex, int offsetInLine, int columnCount, string lineEnding) { }
        public static bool IsEmptyCommaSeparatedLine(this string textLine) { }
        public static string RemoveCommaSeparatedColumn(this string text, int column, int linesCount, int columnsCount, string newLine) { }
        public static string RemoveCommaSeparatedText(this string text, int positionStart, int length, string newLine) { }
        public static string RemoveEmptyLines(this string text) { }
        public static string RemoveText(this string text, int startOffset, int endOffset, string newLine) { }
        public static string TrimCommaSeparatedValues(this string textLine) { }
        public static string TrimEnd(this string text, string trimString) { }
        public static string Truncate(this string value, int maxLength) { }
    }
    public static class Symbols
    {
        public const char Comma = ',';
        public const char HorizontalTab = '\t';
        public const char NewLineEnd = '\n';
        public const char NewLineStart = '\r';
        public const char Quote = '\"';
        public const char Space = ' ';
        public const char VerticalBar = '|';
    }
    public class TextToTextArrayMultiValueConverter : System.Windows.Data.IMultiValueConverter
    {
        public TextToTextArrayMultiValueConverter() { }
        public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) { }
    }
}
namespace Orc.CsvTextEditor.Operations
{
    public class AddLineOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public AddLineOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class DuplicateLineOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public DuplicateLineOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class GotoNextColumnOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public GotoNextColumnOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class GotoPreviousColumnOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public GotoPreviousColumnOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public interface IOperation
    {
        void Execute();
    }
    public abstract class OperationBase : Orc.CsvTextEditor.Operations.IOperation
    {
        protected readonly Orc.CsvTextEditor.ICsvTextEditorInstance CsvTextEditorInstance;
        protected OperationBase(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public abstract void Execute();
    }
    public class RemoveBlankLinesOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public RemoveBlankLinesOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class RemoveColumnOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public RemoveColumnOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class RemoveDuplicateLinesOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public RemoveDuplicateLinesOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class RemoveLineOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public RemoveLineOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
    public class TrimWhitespacesOperation : Orc.CsvTextEditor.Operations.OperationBase
    {
        public TrimWhitespacesOperation(Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        public override void Execute() { }
    }
}