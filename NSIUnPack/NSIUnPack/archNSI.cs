﻿using System;
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
        //Полный путь к файлу
        private string filePath;
        // Имя файла, выцепляется регулярными выражениями
        private string fileName;
        //Расширение файла, выцепляется регулярными выражениями
        private string fileExtension;
        //Дирректория, в которой лежит файл
        private string directoryPath;
        //Регулярное выражение, для выдирания имени и расширения файла в случае расположения на локальном диске
        private string localRegexp = @"(?<directory>[a-zA-Z]:[a-zA-Z0-9\\!_]*\\)(?<file>[\w\.]+).(?<extention>zip|rar|lzh|7z)"; 
        
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
                this.directoryPath = fileMatch.Groups["directory"].Value.ToString();
            }
            
        }

        /// <summary>
        /// Распаковать файл
        /// </summary>
        /// <param name="tempPath">Временная директория</param>
        /// <param name="outputPath">Конечная дирректория</param>
        /// <returns>Получилось ли распаковать</returns>
        public bool UnZipNSI(string tempPath, string outputPath, string archiver, string pechPath, string unknownPath, string v2Out)
        {
            //Проверяем наличие временной директории, если нет то создаём
            if (!Directory.Exists(tempPath + this.fileName + @"\"))
            {
                Directory.CreateDirectory(tempPath + this.fileName + @"\");
            }
            //Распаковываем во временную директорию
            bool isSuccess = ExtractME(archiver, filePath, tempPath+this.fileName+@"\");

            //Если получилось из временной в финальную копируем
            if(isSuccess)
            {
                finalCopy(tempPath + @"\" + this.fileName + @"\", outputPath, pechPath, unknownPath, v2Out);
            }

            return isSuccess;
        }


        //Подчищаем временные папки и исходные файлы
        private void ClearTempDir(string tempDir)
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }

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

        /// <summary>
        /// Раскидывает по директориям извлечённые файлы
        /// </summary>
        /// <param name="fromPath">где искать файлы</param>
        /// <param name="toPath">куда копировать</param>
        /// <param name="pechPath">куда копировать PECH</param>
        /// <param name="unknownPath">куда копировать файлы неизвестного типа</param>
        private void finalCopy(string fromPath, string toPath, string pechPath, string unknownPath, string v2Out)
        {
            DirectoryInfo fromDir = new DirectoryInfo(fromPath);
            FileInfo[] fromFiles = fromDir.GetFiles();
            foreach (FileInfo fi1 in fromFiles)
            {
                //если это архив (FST_ZAKL.RAR), то возвращаем его к архивам
                if (fi1.Extension.ToLower() == ".rar")
                {
                    File.Copy(fi1.FullName, this.directoryPath  + fi1.Name, true);
                }
                //если это печати, то копируем к печатям 
                else if (fi1.Name == "PECH.DBF")
                {
                    string outputPech = pechPath + DateTime.Now.ToShortDateString()+@"\";
                    if (!Directory.Exists(outputPech))
                    {
                        Directory.CreateDirectory(outputPech);
                        File.Copy(fi1.FullName, outputPech + fi1.Name);

                    }

                }
                //
                else if (fi1.Name == "V2.DBF")
                {
                    V2Copy(v2Out, fromPath);
                    V2Current(v2Out,fromPath);
                }
                else if((fi1.Extension.ToLower() == ".dbf")|(fi1.Extension.ToLower() == ".dbt") | (fi1.Extension.ToLower() == ".fpt"))
                {
                    //Ищем в целевой директории файлы с таким же именем, если они есть то оставляем самые свежие
                    if (!File.Exists(toPath + fi1.Name))
                    {
                        File.Copy(fi1.FullName, toPath  + fi1.Name );

                    }
                    else
                    {
                        FileInfo fi2 = new FileInfo(toPath + fi1.Name);
                        if (fi1.LastWriteTime>fi2.LastWriteTime)
                        {
                            File.Copy(fi1.FullName, toPath  + fi1.Name, true);
                        }

                    }
                }
                else
                {
                    if(!Directory.Exists(unknownPath))
                    {
                        Directory.CreateDirectory(unknownPath);
                    }
                    File.Copy(fi1.FullName, unknownPath + fi1.Name, true);
                }

            }
            ClearTempDir(fromPath);
        }

        private void V2Copy(string v2Out, string fromPath)
        {
            FileInfo v2File = new FileInfo(fromPath + "V2.DBF");
            DateTime v2Date = v2File.LastWriteTime;
            string outPath = v2Out + v2Date.Year.ToString("D4") + @"\" + v2Date.Month.ToString("D2") + @"\" + v2Date.Day.ToString("D2") + @"\";
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }
            v2File.CopyTo(outPath + "V2.DBF", true);

        }

        private void V2Current(string v2Out, string fromPath)
        {
            FileInfo v2File = new FileInfo(fromPath + "V2.DBF");
            FileInfo v2Old = new FileInfo(v2Out + "v2current\\V2.DBF");

            if (!Directory.Exists(v2Out + "v2current"))
            {
                Directory.CreateDirectory(v2Out + "v2current");
            }

            if(!v2Old.Exists||(v2File.LastWriteTime > v2Old.LastWriteTime))
            {
                v2File.CopyTo(v2Out + "v2current\\V2.DBF", true);
            }
        }


        public string LocalRegExp
        { get; set; }
    }
}

