using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace treeNote
{
    // Configuration class for treeNote application
    class Config
    {
        // Define default file extension for notes
        public static string ext = ".md";

        // Define default directory for storing notes
        public static string notesDir = "notes/";

        // Define default directory for caching PDFs and thumbnails
        public static string cacheDir = "cache/";

        // Define default size for thumbnail width
        public static int thumbnailSizeX = 500;

        // Calculate thumbnail height based on width to maintain aspect ratio
        public static int thumbnailSizeY = (int)(thumbnailSizeX * 1.41421);

        // Define default node size
        public static int defaultNodeSize = 15;

        // Define scaling factor for child nodes
        public static double childScaler = 0.8;

        // Define default line width for arrows
        public static int defaultLineWidth = 15;

        // Define cropping size for thumbnails
        public static int cropThumbnail = 200;

        // Define background color
        public static Color background = hexStringToColor("#0D1F22");

        // Define color for nodes
        public static Color nodes = hexStringToColor("#F2EFE9");

        // Define color for selected nodes
        public static Color selected = hexStringToColor("#9B2915");

        // Define color for arrows
        public static Color arrows = hexStringToColor("#F2EFE9");

        // Method to load configuration from a file
        public static void loadConfig(string file = ".config")
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(file);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"File {file} was not found. Using default settings instead");
            }

            int lineCount = 0;

            // Read each line of the configuration file
            while (!sr.EndOfStream)
            {
                lineCount++;
                string line = sr.ReadLine();

                // Skip comments and empty lines
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                // Split the line into key and value
                int index = line.IndexOf('=');
                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1, line.Length - index-1).Trim();

                try
                {
                    // Check and set configuration options based on the key
                    switch (key)
                    {
                        case "ext":
                            ext = value;
                            break;

                        case "notesDir":
                            notesDir = value;
                            break;

                        case "thumbnailSize":
                            thumbnailSizeX = Int32.Parse(value);
                            thumbnailSizeY = (int)(thumbnailSizeX * 1.41421);
                            break;

                        case "defaultNodeSize":
                            defaultNodeSize = Int32.Parse(value);
                            break;

                        case "childScaler":
                            childScaler = Double.Parse(value);
                            break;

                        case "defaultLineWidth":
                            defaultLineWidth = Int32.Parse(value);
                            break;

                        case "cropThumbnail":
                            cropThumbnail = Int32.Parse(value);
                            break;

                        case "background":
                            background = hexStringToColor(value);
                            break;

                        case "nodes":
                            nodes = hexStringToColor(value);
                            break;

                        case "selected":
                            selected = hexStringToColor(value);
                            break;

                        case "arrows":
                            arrows = hexStringToColor(value);
                            break;

                        default:
                            Console.WriteLine($"Unrecognized option on line {lineCount}. Ignoring");
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine($"Something went wrong when parsing line {lineCount}: {line}. Ignoring");
                }
            }
        }

        // Method to convert a hexadecimal string to a Color object
        public static Color hexStringToColor(string hex)
        {
            // Remove any '#' prefix from the hex string
            hex = hex.TrimStart('#');

            if (hex.Length != 6 && hex.Length != 8)
                throw new ArgumentException("Hex string must be 6 or 8 characters long.");

            byte red = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte green = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte blue = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            byte alpha = 255; // Default alpha value
            if (hex.Length == 8)
                alpha = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color(red, green, blue, alpha);
        }

        // Method to create a default configuration file
        public static void dumpDefault(string confFile = ".config")
        {
            // Create the configuration file if it doesn't exist
            if (!File.Exists(confFile)) File.Create(confFile);
            StreamWriter sw = new StreamWriter(confFile);

            // Write default configuration options with explanations
            sw.WriteLine("# FILE HANDLING");
            sw.WriteLine("### File extension of all note files");
            sw.WriteLine("ext = .md");
            sw.WriteLine("### Directory where all notes are going to be saved");
            sw.WriteLine("notesDir = notes/");
            sw.WriteLine("### Directory where cache (PDFs and thumbnails) are going to be saved");
            sw.WriteLine("cacheDir = cache/");
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("# THUMBNAIL");
            sw.WriteLine("### How large should thumbnails appear? Thumbnails will be n pixels wide");
            sw.WriteLine("thumbnailSize = 500");
            sw.WriteLine("### PDF files usually have white borders. This will crop them by n pixels");
            sw.WriteLine("cropThumbnail = 200");
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("# NODE OPTIONS");
            sw.WriteLine("### How many pixels should the largest nodes be");
            sw.WriteLine("defaultNodeSize = 15");
            sw.WriteLine("### How much smaller should each child node be (<1)");
            sw.WriteLine("childScaler = 0.8");
            sw.WriteLine("### Line thickness (for arrows)");
            sw.WriteLine("defaultLineWidth = 15");
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("# COLORS");
            sw.WriteLine("background =     #0D1F22");
            sw.WriteLine("nodes =          #F2EFE9");
            sw.WriteLine("### If a node is selected, which color should it be drawn");
            sw.WriteLine("selected =       #9B2915");
            sw.WriteLine("arrows =         #F2EFE9");
            sw.Close();
        }
    }
}

