﻿using LJMB.Command;
using LJMB.Command.Commands;
using Transfermarkt.Console.Options;

namespace Transfermarkt.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Transfermarkt Web Scrapper\n");

            IContext context = new TMContext();
            var f = new FetchCommand(context);
            var p = new ParseCommand(context);
            _ = new ExitCommand(context);

            context.Run();
        }
    }
}
