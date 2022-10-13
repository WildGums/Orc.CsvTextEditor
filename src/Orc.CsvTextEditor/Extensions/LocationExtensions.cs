namespace Orc.CsvTextEditor
{
    using System;

    public static class LocationExtensions
    {
        public static int GetOffsetInLine(this Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            return location.Offset - location.Line.Offset;
        }
    }
}
