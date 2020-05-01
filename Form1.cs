using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObfuscatorTools
{
    public partial class Form1 : Form
    {
        private void TextBox1DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        string DirectoryName = "";
        string ExePath = "";
        private void TextBox1DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array arrayyy = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (arrayyy != null)
                {
                    string text = arrayyy.GetValue(0).ToString();
                    int num = text.LastIndexOf(".", StringComparison.Ordinal);
                    if (num != -1)
                    {
                        string text2 = text.Substring(num);
                        text2 = text2.ToLower();
                        if (text2 == ".exe" || text2 == ".dll")
                        {
                            Activate();
                            OriginalTextbox.Text = text;
                            int num2 = text.LastIndexOf("\\", StringComparison.Ordinal);
                            if (num2 != -1)
                            {
                                DirectoryName = text.Remove(num2, text.Length - num2);
                            }
                            if (DirectoryName.Length == 2)
                            {
                                DirectoryName += "\\";
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private void TextBox2DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array arrayyy = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (arrayyy != null)
                {
                    string text = arrayyy.GetValue(0).ToString();
                    int num = text.LastIndexOf(".", StringComparison.Ordinal);
                    if (num != -1)
                    {
                        string text2 = text.Substring(num);
                        text2 = text2.ToLower();
                        if (text2 == ".exe" || text2 == ".dll")
                        {
                            Activate();
                            ObfuscatedTextbox.Text = text;
                            int num2 = text.LastIndexOf("\\", StringComparison.Ordinal);
                            if (num2 != -1)
                            {
                                DirectoryName = text.Remove(num2, text.Length - num2);
                            }
                            if (DirectoryName.Length == 2)
                            {
                                DirectoryName += "\\";
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string application = OriginalTextbox.Text;
            string obfapplication = ObfuscatedTextbox.Text;
            if (application == string.Empty || obfapplication == string.Empty)
            {
                MessageBox.Show("Please drag 2 assemblies");
                return;
            }
                
            Stopwatch sw = Stopwatch.StartNew();
            Process process = Process.Start(application);
            process.WaitForInputIdle();
            process.Kill();
            StartupLabel.Text = sw.Elapsed.TotalSeconds.ToString() + " s";
            sw.Stop();
            Stopwatch sw2 = Stopwatch.StartNew();
            Process process2 = Process.Start(obfapplication);
            process2.WaitForInputIdle();
            process2.Kill();
            sw2.Stop();
            StartupLabel2.Text = sw2.Elapsed.TotalSeconds.ToString() + " s";
            long OriginalSize = new System.IO.FileInfo(application).Length / 1000;
            long ObfuscatedSize = new System.IO.FileInfo(obfapplication).Length / 1000;
            SizeLabel.Text = (OriginalSize).ToString() + " kb";
            SizeLabel2.Text = (ObfuscatedSize).ToString() + " kb";
            double SizeRatio = (double)ObfuscatedSize / (double)OriginalSize;
            CheckRatio(SizeRatio, SRatio, 20);
            SRatio.Text = (Math.Round(SizeRatio, 2) * 100).ToString() + " %";
            CheckRatio(Math.Round(sw2.Elapsed.TotalSeconds / sw.Elapsed.TotalSeconds, 2), StRatio, 3);
            StRatio.Text = (Math.Round(sw2.Elapsed.TotalSeconds / sw.Elapsed.TotalSeconds, 2) * 100).ToString() + " %";
            double TypeRatio = 0;
            double MethodRatio = 0;
            double FieldRatio = 0;
            double ResRatio = 0;
            double InstrRatio = 0;
            double LocalRatio = 0;
            try
            {
                ModuleDefMD module = ModuleDefMD.Load(application);

                TypeRatio = module.Types.Count;
                ResRatio = module.Resources.Count;
                foreach(TypeDef type in module.Types)
                {
                    MethodRatio += type.Methods.Count;
                    FieldRatio += type.Fields.Count;
                    
                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.HasBody && method.Body.HasVariables)
                        {
                            LocalRatio += method.Body.Variables.Count;
                        }
                        
                        if (method.HasBody && method.Body.HasInstructions)
                        {
                            InstrRatio += method.Body.Instructions.Count;
                        }
                    }
                }
                LocalCount.Text = LocalRatio.ToString();
                TypeCount.Text = TypeRatio.ToString();
                MethodCount.Text = MethodRatio.ToString();
                FieldCount.Text = FieldRatio.ToString();
                InstructionCount.Text = InstrRatio.ToString();
                ResourceCount.Text = ResRatio.ToString();
            }
            catch
            {

            }
            try
            {
                ModuleDefMD obfmodule = ModuleDefMD.Load(obfapplication);
                int TypesCount = obfmodule.Types.Count;
                int MethodsCount = 0;
                int FieldsCount = 0;
                int InstructionsCount = 0;
                int ResourcesCount = obfmodule.Resources.Count;
                int LocalCounts = 0;
                foreach (TypeDef type in obfmodule.Types)
                {
                    MethodsCount += type.Methods.Count;
                    FieldsCount += type.Fields.Count;
                    foreach (MethodDef method in type.Methods)
                    {
                        if(method.HasBody && method.Body.HasVariables)
                        {
                            LocalCounts += method.Body.Variables.Count;
                        }
                   
                        if (method.HasBody && method.Body.HasInstructions)
                        {
                            InstructionsCount += method.Body.Instructions.Count;
                        }
                      
                    }
                }
                LocalCount2.Text = LocalCounts.ToString();
                TypeCount2.Text = TypesCount.ToString();
                MethodCount2.Text = MethodsCount.ToString();
                FieldCount2.Text = FieldsCount.ToString();
                InstructionCount2.Text = InstructionsCount.ToString();
                ResourceCount2.Text = ResourcesCount.ToString();

                CheckRatio(Math.Round(LocalCounts / LocalRatio, 2), TRatio, 70);
                CheckRatio(Math.Round(TypesCount / TypeRatio, 2), TRatio, 50);
                CheckRatio(Math.Round(MethodsCount / MethodRatio, 2), MRatio, 40);
                CheckRatio(Math.Round(FieldsCount / FieldRatio, 2), FRatio, 35);
                CheckRatio(Math.Round(ResourcesCount / ResRatio, 2), RRatio, 10);
                CheckRatio(Math.Round(InstructionsCount / InstrRatio, 2), IRatio, 50);
                LoRatio.Text = (Math.Round(LocalCounts / LocalRatio, 2) * 100).ToString() + " %";
                TRatio.Text = (Math.Round(TypesCount / TypeRatio, 2) * 100).ToString() + " %";
                MRatio.Text = (Math.Round(MethodsCount / MethodRatio, 2) * 100).ToString() + " %";
                FRatio.Text = (Math.Round(FieldsCount / FieldRatio,2) * 100).ToString() + " %";
                RRatio.Text = (Math.Round(ResourcesCount / ResRatio,2) * 100).ToString() + " %";
                IRatio.Text = (Math.Round(InstructionsCount / InstrRatio, 2) * 100).ToString() + " %";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
        }
        public static void CheckRatio(double value, Label label, int LimitValue)
        {
            if (value >= LimitValue)
            {
                label.ForeColor = Color.Red;
            }
            else
            {
                label.ForeColor = Color.Black;
            }
        }
        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void ResourceCount_Click(object sender, EventArgs e)
        {

        }

        private void FieldCount_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void MethodCount_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void TypeCount_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void StartupLabel_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
