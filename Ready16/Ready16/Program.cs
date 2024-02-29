using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ready16
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            /* bool onlyInstance;
             Mutex mutex = new Mutex(true, "Ready16", out onlyInstance);
             if (onlyInstance)
             {
                //удалить сверху вызов если нужно сделать запуск только одно экземпляра программы
                Application.Run(new Form1());
             }    
            */
        }
    }
}
