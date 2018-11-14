namespace JUSToolkit
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public static class Utils
    {
        public static Node DecompressLzss(Node node)
        {
            string tempFile = Path.GetTempFileName();

            using (var substream = new DataStream(node.Stream, 4, node.Stream.Length - 4))
            {
                substream.WriteTo(tempFile);
            }

            string program = System.IO.Path.GetFullPath(@"..\..\") + @"\lib\NDS_Compressors_CUE\lzss.exe";

            string arguments = "-d " + tempFile;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                program = System.IO.Path.GetFullPath(@"../../") + "/lib/NDS_Compressors_CUE/lzss";
            }

            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            process.WaitForExit();

            DataStream fileStream = new DataStream(tempFile, FileOpenMode.Read);
            DataStream memoryStream = new DataStream();
            fileStream.WriteTo(memoryStream);

            fileStream.Dispose();
            File.Delete(tempFile);

            return new Node(node.Name, new BinaryFormat(memoryStream));
        }

        public static String DecompressLzssTemp(string file)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(file, tempFile, true);

            string program = "lzss.exe"; //*** Falta ruta no??
            string arguments = "-d " + tempFile;
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                program = "lzss";
            }

            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            process.WaitForExit();

            return tempFile;
        }

    }
}
