using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AudioVisualization.Services
{
    class StorageService
    {
        private static StorageService _current;
        public static StorageService Current { get { if (_current == null) { _current = new StorageService(); } return _current; } }

        private StorageService() { }

        public async Task<IReadOnlyList<StorageFile>> GetMusicLibrary()
        {
            var files = new List<StorageFile>();

            var folder = KnownFolders.MusicLibrary;
            await AddFilesFromFolder(files, folder);

            return files;
        }

        private async Task AddFilesFromFolder(List<StorageFile> files, StorageFolder folder)
        {
            var currentFiles = await folder.GetFilesAsync();

            foreach(var file in currentFiles)
            {
                files.Add(file);
            }

            var folders = await folder.GetFoldersAsync();

            foreach(var nestedFolder in folders)
            {
                await AddFilesFromFolder(files, nestedFolder);
            }
        }
    }
}
