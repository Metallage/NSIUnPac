using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NSIUnPack
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                if (File.Exists("Path.xml"))
                {
                    Logica nsiLogic = new Logica();
                    nsiLogic.ExtractNSI();
                }
                else
                {
                    Console.WriteLine("Path.xml не найден");
                    Console.ReadLine();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }


        }
    }
}
