namespace Orc.CsvTextEditor
{
    using System;
    using System.Text.RegularExpressions;
    using Controls;
    using ICSharpCode.AvalonEdit;

    public class FindReplaceService : Controls.Services.IFindReplaceService
    {
        private readonly ICsvTextEditorInstance _csvTextEditorInstance;
        private readonly TextEditor _textEditor;

        public FindReplaceService(TextEditor textEditor, ICsvTextEditorInstance csvTextEditorInstance)
        {
            ArgumentNullException.ThrowIfNull(csvTextEditorInstance);
            ArgumentNullException.ThrowIfNull(textEditor);

            _csvTextEditorInstance = csvTextEditorInstance;
            _textEditor = textEditor;
        }

        public string GetInitialFindText()
        {
            return _csvTextEditorInstance?.GetSelectedText().Truncate(20) ?? string.Empty;
        }

        public bool FindNext(string textToFind, Controls.FindReplaceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(textToFind);
            ArgumentNullException.ThrowIfNull(settings);

            var regex = settings.GetRegEx(textToFind);
            var start = regex.Options.HasFlag(RegexOptions.RightToLeft) ? _textEditor.SelectionStart : _textEditor.SelectionStart + _textEditor.SelectionLength;
            var match = regex.Match(_textEditor.Text, start);

            if (!match.Success) // start again from beginning or end
            {
                match = regex.Match(_textEditor.Text, regex.Options.HasFlag(RegexOptions.RightToLeft) ? _textEditor.Text.Length : 0);
            }

            if (!match.Success)
            {
                return false;
            }

            _textEditor.Select(match.Index, match.Length);
            var loc = _textEditor.Document.GetLocation(match.Index);
            _textEditor.ScrollTo(loc.Line, loc.Column);

            return match.Success;
        }

        public void FindAll(string textToFind, FindReplaceSettings settings)
        {
            //TODO
        }

        public bool Replace(string textToFind, string textToReplace, FindReplaceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(textToFind);
            ArgumentNullException.ThrowIfNull(textToReplace);
            ArgumentNullException.ThrowIfNull(settings);

            var regex = settings.GetRegEx(textToFind);
            var input = _textEditor.Text.Substring(_textEditor.SelectionStart, _textEditor.SelectionLength);
            var match = regex.Match(input);

            if (!match.Success || match.Index != 0 || match.Length != input.Length)
            {
                return FindNext(textToFind, settings);
            }

            _textEditor.Document.Replace(_textEditor.SelectionStart, _textEditor.SelectionLength, textToReplace);

            return true;
        }

        public void ReplaceAll(string textToFind, string textToReplace, FindReplaceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(textToFind);
            ArgumentNullException.ThrowIfNull(textToReplace);
            ArgumentNullException.ThrowIfNull(settings);

            var regex = settings.GetRegEx(textToFind, true);
            var offset = 0;

            _textEditor.BeginChange();

            foreach (Match match in regex.Matches(_textEditor.Text))
            {
                _textEditor.Document.Replace(offset + match.Index, match.Length, textToReplace);
                offset += textToReplace.Length - match.Length;
            }

            _textEditor.EndChange();
        }
    }
}
