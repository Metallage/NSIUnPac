using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NSIUnPack
{
    class Logica
    {
        private string tempPath;
        private string inPath ;
        private string pechPath;
        private string unknownPath;
        private string outPath ;
        private string archiver;
        private string regExpString;

        public Logica()
        {
            ParseSettings("Path.xml");
                //Создаём временную директорию, если её нет
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                //Создаём дирректорию вывода, если её нет
                if (!Directory.Exists(outPath))
                {
                    Directory.CreateDirectory(outPath);
                }

            
        }


        public void ExtractNSI()
        {
            List<archNSI> nsiFiles = new List<archNSI>();

            do
            {
                nsiFiles.Clear();
                DirectoryInfo nsiDir = new DirectoryInfo(inPath);
                if (nsiDir.Exists)
                {
                    FileInfo[] nsiArhFiles = nsiDir.GetFiles();

                    foreach (FileInfo nsiFI in nsiArhFiles)
                    {
                        if ((nsiFI.Extension.ToLower() == ".lzh") | (nsiFI.Extension.ToLower() == ".zip") | (nsiFI.Extension.ToLower() == ".rar")|(nsiFI.Extension.ToLower() == ".7z"))
                        {
                            nsiFiles.Add(new archNSI(nsiFI.FullName));
                        }
                        else if(nsiFI.Extension.ToLower() == ".(s)")
                        {
                            File.Delete(nsiFI.FullName);
                        }
                    }

                    foreach(archNSI arN in nsiFiles)
                    {
                        if(regExpString != null)
                        {
                            arN.LocalRegExp = regExpString;
                        }

                        arN.UnZipNSI(tempPath, outPath, archiver, pechPath, unknownPath);
                    }

                }
        }
            while (nsiFiles.Count>0);
            

        }


        //unzip test
        //public void UnZip()
        //{
        //    archNSI nsi1 = new archNSI(@"c:\temp\unp\in\mdp6406.lzh");
        //    bool isSuccess = nsi1.UnZipNSI(tempPath ,@"c:\temp\unp\out\");
        //}

        //for tests
        public void PrintFiles()
        {
            if(Directory.Exists(inPath))
            {

                string[] filesFound = Directory.GetFiles(inPath);

                foreach (string s in filesFound)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Не найдена дирректория");
            }

        }

        /// <summary>
        /// Парсит настройки из XML
        /// </summary>
        /// <param name="settingsPath">путь к xml файлу</param>
        private void ParseSettings(string settingsPath)
        {
            XmlDocument mySettings = new XmlDocument();
            mySettings.Load(settingsPath);
            XmlNodeList pathSettings = mySettings.DocumentElement.ChildNodes;
            foreach(XmlNode xmlPath in pathSettings)
            {
                switch (xmlPath.LocalName)
                {
                    case "inputPath":
                        inPath = xmlPath.InnerText;
                        break;
                    case "tempPath":
                        tempPath = xmlPath.InnerText;
                        break;
                    case "pechPath":
                        pechPath = xmlPath.InnerText;
                        break;
                    case "unknownPath":
                        unknownPath = xmlPath.InnerText;
                        break;
                    case "outputPath":
                        outPath = xmlPath.InnerText;
                        break;
                    case "archiver":
                        archiver = xmlPath.InnerText;
                        break;
                    case "regExp":
                        regExpString = xmlPath.InnerText;
                        break;
                }
            }
        }

        private void OperateNSI()
        {
            DirectoryInfo sourceDir = new DirectoryInfo(inPath);

            //Проверяем директорию с архивами
            if(sourceDir.Exists)
            {

            }
        }

    }
}
