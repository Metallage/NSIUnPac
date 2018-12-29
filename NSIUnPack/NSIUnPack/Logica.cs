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
        private string valutaOut;
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

            //создаём директорию для PECH
            if (!Directory.Exists(pechPath))
            {
                Directory.CreateDirectory(pechPath);
            }

            //Создаём директорию для неизвестных файлов
            if (!Directory.Exists(unknownPath))
            {
                Directory.CreateDirectory(unknownPath);
            }

        }


        public void ExtractNSI()
        {
            List<archNSI> nsiFiles = new List<archNSI>();
            DirectoryInfo nsiDir = new DirectoryInfo(inPath);
            ValutaCop(valutaOut);

            do //Циклично проверяем директорию на наличие архивов
            {
                nsiFiles.Clear();

                if (nsiDir.Exists)
                {
                    // Получеам список файлов
                    FileInfo[] nsiArhFiles = nsiDir.GetFiles();

                    foreach (FileInfo nsiFI in nsiArhFiles)
                    {
                        if ((nsiFI.Extension.ToLower() == ".lzh") | (nsiFI.Extension.ToLower() == ".zip") | (nsiFI.Extension.ToLower() == ".rar")|(nsiFI.Extension.ToLower() == ".7z"))
                        {
                            //Добавляем файл в очередь на распаковку
                            nsiFiles.Add(new archNSI(nsiFI.FullName));
                        }
                        else if(nsiFI.Extension.ToLower() == ".(s)")
                        {
                            // Удаляем сопроводиловки (их никто не читает)
                            File.Delete(nsiFI.FullName);
                        }
                    }

                    foreach(archNSI arN in nsiFiles)
                    {
                        if(regExpString != null)
                        {
                            //Если есть отдельное регулярное выражение в xml, то используем её
                            arN.LocalRegExp = regExpString;
                        }

                        arN.UnZipNSI(tempPath, outPath, archiver, pechPath, unknownPath);
                    }

                }
            }
            while (nsiFiles.Count>0);
        }


        /// <summary>
        /// Копирует валюту Надо тестить
        /// </summary>
        /// <param name="outputVal">Куда копируем</param>
        private void ValutaCop(string outputVal)
        {
            DateTime currentDate = DateTime.Now;
            string incomeValDir = inPath + currentDate.Day.ToString() + "_" + currentDate.Month.ToString() + @"\";
            string valFileName = @"VAL" + currentDate.Day.ToString() + currentDate.Month.ToString() + ".txt";
            if(File.Exists(incomeValDir+valFileName))
            {
                File.Copy(incomeValDir + valFileName, outputVal + currentDate.Year.ToString() + @"\" + currentDate.Month.ToString() + @"\" + valFileName, true);
            }
            if(Directory.Exists(incomeValDir))
            {
                Directory.Delete(incomeValDir, true);
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
                    case "valutaOut":
                        valutaOut = xmlPath.InnerText;
                        break;
                }
            }
        }

    }
}
