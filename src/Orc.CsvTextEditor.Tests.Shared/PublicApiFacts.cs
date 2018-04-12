// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor.Tests
{
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_CsvTextEditor_HasNoBreakingChanges()
        {
            var assembly = typeof(CsvTextEditorControl).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}