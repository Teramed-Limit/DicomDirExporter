namespace DicomDirExporter.Model
{
    public interface IDashboard
    {
        string Name { get; set; }
        string Text { get; set; }
        string Icon { get; set; }
        string Color { get; set; }
    }

    public abstract class BaseDashboard<T> : IDashboard
    {
        public T Value { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public abstract IDashboard Refresh();
    }

    public class Dashboard : BaseDashboard<string>
    {
        public override IDashboard Refresh()
        {
            Color = "#FF41A43C";
            Text = Value;
            return this;
        }
    }

    public class DiskDashboard : BaseDashboard<uint>
    {
        public override IDashboard Refresh()
        {
            if (Value < 5)
                Color = "#f44336";
            else if (Value < 10)
                Color = "#ff9800";
            else
                Color = "#FF41A43C";
            Text = Value.ToString();
            return this;
        }
    }

    public class DriveDashboard : BaseDashboard<double>
    {
        public override IDashboard Refresh()
        {
            if (Value < 10)
                Color = "#f44336";
            else if (Value < 30)
                Color = "#ff9800";
            else
                Color = "#FF41A43C";
            Text = Value + "%";
            return this;
        }
    }

    public class CartridgeDashboard : BaseDashboard<uint>
    {
        public override IDashboard Refresh()
        {
            if (Value < 10)
                Color = "#f44336";
            else if (Value < 30)
                Color = "#ff9800";
            else
                Color = "#FF41A43C";
            Text = Value + "%";
            return this;
        }
    }
}