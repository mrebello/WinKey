using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace WinKey {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine(getVersion() + "\t" + GetWindowsProductKey());
            if (System.Diagnostics.Debugger.IsAttached) { Console.Write("Debug - aperte uma tecla para continuar."); Console.ReadKey(); }
        }

        // FROM Peter Panisz - Windows Key Viewer

        public static string DecodeProductKeyWin8AndUp(byte[] digitalProductId) {
            string text = string.Empty;
            byte b = (byte)((uint)((int)digitalProductId[66] / 6) & 1u);
            digitalProductId[66] = (byte)((digitalProductId[66] & 0xF7u) | (uint)((b & 2) * 4));
            int num = 0;
            for (int num2 = 24; num2 >= 0; num2--) {
                int num3 = 0;
                for (int num4 = 14; num4 >= 0; num4--) {
                    num3 *= 256;
                    num3 = digitalProductId[num4 + 52] + num3;
                    digitalProductId[num4 + 52] = (byte)(num3 / 24);
                    num3 %= 24;
                    num = num3;
                }
                text = "BCDFGHJKMPQRTVWXY2346789"[num3] + text;
            }
            string obj = text.Substring(1, num);
            string text2 = text.Substring(num + 1, text.Length - (num + 1));
            text = obj + "N" + text2;
            for (int i = 5; i < text.Length; i += 6) {
                text = text.Insert(i, "-");
            }
            return text;
        }



        public static string DecodeProductKey(byte[] digitalProductId) {
            char[] array = new char[29];
            List<byte> list = new List<byte>();
            for (int i = 52; i <= 67; i++) {
                list.Add(digitalProductId[i]);
            }
            for (int num = array.Length - 1; num >= 0; num--) {
                if ((num + 1) % 6 == 0) {
                    array[num] = '-';
                }
                else {
                    int num2 = 0;
                    for (int num3 = 14; num3 >= 0; num3--) {
                        int num4 = (num2 << 8) | list[num3];
                        list[num3] = (byte)(num4 / 24);
                        num2 = num4 % 24;
                        array[num] = "BCDFGHJKMPQRTVWXY2346789"[num2];
                    }
                }
            }
            return new string(array);
        }


        public static string getVersion() {
            string text = "";
            string text2 = (Environment.Is64BitOperatingSystem ? " 64-bit" : " 32-bit");
            try {
                text = new ComputerInfo().OSFullName;
            }
            catch {
                text = "";
                text2 = "";
            }
            return text + text2;
        }


        public static string GetWindowsProductKey() {
            byte[] digitalProductId = (byte[])RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("DigitalProductId");
            if ((Environment.OSVersion.Version.Major != 6 || Environment.OSVersion.Version.Minor < 2) && Environment.OSVersion.Version.Major <= 6) {
                return DecodeProductKey(digitalProductId);
            }
            return DecodeProductKeyWin8AndUp(digitalProductId);
        }
    }
}
