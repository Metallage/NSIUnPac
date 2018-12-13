using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSIUnPack
{
    class Logica
    {
        private string tempPath = @"c:\temp\nsi\";
        private string inPath = @"c:\temp\unp\in\";
        private string outPath = @"c:\temp\unp\out\";

        public Logica()
        {
            if (!Directory.Exists(tempPath))
            {
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
                    }

                    foreach(archNSI arN in nsiFiles)
                    {
                        arN.UnZipNSI(tempPath, outPath);
                    }

                }
        }
            while (nsiFiles.Count>0);
            

        }


        //unzip test
        public void UnZip()
        {
            archNSI nsi1 = new archNSI(@"c:\temp\unp\in\mdp6406.lzh");
            bool isSuccess = nsi1.UnZipNSI(tempPath ,@"c:\temp\unp\out\");
        }

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
