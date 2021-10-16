﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HTTPConsole
{
    public static class LoopHandler
    {
        public static void Loop(Func<string> getString, bool verbose, Uri uri, bool display, string pipePath)
        {
            WriteLine("Display counter? (y/n)");
            bool displayCounter = InputHandler.InputYN(getString);

            WriteLine("Enter for statement");
            string s = getString().Trim().ToLower();
            string[] split = s.Split();
            // for 0..100 +1
            if (split[0] != "for")
            {
                WriteLine("Loop malformed, returning. Start your loop with \"for\"");
                return;
            }

            int start = 0, end = -1;
            bool infinite = split.Length == 1;

            if (split.Length > 1) // Range
            {
                try
                {
                    string[] rangeSplit = split[1].Split("..");

                    if (rangeSplit.Length == 1)
                    {
                        if (split[1].Length >= 3) infinite = split[1][^2..] == "..";
                        end = int.Parse(rangeSplit[0]);
                    }
                    else if (rangeSplit.Length == 2)
                    {
                        start = int.Parse(rangeSplit[0]);
                        end = int.Parse(rangeSplit[1]);
                    }
                    else throw new Exception();
                }
                catch
                {
                    WriteLine("Loop range malformed, returning.");
                    return;
                }
            }

            Func<int, int> increment = null;

            if (split.Length > 2) // Increment
            {
                try
                {
                    char c = split[2][0];
                    int by;

                    if (split[2].Length > 1)
                    {
                        by = int.Parse(split[2][1..]);
                    }
                    else
                    {
                        if ("*/^<>".Contains(c)) by = 2;
                        else by = 1;
                    }

                    increment = c switch
                    {
                        '+' => x => x + by,
                        '-' => x => x - by,
                        '*' => x => x * by,
                        '/' => x => x / by,
                        '^' => x => (int)Math.Pow(x, by),
                        '<' => x => x << by,
                        '>' => x => x >> by,
                        _ => throw new Exception()
                    };
                }
                catch
                {
                    WriteLine("Loop increment malformed, returning.");
                    return;
                }
            }
            else
            {
                if (infinite || start <= end) increment = x => x + 1;
                else increment = x => x - 1;
            }

            WriteLine("Enter special instructions (enter twice to continue)");

            Dictionary<Condition, string> conditions = ConditionHandler.Handle(getString);
            // display/break key/header/content/code/uri contains/is value

            WriteLine("Enter macro (finish with END)");

            List<string> macro = new();
            string line;
            do
            {
                line = getString();
                line = line.Replace("/end", "end");
                macro.Add(line);
            }
            while (line.ToLower() != "end");

            WriteLine("Running...");

            for (int i = start, j = 0; infinite || IsBetween(i, start, end); i = increment(i), j++)
            {
                if (displayCounter)
                {
                    string x = $"TRIAL {j} WITH COUNTER {i}";
                    Program.BadLog(x, pipePath, display);
                }

                int c = 0; // current macro

                Program.Command(() => macro[c++], verbose, uri, true, i, pipePath, conditions); // haha funny c++
            }

            void WriteLine(object x)
            {
                if (verbose) Console.WriteLine(x);
            }
        }

        private static bool IsBetween(int x, int start, int end) => 
            start < end 
            ? start <= x && x <= end 
            : start >= x && x >= end;
    }

    /* y
     * for 100
     * break content contains appear to be a valid cookie
     * display content !contains Not very special though
     * 
     * get
     * cookie
     * name=$i
     * 
     * 
     * end
     */
}