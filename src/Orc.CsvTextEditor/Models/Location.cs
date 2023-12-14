namespace Orc.CsvTextEditor
{
    public class Location
    {
        public Location(Column column, Line line)
        {
            Column = column;
            Line = line;
        }

        public Column Column { get; private set; }
        public Line Line { get; private set; }
        public int Offset { get; set; }
    }
}
