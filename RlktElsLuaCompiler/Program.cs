using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RlktElsLuaCompiler
{
    class Program
    {
        static string compiler_name = "luajit.exe";
        static string compiler_command = "-b {0} {1}";
        static string output_folder = "compiled_lua";

        static void InstallContextMenu()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            
            //File Open
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\.lua", "", "RlktLua", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\RlktLua\\shell\\RlktElsLuaCompilerOpen", "", "Open .Lua", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\RlktLua\\shell\\RlktElsLuaCompilerOpen\\command", "", string.Format("\"{0}\" \"open\" \"%1\"", path), RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\RlktLua\\shell\\RlktElsLuaCompilerX", "", "Compile lua", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\RlktLua\\shell\\RlktElsLuaCompilerX\\command", "", string.Format("\"{0}\" \"compile\" \"%1\"", path), RegistryValueKind.String);

            //Right click on folder
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_compile", "", "Compile to .LUA", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_compile\\command", "", string.Format("\"{0}\" compile-folder \"%v\"", path), RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_compile", "Icon", path, RegistryValueKind.String);

            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_compile", "", "Compile to .LUA", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_compile\\command", "", string.Format("\"{0}\" compile-folder \"%v\"", path), RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_compile", "Icon", path, RegistryValueKind.String);
        }

        static int Main(string[] args)
        {
            InstallContextMenu();

            if (args.Length != 2)
                return -1;

            string operation = args[0];
            if (operation == "compile-folder")
            {
                string folderpath = args[1];
                foreach(string file in Directory.EnumerateFiles(folderpath))
                {
                    if (file.EndsWith(".lua") == false)
                        continue;

                    string compilerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    if (!File.Exists(Path.Combine(compilerPath, compiler_name)))
                    {
                        MessageBox.Show(String.Format("Compiler not found."));
                        return -2;
                    }

                    string dirPath = Path.GetDirectoryName(file);
                    string fileName = Path.GetFileName(file);
                    string newFilePath = Path.Combine(dirPath, output_folder, fileName);
                    if (!Directory.Exists(Path.Combine(dirPath, output_folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(dirPath, output_folder));
                    }

                    Process process = new Process();
                    process.StartInfo.FileName = Path.Combine(compilerPath, compiler_name);
                    process.StartInfo.Arguments = string.Format(compiler_command, file, newFilePath);
                    process.StartInfo.WorkingDirectory = compilerPath;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();

                    string ErrMsg = process.StandardError.ReadToEnd();

                    process.WaitForExit();
                    int exitcode = process.ExitCode;
                    if (exitcode != 0)
                    {
                        MessageBox.Show(ErrMsg, fileName);
                    }
                }

            }
            else if (operation == "compile")
            {
                string filepath = args[1];

                if (!File.Exists(filepath))
                {
                    MessageBox.Show(String.Format("Invalid file {0}", filepath));
                    return -1;
                }
                string compilerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!File.Exists( Path.Combine(compilerPath, compiler_name) ))
                {
                    MessageBox.Show(String.Format("Compiler not found."));
                    return -2;
                }

                string dirPath = Path.GetDirectoryName(filepath);
                string fileName = Path.GetFileName(filepath);
                string newFilePath = Path.Combine(dirPath, output_folder, fileName); 
                if(!Directory.Exists( Path.Combine(dirPath, output_folder) ))
                {
                    Directory.CreateDirectory( Path.Combine(dirPath, output_folder) );
                }


                Process process = new Process();
                process.StartInfo.FileName = Path.Combine(compilerPath, compiler_name);
                process.StartInfo.Arguments = string.Format(compiler_command, filepath, newFilePath);
                process.StartInfo.WorkingDirectory = compilerPath;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                string ErrMsg = process.StandardError.ReadToEnd();

                process.WaitForExit();
                int exitcode = process.ExitCode;
                if (exitcode != 0)
                    MessageBox.Show(ErrMsg);

                return 0;
            }
            else
            {
                Process process = new Process();
                process.StartInfo.FileName = "code.exe";
                process.StartInfo.Arguments = args[1];
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.Start();
            }

            return 0;
        }
    }
}
