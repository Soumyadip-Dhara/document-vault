using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
namespace documentvaultapi.Helper
{
    public static class FileHelper
    {
        public static void RenameFile(string sourceFilePath, string newFileName)
        {
            if (File.Exists(sourceFilePath))
            {
                string directory = Path.GetDirectoryName(sourceFilePath);
                string newFilePath = Path.Combine(directory, newFileName);

                if (File.Exists(newFilePath))
                {
                    throw new IOException("A file with the new name already exists.");
                }

                File.Move(sourceFilePath, newFilePath);
            }
            else
            {
                throw new FileNotFoundException("Source file not found.");
            }
        }
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                //throw new FileNotFoundException("File not found.");
                Console.WriteLine(filePath+" File not found.");
            }
        }
        public static void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                Console.WriteLine("Source file path cannot be null or whitespace.", nameof(sourceFilePath));
            }

            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                Console.WriteLine("Destination file path cannot be null or whitespace.", nameof(destinationFilePath));
            }
            string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Console.WriteLine("Destination directory path cannot be null or whitespace.", nameof(destinationFilePath));
            }

            CreateFolderIfNotExists(destinationDirectory);

            File.Move(sourceFilePath, destinationFilePath);
        }

        public static void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = false)
        {
            if (File.Exists(sourceFilePath))
            {
                File.Copy(sourceFilePath, destinationFilePath, overwrite);
            }
            else
            {
                throw new FileNotFoundException("Source file not found.");
            }
        }

        public static void CreateFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public static string ReadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                throw new FileNotFoundException("File not found.");
            }
        }
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
        public static void AppendToFile(string filePath, string content)
        {
            if (File.Exists(filePath))
            {
                File.AppendAllText(filePath, content);
            }
            else
            {
                CreateFolderIfNotExists(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, content);
            }
        }
        public static string[] ListFilesInDirectoryByExtension(string directoryPath, string fileExtension)
        {
            if (Directory.Exists(directoryPath))
            {
                fileExtension = fileExtension.ToLower();
                return Directory.GetFiles(directoryPath).Where(f => f.EndsWith($".{fileExtension}", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            else
            {
                CreateFolderIfNotExists(directoryPath);
                throw new DirectoryNotFoundException("The specified directory does not exist.");
            }
        }

        public static byte[] FileToByteArray(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            return File.ReadAllBytes(filePath);
        }
        public static void ExtractZip(string zipFilePath, string extractPath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(extractPath, entry.FullName);

                        string directoryPath = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        if (entry.Name == "")
                        {
                            if (!Directory.Exists(destinationPath))
                            {
                                Directory.CreateDirectory(destinationPath);
                            }
                        }
                        else
                        {
                            if (File.Exists(destinationPath))
                            {
                                throw new IOException($"The file '{destinationPath}' already exists.");
                            }
                            else
                            {
                                entry.ExtractToFile(destinationPath);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine($"Error: File '{zipFilePath}' not found.");
                Console.WriteLine(fnfe.Message);
            }
            catch (InvalidDataException ide)
            {
                Console.WriteLine("Error: Invalid zip file.");
                Console.WriteLine(ide.Message);
            }
            catch (UnauthorizedAccessException uae)
            {
                Console.WriteLine("Error: Access to the path is denied.");
                Console.WriteLine(uae.Message);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("Error: An I/O error occurred.");
                Console.WriteLine(ioe.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.");
                Console.WriteLine(ex.Message);
            }
        }
        public static DateTime GetFileCreationTimestamp(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    return fileInfo.CreationTime;
                }
                else
                {
                    throw new FileNotFoundException("File not found.", filePath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied: {ex.Message}", ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid file path: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}", ex);
            }
        }

        public static void WriteToFile(string fileName, string fileData, string filePath)
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                // Combine the file path and file name
                string fullPath = Path.Combine(filePath, fileName);

                // Compress the file data
                byte[] compressedData = CompressData(fileData);

                // Write the compressed data to the file
                File.WriteAllBytes(fullPath, compressedData);

                Console.WriteLine($"File '{fileName}' has been written to '{filePath}' successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access to the path '{filePath}' is denied. Details: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Error: The directory '{filePath}' does not exist. Details: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: An I/O error occurred while writing the file. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred. Details: {ex.Message}");
            }
        }

        public static string ReadFromFile(string filePath)
        {
            try
            {
                // Read the compressed data from the file
                byte[] compressedData = File.ReadAllBytes(filePath);

                // Decompress the file data
                string fileData = DecompressData(compressedData);

                Console.WriteLine($"File '{filePath}' has been read from '{filePath}' successfully.");
                return fileData;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access to the path '{filePath}' is denied. Details: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: The file '{filePath}' was not found in the directory '{filePath}'. Details: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: An I/O error occurred while reading the file. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred. Details: {ex.Message}");
            }

            return null;
        }

        public static byte[] CompressData(string data)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(byteArray, 0, byteArray.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public static string DecompressData(byte[] compressedData)
        {
            using (var memoryStream = new MemoryStream(compressedData))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzipStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        public static void DeleteAllFilesInFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            if (!Directory.Exists(folderPath))
                return;

            var files = Directory.GetFiles(folderPath);
            if (files == null)
                return;

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // Ignore exceptions
                }
            }
        }
        public static void CreateFolderIfNotExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Folder does not exist: {folderPath}");
                Console.WriteLine($"Creating folder: {folderPath}");
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Folder created: {folderPath}");
            }
        }
        public static bool FolderHasFiles(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return false;

            if (!Directory.Exists(folderPath))
                return false;

            return Directory.GetFiles(folderPath).Length > 0;
        }
        public static string JsonToBase64<T>(T data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(jsonBytes);
        }
        public static string FileToBase64(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return string.Empty;

            if (!File.Exists(filePath))
                return string.Empty;

            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }
        public static void WriteFile(string filePath, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Console.WriteLine("Error: File path is null or empty.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine("Error: File content is null or empty.");
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, content);

                Console.WriteLine($"File '{filePath}' has been written successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Error: Access to the path '{filePath}' is denied. Details: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: An I/O error occurred while writing the file. Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: An unexpected error occurred. Details: {ex.Message}");
            }
        }
    }
}
