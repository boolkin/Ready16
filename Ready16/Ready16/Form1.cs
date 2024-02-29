using System;
using HslCommunication.Profinet.Melsec;
using HslCommunication;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Configuration;

namespace Ready16
{
    public partial class Form1 : Form
    {
        string log = "", header = "No header", textFile = "text.txt", ackn = "test";
        string stat = "test", lastStat = "test";
        int status = 0, last = 0;
        string ipaddr = ConfigurationManager.AppSettings["ip"];
        int port = Int16.Parse(ConfigurationManager.AppSettings["port"]);


        private void button16_Click(object sender, EventArgs e)
        {
            button16.Enabled = false;
            if (ackn != "test")
            {
                
                MelsecMcNet melsec_net = new MelsecMcNet(ipaddr, port);
                OperateResult connect = melsec_net.ConnectServer();
                if (connect.IsSuccess)
                {
                    //log += "[" + ackn + "]";
                    string[] wordBit = ackn.Split('.');
                    // находим номер бита, и преобразуем его в число 
                    int num = Int16.Parse(wordBit[1],System.Globalization.NumberStyles.HexNumber);
                    int val = 1 << num;
                    //Считываем старое значение
                    OperateResult<Int16[]> oldvalue = melsec_net.ReadInt16(wordBit[0], 1);
                    if (oldvalue.IsSuccess)
                    {
                        //выставляем нужный бит в 1
                        int newvalue = oldvalue.Content[0] | (Int16)val;
                        int[] valuesToWrite = new int[1] { newvalue };
                        OperateResult write = melsec_net.Write(wordBit[0], valuesToWrite);
                        if (!write.IsSuccess)
                        {
                            log += "Ошибка записи Ackn ";
                        }
                        System.Threading.Thread.Sleep(100);
                        valuesToWrite[0] = oldvalue.Content[0];
                        write = melsec_net.Write(wordBit[0], valuesToWrite);
                    }
                    melsec_net.ConnectClose();
                }
                else
                {
                    log += "Не смог подключиться Ackn. ";
                }
            }
            textLog.Text = log;
        }


        public Form1()
        {   
            InitializeComponent();
            ReadArgs();
            ButtonsColorChange();
            LabelsTextChange();
        }
        private void ReadArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                //считываем аргументы и их значениея
                string[] argVals = args[i].Split(':');
                // Status of ready16
                if (argVals[0] == "s" | argVals[0] == "S")
                {
                    stat = argVals[1];
                }
                else if (argVals[0] == "f" | argVals[0] == "F")
                {
                    textFile = argVals[1];
                }
                else if (argVals[0] == "l" | argVals[0] == "L")
                {
                    lastStat = argVals[1];
                }
                else if (argVals[0] == "a" | argVals[0] == "A")
                {
                    ackn = argVals[1];
                }
                else if (argVals[0] == "h" | argVals[0] == "H")
                {
                    header = argVals[1];
                }
                else
                {
                    log = "Есть не распознанный аргумент. ";
                }
            }
            if (stat != null | lastStat != null)
            {
                if (stat == "test")
                {
                    status = 65533;
                    last = 65531;
                }
                else
                {
                    MelsecMcNet melsec_net = new MelsecMcNet(ipaddr, port);
                    OperateResult connect = melsec_net.ConnectServer();
                    if (connect.IsSuccess)
                    {
                        OperateResult<Int16[]> readStat = melsec_net.ReadInt16(stat, 1);
                        if (readStat.IsSuccess)
                        {
                            status = readStat.Content[0];
                        }
                        else
                        {
                            log += "Не смог прочитать статус. ";
                        }
                        if (lastStat != "test")
                        {
                            //log += "[" + lastStat + "]";
                            OperateResult<Int16[]> readLast = melsec_net.ReadInt16(lastStat, 1);
                            if (readLast.IsSuccess)
                            {
                                last = readLast.Content[0];
                            }
                            else
                            {
                                log += "Не смог прочитать LASTстатус. ";
                            }
                        }
                        melsec_net.ConnectClose();
                    }
                    else log += "Не смог подключиться. ";
                }

            }
        }
        private void ButtonsColorChange()
        {
            //status = Int32.Parse(stat);
            //last = Int32.Parse(lastStat);
            Button[] buttons = new[] { button0, button1, button2, button3, button4, button5,
                    button6, button7 , button8, button9, button10, button11,button12, button13, button14, button15 };
            for (int i = 0; i < buttons.Length; i++)
            {
                int bit = status >> i & 1;
                int border = last >> i & 1;
                Color color;
                if (bit == 1)
                {
                    buttons[i].BackColor = Color.Green;
                    color = Color.Green;
                }
                else
                {
                    buttons[i].BackColor = Color.Red;
                    color = Color.Red;
                }
                
                if (border == 1)
                {
                    buttons[i].FlatAppearance.BorderColor = color;
                }
                else
                {
                    buttons[i].FlatAppearance.BorderColor = Color.Yellow;
                }
            }

        }
        private void LabelsTextChange()
        {
            Label[] labels = new[] {  label0, label1, label2, label3, label4, label5,
                    label6, label7 , label8, label9, label10, label11,label12, label13, label14, label15 };
            if (File.Exists(textFile))
            {
                string[] labelsText = File.ReadAllLines(textFile, Encoding.GetEncoding(1251));
                for (int i = 0; i < labels.Length; i++)
                {
                    if (i < labelsText.Length) labels[i].Text = labelsText[i];
                }
            }
            else {
                log += "Отсутствует файл текста. ";
            }
            
            textLog.Text = log;
            this.Text = this.Text + header;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
