// <copyright file="Program.cs" company="Nic Jansma">
//  Copyright (c) Nic Jansma 2012 All Right Reserved
// </copyright>
// <author>Nic Jansma</author>
// <email>nic@nicj.net</email>
namespace RenameRegex
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// RenameRegex command line program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main command line
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>0 on success</returns>
        public static int Main(string[] args)
        {
            string nameSearch;
            string nameReplace;
            string fileMatch;
            bool pretend;
            if (!GetArguments(args, out fileMatch, out nameSearch, out nameReplace, out pretend))
            {
                Usage();
                return 1;
            }

            // enumerate all files
            string[] files = Directory.GetFiles(System.Environment.CurrentDirectory, fileMatch);

            if (files.Length == 0)
            {
                Console.WriteLine(@"No files match!");
                return 1;
            }

            string pretendModeNotification = pretend ? " (pretend)" : String.Empty;

            //
            // loop through each file, renaming via a regex
            //
            foreach (string fullFile in files)
            {                
                // split into file and path
                string fileName = Path.GetFileName(fullFile);
                string fileDir  = Path.GetDirectoryName(fullFile);

                // rename via a regex
                string fileNameAfter = Regex.Replace(fileName, nameSearch, nameReplace, RegexOptions.IgnoreCase);

                // write what we changed (or would have)
                if (fileName != fileNameAfter)
                {
                    Console.WriteLine(@"{0} -> {1}{2}", fileName, fileNameAfter, pretendModeNotification);
                }

                // move file
                if (!pretend && fileName != fileNameAfter)
                {
                    try
                    {
                        File.Move(fileDir + @"\" + fileName, fileDir + @"\" + fileNameAfter);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine(@"WARNING: Could note move {0} to {1}", fileName, fileNameAfter);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the program arguments
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        /// <param name="fileMatch">File matching pattern</param>
        /// <param name="nameSearch">Search expression</param>
        /// <param name="nameReplace">Replace expression</param>
        /// <param name="pretend">Whether or not to only show what would happen</param>
        /// <returns>True if argument parsing was successful</returns>
        private static bool GetArguments(
            string[] args, 
            out string fileMatch, 
            out string nameSearch, 
            out string nameReplace, 
            out bool pretend)
        {
            // defaults
            fileMatch   = String.Empty;
            nameSearch  = String.Empty;
            nameReplace = String.Empty;
            pretend     = false;

            // check for all arguments
            if (args == null || args.Length < 3 || args.Length > 4)
            {
                return false;
            }

            // set from arguments
            fileMatch   = args[0];
            nameSearch  = args[1];
            nameReplace = args[2];
            pretend     = false;

            if (args.Length == 4 && args[3].Equals("/p", StringComparison.OrdinalIgnoreCase))
            {
                pretend = true;
            }

            return true;
        }

        /// <summary>
        /// Program usage
        /// </summary>
        private static void Usage()
        {
            // get the assembly version
            Assembly assembly = Assembly.GetExecutingAssembly(); 
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location); 
            string version = fvi.ProductVersion; 

            Console.WriteLine(@"Rename Regex (RR) v{0} by Nic Jansma, http://nicj.net", version);
            Console.WriteLine();
            Console.WriteLine(@"Usage: RR file-match search replace [/p]");
            Console.WriteLine(@"        /p: pretend (show what will be renamed)");
            return;
        }
    }
}