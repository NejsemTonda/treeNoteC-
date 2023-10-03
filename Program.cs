using System;
using System.IO;

namespace treeNote
{
    // The main program class
    class Program
    {
        public static void Main(string[] args)
        {
            // Initialize index to 0 for parsing command line arguments
            int index = 0;
            // Initialize config flag to false
            bool config = false;
            // Initialize configFile to "none"
            string configFile = "none";

            // Print the command line arguments
            Console.WriteLine(string.Join(" ", args));

            // Loop through each argument in the command line arguments
            while (index < args.Length)
            {
                Console.WriteLine(index);
                // Get the current argument
                string token = args[index];

                // Check if the argument starts with "--"
                if (token.StartsWith("--"))
                {
                    switch (token)
                    {
                        case "--help":
                            // Print help information and exit with code 0
                            printHelp();
                            Environment.Exit(0);
                            break;

                        case "--init":
                            // Initialize the application
                            Initializer.init();
                            break;

                        case "--default":
                            // Create a new .config file with default settings and exit with code 0
                            Config.dumpDefault();
                            Environment.Exit(0);
                            break;

                        case "--config":
                            // Set the config flag to true and move to the next argument to get the config file path
                            config = true;
                            ++index;
                            configFile = args[index];
                            break;

                        default:
                            // Print an error message for an invalid argument and exit with code 1
                            Console.WriteLine($"Invalid argument at {index}");
                            printHelp();
                            Environment.Exit(1);
                            break;
                    }
                }
                // Check if the argument starts with "-"
                else if (token.StartsWith("-"))
                {
                    switch (token)
                    {
                        case "-h":
                            // Print help information and exit with code 0
                            printHelp();
                            Environment.Exit(0);
                            break;

                        case "-i":
                            // Initialize the application
                            Initializer.init();
                            break;

                        case "-d":
                            // Create a new .config file with default settings and exit with code 0
                            Config.dumpDefault();
                            Environment.Exit(0);
                            break;

                        case "-c":
                            // Set the config flag to true and move to the next argument to get the config file path
                            config = true;
                            ++index;
                            configFile = args[index];
                            break;

                        default:
                            // Print an error message for an invalid argument and exit with code 1
                            Console.WriteLine($"Invalid argument at {index}");
                            printHelp();
                            Environment.Exit(1);
                            break;
                    }
                }
                // Move to the next argument
                ++index;
            }

            // If the config flag is set
            if (config)
            {
                // Check if the configFile argument looks like a flag (starts with "-" or "--")
                if (configFile.StartsWith("-") || configFile.StartsWith("--"))
                {
                    // Print an error message indicating that a config file should be specified
                    Console.WriteLine($"Config file should be specified, but {configFile} looks like a flag");
                    Environment.Exit(1);
                }

                // Check if the specified config file exists
                if (!File.Exists(configFile))
                {
                    // Print an error message indicating that the config file doesn't exist
                    Console.WriteLine($"Config file {configFile} doesn't exist");
                    Environment.Exit(1);
                }

                // Load the configuration from the specified config file
                Config.loadConfig(configFile);
            }

            // Create a new instance of the Game1 class and run the game
            using var game = new treeNote.Game1();
            game.Run();
        }

        // Function to print help information
        public static void printHelp()
        {
            Console.WriteLine(@"treeNote - A tree structure note-taking application made by Václav Krňák
Usage:
    treeNote [arguments]       Run this application

Arguments:
    -h, --help                Print this help
    -i, --init                Initialize folders and files
    -d, --default             Create a new .config file with default settings
    -c, --config cfile        Use specified cfile as .config file");
        }
    }
}

