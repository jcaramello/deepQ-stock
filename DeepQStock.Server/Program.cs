using Microsoft.Owin.Hosting;
using System;

namespace DeepQStock.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Owin server running at: {baseAddress}");
                Console.ReadLine();
            }
        }
    }
}
