using System;
using System.Windows.Forms;
using TaskManagerApp; 
namespace TaskManagerApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TaskManagerCalendarForm());
        }
    }
}
