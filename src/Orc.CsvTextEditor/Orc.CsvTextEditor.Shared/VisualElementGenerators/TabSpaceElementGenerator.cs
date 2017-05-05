﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabSpaceElementGenerator.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    internal class TabSpaceElementGenerator : VisualLineElementGenerator
    {
        #region Fields
        private int[][] _lines;
        private int _tabWidth;

        private bool _freezeInProgress;
        private int _activeRowIndex;
        private int _activeColumnIndex;
        private int _activeCellRealLength;
        #endregion

        #region Properties
        public int[][] Lines
        {
            get { return _lines; }
            private set
            {
                if (Equals(value, _lines))
                {
                    return;
                }

                _lines = value;
                ColumnWidth = CalculateColumnWidth(_lines);
            }
        }

        public int[] ColumnWidth { get; private set; }
        public int ColumnCount => Lines?[0].Length ?? 0;
        public string NewLine { get; private set; }
        #endregion

        public void Refresh(string text)
        {
            text = text ?? string.Empty;

            NewLine = text.GetNewLineSymbol();
            
            // Some files do not respect the Environment.NewLine so we need to add "\n"
            var lines = text.Split(new[] { NewLine }, StringSplitOptions.None);

            var minFieldsCount = int.MaxValue;
            var maxFieldsCount = int.MinValue;

            var columnWidthByLine = new int[lines.Length][];

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var fieldsPreview = line.Split(Symbols.Comma);

                List<int> fields = new List<int>();

                var quotesCount = 0;
                var fieldLength = 0;
                foreach (var field in fieldsPreview)
                {
                    quotesCount += field.Count(x => x == Symbols.Quote);
                    fieldLength += field.Length;

                    if (quotesCount % 2 == 0)
                    {
                        fields.Add(fieldLength);
                        fieldLength = 0;
                        quotesCount = 0;

                        continue;
                    }
                }

                var fieldsCount = fields.Count;

                if (minFieldsCount > fieldsCount)
                {
                    minFieldsCount = fieldsCount;
                }

                if (maxFieldsCount < fieldsCount)
                {
                    maxFieldsCount = fieldsCount;
                }

                var lengths = new int[fieldsCount];
                for (int i = 0; i < fieldsCount; i++)
                {
                    lengths[i] = fields[i] + 1;
                }

                columnWidthByLine[index] = lengths;
            }

            Lines = columnWidthByLine;
        }

        public bool RefreshLocation(TextLocation affectedLocation, int length)
        {
            var columnWidth = ColumnWidth;
            var columnWidthByLine = Lines;

            var columnNumberWithOffset = GetColumn(affectedLocation);

            var affectedColumn = columnNumberWithOffset.ColumnNumber;
            var affectedLine = affectedLocation.Line - 1;
            var oldWidth = columnWidthByLine[affectedLine][affectedColumn];

            var newWidth = oldWidth + length;

            columnWidthByLine[affectedLine][affectedColumn] = newWidth;

            int? changedWidth = null;
           
            // Increasing column
            if (length >= 0 && columnWidth[affectedColumn] <= newWidth)
            {
                changedWidth = newWidth;
            }

            // Decreasing column
            if (length <= 0 && columnWidth[affectedColumn] >= newWidth)
            {
                changedWidth = columnWidthByLine.Where(x => x.Length > affectedColumn).Select(x => x[affectedColumn]).Max();
            }

            if (changedWidth.HasValue)
            {
                FreezeColumnResizing(affectedLine, affectedColumn);

                if (_freezeInProgress && affectedLine == _activeRowIndex && affectedColumn == _activeColumnIndex)
                {
                    _activeCellRealLength = changedWidth.Value;     
                }
                else
                {
                    columnWidth[affectedColumn] = changedWidth.Value;
                }

                return true;
            }

            return false;
        }
      
        public void FreezeColumnResizing(int rowIndex, int columnIndex)
        {
            if (_freezeInProgress && (_activeRowIndex != rowIndex || _activeColumnIndex != columnIndex))
            {
                UnfreezeColumnResizing();
            }

            _freezeInProgress = true;
            _activeRowIndex = rowIndex;
            _activeColumnIndex = columnIndex;
        }

        public bool UnfreezeColumnResizing()
        {
            if (!_freezeInProgress) return false;

            _freezeInProgress = false;
            ColumnWidth[_activeColumnIndex] = _activeCellRealLength;
            return true;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            if (CurrentContext.VisualLine.LastDocumentLine.EndOffset == offset)
            {
                return null;
            }

            return new EmptyVisualLineElement(_tabWidth + 1, 1);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            if (Lines == null)
            {
                return startOffset;
            }

            var location = CurrentContext.Document.GetLocation(startOffset);

            var columnNumberWithOffset = GetColumn(location);
            var locationLine = location.Line;

            if (columnNumberWithOffset.ColumnNumber == ColumnWidth.Length - 1)
            {
                if (Lines.Length == locationLine)
                {
                    return CurrentContext.VisualLine.LastDocumentLine.EndOffset;
                }

                columnNumberWithOffset = new ColumnNumberWithOffset
                {
                    ColumnNumber = 0,
                    OffsetInLine = Lines[locationLine][0]
                };

                locationLine++;
            }

            var curCellWidth = this._freezeInProgress && 
                _activeRowIndex == locationLine - 1 && 
                columnNumberWithOffset.ColumnNumber == _activeColumnIndex 
                ?
                _activeCellRealLength : ColumnWidth[columnNumberWithOffset.ColumnNumber]; 

            _tabWidth = curCellWidth - Lines[locationLine - 1][columnNumberWithOffset.ColumnNumber];

            return CurrentContext.Document.GetOffset(new TextLocation(locationLine, columnNumberWithOffset.OffsetInLine));
        }

        public ColumnNumberWithOffset GetColumn(TextLocation location)
        {
            var lines = Lines;

            var currentLineIndex = location.Line - 1;
            var currentColumnIndex = location.Column - 1;

            var currentLine = lines[currentLineIndex];

            var sum = 0;
            var i = 0;

            while (currentLine.Length > i && sum <= currentColumnIndex)
            {
                sum += currentLine[i];
                i++;
            }

            var column = new ColumnNumberWithOffset
            {
                ColumnNumber = i - 1,
                OffsetInLine = sum,
                Length = currentLine[i - 1]
            };

            return column;
        }

        private int[] CalculateColumnWidth(int[][] columnWidthByLine)
        {
            if (columnWidthByLine.Length == 0)
            {
                return new int[0];
            }

            var accum = new int[columnWidthByLine[0].Length];

            foreach (var line in columnWidthByLine)
            {
                if (line.Length > accum.Length)
                {
                    throw new ArgumentException("Records in CSV have to contain the same number of fields");
                }

                var length = Math.Min(accum.Length, line.Length);

                for (var i = 0; i < length; i++)
                {
                    accum[i] = Math.Max(accum[i], line[i]);
                }
            }

            return accum.ToArray();
        }
    }
}