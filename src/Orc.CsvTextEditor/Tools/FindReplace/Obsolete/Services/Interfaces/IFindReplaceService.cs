// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindReplaceService.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    public interface IFindReplaceService
    {
        #region Methods
        bool FindNext(string textToFind, FindReplaceSettings settings);
        bool Replace(string textToFind, string textToReplace, FindReplaceSettings settings);
        void ReplaceAll(string textToFind, string textToReplace, FindReplaceSettings settings);
        #endregion
    }
}
