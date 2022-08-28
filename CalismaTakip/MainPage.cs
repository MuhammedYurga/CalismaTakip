using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace CalismaTakip
{
    public partial class MainPage : Telerik.WinControls.UI.RadTabbedForm
    {
        public List<DateTime> workedDaysList;
        public List<DateTime> nonWorkedDaysList;
        public MainPage()
        {
            //veriler.Default.workdays = new DateTime(2022, 8, 21) + "|" + new DateTime(2022, 8, 22) + "|" + new DateTime(2022, 8, 24)
            //    + "|" + new DateTime(2022, 8, 25) + "|" + new DateTime(2022, 8, 26) + "|" + new DateTime(2022, 8, 27);
            //veriler.Default.nonworkdays = new DateTime(2022, 8, 23) + "";
            //veriler.Default.startday = new DateTime(2022, 8, 21);
            InitializeComponent();
            this.AllowAero = false;
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private async void MainPage_Load(object sender, EventArgs e)
        {
            await updateAll();
            updateImage();
            day = DateTime.Now.Date;
            timer1.Start();
        }

        private async Task updateAll()
        {
            await Task.Run(() =>
            {
                lblStartDate.Text = "Başlangıç Tarihi: " + veriler.Default.startday.ToShortDateString();
                workedDaysList = new List<DateTime>();
                workedDaysList = listdays(veriler.Default.workdays);
                if(workedDaysList.Count != 0)
                {
                    veriler.Default.lastWorked = Convert.ToDateTime(veriler.Default.workdays.Split('|').Last()).Date;
                }
                else
                {
                    veriler.Default.lastWorked = default;
                }
                if (veriler.Default.lastWorked.Date == DateTime.Now.Date)
                {
                    btnIWorked.Visible = false;
                    lblDoYouWorked.Text = "Bugün Çalıştınız :)";
                }
                else
                {
                    btnIWorked.Visible = true;
                    lblDoYouWorked.Text = "! Bugün Çalışmadınız";
                }
                //nonWorkedDaysList = listdays(veriler.Default.nonworkdays);
                //veriler.Default.lastNonWorked = Convert.ToDateTime(veriler.Default.nonworkdays.Split('|').Last()).Date;
                nonWorkedDaysList = new List<DateTime>();
                calculateNonWorked();
                veriler.Default.Save();
                textNotlariniz.Text = veriler.Default.not;
                lblTotalDays.Text = calculateDays().ToString();
                lblWorkedDays.Text = calculateWorked().ToString();
                lblNonWorkedDays.Text = (veriler.Default.nonworkeddays = nonWorkedDaysList.Count).ToString();
                lblCombo.Text = calculateCombo().ToString();
                lblFlowerStatus.Text = "Çiçek Durumu:  " + calculateState().ToString() + " / 30";
                radProgressBar1.Value1 = calculatePoint();
            });            
        }
        private async void radTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                textNotlariniz.Text += "\r\n";
                textNotlariniz.Select(textNotlariniz.Text.Length, 0);
            }
        }

        private async void textNotlariniz_TextChanged(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                veriler.Default.not = textNotlariniz.Text;
                veriler.Default.Save();
            });
        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private System.Diagnostics.Process _keyboard = null;
        private async void textNotlariniz_Leave(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (_keyboard != null)
                {
                    _keyboard.Kill();
                    _keyboard.Dispose();
                    _keyboard = null;
                }
            });
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (_keyboard == null)
            {
                _keyboard = System.Diagnostics.Process.Start("osk.exe");
            }
        }
        private int calculateCombo()
        {
            if(workedDaysList.Count != 0)
            {
                int fark = (DateTime.Now.Date - veriler.Default.lastWorked.Date).Days;
                if (fark == 0 || fark == 1)
                {
                    if(nonWorkedDaysList.Count != 0)
                    {
                        veriler.Default.combo = (veriler.Default.lastWorked - nonWorkedDaysList.Last()).Days;
                        return (veriler.Default.lastWorked - nonWorkedDaysList.Last()).Days;
                    }
                    else
                    {
                        veriler.Default.combo = (veriler.Default.lastWorked - veriler.Default.startday).Days+1;
                        return (veriler.Default.lastWorked - veriler.Default.startday).Days+1;
                    }
                }
                else
                {
                    veriler.Default.combo = 0;
                    veriler.Default.lastNonWorked = DateTime.Now.AddDays(-1).Date;
                    return 0;
                }
            }
            else
            {
                veriler.Default.combo = 0;
                return 0;
            }
            
        }
        private async void btnIWorked_Click(object sender, EventArgs e)
        {
            if(veriler.Default.workdays == "")
            {
                veriler.Default.workdays = DateTime.Now.Date.ToString();
            }
            else
            {
                veriler.Default.workdays += "|" + DateTime.Now.Date.ToString();
            }            
            await updateAll();
            updateImage();
            //lblDoYouWorked.Text = "Bugün Çalıştınız :)";
            //veriler.Default.lastWorked = DateTime.Now.Date;
            //veriler.Default.workeddays++;
            //veriler.Default.combo++;
            //await updateFiles(workedDaysList, DateTime.Now.Date);
            //await updateAll();
            //updateImage();
            //veriler.Default.Save();
            //btnIWorked.Visible = false;
            //calculateNonWorked();
        }

        private async Task updateFiles(List<DateTime> list, DateTime date)
        {
            await Task.Run(() =>
            {
                if (list == workedDaysList)
                {
                    if(veriler.Default.workdays == "")
                    {
                        veriler.Default.workdays = date.Date.ToString();
                    }
                    workedDaysList.Add(date.Date);
                }
                else if (list == nonWorkedDaysList)
                {
                    if(veriler.Default.nonworkdays == "")
                    {
                        veriler.Default.nonworkdays = date.Date.ToString();
                    }
                    else
                    {
                        veriler.Default.nonworkdays += "|" + date.Date.ToString();
                    }                    
                    nonWorkedDaysList.Add(date.Date);
                }
            });
        }

        private int calculateWorked()
        {
            if(workedDaysList.Count != 0)
            {
                int result = (DateTime.Now.Date - veriler.Default.startday).Days - nonWorkedDaysList.Count;
                if (workedDaysList.Last().Date == DateTime.Now.Date)
                {
                    result++;
                }
                veriler.Default.workeddays = result;
                return result;
            }
            else
            {
                veriler.Default.workeddays = 0;
                return 0;
            }
        }

        private int calculateDays()
        {
            int days = veriler.Default.days = (DateTime.Now.Date - veriler.Default.startday).Days + 1;
            return veriler.Default.days;
        }

        private int calculatePoint()
        {
            int point = veriler.Default.workeddays - (veriler.Default.nonworkeddays * 2);
            if (point < 0) point = 0;
            if (point > 60) point = 60;
            veriler.Default.photoPoints = point;
            return point;
        }

        private void updateImage()
        {
            Bitmap bmp = new Bitmap(GetImage.Get(veriler.Default.photoNumber));
            pictureBox.Image = bmp;
        }

        private int calculateState()
        {
            int status = (veriler.Default.workeddays - (veriler.Default.nonworkeddays * 2)) / 2;
            if (status < 0) status = 0;
            if (status > 30) status = 30;
            veriler.Default.photoNumber = status;
            return status;
        }
        private void calculateNonWorked()
        {
            for(int i = 0; i < (DateTime.Now.Date-veriler.Default.startday.Date).Days; i++)
            {
                if (!workedDaysList.Contains(veriler.Default.startday.Date.AddDays(i)))
                {
                    nonWorkedDaysList.Add(veriler.Default.startday.Date.AddDays(i));
                }
            }
        }
        private List<DateTime> listdays(string str)
        {
            List<DateTime> list = new List<DateTime>();
            if (str != "")
            {
                string[] listArr = str.Split('|');
                foreach (string item in listArr)
                {
                    list.Add(Convert.ToDateTime(item));
                }
            }
            return list;
        }
        public DateTime day;
        private async void timer1_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (day.Date != DateTime.Now.Date)
                {
                    updateAll();
                    updateImage();
                    day = DateTime.Now.Date;
                }
            });
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            veriler.Default.startday = DateTime.Now.Date;
            veriler.Default.workdays = "";
            updateAll();
            updateImage();
            day = DateTime.Now.Date;
        }
    }
}