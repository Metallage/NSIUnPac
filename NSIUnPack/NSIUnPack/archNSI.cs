using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace NSIUnPack
{
    class archNSI
    {
        private string filePath;

        public archNSI(string filePath)
        {
            this.filePath = filePath;
        }

        public bool UnZipNSI(string outputPath)
        {
            bool isSuccess = false;


            return isSuccess;
        }

        public bool ExterminateMe()
        {
            bool exterminated = false;


            return exterminated;
        }
        /// <summary>
        /// Нашел эту функцию в гугле, нужна для извлечения файлов с помощью 7zip
        /// </summary>
        /// <param name="archiverEXE">полный путь к 7zip.exe</param>
        /// <param name="archiveFile">Файл архива</param>
        /// <param name="outputFolder">Дирректория назначения распаковки</param>
        /// <returns></returns>
        private bool ExtractME(string archiverEXE, string archiveFile, string outputFolder) 
        {
            bool isSucsess = false;
            try
            {
                // Предварительные проверки
                if (!File.Exists(archiverEXE))
                    throw new Exception("Архиватор 7z по пути \"" + archiverEXE +
                    "\" не найден");
                if (!File.Exists(archiveFile))
                    throw new Exception("Файл архива \"" + archiveFile +
                    "\" не найден");
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                // Формируем параметры вызова 7z
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = archiverEXE;
                // Распаковать (для полных путей - x)
                startInfo.Arguments = " e";
                // На все отвечать yes
                startInfo.Arguments += " -y";
                // Файл, который нужно распаковать
                startInfo.Arguments += " " + "\"" + archiveFile + "\"";
                // Папка распаковки
                startInfo.Arguments += " -o" + "\"" + outputFolder + "\"";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                int sevenZipExitCode = 0;
                using (Process sevenZip = Process.Start(startInfo))
                {
                    sevenZip.WaitForExit();
                    sevenZipExitCode = sevenZip.ExitCode;
                }
                // Если с первого раза не получилось,
                //пробуем еще раз через 1 секунду

                if (sevenZipExitCode == 0 || sevenZipExitCode == 1)
                {
                    isSucsess = true;
                }
                else
                {
                    using (Process sevenZip = Process.Start(startInfo))
                    {
                        Thread.Sleep(1000);
                        sevenZip.WaitForExit();
                        switch (sevenZip.ExitCode)
                        {
                            case 0: isSucsess = true;
                                break; // Без ошибок и предупреждений
                            case 1: isSucsess = true;
                                break; // Есть некритичные предупреждения
                            case 2: throw new Exception("Фатальная ошибка");
                            case 7: throw new Exception("Ошибка в командной строке");
                            case 8:
                                throw new Exception("Недостаточно памяти для выполнения операции");
                            case 225:
                                throw new Exception("Пользователь отменил выполнение операции");
                            default:
                                throw new Exception("Архиватор 7z вернул недокументированный код ошибки: " 
                                    + sevenZip.ExitCode.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SevenZip.ExtractFromArchive: " + e.Message);
            }
            return isSucsess;
        }

    }
}
