using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BuildProto
{
    class Program
    {
        static string path = "ProtoDic.cs";       
        static string csNameSpce = "Proto";
        static string csClassName = "ProtoDic";
        static string csOutPutDir = "output";
        static string inputDir = "input";
        static string sourcePath = inputDir + "/" + "ProtoId.txt";
        static List<string> paths = new List<string>();
        static List<string> files = new List<string>();
        static void Main(string[] args)
        {
            files.Clear();
            string dir = System.Environment.CurrentDirectory;
            paths.Clear(); 
            files.Clear(); 
            Recursive(dir);

            string protogen = "C:/Users/liangjx/Desktop/protobuf-net-master (1)/protobuf-net-master/ProtoGen/bin/Debug/protogen.exe";

            Console.WriteLine("Starting Build Proto File...");
            foreach (string f in files)
            {
                string name = Path.GetFileName(f);
                string ext = Path.GetExtension(f);
                string prefix = name.Replace(ext, string.Empty);

                if (!ext.Equals(".proto"))
                {
                    continue;
                }
                if(Directory.Exists(csOutPutDir))
                {
                    DeleteFolder(csOutPutDir);
                }
                else
                {
                    Directory.CreateDirectory(csOutPutDir);
                }

                //------编译cs----------
                string argstr = " -i:" +inputDir + "/"+ prefix + ext + " -o:" + csOutPutDir + "/" + prefix + "_pb.cs";
                ExecuteOne(protogen, argstr.ToLower(), dir);
            }
            ReadProto();
        }

        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        static void Recursive(string path)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta")) continue;
                files.Add(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                paths.Add(dir.Replace('\\', '/'));
                Recursive(dir);
            }
        }

        static void ExecuteOne(string proc, string args, string dir)
        {
            Console.WriteLine(proc + " " + args);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = proc;
            info.Arguments = args;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;

            Process pro = Process.Start(info);
            pro.WaitForExit();
        }

        public static void ReadProto()
        {
            string protoText = "";
            protoText += "using System;\n";
            protoText += "using System.Collections.Generic;\n";
            protoText += string.Format("namespace {0}\n", csNameSpce);
            protoText += "{\n";
            protoText += string.Format("\tpublic class {0}\n", csClassName);
            protoText += "\t{\n";
            protoText += "\t\tpublic static List<int> _protoId = new List<int>() \n";
            protoText += "\t\t{\n";

            string[] lines = File.ReadAllLines(sourcePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("//") || line == "")
                {
                    continue;
                }
                string[] protoInfo = line.Split('=');
                if (protoInfo.Length < 2)
                {
                    continue;
                }
                protoText += string.Format("\t\t\t{0},\n", protoInfo[1].Trim());
            }
            protoText += "\t\t};\n";

            protoText += "\t\tpublic static List<Type> _protoType = new List<Type>() \n";
            protoText += "\t\t{\n";
            foreach (string line in lines)
            {
                if (line.StartsWith("//") || line == "")
                {
                    continue;
                }
                string[] protoInfo = line.Split('=');
                if (protoInfo.Length < 2)
                {
                    continue;
                }
                protoText += string.Format("\t\t\ttypeof({0}),\n", protoInfo[0].Trim());
            }
            protoText += "\t\t};\n";

            protoText += "\t\tpublic static Type GetProtoTypeByProtoId(int protoId) \n";
            protoText += "\t\t{\n";
            protoText += "\t\t\tint index = _protoId.IndexOf(protoId);\n";
            protoText += "\t\t\treturn _protoType[index];\n";
            protoText += "\t\t}\n";

            protoText += "\t\tpublic static int GetProtoIdByProtoType(Type type) \n";
            protoText += "\t\t{\n";
            protoText += "\t\t\tint index = _protoType.IndexOf(type);\n";
            protoText += "\t\t\treturn _protoId[index];\n";
            protoText += "\t\t}\n";

            protoText += "\t\tpublic static bool ContainProtoId(int protoId) \n";
            protoText += "\t\t{\n";
            protoText += "\t\t\tif(_protoId.Contains(protoId))\n";
            protoText += "\t\t\t{\n";
            protoText += "\t\t\t\treturn true;\n";
            protoText += "\t\t\t}\n";
            protoText += "\t\t\treturn false;\n";
            protoText += "\t\t}\n";

            protoText += "\t\tpublic static bool ContainProtoType(Type type) \n";
            protoText += "\t\t{\n";
            protoText += "\t\t\tif(_protoType.Contains(type))\n";
            protoText += "\t\t\t{\n";
            protoText += "\t\t\t\treturn true;\n";
            protoText += "\t\t\t}\n";
            protoText += "\t\t\treturn false;\n";
            protoText += "\t\t}\n";

            protoText += "\t}\n";
            protoText += "}\n";
            WriteProto(protoText);
        }

        public static void WriteProto(string text)
        {
            string dirPaht = csOutPutDir + "/" + path;
            if (Directory.Exists(csOutPutDir))
            {
                Directory.CreateDirectory(csOutPutDir);
            }

            FileStream fs = new FileStream(dirPaht, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(text);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
