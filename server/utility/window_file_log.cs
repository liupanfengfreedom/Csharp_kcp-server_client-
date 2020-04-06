using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace ChatServer
{
public class window_file_log {
    static string newFullPath="";
    static int i = 0;
    // Use this for initialization
    /// <summary>
    /// A static constructor is called automatically to initialize the class before the first instance is created or any static members are referenced.
    /// </summary>
    static window_file_log()
    {

        i++;
        string fileNameOnly = "/mylog";
        string extension = ".log";
        string path = AppDomain.CurrentDomain.BaseDirectory;
        int count = 1;
tryagain:
        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        newFullPath = path + tempFileName + extension;
        if (File.Exists(newFullPath))
        {
            goto tryagain;
        }
        else {
            using (StreamWriter sw = File.CreateText(newFullPath))
            {
                sw.WriteLine("log begin");
                sw.WriteLine("..............");
            } // Create a file to write to.
        }
    }
    public static void Log(string s) {

        using (StreamWriter sw = File.AppendText(newFullPath))
        {
            sw.WriteLine(DateTime.Now + " : "+s);
        }
    }

    }
}