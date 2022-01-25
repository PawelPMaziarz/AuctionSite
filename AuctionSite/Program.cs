using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSite.Data;

namespace AuctionSite
{
    class Program
    {
        static void Main(string[] args)
        {

            Controller controller = new Controller();

            controller.Main();

            Console.WriteLine("Do widzenia");
            Console.ReadLine();

        }
    }
}
