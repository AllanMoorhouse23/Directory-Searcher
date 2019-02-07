using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DirectorySearcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> _files = new List<string>();
        
        private void button1_Click(object sender, EventArgs e)
        {

            string directory;
            directory = textBoxSearchPath.Text;
            currentIndex = 0;
           
            backgroundSearcher.RunWorkerAsync(argument: directory);
        }

        int percentage;
        int currentIndex;
        int fileCount;
        string output;

        void DirSearch(string sDir)
        {
            try
            {
                
                backgroundSearcher.ReportProgress(percentage);
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        string test = f.Replace("\\" , @"/");
                       // Console.WriteLine(f);
                        //backgroundSearcher.ReportProgress();
                        output += test + "\r\n";
                       
                        currentIndex++;
                        percentage = (currentIndex + 1) * 100 / fileCount;
                    }
                    //Searches the sub folders by calling the search again
                    DirSearch(d);
                }

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Create Stream writer
            StreamWriter writer;
            //Catch any errors that may occur
            try
            {
                //Create filter for save dialog
                const string FILTER = "Text Files|*.txt|All Files|*.*";
                //Set filter for save dialog
                saveFileDialog1.Filter = FILTER;
                //Open save file dialog and if the ok button was pressed
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Sets the write location to the save dialog file name
                    writer = File.CreateText(saveFileDialog1.FileName);
                    //Write all of the formated text from the textbox to the file
                    writer.Write(textBoxOutput.Text);
                    //Close the file
                    writer.Close();
                }
            }
            //If any errors occured
            catch (Exception ex)
            {
                //Display error message
                MessageBox.Show(ex.Message);
            }
        }
        
        private void backgroundSearcher_DoWork(object sender, DoWorkEventArgs e)
        {
            string directory = (string)e.Argument;
            fileCount = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Length;
            //Console.WriteLine("File count: " + fileCount);
            output = "";
            DirSearch(directory);
        }

        private void backgroundSearcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Console.WriteLine("Done");
            lblFileCount.Text = fileCount.ToString();
            progressBar1.Value = 100;
            lblPercentage.Text = "100%";
            textBoxOutput.Text = output;
        }

        private void backgroundSearcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblPercentage.Text = e.ProgressPercentage.ToString() + "%";
            //Console.WriteLine(e.ProgressPercentage);
               
        }
    }
}
