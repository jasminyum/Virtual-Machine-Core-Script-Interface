namespace modbus_portal
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Initialize metodunu tamamen kaldırıp manuel ayağa kaldırın:
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            Application.Run(new Form1());
        }

    }
}