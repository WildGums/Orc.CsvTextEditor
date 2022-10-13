namespace Orc.CsvTextEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    internal class TabSpaceElementGenerator : VisualLineElementGenerator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, int> _tabWidths = new Dictionary<int, int>();

        private int _activeCellRealLength;
        private int _activeColumnIndex;
        private int _activeRowIndex;
        private bool _freezeInProgress;

        private int[][] _lines = Array.Empty<int[]>();

        public TabSpaceElementGenerator()
        {
            ColumnWidth = Array.Empty<int>();
            NewLine = Environment.NewLine;
        }

        public int[][] Lines
        {
            get => _lines;
            private set
            {
                if (Equals(value, _lines))
                {
                    return;
                }

                _lines = value;
            }
        }

        public int[] ColumnWidth { get; private set; }
        public int ColumnCount
        {
            get
            {
                var lines = Lines;
                if (lines.Length <= 0)
                {
                    return 0;
                }

                return lines[0].Length;
            }
        }

        public string NewLine { get; private set; }

        public void Refresh(string text)
        {
            text ??= string.Empty;

            NewLine = text.GetNewLineSymbol();

            // Some files do not respect the Environment.NewLine so we need to add "\n"
            var lines = text.Split(new[] {NewLine}, StringSplitOptions.None);

            var minFieldsCount = int.MaxValue;
            var maxFieldsCount = int.MinValue;

            var columnWidthByLine = new int[lines.Length][];

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var fieldsPreview = line.Split(Symbols.Comma);

                var fields = new List<int>();

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
                    }
                    else
                    {
                        fieldLength++;
                    }
                }

                if (fieldLength != 0)
                {
                    fields.Add(fieldLength);
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
                for (var i = 0; i < fieldsCount; i++)
                {
                    lengths[i] = fields[i] + 1;
                }

                columnWidthByLine[index] = lengths;
            }

            NormalizeColumnsCount(columnWidthByLine);

            ColumnWidth = CalculateColumnWidth(columnWidthByLine);
            Lines = columnWidthByLine;
        }

        public bool RefreshLocation(TextLocation affectedLocation, int length)
        {
            var columnWidth = ColumnWidth;
            var columnWidthByLine = Lines;

            var column = GetColumn(affectedLocation);

            var columnIndex = column?.Index ?? 0;
            var rowIndex = affectedLocation.Line - 1;
            var oldWidth = columnWidthByLine[rowIndex][columnIndex];

            var newWidth = oldWidth + length;

            columnWidthByLine[rowIndex][columnIndex] = newWidth;

            int? changedWidth = null;

            // Increasing column
            if (length >= 0 && columnWidth[columnIndex] <= newWidth)
            {
                changedWidth = newWidth;
            }

            // Decreasing column
            if (length <= 0 && columnWidth[columnIndex] >= newWidth)
            {
                changedWidth = columnWidthByLine.Where(x => x.Length > columnIndex).Select(x => x[columnIndex]).Max();
            }

            if (!changedWidth.HasValue)
            {
                return false;
            }

            FreezeColumnResizing(rowIndex, columnIndex);

            if (_freezeInProgress && rowIndex == _activeRowIndex && columnIndex == _activeColumnIndex)
            {
                _activeCellRealLength = changedWidth.Value;
            }
            else
            {
                columnWidth[columnIndex] = changedWidth.Value;
            }

            return true;
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

        public override VisualLineElement? ConstructElement(int offset)
        {
            if (offset < 0)
            {
                return null;
            }

            if (CurrentContext.VisualLine.LastDocumentLine.EndOffset == offset)
            {
                return null;
            }

            if (!_tabWidths.TryGetValue(offset, out var tabWidth))
            {
                tabWidth = 0;
            }

            return tabWidth < 0 ? null : new EmptyVisualLineElement(tabWidth + 1, 1);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            if (Lines is null)
            {
                return startOffset;
            }

            var location = CurrentContext.Document.GetLocation(startOffset);

            var column = GetColumn(location);
            var locationLine = location.Line;

            if (locationLine > Lines.Length)
            {
                return startOffset;
            }

            if (column is null || column.Index == ColumnWidth.Length - 1)
            {
                if (Lines.Length == locationLine)
                {
                    return CurrentContext.VisualLine.LastDocumentLine.EndOffset;
                }

                column = new Column
                {
                    Index = 0,
                    Offset = Lines[locationLine][0]
                };

                locationLine++;
            }

            var curCellWidth = _freezeInProgress &&
                               _activeRowIndex == locationLine - 1 &&
                               column.Index == _activeColumnIndex
                ? _activeCellRealLength
                : ColumnWidth[column.Index];

            var tabWidth = curCellWidth - Lines[locationLine - 1][column.Index];

            try
            {
                var offset = CurrentContext.Document.GetOffset(new TextLocation(locationLine, column.Offset + column.Width));
                _tabWidths[offset] = tabWidth;
                return offset;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get first interested offset");
                return startOffset;
            }
        }

        public Column? GetColumn(TextLocation location)
        {
            var lines = Lines;

            var currentLineIndex = location.Line - 1;
            var currentColumnIndex = location.Column - 1;

            if (currentLineIndex >= lines.Length)
            {
                return null;
            }

            var currentLine = lines[currentLineIndex];

            var sum = 0;
            var i = 0;

            while (currentLine.Length > i && sum <= currentColumnIndex)
            {
                sum += currentLine[i];
                i++;
            }

            var column = new Column
            {
                Index = i - 1,
                Offset = sum - currentLine[i - 1],
                Width = currentLine[i - 1]
            };

            return column;
        }

        private static void NormalizeColumnsCount(IList<int[]> columnWidthByLine)
        {
            if (columnWidthByLine.Count == 0)
            {
                return;
            }

            var colCount = columnWidthByLine.Min(x => x.Length);

            for (var index = 0; index < columnWidthByLine.Count; index++)
            {
                var line = columnWidthByLine[index];
                if (line.Length == colCount)
                {
                    continue;
                }

                var normalizedLine = new int[colCount];
                var lastIndex = colCount - 1;

                for (var i = 0; i < lastIndex; i++)
                {
                    normalizedLine[i] = line[i];
                }

                var lastColWidth = 0;
                for (var i = lastIndex; i < line.Length; i++)
                {
                    lastColWidth += line[i];
                }

                normalizedLine[lastIndex] = lastColWidth;

                columnWidthByLine[index] = normalizedLine;
            }
        }

        private static int[] CalculateColumnWidth(IReadOnlyList<int[]> columnWidthByLine)
        {
            if (columnWidthByLine.Count == 0)
            {
                return new int[0];
            }

            var maxArray = new int[columnWidthByLine[0].Length];
            foreach (var line in columnWidthByLine)
            {
                if (line.Length > maxArray.Length)
                {
                    throw new ArgumentException("Records in CSV have to contain the same number of fields");
                }

                var length = Math.Min(maxArray.Length, line.Length);

                for (var i = 0; i < length; i++)
                {
                    maxArray[i] = Math.Max(maxArray[i], line[i]);
                }
            }

            return maxArray.ToArray();
        }
    }
}
