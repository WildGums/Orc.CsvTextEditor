// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindReplaceViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System.Media;
    using Catel;
    using Catel.MVVM;

    internal class FindReplaceViewModel : ViewModelBase
    {
        #region Fields
        private readonly ICsvTextEditorInstance _csvTextEditorInstance;
        private readonly IFindReplaceSerivce _csvTextEditorFindReplaceSerivce;
        #endregion

        #region Constructors
        public FindReplaceViewModel(ICsvTextEditorInstance csvTextEditorInstance, IFindReplaceSerivce csvTextEditorFindReplaceSerivce)
        {
            Argument.IsNotNull(() => csvTextEditorInstance);
            Argument.IsNotNull(() => csvTextEditorFindReplaceSerivce);

            _csvTextEditorInstance = csvTextEditorInstance;
            _csvTextEditorFindReplaceSerivce = csvTextEditorFindReplaceSerivce;

            FindNext = new Command<string>(OnFindNext);
            Replace = new Command<object>(OnReplace);
            ReplaceAll = new Command<object>(OnReplaceAll);

            FindReplaceSettings = new FindReplaceSettings();

            TextToFind = _csvTextEditorInstance.GetSelectedText().Truncate(20);
            TextToFindForReplace = _csvTextEditorInstance.GetSelectedText().Truncate(20);
        }
        #endregion

        #region Properties
        public override string Title => "Find and Replace";

        [Model]
        public FindReplaceSettings FindReplaceSettings { get; set; }
        public string TextToFind { get; set; }
        public string TextToFindForReplace { get; set; }
        public Command<string> FindNext { get; private set; }
        public Command<object> Replace { get; private set; }
        public Command<object> ReplaceAll { get; private set; }
        #endregion

        private void OnReplaceAll(object parameter)
        {
            var values = (object[]) parameter;
            var textToFind = values[0] as string ?? string.Empty;
            var replacementText = values[1] as string ?? string.Empty;

            _csvTextEditorFindReplaceSerivce.ReplaceAll(textToFind, replacementText, FindReplaceSettings);

            _csvTextEditorInstance.RefreshView();
        }

        private void OnReplace(object parameter)
        {
            var values = (object[]) parameter;
            var textToFind = values[0] as string ?? string.Empty;
            var replacementText = values[1] as string ?? string.Empty;

            if (!_csvTextEditorFindReplaceSerivce.Replace(textToFind, replacementText, FindReplaceSettings))
            {
                SystemSounds.Beep.Play();
            }

            _csvTextEditorInstance.RefreshView();
        }

        private void OnFindNext(string text)
        {
            var textToFind = text ?? string.Empty;

            if (!_csvTextEditorFindReplaceSerivce.FindNext(textToFind, FindReplaceSettings))
            {
                SystemSounds.Beep.Play();
            }
        }
    }
}