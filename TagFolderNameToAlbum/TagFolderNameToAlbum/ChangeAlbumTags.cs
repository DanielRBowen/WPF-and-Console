using System.IO;

namespace TagFolderNameToAlbum
{
    internal static class ChangeAlbumTags
    {
        public static void ChangeAlbumTagsOfFolder(string folderPath)
        {
            var lastBackslashIndex = folderPath.LastIndexOf(@"\") + 1;
            var folderName = folderPath.Substring(lastBackslashIndex, folderPath.Length - lastBackslashIndex);
            folderName = Path.GetFileName(folderPath);
            var mp3FilePaths = Directory.EnumerateFiles(folderPath, "*.mp3", SearchOption.TopDirectoryOnly);

            foreach (var mp3FilePath in mp3FilePaths)
            {
                ChangeAlbumOfFile(mp3FilePath, folderName);
            }
        }

        /// <summary>
        /// Changes the Album of the file for the given file path
        /// 
        /// Tag lib library is here: https://github.com/mono/taglib-sharp/
        /// </summary>
        /// <param name="filePath"></param>
        private static void ChangeAlbumOfFile(string filePath, string folderName)
        {
            var tagFile = TagLib.File.Create(filePath);
            tagFile.Tag.Album = folderName;
            tagFile.Save();
        }
    }
}
