using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Injector
{
	public class Form1 : Form
	{
		private string line = null;

		private string myDoc = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\inj.txt";

		private string dllFile;

		private IContainer components = null;

		private Button button1;

		private Button button2;

		private OpenFileDialog openFileDialog1;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (!File.Exists(myDoc))
			{
				File.Create(myDoc).Dispose();
			}
			using (StreamReader streamReader = new StreamReader(myDoc, Encoding.Default))
			{
				line = streamReader.ReadLine();
				Console.WriteLine(line);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = line,
				RestoreDirectory = true,
				Title = "Select DLL File",
				DefaultExt = "dll",
				Filter = "Dll Files (*.dll)|*.dll",
				FilterIndex = 1,
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (StreamWriter streamWriter = File.CreateText(myDoc))
				{
					streamWriter.WriteLine(openFileDialog.FileName);
				}
				dllFile = openFileDialog.FileName;
				string fileName = Path.GetFileName(openFileDialog.FileName);
				button2.Text = fileName;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (button2.Text == "Select DLL")
			{
				MessageBox.Show("No DLL Selected, Please select a DLL", "Error");
				return;
			}
			
			Process emb =  System.Diagnostics.Process.Start("emb.exe");
			
			Thread.Sleep(1000);
		
			string strDLLName = dllFile;
			string proc = "csgo";
			int processId = GetProcessId(proc);
			if (processId >= 0)
			{
				IntPtr hProcess = OpenProcess(2035711u, 1, processId);
				bool flag = false;
				InjectDLL(hProcess, strDLLName);
			}
		}

		[DllImport("kernel32")]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll")]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
		internal static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

		public int GetProcessId(string proc)
		{
			try
			{
				Process[] processesByName = Process.GetProcessesByName(proc);
				return processesByName[0].Id;
			}
			catch (IndexOutOfRangeException)
			{
				MessageBox.Show("Prozess nicht gefunden!", "Error");
			}
			return -1;
		}

		public void InjectDLL(IntPtr hProcess, string strDLLName)
		{
			int num = strDLLName.Length + 1;
			IntPtr intPtr = VirtualAllocEx(hProcess, (IntPtr)null, (uint)num, 4096u, 64u);
			WriteProcessMemory(hProcess, intPtr, strDLLName, (UIntPtr)(ulong)num, out IntPtr lpNumberOfBytesWritten);
			UIntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
			bool flag = false;
			IntPtr intPtr2 = CreateRemoteThread(hProcess, (IntPtr)null, 0u, procAddress, intPtr, 0u, out lpNumberOfBytesWritten);
			bool flag2 = false;
			int num2 = WaitForSingleObject(intPtr2, 10000);
			if ((long)num2 == 128 || (long)num2 == 258 || num2 == uint.MaxValue)
			{
				MessageBox.Show(" hThread [ 2 ] Error! \n ");
				bool flag3 = true;
				CloseHandle(intPtr2);
			}
			else
			{
				Thread.Sleep(1000);
				VirtualFreeEx(hProcess, intPtr, (UIntPtr)0uL, 32768u);
				bool flag4 = true;
				CloseHandle(intPtr2);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
		}

		private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(376, 62);
            this.button1.TabIndex = 0;
            this.button1.Text = "Inject";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(376, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Select DLL";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 113);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "TrustFucker - Injector";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

		}
	}
}
