using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace NSIUnPack
{
    class archNSI
    {
        //Путь к файлу
        private string filePath;
        // Имя файла, выцепляется регулярными выражениями
        private string fileName;
        //Расширение файла, выцепляется регулярными выражениями
        private string fileExtension; 
        //Регулярное выражение, для выдирания имени и расширения файла в случае расположения на локальном диске
        private string localRegexp = @"[a-zA-Z]:[a-zA-Z0-9\\]*\\(?<file>[\w]+).(?<extention>zip|rar|lzh|7z)"; 
        
        /// <summary>
        /// Логический объект типа файл архива (конструктор)
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public archNSI(string filePath)
        {
            this.filePath = filePath;
            //пробуем проверить путь 
            Regex fileReg = new Regex(localRegexp);
            //Если парсится один файл то всё хорошо
            if (fileReg.Matches(filePath).Count == 1) 
            {
                Match fileMatch = fileReg.Match(filePath);
                this.fileName = fileMatch.Groups["file"].Value.ToString();
                this.fileExtension = fileMatch.Groups["extention"].Value.ToString();
            }
            
        }

        /// <summary>
        /// Распаковать файл
        /// </summary>
        /// <param name="tempPath">Временная директория</param>
        /// <param name="outputPath">Конечная дирректория</param>
        /// <returns>Получилось ли распаковать</returns>
        public bool UnZipNSI(string tempPath, string outputPath)
        {
            //Проверяем наличие временной директории, если нет то создаём
            if (!Directory.Exists(tempPath + @"\" + this.fileName + @"\"))
            {
                Directory.CreateDirectory(tempPath + @"\" + this.fileName + @"\");
            }
            //Распаковываем во временную директорию
            bool isSuccess = ExtractME(@"c:\temp\unp\7z\7z.exe", filePath, tempPath+@"\"+this.fileName+@"\");

            //Если получилось из временной в финальную копируем
            if(isSuccess)
            {
                finalCopy(tempPath + @"\" + this.fileName + @"\", outputPath);
            }

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
        /// <returns>Успешно ли распаковалось</returns>
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

        private void finalCopy(string fromPath, string toPath)
        {
            DirectoryInfo fromDir = new DirectoryInfo(fromPath);
            FileInfo[] fromFiles = fromDir.GetFiles();
            foreach(FileInfo fi1 in fromFiles)
            {
                File.Copy(fi1.FullName,toPath+@"\"+fi1.Name+"."+fi1.Extension);
            }
        }
    }
}
