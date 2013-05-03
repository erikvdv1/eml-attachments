using System;
using System.IO;

namespace EmlAttachments
{
    /// <summary>
    /// The main class for starting the program.
    /// </summary>
    class Program
    {
        // TODO: default namespace
        // TODO: license

        /// <summary>
        /// Entrypoint.
        /// </summary>
        /// <param name="args">One argument, the .eml file name.</param>
        static void Main(string[] args)
        {
            // Check arguments
            if (args.Length != 1 || args[0] == "/?")
            {
                WriteHelp();
                return;
            }

            // Check if file exists
            string fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Cannot find file: {0}.", fileName);
                return;
            }

            // Extract attachments
            Email email = new Email(fileName);
            int count = email.SaveAttachments(Path.GetDirectoryName(fileName));
            Console.WriteLine("{0} attachments extracted.", count);
        }

        /// <summary>
        /// Writes the help text for commandline users.
        /// </summary>
        private static void WriteHelp()
        {
            String assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Console.WriteLine("Extracts the attachments of an .eml file.");
            Console.WriteLine();
            Console.WriteLine(@"{0} drive:\path\to\eml\file", assemblyName);
        }
    }
}
