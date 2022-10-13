namespace Orc.CsvTextEditor
{
    using System;

    public class CaretTextLocationChangedEventArgs : EventArgs
    {
        public CaretTextLocationChangedEventArgs(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            Location = location;
        }

        public Location Location { get; }
    }
}
