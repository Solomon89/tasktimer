using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CalculatorOfWorkingTime
{
    public partial class FormToSavePlase : Form
    {
        public string SavePath { get; set; }
        public FormToSavePlase(string filePath)
        {
            InitializeComponent();

            textBox1.Text = (filePath != "") ? filePath : Environment.CurrentDirectory.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.SelectedPath = (textBox1.Text != "") ? textBox1.Text : Environment.CurrentDirectory.ToString();
            FBD.ShowDialog();
            textBox1.Text = FBD.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tryToSave();
        }

        private void tryToSave()
        {

            if (textBox1.Text != "")
            {
                SavePath = textBox1.Text;
                this.Close();

            }
        }

    }
}
