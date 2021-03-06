using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterFilesRepository
    {
        [Inject] private PathSystem PathSystem { get; set; }
        
        private const int ChunkSize = 60 * 1024;

        private readonly HashSet<int> _fileIds = new HashSet<int>();
        private readonly Dictionary<int, string> _hashMap = new Dictionary<int, string>();
        private readonly Dictionary<int, byte[]> _fileBytesCache = new Dictionary<int, byte[]>();
        private readonly Dictionary<int, byte[][]> _chunksCache = new Dictionary<int, byte[][]>();
        
        public void AddPackageFiles(Package package)
        {
            _fileIds.Clear();
            _hashMap.Clear();
            _fileBytesCache.Clear();
            _chunksCache.Clear();
            
            foreach (Question question in PackageTools.GetAllQuestions(package))
            {
                AddStoryFiles(question.QuestionStory);
                AddStoryFiles(question.AnswerStory);
            }

            Debug.Log($"Master files are added, amount: {_fileIds.Count}");
        }

        private void AddStoryFiles(List<StoryDot> story)
        {
            foreach (StoryDot storyDot in story)
            {
                if (storyDot is FileStoryDot fileStoryDot)
                {
                    fileStoryDot.FileId = AddFile(fileStoryDot.Path);
                    if (fileStoryDot.FileId != 0)
                    {
                        fileStoryDot.ChunksAmount = GetFileChunksAmount(fileStoryDot.FileId);
                        BuildChunksCache(fileStoryDot.FileId, fileStoryDot.ChunksAmount);
                    }
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
                    string destFileName = PathSystem.GetPath(hash);
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

        private void BuildChunksCache(int fileId, int chunksAmount)
        {
            if (_fileBytesCache.ContainsKey(fileId))
            {
                Debug.Log($"Notification: file with the same id '{fileId}' was cached before");
                return;
            }
            
            string path = PathSystem.GetPath(fileId);
            byte[] bytes = File.ReadAllBytes(path);
            _fileBytesCache.Add(fileId, bytes);
            byte[][] chunks = new byte[chunksAmount][];
            _chunksCache.Add(fileId, chunks);
            for (int chunkIndex = 0; chunkIndex < chunksAmount; chunkIndex++)
                chunks[chunkIndex] = FetchChunk(bytes, chunkIndex);
        }

        public bool Has(int fileId)
        {
            return File.Exists(PathSystem.GetPath(fileId));
        }
        
        public byte[] GetBytes(int fileId)
        {
            if (Has(fileId))
            {
                if (!_fileBytesCache.ContainsKey(fileId))
                    _fileBytesCache.Add(fileId, File.ReadAllBytes(PathSystem.GetPath(fileId)));

                return _fileBytesCache[fileId];
            }

            throw new Exception($"Request bytes when file doesn't exist, fileId: {fileId}");
        }
        
        private byte[] FetchChunk(byte[] bytes, int chunkIndex)
        {
            int sourceStartIndex = chunkIndex * ChunkSize;
            int size = Mathf.Min(bytes.Length - sourceStartIndex, ChunkSize);
            byte[] chunk = new byte[size];
            Array.Copy(bytes, sourceStartIndex, chunk, 0, size);
            return chunk;
        }
        
        public byte[] GetFileChunk(int fileId, int chunkIndex)
        {
            return _chunksCache[fileId][chunkIndex];
        }

        private int GetFileChunksAmount(int fileId)
        {
            string path = PathSystem.GetPath(fileId);
            FileInfo fileInfo = new FileInfo(path);
            int size = (int) fileInfo.Length;
            return size / ChunkSize + (size % ChunkSize == 0 ? 0 : 1);
        }

        public void CopyToTempVideoFilePath(int fileId)
        {
            string path = PathSystem.GetPath(fileId);
            File.Copy(path, PathSystem.Data.TempVideoFilePath, overwrite: true);
        }
    }
}