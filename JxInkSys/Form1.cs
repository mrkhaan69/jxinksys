using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace JxInkSys
{
    public partial class Form1 : Form
    {
        private SerialPort ComDevice = new SerialPort();

        private bool SysInit = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BordS.SelectedIndex = 0;
            comList.Items.AddRange(SerialPort.GetPortNames());
            if (comList.Items.Count > 0)
            {
                comList.SelectedIndex = 0;
                ComDevice.DataReceived += ComDevice_DataReceived;
                ConInkSys.Enabled = true;
            }
            else
            {
                comList.Text = "No serial port detected";
                ConInkSys.Enabled = false;
            }
        }

        void ComDevice_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (SysInit == true)
            {
                CommParameter m_Setinfo = new CommParameter();
                if (ComDevice.BytesToRead < Marshal.SizeOf(m_Setinfo))
                    return;
                if (ComDevice.BytesToRead == Marshal.SizeOf(m_Setinfo))
                {
                    byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                    ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);
                    {
                        IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(m_Setinfo));
                        try
                        {
                            Marshal.Copy(m_ReDatas, 0, buffer, Marshal.SizeOf(m_Setinfo));
                            m_Setinfo = (CommParameter)Marshal.PtrToStructure(buffer, typeof(CommParameter));
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            ShowScrenn(m_Setinfo);
                        }));
                    }
                }
                else
                {
                    byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                    ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);
                }
            }
            else
            {
                CommReadAir m_runinfo = new CommReadAir();
                if (ComDevice.BytesToRead < Marshal.SizeOf(m_runinfo))
                    return;
                if (ComDevice.BytesToRead == Marshal.SizeOf(m_runinfo))
                {
                    byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                    ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);
                    {
                        IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(m_runinfo));
                        try
                        {
                            Marshal.Copy(m_ReDatas, 0, buffer, Marshal.SizeOf(m_runinfo));
                            m_runinfo = (CommReadAir)Marshal.PtrToStructure(buffer, typeof(CommReadAir));
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(buffer);
                        }
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            ShowRunInfo(m_runinfo);
                        }));
                    }
                }
                else
                {
                    byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                    ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);
                }

            }

        }

        private void ShowRunInfo(CommReadAir m_runinfo)
        {
            int m_pr = 0;
            double m_temp = 0;

            m_runinfo.PCurrAirs = (short)((short)((m_runinfo.PCurrAirs & 0xFF00) >> 8) | (short)((m_runinfo.PCurrAirs & 0x00FF) << 8));
            m_pr = m_runinfo.PCurrAirs;
            m_temp = m_pr / 10.0;
            CurrPressink.Text = m_temp.ToString();

            m_runinfo.CurrNp1 = (short)((short)((m_runinfo.CurrNp1 & 0xFF00) >> 8) | (short)((m_runinfo.CurrNp1 & 0x00FF) << 8));
            m_runinfo.CurrNp2 = (short)((short)((m_runinfo.CurrNp2 & 0xFF00) >> 8) | (short)((m_runinfo.CurrNp2 & 0x00FF) << 8));
            m_runinfo.CurrNp3 = (short)((short)((m_runinfo.CurrNp3 & 0xFF00) >> 8) | (short)((m_runinfo.CurrNp3 & 0x00FF) << 8));
            m_runinfo.CurrNp4 = (short)((short)((m_runinfo.CurrNp4 & 0xFF00) >> 8) | (short)((m_runinfo.CurrNp4 & 0x00FF) << 8));
 
            Press1.Text = ((double)(m_runinfo.CurrNp1 / 100.0)).ToString() + "kP";
            Press2.Text = ((double)(m_runinfo.CurrNp2 / 100.0)).ToString() + "kP";
            Press3.Text = ((double)(m_runinfo.CurrNp3 / 100.0)).ToString() + "kP";
            Press4.Text = ((double)(m_runinfo.CurrNp4 / 100.0)).ToString() + "kP";

            m_runinfo.CurrTemp1 = (ushort)((short)((m_runinfo.CurrTemp1 & 0xFF00) >> 8) | (short)((m_runinfo.CurrTemp1 & 0x00FF) << 8));
            m_runinfo.CurrTemp2 = (ushort)((short)((m_runinfo.CurrTemp2 & 0xFF00) >> 8) | (short)((m_runinfo.CurrTemp2 & 0x00FF) << 8));
            m_runinfo.CurrTemp3 = (ushort)((short)((m_runinfo.CurrTemp3 & 0xFF00) >> 8) | (short)((m_runinfo.CurrTemp3 & 0x00FF) << 8));
            m_runinfo.CurrTemp4 = (ushort)((short)((m_runinfo.CurrTemp4 & 0xFF00) >> 8) | (short)((m_runinfo.CurrTemp4 & 0x00FF) << 8));
            m_runinfo.CurrInkS = (ushort)((short)((m_runinfo.CurrInkS & 0xFF00) >> 8) | (short)((m_runinfo.CurrInkS & 0x00FF) << 8));

            Temp1.Text = m_runinfo.CurrTemp1.ToString() + "℃";
            Temp2.Text = m_runinfo.CurrTemp2.ToString() + "℃";
            Temp3.Text = m_runinfo.CurrTemp3.ToString() + "℃";
            Temp4.Text = m_runinfo.CurrTemp4.ToString() + "℃";

            m_runinfo.M_InkM_RunT1 = (ushort)((short)((m_runinfo.M_InkM_RunT1 & 0xFF00) >> 8) | (short)((m_runinfo.M_InkM_RunT1 & 0x00FF) << 8));
            m_runinfo.M_InkM_RunT2 = (ushort)((short)((m_runinfo.M_InkM_RunT2 & 0xFF00) >> 8) | (short)((m_runinfo.M_InkM_RunT2 & 0x00FF) << 8));
            m_runinfo.M_InkM_RunT3 = (ushort)((short)((m_runinfo.M_InkM_RunT3 & 0xFF00) >> 8) | (short)((m_runinfo.M_InkM_RunT3 & 0x00FF) << 8));
            m_runinfo.M_InkM_RunT4 = (ushort)((short)((m_runinfo.M_InkM_RunT4 & 0xFF00) >> 8) | (short)((m_runinfo.M_InkM_RunT4 & 0x00FF) << 8));

            InkT1.Text = m_runinfo.M_InkM_RunT1.ToString() + "s";
            InkT2.Text = m_runinfo.M_InkM_RunT2.ToString() + "ml";
            InkT3.Text = m_runinfo.M_InkM_RunT3.ToString() + "s";
            InkT4.Text = m_runinfo.M_InkM_RunT4.ToString() + "ml";

            if (InkSstatic.Checked == true)
            {
                if ((m_runinfo.CurrInkS & 0x01) == 0x01)
                    Inks1.BackColor = Color.Black;
                else
                    Inks1.BackColor = Color.White;
                if ((m_runinfo.CurrInkS & 0x02) == 0x02)
                    Inks2.BackColor = Color.Black;
                else
                    Inks2.BackColor = Color.White;
                if ((m_runinfo.CurrInkS & 0x04) == 0x04)
                    Inks3.BackColor = Color.Black;
                else
                    Inks3.BackColor = Color.White;
                if ((m_runinfo.CurrInkS & 0x08) == 0x08)
                    Inks4.BackColor = Color.Black;
                else
                    Inks4.BackColor = Color.White;
            }
            else
            {
                if ((m_runinfo.CurrInkS & 0x01) == 0x01)
                    Inks1.BackColor = Color.White;
                else
                    Inks1.BackColor = Color.Black;
                if ((m_runinfo.CurrInkS & 0x02) == 0x02)
                    Inks2.BackColor = Color.White;
                else
                    Inks2.BackColor = Color.Black;
                if ((m_runinfo.CurrInkS & 0x04) == 0x04)
                    Inks3.BackColor = Color.White;
                else
                    Inks3.BackColor = Color.Black;
                if ((m_runinfo.CurrInkS & 0x08) == 0x08)
                    Inks4.BackColor = Color.White;
                else
                    Inks4.BackColor = Color.Black;
            }

        }

        private void ShowScrenn(CommParameter m_runinfo)
        {
            int m_pr = 0;
            double m_temp = 0;
            if (SysInit == true)
            {
                m_runinfo.EnableAirAdjust = (ushort)((short)((m_runinfo.EnableAirAdjust & 0xFF00) >> 8) | (short)((m_runinfo.EnableAirAdjust & 0x00FF) << 8));
                m_runinfo.EnableAddInk = (ushort)((short)((m_runinfo.EnableAddInk & 0xFF00) >> 8) | (short)((m_runinfo.EnableAddInk & 0x00FF) << 8));
                m_runinfo.EnableTempAdjust = (ushort)((short)((m_runinfo.EnableTempAdjust & 0xFF00) >> 8) | (short)((m_runinfo.EnableTempAdjust & 0x00FF) << 8));
                m_runinfo.EnablePressInk = (ushort)((short)((m_runinfo.EnablePressInk & 0xFF00) >> 8) | (short)((m_runinfo.EnablePressInk & 0x00FF) << 8));
                m_runinfo.InkSensorStatic = (ushort)((short)((m_runinfo.InkSensorStatic & 0xFF00) >> 8) | (short)((m_runinfo.InkSensorStatic & 0x00FF) << 8));
                m_runinfo.EnableIndex = (ushort)((short)((m_runinfo.EnableIndex & 0xFF00) >> 8) | (short)((m_runinfo.EnableIndex & 0x00FF) << 8));

                if (m_runinfo.EnableAirAdjust == 1 )
                    EnAirAdjust.Checked = true;
                else
                    EnAirAdjust.Checked = false;
                if (m_runinfo.EnableAddInk == 1)
                    EnAddInk.Checked = true;
                else
                    EnAddInk.Checked = false;
                if (m_runinfo.EnableTempAdjust == 1)
                    EnTempAdjust.Checked = true;
                else
                    EnTempAdjust.Checked = false;
                if (m_runinfo.EnablePressInk == 1)
                    EnPressInk.Checked = true;
                else
                    EnPressInk.Checked = false;
                if (m_runinfo.InkSensorStatic == 1)
                    InkSstatic.Checked = true;
                else
                    InkSstatic.Checked = false;

                if ((m_runinfo.EnableIndex & 0x01) == 0x01)
                    En1.Checked = true;
                else
                    En1.Checked = false;
                if ((m_runinfo.EnableIndex & 0x02) == 0x02)
                    En2.Checked = true;
                else
                    En2.Checked = false;
                if ((m_runinfo.EnableIndex & 0x04) == 0x04)
                    En3.Checked = true;
                else
                    En3.Checked = false;
                if ((m_runinfo.EnableIndex & 0x08) == 0x08)
                    En4.Checked = true;
                else
                    En4.Checked = false;

                m_runinfo.npu = (short)((short)((m_runinfo.npu & 0xFF00) >> 8) | (short)((m_runinfo.npu & 0x00FF) << 8));
                m_pr = m_runinfo.npu;
                m_temp =Math.Abs(m_pr) / 10.0;
                PressInkAirtext.Text = m_temp.ToString();

                m_runinfo.PressTime = (ushort)((short)((m_runinfo.PressTime & 0xFF00) >> 8) | (short)((m_runinfo.PressTime & 0x00FF) << 8));
                m_pr = m_runinfo.PressTime;
                PressInkTimetext.Text = (m_pr/10).ToString();

                m_runinfo.np1 = (short)((short)((m_runinfo.np1 & 0xFF00) >> 8) | (short)((m_runinfo.np1 & 0x00FF) << 8));
                m_runinfo.np2 = (short)((short)((m_runinfo.np2 & 0xFF00) >> 8) | (short)((m_runinfo.np2 & 0x00FF) << 8));
                m_runinfo.np3 = (short)((short)((m_runinfo.np3 & 0xFF00) >> 8) | (short)((m_runinfo.np3 & 0x00FF) << 8));
                m_runinfo.np4 = (short)((short)((m_runinfo.np4 & 0xFF00) >> 8) | (short)((m_runinfo.np4 & 0x00FF) << 8));
                m_temp = m_runinfo.np1 / 100.0;
                RPress1.Text = "-" + m_temp.ToString();
//                RPress1.Text =  m_temp.ToString();
                m_temp = m_runinfo.np2 / 100.0;
                RPress2.Text = "-" + m_temp.ToString();
                m_temp = m_runinfo.np3 / 100.0;
                RPress3.Text = "-" + m_temp.ToString();
//                RPress3.Text = m_temp.ToString();
                m_temp = m_runinfo.np4 / 100.0;
                RPress4.Text = "-" + m_temp.ToString();

                m_runinfo.M_Low1 = (ushort)((short)((m_runinfo.M_Low1 & 0xFF00) >> 8) | (short)((m_runinfo.M_Low1 & 0x00FF) << 8));
                Motor1.Text = m_runinfo.M_Low1.ToString();
                m_runinfo.M_Low2 = (ushort)((short)((m_runinfo.M_Low2 & 0xFF00) >> 8) | (short)((m_runinfo.M_Low2 & 0x00FF) << 8));
                Motor2.Text = m_runinfo.M_Low2.ToString();
                m_runinfo.M_Low3 = (ushort)((short)((m_runinfo.M_Low3 & 0xFF00) >> 8) | (short)((m_runinfo.M_Low3 & 0x00FF) << 8));
                Motor3.Text = m_runinfo.M_Low3.ToString();
                m_runinfo.M_Low4 = (ushort)((short)((m_runinfo.M_Low4 & 0xFF00) >> 8) | (short)((m_runinfo.M_Low4 & 0x00FF) << 8));
                Motor4.Text = m_runinfo.M_Low4.ToString();
                m_runinfo.AddInkMotor = (ushort)((short)((m_runinfo.AddInkMotor & 0xFF00) >> 8) | (short)((m_runinfo.AddInkMotor & 0x00FF) << 8));
                AddMotor.Text = m_runinfo.AddInkMotor.ToString();

                m_runinfo.Temperature0 = (ushort)((short)((m_runinfo.Temperature0 & 0xFF00) >> 8) | (short)((m_runinfo.Temperature0 & 0x00FF) << 8));
                RTemp1.Text = m_runinfo.Temperature0.ToString();
                m_runinfo.Temperature1 = (ushort)((short)((m_runinfo.Temperature1 & 0xFF00) >> 8) | (short)((m_runinfo.Temperature1 & 0x00FF) << 8));
                RTemp2.Text = m_runinfo.Temperature1.ToString();
                m_runinfo.Temperature2 = (ushort)((short)((m_runinfo.Temperature2 & 0xFF00) >> 8) | (short)((m_runinfo.Temperature2 & 0x00FF) << 8));
                RTemp3.Text = m_runinfo.Temperature2.ToString();
                m_runinfo.Temperature3 = (ushort)((short)((m_runinfo.Temperature3 & 0xFF00) >> 8) | (short)((m_runinfo.Temperature3 & 0x00FF) << 8));
                RTemp4.Text = m_runinfo.Temperature3.ToString();

                SysInit = false;
            }

        }

        private void ConInkSys_Click(object sender, EventArgs e)
        {
            if (ComDevice.IsOpen == false)
            {
                //设置串口相关属性
                ComDevice.PortName = comList.SelectedItem.ToString();
                ComDevice.BaudRate = 115200;
                ComDevice.Parity = Parity.None;
                ComDevice.DataBits = 8;
                ComDevice.StopBits = StopBits.One;
                try
                {
                    //开启串口
                    ComDevice.Open();
                    SysInfoFlash.Enabled = true;
                    SetAirs.Enabled = true;
                    SetMotors.Enabled = true;
                    SetSys.Enabled = true;
                    SetTemps.Enabled = true;
                    SetPressInk.Enabled = true;
                    PressInk1.Enabled = true;
                    PressInk3.Enabled = true;
                    AllPressInk.Enabled = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Serial port opening failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

  
        private void SysInfoFlash_Click(object sender, EventArgs e)
        {
            SysInit = true;
        }

        private void RunInfo_Tick(object sender, EventArgs e)
        {
            ushort m_re = 0;

            if(ComDevice.IsOpen == true )
            {
                if(SysInit == false)
                {
                    byte[] GetAirInfo = new byte[8] { 0x89, 0x03, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00 };
                    GetAirInfo[0] += (byte)BordS.SelectedIndex;
                    m_re = Struct_Transform.GetCrcData(GetAirInfo, 6);

                    GetAirInfo[6] = (byte)(m_re >> 8);
                    GetAirInfo[7] = (byte)(m_re);

                    ComDevice.Write(GetAirInfo, 0, 8);
                }
                else
                {
                    byte[] GetAirInfo = new byte[8] { 0x89, 0x03, 0x00, 0x0E, 0x00, 0x15, 0x00, 0x00 };
                    GetAirInfo[0] += (byte)BordS.SelectedIndex;
                    m_re = Struct_Transform.GetCrcData(GetAirInfo, 6);

                    GetAirInfo[6] = (byte)(m_re >> 8);
                    GetAirInfo[7] = (byte)(m_re);

                    ComDevice.Write(GetAirInfo, 0, 8);
                }

            
            }

        }

        private void SetAirs_Click(object sender, EventArgs e)
        {
            SendCommParameter m_p = new SendCommParameter();
            float m_p1;
            short m_temp1;

            m_p1 = float.Parse(RPress1.Text);
            m_temp1 = Math.Abs((short)(m_p1 * 100));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.np1 = (ushort)(m_temp1);

            m_p1 = float.Parse(RPress2.Text);
            m_temp1 = Math.Abs((short)(m_p1 * 100));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.np2 = (ushort)(m_temp1);

            m_p1 = float.Parse(RPress3.Text);
            m_temp1 = Math.Abs((short)(m_p1 * 100));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.np3 = (ushort)(m_temp1);

            m_p1 = float.Parse(RPress4.Text);
            m_temp1 = Math.Abs((short)(m_p1 * 100));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.np4 = (ushort)(m_temp1);

            m_p1 = float.Parse(AddMotor.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.AddInkMotor = (ushort)(m_temp1);

            m_p1 = float.Parse(Motor1.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.M_Low1 = (ushort)(m_temp1);

            m_p1 = float.Parse(Motor2.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.M_Low2 = (ushort)(m_temp1);

            m_p1 = float.Parse(Motor3.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.M_Low3 = (ushort)(m_temp1);

            m_p1 = float.Parse(Motor4.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.M_Low4 = (ushort)(m_temp1);

            m_p1 = float.Parse(RTemp1.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.Temperature0 = (ushort)(m_temp1);

            m_p1 = float.Parse(RTemp2.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.Temperature1 = (ushort)(m_temp1);

            m_p1 = float.Parse(RTemp3.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.Temperature2 = (ushort)(m_temp1);

            m_p1 = float.Parse(RTemp4.Text);
            m_temp1 = Math.Abs((short)(m_p1));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.Temperature3 = (ushort)(m_temp1);

            m_p1 = float.Parse(PressInkAirtext.Text);
            m_temp1 = Math.Abs((short)(m_p1 * 10));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.npu = (ushort)(m_temp1);

            m_p1 = float.Parse(PressInkTimetext.Text);
            m_temp1 = Math.Abs((short)(m_p1*10));
            m_temp1 = (short)((short)((m_temp1 & 0xFF00) >> 8) | (short)((m_temp1 & 0x00FF) << 8));
            m_p.PressTime = (ushort)(m_temp1); 

            if (EnAddInk.Checked)
                m_p.EnableAddInk = (ushort)0x0100;
            else
                m_p.EnableAddInk = 0x0000;

            if (EnAirAdjust.Checked)
                m_p.EnableAirAdjust = (ushort)0x0100;
            else
                m_p.EnableAirAdjust = 0x0000;

            if (EnPressInk.Checked)
                m_p.EnablePressInk = (ushort)0x0100;
            else
                m_p.EnablePressInk = 0x0000;

            if (EnTempAdjust.Checked)
                m_p.EnableTempAdjust = (ushort)0x0100;
            else
                m_p.EnableTempAdjust = 0x0000;

            if (InkSstatic.Checked)
                m_p.InkSensorStatic = (ushort)0x0100;
            else
                m_p.InkSensorStatic = 0x0000;

            m_p.EnableIndex = 0;
            if (En1.Checked)
                m_p.EnableIndex |= (ushort)0x0100;
            if (En2.Checked)
                m_p.EnableIndex |= (ushort)0x0200;
            if (En3.Checked)
                m_p.EnableIndex |= (ushort)0x0400;
            if (En4.Checked)
                m_p.EnableIndex |= (ushort)0x0800;


 
            byte[] m_SendData = Struct_Transform.StructToBytes(m_p);
            byte[] m_data = new byte[Marshal.SizeOf(m_p) + 9];
            m_data[0] = (byte)(0x89 + BordS.SelectedIndex);
            m_data[1] = 0x10;
            m_data[2] = 0x00;
            m_data[3] = 0x0E;
            m_data[4] = 0x00;
            m_data[5] = 0x15;
            m_data[6] = 0x2A;
            for (int i = 0; i < Marshal.SizeOf(m_p); i++)
            {
                m_data[7 + i] = m_SendData[i];
            }
            m_temp1 = (short)Struct_Transform.GetCrcData(m_data, Marshal.SizeOf(m_p) + 7);

            m_data[Marshal.SizeOf(m_p) + 7] = (byte)(m_temp1 >> 8);
            m_data[Marshal.SizeOf(m_p) + 8] = (byte)(m_temp1);

            RunInfo.Enabled = false;
            ComDevice.Write(m_data, 0, Marshal.SizeOf(m_p) + 9);
            Thread.Sleep(50);
            if (ComDevice.BytesToRead < 5)
            {
                Thread.Sleep(50);
            }
            if (ComDevice.BytesToRead != 8)
                MessageBox.Show("Communication abnormality!");
            byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);

            RunInfo.Enabled = true;
            SysInit = true;

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox m_s = sender as TextBox;
            //允许输入数字、小数点、删除键和负号  
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != (char)('.') && e.KeyChar != (char)('-'))
            {
                MessageBox.Show("Please enter a number");
                m_s.Text = "";
                e.Handled = true;
            }
            if (e.KeyChar == (char)('-'))
            {
                if (m_s.Text != "")
                {
                    MessageBox.Show("Please enter a number");
                    m_s.Text = "";
                    e.Handled = true;
                }
            }
            /*小数点只能输入一次*/
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text.IndexOf('.') != -1)
            {
                MessageBox.Show("Please enter a number");
                m_s.Text = "";
                e.Handled = true;
            }
            /*第一位不能为小数点*/
            if (e.KeyChar == (char)('.') && ((TextBox)sender).Text == "")
            {
                MessageBox.Show("Please enter a number");
                m_s.Text = "";
                e.Handled = true;
            }
            /*第一位是0，第二位必须为小数点*/
            if (e.KeyChar != (char)('.') && ((TextBox)sender).Text == "0")
            {
                MessageBox.Show("Please enter a number");
                m_s.Text = "";
                e.Handled = true;
            }
            /*第一位是负号，第二位不能为小数点*/
            if (((TextBox)sender).Text == "-" && e.KeyChar == (char)('.'))
            {
                MessageBox.Show("Please enter a number");
                m_s.Text = "";
                e.Handled = true;
            }
        }

        private void PressInk1_Click(object sender, EventArgs e)
        {
            ushort m_re = 0;
            if (ComDevice.IsOpen == true)
            {
                RunInfo.Enabled = false;

                byte[] GetAirInfo = new byte[11] { 0x89, 0x10, 0x00, 0x23, 0x00, 0x01, 0x02, 0x00, 0x12, 0x00, 0x00 };
                GetAirInfo[0] += (byte)BordS.SelectedIndex;
                m_re = Struct_Transform.GetCrcData(GetAirInfo, 9);

                GetAirInfo[9] = (byte)(m_re >> 8);
                GetAirInfo[10] = (byte)(m_re);

                ComDevice.Write(GetAirInfo, 0, 11);
                Thread.Sleep(50);
                if (ComDevice.BytesToRead < 5)
                {
                    Thread.Sleep(50);
                }
                if (ComDevice.BytesToRead != 8)
                    MessageBox.Show("Communication abnormality!");
                byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);

                RunInfo.Enabled = true;
                SysInit = true;
            }
        }

  

        private void PressInk3_Click(object sender, EventArgs e)
        {
            ushort m_re = 0;
            if (ComDevice.IsOpen == true)
            {
                RunInfo.Enabled = false;

                byte[] GetAirInfo = new byte[11] { 0x89, 0x10, 0x00, 0x23, 0x00, 0x01, 0x02, 0x00, 0x22, 0x00, 0x00 };
                GetAirInfo[0] += (byte)BordS.SelectedIndex;
                m_re = Struct_Transform.GetCrcData(GetAirInfo, 9);

                GetAirInfo[9] = (byte)(m_re >> 8);
                GetAirInfo[10] = (byte)(m_re);

                ComDevice.Write(GetAirInfo, 0, 11);
                Thread.Sleep(50);
                if (ComDevice.BytesToRead < 5)
                {
                    Thread.Sleep(50);
                }
                if (ComDevice.BytesToRead != 8)
                    MessageBox.Show("Communication abnormality!");
                byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);

                RunInfo.Enabled = true;
                SysInit = true;
            }
        }

        private void AllPressInk_Click(object sender, EventArgs e)
        {
            ushort m_re = 0;
            if (ComDevice.IsOpen == true)
            {
                RunInfo.Enabled = false;

                byte[] GetAirInfo = new byte[11] { 0x89, 0x10, 0x00, 0x23, 0x00, 0x01, 0x02, 0x00, 0x32, 0x00, 0x00 };

                GetAirInfo[0] += (byte)BordS.SelectedIndex;
                m_re = Struct_Transform.GetCrcData(GetAirInfo, 9);

                GetAirInfo[9] = (byte)(m_re >> 8);
                GetAirInfo[10] = (byte)(m_re);

                ComDevice.Write(GetAirInfo, 0, 11);
                Thread.Sleep(50);
                if (ComDevice.BytesToRead < 5)
                {
                    Thread.Sleep(50);
                }
                if (ComDevice.BytesToRead != 8)
                    MessageBox.Show("Communication abnormality!");
                byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);

                RunInfo.Enabled = true;
                SysInit = true;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void comList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ushort m_re = 0;
            if (ComDevice.IsOpen == true)
            {
                RunInfo.Enabled = false;

                byte[] GetAirInfo = new byte[11] { 0x89, 0x10, 0x00, 0x23, 0x00, 0x01, 0x02, 0x00, 0x01, 0x00, 0x00 };
                GetAirInfo[0] += (byte)BordS.SelectedIndex;
                m_re = Struct_Transform.GetCrcData(GetAirInfo, 9);

                GetAirInfo[9] = (byte)(m_re >> 8);
                GetAirInfo[10] = (byte)(m_re);

                ComDevice.Write(GetAirInfo, 0, 11);
                Thread.Sleep(50);
                if (ComDevice.BytesToRead < 5)
                {
                    Thread.Sleep(50);
                }
                if (ComDevice.BytesToRead != 8)
                    MessageBox.Show("Communication abnormality!");
                byte[] m_ReDatas = new byte[ComDevice.BytesToRead];
                ComDevice.Read(m_ReDatas, 0, m_ReDatas.Length);

                RunInfo.Enabled = true;
                SysInit = true;
            }
        }




    }
}
