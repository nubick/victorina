using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Victorina
{
    public class MasterFilesRepository : FilesRepository
    {
        private const int ChunkSize = 60 * 1024;

        private readonly HashSet<int> _fileIds = new HashSet<int>();
        private readonly Dictionary<int, string> _hashMap = new Dictionary<int, string>();
        
        private readonly Dictionary<int, byte[]> _fileBytesCache = new Dictionary<int, byte[]>();
        
        public void AddPackageFiles(Package package)
        {
            _fileIds.Clear();
            _hashMap.Clear();
            _fileBytesCache.Clear();
            
            EnsurePackageFilesDirectory();
            
            var questions = package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions));
            foreach (Question question in questions)
            {
                AddStoryFiles(question.QuestionStory);
                AddStoryFiles(question.AnswerStory);
            }

            Debug.Log($"Master Files were added, amount: {_fileIds.Count}");
        }

        private void AddStoryFiles(List<StoryDot> story)
        {
            foreach (StoryDot storyDot in story)
            {
                if (storyDot is FileStoryDot fileStoryDot)
                {
                    fileStoryDot.FileId = AddFile(fileStoryDot.SiqPath);
                    if (fileStoryDot.FileId != 0)
                        fileStoryDot.ChunksAmount = GetFileChunksAmount(fileStoryDot.FileId);
                }
            }
        }
        
        private int AddFile(string path)
        {
            int hash = 0;
            if (File.Exists(path))
            {
                hash = path.GetHashCode();
                if (_hashMap.ContainsKey(hash))
                {
                    string hashPath = _hashMap[hash];
                    if (hashPath.Equals(path))
                        Debug.Log($"There is the same file in different story dots, path: {path}");
                    else
                        Debug.LogWarning($"There is collision between two files, path1: {path}, path2: {hashPath}");
                }
                else
                {
                    string destFileName = GetPath(hash);
                    File.Copy(path, destFileName, overwrite: true);

                    _fileIds.Add(hash);
                    _hashMap.Add(hash, path);
                }
            }
            else
            {
                Debug.LogWarning($"Can't add master file, path is empty: {path}");
            }
            return hash;
        }
        
        public bool Has(int fileId)
        {
            return File.Exists(GetPath(fileId));
        }
        
        public byte[] GetBytes(int fileId)
        {
            if (Has(fileId))
            {
                if (!_fileBytesCache.ContainsKey(fileId))
                    _fileBytesCache.Add(fileId, File.ReadAllBytes(GetPath(fileId)));

                return _fileBytesCache[fileId];
            }

            throw new Exception($"Request bytes when file doesn't exist, fileId: {fileId}");
        }

        public byte[] GetFileChunk(int fileId, int chunkIndex)
        {
            byte[] bytes = GetBytes(fileId);
            int sourceStartIndex = chunkIndex * ChunkSize;
            int size = Mathf.Min(bytes.Length - sourceStartIndex, ChunkSize);
            byte[] chunk = new byte[size];
            Array.Copy(bytes, sourceStartIndex, chunk, 0, size);
            return chunk;
        }

        private int GetFileChunksAmount(int fileId)
        {
            string path = GetPath(fileId);
            FileInfo fileInfo = new FileInfo(path);
            int size = (int) fileInfo.Length;
            return size / ChunkSize + (size % ChunkSize == 0 ? 0 : 1);
        }
    }
}