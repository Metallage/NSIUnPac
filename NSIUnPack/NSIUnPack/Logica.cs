﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSIUnPack
{
    class Logica
    {
        private string tempPath = @"c:\temp\nsi\";
        private string inPath = @"C:\temp\inNSI";
        private string outPath = @"c:\Temp\outNSI";

        public Logica()
        { }

        //unzip test
        public void UnZip()
        {
            archNSI nsi1 = new archNSI(@"c:\temp\unp\in\1.zip");
            bool isSuccess = nsi1.UnZipNSI(@"c:\temp\unp\out\
");
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


    }
}
