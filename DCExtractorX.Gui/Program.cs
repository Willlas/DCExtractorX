using Custom.Diagnostics;
using DCExtractorX.Gui.Diagnostics;

namespace DCExtractorX.Gui;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Log.Current = new GuiLog();
        SettingsStore.Load();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new DCExtractorForm());
    }
}