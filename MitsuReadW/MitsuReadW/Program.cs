using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication;
using System.Configuration;
using Microsoft.Win32;

namespace MitsuReadW
{
    internal class Program
    {
        private static MelsecMcNet melsec_net = null;
        private static string ipaddr = ConfigurationManager.AppSettings["ip"];
        private static int port = Int16.Parse(ConfigurationManager.AppSettings["port"]);
        static string wordAddr, fileName = "readW.txt";
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    //считываем аргументы и их значениея
                    string[] argVals = args[i].Split(':');
                    // Status of word
                    if (argVals[0] == "s" | argVals[0] == "S")
                    {
                        wordAddr = argVals[1];
                    }
                    if (argVals[0] == "n" | argVals[0] == "N")
                    {
                        fileName = argVals[1];
                    }
                }

                MelsecMcNet melsec_net = new MelsecMcNet(ipaddr, port);
                OperateResult connect = melsec_net.ConnectServer();
                if (connect.IsSuccess)
                {
                    //Console.WriteLine(wordAddr);
                    OperateResult<Int32[]> readStat = melsec_net.ReadInt32(wordAddr, 1);
                    if (readStat.IsSuccess)
                    {
                        int status = readStat.Content[0];
                        //File.WriteAllText("C:\\Ready16\\"+fileName, status.ToString());
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                        key.CreateSubKey("ready16");
                        key = key.OpenSubKey("ready16", true);
                        key.SetValue("value", status);
                    }
                    else
                    {
                        Console.WriteLine("Не смог прочитать статус. ");
                        Console.ReadKey();
                    }
                    melsec_net.ConnectClose();
                }
                else
                {
                    Console.WriteLine("Не смог подключиться. ");
                    Console.ReadKey();
                }
            }
        }
    }
}
