using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSIUnPack
{
    class Program
    {
        static void Main(string[] args)
        {
            Logica nsiLogic = new Logica();
            //nsiLogic.PrintFiles();
            nsiLogic.ExtractNSI();


        }
    }
}
