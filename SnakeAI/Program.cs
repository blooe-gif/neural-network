using System;
using System.Windows.Forms;
using SnakeAI.UI;

namespace SnakeAI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
