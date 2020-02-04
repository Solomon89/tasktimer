using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CalculatorOfWorkingTime
{
    public partial class Form1 : Form
    {

        int id;
        int hightMultiplaer;
        DateTime LastOn;
        DateTime LabelMeaning;
        List<progect> ListOfProgects;
        string plaseToSave;

        public Form1()
        {
            InitializeComponent();
            ListOfProgects = new List<progect>();
            label1.Text = new DateTime().TimeOfDay.ToString();
            plaseToSave = LoadPlaseOfPath();
            LoadToDayTime(DateTime.Now.Date.ToString().Replace(".", "-").Substring(0, 10));
        }
        private string LoadPlaseOfPath()
        {
            string filePath = Environment.CurrentDirectory.ToString();
            filePath += "\\settings.ini";
            string returnedPlase = "";
            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    string st = sr.ReadLine();
                    if (st.Contains("Path"))
                    {
                        returnedPlase = st.Substring(st.IndexOf("]")+1, st.IndexOf(";") - 1 - st.IndexOf("]"));

                    }
                }

            }
            else
            {
                returnedPlase = ShowBroseDialog();
            }
            return returnedPlase;

        }
        private void LoadToDayTime(string Date)
        {
            string file = plaseToSave + Date + ".tst";
            if (File.Exists(file))
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    string st = sr.ReadLine();
                    DateTime dt = Convert.ToDateTime(st);
                    label1.Text = dt.TimeOfDay.ToString();
                    LabelMeaning = dt;
                }
            }

            file = plaseToSave + "\\work.tst";
            if (File.Exists(file))
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    while (true)
                    {
                        string st = sr.ReadLine();
                        if (st==null)
                        {
                            break;
                        }
                        if (st.Contains("@"))
                        {
                            id++;
                            string nameofProject = st.Replace("@", "");
                            st = sr.ReadLine();
                            DateTime StartDate = Convert.ToDateTime(st);
                            st = sr.ReadLine();
                            string timeToProgect = st;
                            st = sr.ReadLine();
                            bool finished = Convert.ToBoolean(st);
                            string Interesing = sr.ReadLine();
                            string Hard = sr.ReadLine();
                            string Depressing = sr.ReadLine();
                            progect CurrentProject = new progect(nameofProject, StartDate, timeToProgect,id,finished,Interesing,Hard,Depressing);
                            ListOfProgects.Add(CurrentProject);
                            if (!CurrentProject.Finished)
                            {
                                List<Control> Controls = GetExampleOfControls(id, true);
                                ReplaseTextofControls(Controls, CurrentProject);
                                this.panel1.Controls.AddRange(Controls.ToArray());
                            }
                        }
                    }

                }
            }

        }
        private void ReplaseTextofControls(List<Control> Controls, progect CurrentProject)
        {
            foreach (Control item in Controls)
            {
                if (item.Name.Contains("TB_NameOfProgect"))
                {
                    item.Text = CurrentProject.ProjectName;
                }
                if (item.Name.Contains("TB_Interesing"))
                {
                    item.Text = CurrentProject.Interesing;
                }
                if (item.Name.Contains("TB_Hard"))
                {
                    item.Text = CurrentProject.Hard;
                }
                if (item.Name.Contains("TB_Depressing"))
                {
                    item.Text = CurrentProject.Depressing;
                }
                if (item.Name.Contains("LB"))
                {
                    item.Text = CurrentProject.reternTimeString();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switchTimer();
        }
        private void switchTimer()
        {
            bool timerOn = timer1.Enabled;
            timerOn = !timerOn;
            if (timerOn)
            {
                button1.Text = "Выключить";
                this.BackColor = Color.FromArgb(171, 177, 167);
                LastOn = Convert.ToDateTime(label1.Text);
            }
            else
            {
                button1.Text = "Включить";
                this.BackColor = Color.White;
                DateTime NowTime = Convert.ToDateTime(label1.Text);
                label2.Text = NowTime.Subtract(LastOn).ToString();
                swichOffAllTimers();
            }

            timer1.Enabled = timerOn;
        }     
        private void switchTimer(Button CurrentButton)
        {
            bool timerOn = timer1.Enabled;
            CurrentButton.Text = (CurrentButton.Text == "Запустить проект") ? "Остановить проект" : "Запустить проект";
            if (!timerOn)
            {
                switchTimer();
            }
            int idOfControl = Convert.ToInt32(CurrentButton.Name.Replace("BT_StartStopProgect", ""));
            switchCurrentProgect(idOfControl);
        }
        private void switchCurrentProgect(int idOfControl)
        {
            List<progect> ListCurrentProgect = ListOfProgects.Where(item => item.idOfConttrols == idOfControl).ToList();
            progect CurrentProject = ListCurrentProgect[0];
            CurrentProject.Switch = !CurrentProject.Switch; 
        }
        private void swichOffAllTimers()
        {
            List<progect> ListOnProjects = ListOfProgects.Where(item => item.Switch == true).ToList();
            foreach (progect item in ListOnProjects)
            {
                item.Switch = false;
            }
            foreach (Control item in panel1.Controls)
            {
                if (item.Text == "Остановить проект" )
                {
                    item.Text = "Запустить проект";
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                DateTime DateTime = Convert.ToDateTime(label1.Text);
                DateTime = DateTime.AddSeconds(1);
                label1.Text = DateTime.TimeOfDay.ToString();
                LabelMeaning = DateTime;
                tickProgects();
            }
        }
        private void tickProgects()
        {
            List<progect> onlineProgects = ListOfProgects.Where(item => item.Switch == true).ToList();
            foreach (progect item in onlineProgects)
            {
                item.newSecond();
                List<Control> ListOfCurrentLB = panel1.Controls.Find("LB_Timer" + item.idOfConttrols.ToString(), true).ToList();
                Label CurrentLabel = (Label)ListOfCurrentLB[0];
                CurrentLabel.Text = item.reternTimeString();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string save = LabelMeaning.ToString();
            string fileName = DateTime.Now.Date.ToString().Replace(".", "-").Substring(0, 10);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter( plaseToSave + fileName + ".tst"))
            {
                file.Write(save);
            }
            save = "";
            foreach (progect item in ListOfProgects)
            {
                save += item.returnStringOfProgect();
            }
            fileName = plaseToSave +"\\work.tst";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.Write(save);
            }
            fileName = Environment.CurrentDirectory.ToString() + "\\settings.ini";
            save = "[Path]" + plaseToSave + ";";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.Write(save);
            }
        }
        private List<Control> GetExampleOfControls(int id,bool EnabledButton)
        {
            int left = 0;
            hightMultiplaer++;
            int top = hightMultiplaer * 25;
            List<Control> ReturnedControls = new List<Control>();
            TextBox TB_NameOfProgect = new TextBox
            {
                Name = "TB_NameOfProgect" + id.ToString(),
                Width = 400,
                Top = top,
               
            };
            TB_NameOfProgect.LostFocus += TB_NameOfProgect_LostFocus;
            TB_NameOfProgect.KeyUp += TB_NameOfProgect_KeyUp;
            ReturnedControls.Add(TB_NameOfProgect);

            left += TB_NameOfProgect.Width + 10;

            TextBox TB_Interesing = new TextBox
            {
                Name = "TB_Interesing_" + id.ToString(),
                Width = 60,
                Top = top,
                
                Left = left
            };
            TB_Interesing.LostFocus += LostFocusTB;
            ReturnedControls.Add(TB_Interesing);

            left += TB_Interesing.Width + 10;

            TextBox TB_Hard = new TextBox
            {
                Name = "TB_Hard_" + id.ToString(),
                Width = 60,
                Top = top,
               
                Left = left
            };
            TB_Hard.LostFocus += LostFocusTB;
            ReturnedControls.Add(TB_Hard);

            left += TB_Hard.Width + 10;

            TextBox TB_Depressing = new TextBox
            {
                Name = "TB_Depressing_" + id.ToString(),
                Width = 60,
                Top = top,
                
                Left = left
            };
            TB_Depressing.LostFocus += LostFocusTB;
            ReturnedControls.Add(TB_Depressing);

            left += TB_Depressing.Width + 10;

            Label LB_Timer = new Label();
            LB_Timer.Name = "LB_Timer" + id.ToString();
            LB_Timer.Text = "00:00:0000";
            LB_Timer.Top = top;
            LB_Timer.Left = left;

            ReturnedControls.Add(LB_Timer);

            left += LB_Timer.Width + 10;

            Button BT_StartStopProgect = new Button();
            BT_StartStopProgect.Text = "Запустить проект";
            BT_StartStopProgect.Click += new EventHandler(BT_Click);
            BT_StartStopProgect.Left = left;
            BT_StartStopProgect.Name = "BT_StartStopProgect" + id.ToString();
            BT_StartStopProgect.Top = top;
            BT_StartStopProgect.Enabled = EnabledButton;
            ReturnedControls.Add(BT_StartStopProgect);

            left += BT_StartStopProgect.Width + 10;

            Button BT_Finish = new Button();
            BT_Finish.Text = "Закончить";
            BT_Finish.Click += new EventHandler(BT_Finish_Click);
            BT_Finish.Left = left;
            BT_Finish.Name = "BT_Finish" + id.ToString();
            BT_Finish.Top = top;
            BT_Finish.Enabled = EnabledButton;
            ReturnedControls.Add(BT_Finish);

            left += BT_Finish.Width + 10;

            panel1.Height += 25;
            this.Height += 25;

            return ReturnedControls;
        }
        private void BT_Finish_Click(object sender, EventArgs e)
        {
            Button BT = (Button)sender;
            int idControl = Convert.ToInt32(BT.Name.Replace("BT_Finish", ""));
            progect CurrentProject = ListOfProgects.Where(item => item.idOfConttrols == idControl).ToList()[0];
            CurrentProject.Finished = !CurrentProject.Finished;
            if (CurrentProject.Finished)
            {
                BT.BackColor = Color.Gold;
            }
            else
            {
                BT.BackColor = Color.White;   
            }
                    
        }
        private void TB_NameOfProgect_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            int idControl = Convert.ToInt32(TB.Name.Replace("TB_NameOfProgect", ""));
            Control[] s = panel1.Controls.Find("BT_StartStopProgect" + idControl, true);
            s[0].Enabled = (TB.Text != "") ? true : false;
            s = panel1.Controls.Find("BT_Finish" + idControl, true);
            s[0].Enabled = (TB.Text != "") ? true : false;
        }
        private void TB_NameOfProgect_LostFocus(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (TB.Text != "")
            {
                TB.Enabled = false;
                int idControl = Convert.ToInt32(TB.Name.Replace("TB_NameOfProgect", ""));
                CreateNewProgect(TB.Text,idControl); 
            }

        }
        private void CreateNewProgect(string ProjectName,int idControl)
        {
            progect NewProject = new progect(ProjectName, DateTime.Now,idControl);
            ListOfProgects.Add(NewProject);
        }
        private void BT_Click(object sender, EventArgs e)
        {
            Button bt = (sender as Button);
            switchTimer(bt);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            id++;
            List<Control> Controls = GetExampleOfControls(id,false);
            this.panel1.Controls.AddRange(Controls.ToArray());
        }
        private void SaveRezults_Click(object sender, EventArgs e)
        {
            plaseToSave = ShowBroseDialog();          
        }
        private string ShowBroseDialog()
        {
            FormToSavePlase FTSP = new FormToSavePlase(plaseToSave);
            FTSP.ShowDialog();
            return FTSP.SavePath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void LostFocusTB(object sender, EventArgs e)
        {
            TextBox CurrentTB = (TextBox)sender;
            int startType = CurrentTB.Name.IndexOf("_");
            int endType = CurrentTB.Name.IndexOf("_", startType+1);
            string type = CurrentTB.Name.Substring(startType+1, endType - startType-1);
            int id = int.Parse(CurrentTB.Name.Replace("TB_" + type + "_", ""))-1;
            ListOfProgects[id].GetType().GetProperty(type).SetValue(ListOfProgects[id], CurrentTB.Text,null);
 
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
    public class progect
    {
        public string ProjectName { get; set; }
        public bool Switch { get; set; }
        public DateTime DateOfStart { get; set; }
        public string Interesing { get; set; }
        public string Hard { get; set; }
        public string Depressing { get; set; }
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }
        public int Days { get; set; }
        public int idOfConttrols { get; set; }
        public bool Finished { get; set; }

        public progect(string _Name, DateTime _DateOfStart,int _idOfControls)
        {
            this.ProjectName = _Name;
            this.DateOfStart = _DateOfStart;
            this.idOfConttrols = _idOfControls;
            this.Seconds = 0;
            this.Minutes = 0;
            this.Hours = 0;
            this.Days = 0;
            this.Finished = false;
            
        }
        public progect(string _Name, DateTime _DateOfStart, string _DateOfProgect, int _idOfControl, bool _Finished, string Interesing, string Hard, string Depressing)
        {
            this.ProjectName = _Name;
            this.DateOfStart = _DateOfStart;
            this.idOfConttrols = _idOfControl;
            this.Seconds = Convert.ToInt32(_DateOfProgect.Substring(10, 2));
            this.Minutes = Convert.ToInt32(_DateOfProgect.Substring(7, 2));
            this.Hours = Convert.ToInt32(_DateOfProgect.Substring(4, 2));
            this.Days = Convert.ToInt32(_DateOfProgect.Substring(0, 3));
            this.Finished = _Finished;
            this.Interesing = Interesing;
            this.Hard = Hard;
            this.Depressing = Depressing;

        }
        public string newSecond()
        {
            Seconds++;
            if (Seconds >= 60)
            {
                Minutes++;
                Seconds = 0;
            }
            if (Minutes >= 60)
            {
                Hours++;
                Minutes = 0;
            }
            if (Hours >= 24)
            {
                Days++;
                Hours = 0;
            }
            return Hours.ToString() + ":" + Minutes.ToString() + ":" + Seconds.ToString() + " Дней:" + Days.ToString();
        }
        public string returnStringOfProgect()
        {
            string returnStringOfProgect = "";
            returnStringOfProgect += "@" + this.ProjectName + "\r\n";
            returnStringOfProgect += DateOfStart.ToString() + "\r\n";
            returnStringOfProgect += reternTimeString() + "\r\n";
            returnStringOfProgect += Finished + "\r\n";
            returnStringOfProgect += Interesing + "\r\n";
            returnStringOfProgect += Hard + "\r\n";
            returnStringOfProgect += Depressing + "\r\n";
            return returnStringOfProgect;
        }
        public string reternTimeString()
        {
            string returnString = "";
            if (Days < 100)
            {
                returnString += "0";
                if (Days < 10)
                {
                    returnString += "0";
                }
            }
            returnString += Days.ToString();
            returnString += ":";
            if (Hours < 10)
            {
                returnString += "0";
            }
            returnString += Hours.ToString();
            returnString += ":";
            if (Minutes < 10)
            {
                returnString += "0";
            }
            returnString += Minutes.ToString();
            returnString += ":";
            if (Seconds < 10)
            {
                returnString += "0";
            }
            returnString += Seconds.ToString();
            return returnString;
        }
    }
}