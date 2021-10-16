﻿using System;
using System.Net;

namespace HTTPConsole
{
    public static class CookieHandler
    {
        public static void Handle(HttpWebRequest request, Uri uri, Func<string> getString, bool verbose)
        {
            if (request.SupportsCookieContainer)
            {
                WriteLine("COOKIE=VALUE");

                request.CookieContainer ??= new CookieContainer();
                while (true)
                {
                    string cookieString = InputHandler.Input(getString, s => s.Contains('=') || string.IsNullOrWhiteSpace(s));

                    if (string.IsNullOrWhiteSpace(cookieString)) break;

                    try
                    {
                        int equalsIndex = cookieString.IndexOf('=');
                        string key = cookieString.Substring(0, equalsIndex);
                        string value = cookieString.Substring(equalsIndex + 1);

                        request.CookieContainer.Add(new Cookie(key, value, null, uri.Host));
                    }
                    catch (Exception e)
                    {
                        WriteLine($"{e.Message}... Please try again.");
                    }
                }
            }
            else WriteLine("Cookies are not supported for this type of request.");

            WriteLine("PROPERTY VALUE (press enter twice to continue)");

            void WriteLine(object x)
            {
                if (verbose) Console.WriteLine(x);
            }
        }
    }
}
