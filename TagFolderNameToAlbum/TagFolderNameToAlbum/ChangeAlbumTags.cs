using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TagFolderNameToAlbum
{
    internal static class ChangeAlbumTags
    {
        internal static async Task ChangeAlbumTagsOfFolderAsync(string folderPath, bool includeSubdirectories = false)
        {
            await Task.Delay(500);
            await Task.Run(() => ChangeAlbumTagsOfFolder(folderPath, includeSubdirectories));
            await Task.Delay(500);
        }

        public static void ChangeAlbumTagsOfFolder(string folderPath, bool includeSubdirectories = false)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    throw new InvalidOperationException("Folder does not exist");
                }

                var folderName = Path.GetFileName(folderPath);
                IEnumerable<string> mp3FilePaths;

                if (includeSubdirectories)
                {
                    mp3FilePaths = TagsDirectory.GetFiles(folderPath, @"\.mp3|\.flac|\.ogg|\.wav", SearchOption.AllDirectories);
                }
                else
                {
                    mp3FilePaths = TagsDirectory.GetFiles(folderPath, @"\.mp3|\.flac|\.ogg|\.wav", SearchOption.TopDirectoryOnly);
                }

                if (mp3FilePaths.Count() == 0)
                {
                    throw new InvalidDataException("No mp3, flac, ogg, or wav files found in folder.");
                }

                foreach (var mp3FilePath in mp3FilePaths)
                {
                    ChangeAlbumOfFile(mp3FilePath, folderName);
                }
            }
            catch (Exception exception)
            {
                throw exception;
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
