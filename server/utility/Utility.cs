using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace ChatServer
{
    class Utility
    {
        static string path = @"C:\Program Files\Epic Games\UE_4.22\Engine\Build\BatchFiles\RunUAT.bat";
        static string Arguments = "BuildCookRun -project=D:\\ueprojecttest/MyProject/MyProject.uproject  -noP4 -platform=Android -clientconfig=Development -serverconfig=Development -cook -allmaps -stage -pak -archive";

        public static void CommandRun(string exe, string arguments)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = exe,
                    Arguments = arguments,
                    UseShellExecute = true,//true mean can launch .bat file
                };
                Process p = Process.Start(info);
                //p.WaitForExit();
            }
            catch (Exception e)
            {
                // Log.Error(e);
            }
        }
        public static bool IsValidJson(string Input)
        {
            Input = Input.Trim();
                try
                {
                    var obj = JToken.Parse(Input);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
        }
    }
    public static class ByteArraySerializer
    {
        public static byte[] Serialize<T>(this T m)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, m);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
