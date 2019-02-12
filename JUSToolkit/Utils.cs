namespace JUSToolkit
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public static class Utils
    {
        public static BinaryFormat Lzss(BinaryFormat bf, string mode)
        {
            string tempFile = Path.GetTempFileName();

            if (mode == "-d")
            {
                using (var substream = new DataStream(bf.Stream, 4, bf.Stream.Length - 4))
                {
                    substream.WriteTo(tempFile);
                }
            }
            else
            {
                using (var substream = new DataStream(bf.Stream, 0, bf.Stream.Length))
                {
                    substream.WriteTo(tempFile);
                }
            }



            string program = System.IO.Path.GetFullPath(@"..\..\") + @"\lib\NDS_Compressors_CUE\lzss.exe";

            string arguments = mode + " " + tempFile;
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
            if (mode != "-d")
            {
                memoryStream.Seek(0);
                memoryStream.Write(Encoding.ASCII.GetBytes("DSCP"), 0, 4);
            }

            fileStream.WriteTo(memoryStream);

            fileStream.Dispose();
            File.Delete(tempFile);

            return new BinaryFormat(memoryStream);
        }

    }
}
