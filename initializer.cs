using System;
using System.IO;
using System.Numerics;

namespace treeNote
{
    // Class responsible for initializing and managing the application
    class Initializer
    {
        // Message to display when the application is not initialized properly
        public static string noInitMessage = "The application was not initialized properly. Some important files or folders might be missing. Try running ./treeNote --init first";

        // Create the master node of the tree
        public static Node createMasterNode()
        {
            // Create a root node with coordinates (0,0) and name "master"
            Node master = new Node(new Vector2(0, 0), 0, "master");
            StreamReader sr;
            try
            {
                // Attempt to read from the session file
                sr = new StreamReader(".session");

                while (!sr.EndOfStream)
                {
                    // Parse each line in the session file to construct the tree
                    master.childs.Add(parseLine(0, sr));
                }
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                // If the session file is not found, display an error message
                printNoInit();
            }
            return master;
        }

        // Parse a line of text to create a node in the tree
        static Node parseLine(int depth, StreamReader sr)
        {
            string line = sr.ReadLine();
            string[] tokens = line.Split(";");
            string name = tokens[0];
            Vector2 pos = new Vector2(float.Parse(tokens[1].Split(",")[0]), float.Parse(tokens[1].Split(",")[1]));
            int childCount = Int32.Parse(tokens[2]);

            // Create a new node with parsed information
            Node toReturn = new Node(pos, Config.defaultNodeSize * (Math.Pow(Config.childScaler, depth)), name);
            for (int i = 0; i < childCount; ++i)
            {
                // Recursively parse child nodes
                Node child = parseLine(depth + 1, sr);
                toReturn.childs.Add(child);
            }
            return toReturn;
        }

        // Save the tree structure to the session file when exiting
        public static void onExit(Node masterNode)
        {
            StreamWriter sw;
            try
            {
                // Attempt to create or overwrite the session file
                sw = new StreamWriter(".session");
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException(".session File was deleted");
            }
            foreach (Node n in masterNode.childs)
            {
                // Dump each node and its children to the session file
                dumpNode(n, sw);
            }
            sw.Close();
        }

        // Write node information to the session file
        static void dumpNode(Node n, StreamWriter sw)
        {
            sw.WriteLine(
                n.name + ";" +
                n.pos.X + "," + n.pos.Y + ";" +
                n.childs.Count
            );

            foreach (Node n2 in n.childs)
            {
                // Recursively dump child nodes
                dumpNode(n2, sw);
            }
        }

        // Initialize the application by creating necessary files and directories
        public static void init()
        {
            StreamWriter sw;

            // Create the session file and write initial data
            File.Create(".session").Close();
            sw = new StreamWriter(".session");
            sw.WriteLine("treeNote;0,0;0");
            sw.Close();

            // Create the notes directory and initial note files
            Directory.CreateDirectory(Config.notesDir);
            File.Create("treeNote" + Config.ext).Close();
            sw = new StreamWriter(Config.notesDir + "treeNote" + Config.ext);
            sw.WriteLine("# treeNote");
            sw.Close();

            File.Create(Config.notesDir + "none" + Config.ext).Close();
            sw = new StreamWriter(Config.notesDir + "none" + Config.ext);
            sw.WriteLine("# none");
            sw.Close();

            // Create the cache directory
            Directory.CreateDirectory(Config.cacheDir);

            // Create an "edit" file
            File.Create("edit" + Config.ext).Close();
        }

        // Display an error message when initialization is not successful
        public static void printNoInit()
        {
            Console.WriteLine(noInitMessage);
            Environment.Exit(1);
        }
    }
}

