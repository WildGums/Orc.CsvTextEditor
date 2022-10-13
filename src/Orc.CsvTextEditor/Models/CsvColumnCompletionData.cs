namespace Orc.CsvTextEditor
{
    using System;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Editing;

    public class CsvColumnCompletionData : ICompletionData
    {
        public CsvColumnCompletionData(string text)
        {
            Text = text;
        }

        public System.Windows.Media.ImageSource? Image => null;

        public string Text { get; }
        public object Content => Text;
        public object Description => Text;
        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            ArgumentNullException.ThrowIfNull(textArea);
            ArgumentNullException.ThrowIfNull(completionSegment);
            ArgumentNullException.ThrowIfNull(insertionRequestEventArgs);

            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
