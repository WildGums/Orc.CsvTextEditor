[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://schemas.wildgums.com/orc/csvtexteditor", "Orc.CsvTextEditor")]
[assembly: System.Windows.Markup.XmlnsPrefixAttribute("http://schemas.wildgums.com/orc/csvtexteditor", "orccsvtexteditor")]
[assembly: System.Windows.ThemeInfoAttribute(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public class static ModuleInitializer
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
        public int Index;
        public int Offset;
        public int Width;
        public Column() { }
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
        public static readonly System.Windows.DependencyProperty ScopeProperty;
        public static readonly System.Windows.DependencyProperty TextProperty;
        public CsvTextEditorControl() { }
        [Catel.MVVM.Views.ViewToViewModelAttribute("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.ViewToViewModel)]
        public object Scope { get; set; }
        [Catel.MVVM.Views.ViewToViewModelAttribute("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.TwoWayViewWins)]
        public string Text { get; set; }
        public void InitializeComponent() { }
    }
    public class CsvTextEditorInitializer : Orc.CsvTextEditor.ICsvTextEditorInitializer
    {
        public CsvTextEditorInitializer(Catel.IoC.ITypeFactory typeFactory) { }
        public virtual void Initialize(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
    }
    public abstract class CsvTextEditorToolBase : Orc.CsvTextEditor.ICsvTextEditorTool
    {
        public CsvTextEditorToolBase(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance) { }
        protected Orc.CsvTextEditor.ICsvTextEditorInstance CsvTextEditorInstance { get; }
        public bool IsOpened { get; }
        public abstract string Name { get; }
        protected ICSharpCode.AvalonEdit.TextEditor TextEditor { get; }
        public event System.EventHandler<System.EventArgs> Closed;
        public event System.EventHandler<System.EventArgs> Opened;
        public virtual void Close() { }
        protected abstract void OnOpen();
        public void Open() { }
    }
    public class CsvTextSynchronizationScope : System.IDisposable
    {
        public CsvTextSynchronizationScope(Orc.CsvTextEditor.CsvTextSynchronizationService csvTextSynchronizationService) { }
        public void Dispose() { }
    }
    public class CsvTextSynchronizationService
    {
        public CsvTextSynchronizationService() { }
        public bool IsSynchronizing { get; set; }
        public System.IDisposable SynchronizeInScope() { }
    }
    public class FindReplaceService : Orc.CsvTextEditor.IFindReplaceSerivce
    {
        public FindReplaceService(ICSharpCode.AvalonEdit.TextEditor textEditor) { }
        public bool FindNext(string textToFind, Orc.CsvTextEditor.FindReplaceSettings settings) { }
        public bool Replace(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings) { }
        public void ReplaceAll(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings) { }
    }
    public class FindReplaceSettings : Catel.Data.ModelBase
    {
        public static readonly Catel.Data.PropertyData CaseSensitiveProperty;
        public static readonly Catel.Data.PropertyData IsSearchUpProperty;
        public static readonly Catel.Data.PropertyData UseRegexProperty;
        public static readonly Catel.Data.PropertyData UseWildcardsProperty;
        public static readonly Catel.Data.PropertyData WholeWordProperty;
        public FindReplaceSettings() { }
        public bool CaseSensitive { get; set; }
        public bool IsSearchUp { get; set; }
        public bool UseRegex { get; set; }
        public bool UseWildcards { get; set; }
        public bool WholeWord { get; set; }
    }
    public class FindReplaceTextEditorTool : Orc.CsvTextEditor.CsvTextEditorToolBase
    {
        public FindReplaceTextEditorTool(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, Catel.Services.IUIVisualizerService uiVisualizerService, Catel.IoC.ITypeFactory typeFactory) { }
        public override string Name { get; }
        public override void Close() { }
        protected override void OnOpen() { }
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
        void Initialize(ICSharpCode.AvalonEdit.TextEditor textEditor, Orc.CsvTextEditor.ICsvTextEditorInstance textEditorInstance);
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
        System.Collections.Generic.IEnumerable<Orc.CsvTextEditor.ICsvTextEditorTool> Tools { get; }
        public event System.EventHandler<Orc.CsvTextEditor.CaretTextLocationChangedEventArgs> CaretTextLocationChanged;
        public event System.EventHandler<System.EventArgs> TextChanged;
        void AddTool(Orc.CsvTextEditor.ICsvTextEditorTool tool);
        void Copy();
        void Cut();
        void DeleteNextSelectedText();
        void DeletePreviousSelectedText();
        void ExecuteOperation<TOperation>()
            where TOperation : Orc.CsvTextEditor.Operations.IOperation;
        Orc.CsvTextEditor.Location GetLocation();
        string GetText();
        void GotoPosition(int lineIndex, int columnIndex);
        void Initialize(string text);
        void Paste();
        void Redo();
        void RefreshView();
        void RemoveTool(Orc.CsvTextEditor.ICsvTextEditorTool tool);
        void ResetIsDirty();
        void SetText(string text);
        void Undo();
    }
    public class static ICsvTextEditorInstanceExtensions
    {
        public static Orc.CsvTextEditor.ICsvTextEditorTool GetToolByName(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, string toolName) { }
        public static void ShowTool<T>(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance)
            where T : Orc.CsvTextEditor.ICsvTextEditorTool { }
        public static void ShowTool(this Orc.CsvTextEditor.ICsvTextEditorInstance csvTextEditorInstance, string toolName) { }
    }
    public interface ICsvTextEditorTool
    {
        bool IsOpened { get; }
        string Name { get; }
        public event System.EventHandler<System.EventArgs> Closed;
        public event System.EventHandler<System.EventArgs> Opened;
        void Close();
        void Open();
    }
    public interface IFindReplaceSerivce
    {
        bool FindNext(string textToFind, Orc.CsvTextEditor.FindReplaceSettings settings);
        bool Replace(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings);
        void ReplaceAll(string textToFind, string textToReplace, Orc.CsvTextEditor.FindReplaceSettings settings);
    }
    public class static KeyGestureExtensions
    {
        public static bool IsKeyAndModifierEquals(this System.Windows.Input.KeyGesture left, System.Windows.Input.KeyGesture right) { }
    }
    public class Line
    {
        public int Index;
        public int Length;
        public int Offset;
        public Line() { }
    }
    public class Location
    {
        public Location() { }
        public Orc.CsvTextEditor.Column Column { get; set; }
        public Orc.CsvTextEditor.Line Line { get; set; }
        public int Offset { get; set; }
    }
    public class static LocationExtensions
    {
        public static int GetOffsetInLine(this Orc.CsvTextEditor.Location location) { }
    }
    public class static StringExtensions
    {
        public static string DuplicateTextInLine(this string text, int startOffset, int endOffset, string newLine) { }
        public static string[] GetLines(this string text, out string newLineSymbol) { }
        public static string GetNewLineSymbol(this string text) { }
        public static string GetWordFromOffset(this string text, int offset) { }
        public static string InsertCommaSeparatedColumn(this string text, int column, int linesCount, int columnsCount, string newLine) { }
        public static string InsertLineWithTextTransfer(this string text, int insertLineIndex, int offsetInLine, int columnCount, string lineEnding) { }
        public static bool IsEmptyCommaSeparatedLine(this string textLine) { }
        public static string RemoveCommaSeparatedColumn(this string text, int column, int linesCount, int columnsCount, string newLine) { }
        public static string RemoveCommaSeparatedText(this string text, int positionStart, int lenght, string newLine) { }
        public static string RemoveText(this string text, int startOffset, int endOffset, string newLine) { }
        public static string TrimCommaSeparatedValues(this string textLine) { }
        public static string TrimEnd(this string text, string trimString) { }
    }
    public class static Symbols
    {
        public const char Comma = ',';
        public const char HorizontalTab = '\t';
        public const char NewLineEnd = '\n';
        public const char NewLineStart = '\r';
        public const char Quote = '\"';
        public const char Space = ' ';
        public const char VerticalBar = '|';
    }
    public class static TextEditorExtensions
    {
        public static System.Collections.Generic.IList<ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData> GetCompletionDataForText(this ICSharpCode.AvalonEdit.TextEditor textEditor, string autocompletionText, int columnIndex, int[][] scheme) { }
        public static void SetCaretToSpecificLineAndColumn(this ICSharpCode.AvalonEdit.TextEditor textEditor, int lineIndex, int columnIndex, int[][] columnWidthByLine) { }
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
        protected readonly Orc.CsvTextEditor.ICsvTextEditorInstance _csvTextEditorInstance;
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
namespace XamlGeneratedNamespace
{
    public sealed class GeneratedInternalTypeHelper : System.Windows.Markup.InternalTypeHelper
    {
        public GeneratedInternalTypeHelper() { }
        protected override void AddEventHandler(System.Reflection.EventInfo eventInfo, object target, System.Delegate handler) { }
        protected override System.Delegate CreateDelegate(System.Type delegateType, object target, string handler) { }
        protected override object CreateInstance(System.Type type, System.Globalization.CultureInfo culture) { }
        protected override object GetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, System.Globalization.CultureInfo culture) { }
        protected override void SetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, object value, System.Globalization.CultureInfo culture) { }
    }
}