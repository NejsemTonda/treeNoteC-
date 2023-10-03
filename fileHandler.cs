using System;
using System.IO;
using System.Text;

namespace treeNote
{
    // Define a class named FileHandler
    class FileHandler
    {
        // Define public and private member variables
        public string openedFile; // Stores the name of the currently opened file
        string editFile; // Stores the name of the file used for editing

        // Constructor for FileHandler
        public FileHandler()
        {
            // Initialize the openedFile and editFile variables
            openedFile = "none" + Config.ext; // Initialize openedFile with "none" and a file extension specified in Config
            editFile = "edit" + Config.ext; // Initialize editFile with "edit" and a file extension specified in Config

            try
            {
                // Attempt to copy the contents of a file located at Config.notesDir + openedFile to editFile
                copyFile(Config.notesDir + openedFile, editFile);
            }
            catch (DirectoryNotFoundException)
            {
                // Handle the exception if the directory is not found by calling Initializer.printNoInit()
                Initializer.printNoInit();
            }
        }

        // Define a method to copy the contents of one file to another
        void copyFile(string loc, string dest)
        {
            // Use a StringBuilder to efficiently concatenate the contents of the source file
            string line;
            var sb = new StringBuilder();
            StreamReader file = new StreamReader(loc);

            while ((line = file.ReadLine()) != null)
                sb.AppendLine(line);

            file.Close();

            // Write the concatenated contents to the destination file
            using (StreamWriter files = new StreamWriter(dest))
            {
                files.WriteLine(sb.ToString().Trim());
            }
        }

        // Define a method to handle changes made to a file
        public void changedToFile(string file)
        {
            // Retrieve the current topic (name) of the opened file
            string currentTopic = getCurrentFileName();
            string currentName = currentTopic + Config.ext;

            // Check if a file with the current name exists
            if (!File.Exists(Config.notesDir + currentName))
            {
                // If not, delete the previous file and create a new one with the current name
                deleteFile(Config.notesDir + openedFile + Config.ext);
                Console.WriteLine($"File {Config.notesDir + openedFile + Config.ext} was deleted");

                File.Create(Config.notesDir + currentName).Close();
                Console.WriteLine($"File {Config.notesDir + currentName} was created");
            }

            try
            {
                // Copy the contents of the editFile to the file with the current name
                copyFile(editFile, Config.notesDir + currentName);
            }
            catch (DirectoryNotFoundException)
            {
                // Handle the exception if the directory is not found by calling Initializer.printNoInit()
                Initializer.printNoInit();
            }

            // Regenerate the thumbnail so that the changes are visible
            Convertor.convert(Config.notesDir + currentName);

            Console.WriteLine("Loading: " + file);

            // If the user created a new node, create a new file for it
            if (!File.Exists(Config.notesDir + file + Config.ext))
            {
                StreamWriter sw = new StreamWriter(Config.notesDir + file + Config.ext);
                sw.WriteLine("# " + file);
                sw.Close();
            }

            try
            {
                // Copy the contents of the file with the specified name to the editFile
                copyFile(Config.notesDir + file + Config.ext, editFile);
            }
            catch (DirectoryNotFoundException)
            {
                // Handle the exception if the directory is not found by calling Initializer.printNoInit()
                Initializer.printNoInit();
            }

            // Update the currently opened file name
            openedFile = getCurrentFileName();
        }

        // Define a method to retrieve the current file name
        public string getCurrentFileName()
        {
            StreamReader file = new StreamReader(editFile);
            string name = file.ReadLine();
            file.Close();

            // Return the current file name without the "#" character and leading/trailing whitespace
            if (name != null) return name.Substring(2).Trim();
            return "unnamedFile";
        }

        // Define a static method to delete a file
        public static void deleteFile(string fileName)
        {
            // Check if the file exists and delete it if it does
            if (File.Exists(fileName)) File.Delete(fileName);
            else Console.WriteLine($"Tried to delete {fileName}, but the file doesn't exist.");
        }

		//This function checks if the node has any children with name that denotes that it should be delete. If it does, delete this node.
		public static void checkForDelete(Node node)
		{
			foreach(Node n in node.childs)
			{
				//Keyword for delting
				if(n.name == "Delete" || n.name == "delete" || n.name == ("del")){
					node.childs.Remove(n);
					return;
				}
			}
		}
    }
}

