using LJMB.Command;
using Transfermarkt.Console.Arguments;

namespace Transfermarkt.Console
{
    public static class CommandUtils
    {
        public static (int i1, int i2, int i3) GetIndexes(this IArgument ind)
        {
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;

            if (ind is Index1Argument)
            {
                i1 = (ind as Index1Argument).Index1;
            }
            else if (ind is Index2Argument)
            {
                i1 = (ind as Index2Argument).Index1;
                i2 = (ind as Index2Argument).Index2;
            }
            else if (ind is Index3Argument)
            {
                i1 = (ind as Index3Argument).Index1;
                i2 = (ind as Index3Argument).Index2;
                i3 = (ind as Index3Argument).Index3;
            }

            return (i1, i2, i3);
        }
    }
}
