using System;
using System.Globalization;
using Registration.Business;
using Registration.Business.Models;

namespace Registration
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new User("John Doe",
                "870101XXXX",
                new RegionInfo("SE"),
                new DateTimeOffset(1987, 01, 29, 00, 00, 00, TimeSpan.FromHours(2)));

            var processor = new UserProcessor();

            var result = processor.Register(user);

            Console.WriteLine(result);
        }
    }
}
