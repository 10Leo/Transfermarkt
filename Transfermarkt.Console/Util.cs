using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Transfermarkt.Console
{
    public static class Util
    {
        public static Command ParseCommand(string line)
        {
            Command cmd = new Command();
            try
            {
                string[] splitedLine = line.Split(' ');

                if (splitedLine.Length > 0)
                {
                    var first = splitedLine[0].ToLowerInvariant();
                    // First arg must be either f(etch) or p(arse)
                    if (first.StartsWith("f"))
                    {
                        cmd.CommandType = CommandType.F;
                    }
                    else if (first.StartsWith("p"))
                    {
                        cmd.CommandType = CommandType.P;
                    }
                    else
                    {
                        throw new Exception("Command not specified");
                    }
                }

                bool hasExtra = false;
                int index = 1;

                if (splitedLine.Length > 1)
                {
                    Regex r = new Regex("^[0-9]*");
                    var second = splitedLine[1].ToLowerInvariant();
                    // Second arg can be the year
                    if (second.StartsWith("y"))
                    {
                        int y = ParseYear(second);
                        cmd.ExtraArgs.Add((ExtraCommand.Y, y.ToString()));
                        hasExtra = true;
                        index = 2;
                    }
                    else if(r.IsMatch(second))
                    {
                        hasExtra = false;
                        index = 1;
                    }
                    else
                    {
                        throw new Exception("Command not found.");
                    }
                }

                if (splitedLine.Length <= index)
                {
                    throw new Exception("No options passed.");
                }

                for (int i = index; i < splitedLine.Length; i++)
                {
                    string opt = splitedLine[i];

                    var sp = opt.Split('.');

                    var i1 = int.Parse(sp[0]);
                    var i2 = -1;
                    if (sp.Length > 1)
                    {
                        i2 = int.Parse(sp[1]);
                    }
                    cmd.Options.Add((i1, i2));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error reading or interpreting supplied numbers.");
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return cmd;
        }

        private static int ParseYear(string yearCmd)
        {
            Regex r = new Regex(@": *");
            var splitted = r.Split(yearCmd);

            return int.Parse(splitted[1]);
        }
    }
}
