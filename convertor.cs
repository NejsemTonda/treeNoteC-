using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace treeNote
{
    // This class provides functionality for converting files to PDF format and creating thumbnails.
    class Convertor
    {
        // This method takes a file name as input and converts it to PDF format.
        public static void convert(string fileName)
        {
            // Find the position of the last period (.) in the file name to determine the file extension.
            int fileExtPos = fileName.LastIndexOf(".");

            // Find the position of the last forward slash (/) in the file name to extract the topic.
            int fileSlashPos = fileName.LastIndexOf("/");

            // Extract the topic from the file name based on the positions of the period and forward slash.
            string topic = fileName.Substring(fileSlashPos + 1, fileExtPos - fileSlashPos - 1);

            // Define the names for the PDF and thumbnail files using the topic.
            string pdfName = Config.cacheDir + topic + ".pdf";
            string pngName = Config.cacheDir + topic + "-thumbnail";

            // Print a message indicating the start of the conversion process.
            Console.WriteLine($"Converting to PDF for {fileName}");

            // Configure the process to run the 'pandoc' command to convert the file to PDF.
            ProcessStartInfo pandocStartInfo = new ProcessStartInfo()
            {
                FileName = "pandoc",
                Arguments = $"\"{fileName}\" -o \"{pdfName}\""
            };
            Process pandocProc = new Process() { StartInfo = pandocStartInfo, };

            // Extract the name from the file path to be used in creating the thumbnail.
            string name = fileName.Substring(fileSlashPos + 1, fileExtPos - fileSlashPos - 1);

            // Configure the process to run the 'pdftoppm' command to create a thumbnail from the PDF.
            ProcessStartInfo pdftoppmStartInfo = new ProcessStartInfo()
            {
                FileName = "pdftoppm",
                Arguments = $"\"{pdfName}\" \"{pngName}\" -png"
            };
            Process pdftoppmProc = new Process() { StartInfo = pdftoppmStartInfo, };

            // Print a message indicating the start of thumbnail creation.
            Console.WriteLine($"Creating thumbnail for {fileName}");

            // Start the 'pandoc' process and wait for it to complete before starting 'pdftoppm'.
            Task.Run(() =>
            {
                pandocProc.Start();
                pandocProc.WaitForExit();
            }).ContinueWith(o =>
            {
                pdftoppmProc.Start();
            });
        }
    }
}

