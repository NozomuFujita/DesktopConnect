using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DisplaySystem
{
    public class UseWindowManagment
    {
        private string filePath;


        public UseWindowManagment()
        {
            filePath = Path.Combine(Application.streamingAssetsPath + "\\Window", "ExclusionList_Default.csv");
        }


        public List<string> ReadExclusionList()
        {
            var exclusionWindow = new List<string>();

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {

                while (!sr.EndOfStream)
                {
                    string className = sr.ReadLine();
                    className = className.Trim();
                    if(className != string.Empty)
                    {
                        exclusionWindow.Add(className);
                    }
                }
            }

            return exclusionWindow;
        }

        public void WriteExclusionList(string className)
        {
            UnityEngine.Debug.Log("USE");

            using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using (var sw = new StreamWriter(fs))
            {
                className = className.Trim();
                sw.WriteLine(className);
            }
        }
    }
}
