using System;

namespace EngineTest
{
    class EngineUiTest
    {
        const char BLANK = ' ';
        const char DOT = '.';
        const char X = 'x';
        const int cMaxLineChars = 79;
        const int cHalf = cMaxLineChars / 2;
        static char[] LINE = new char[cMaxLineChars];
        delegate double FUNC(double X);

        static void Main(string[] args)
        {
            DoThePlot(Math.Sin);
            Console.WriteLine("For another plot, press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            DoThePlot(Sinc);
            Console.ReadKey();
        }
        static void DoThePlot(FUNC TheDelegate)
        {
            fillUp(LINE, WithChar: DOT); // line of dots for "vertical" axis
            Console.WriteLine(LINE);
            fillUp(LINE, WithChar: BLANK); // clear the line
            PlotFunc(TheDelegate);
        }
        // just another function to show that this delegate points to functions with same signature
        static double Sinc(double x)
        {
            return Math.Sin(x) / x;
        }
        static void fillUp(char[] line, char WithChar = '\0')
        {
            for (int i = 0; i < line.Length; i++)
            {
                line[i] = WithChar;
            }
        }
        static void PlotFunc(FUNC f)
        {
            double maxval = 9.0; //arbitrary values
            double delta = 0.2; //size of iteration steps
            int loc;
            LINE[cHalf] = DOT; // for "horizontal" axis
            for (double x = 0.0001; x < maxval; x += delta) //0.0001 to avoid DIV/0 error
            {
                loc = (int)Math.Round(f(x) * cHalf) + cHalf;
                LINE[loc] = X;
                Console.WriteLine(LINE);
                fillUp(LINE, WithChar: BLANK); // blank the line, remove X point
                LINE[cHalf] = DOT; // for horizontal axis
            }
        }
    }
}

