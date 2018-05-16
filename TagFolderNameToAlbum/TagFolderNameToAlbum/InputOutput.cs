using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TagFolderNameToAlbum
{
    class InputOutput
    {
        private static string FolderName { get; set; }

        internal static Task ChangeAlbumTagsOfFolderAsync(string folderPath)
        {
            var positionOfLastBackslash = folderPath.LastIndexOf($"\\") + 1;
            FolderName = folderPath.Substring(positionOfLastBackslash, folderPath.Length - positionOfLastBackslash);

            return Task.Run(() => ChangeAlbumTagsOfFolder(folderPath));
        }

        private static void ChangeAlbumTagsOfFolder(string folderPath)
        {
            var mp3Files = Directory.GetFiles(folderPath, "*.mp3")
                                     .Select(Path.GetFullPath)
                                     .ToList();


            mp3Files.ForEach(ChangeAlbumOfFile);
        }

        /// <summary>
        /// Changes the Album of the file for the given file path
        /// 
        /// Tag lib library is here: https://github.com/mono/taglib-sharp/
        /// </summary>
        /// <param name="filePath"></param>
        private static void ChangeAlbumOfFile(string filePath)
        {
            TagLib.File tagFile = TagLib.File.Create(filePath);
            tagFile.Tag.Album = FolderName;
            tagFile.Save();
        }
    }
}
