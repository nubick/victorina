using System;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class ImageStoryDotView : StoryDotView
    {
        private int? _pendingFileId;
        
        [Inject] private MasterFilesRepository MasterFilesRepository { get; set; }
        
        public Sprite NoImageSprite;
        public Image ImageBorder;
        public Image Image;

        public void Initialize()
        {
            MetagameEvents.ClientFileDownloaded.Subscribe(OnClientFileDownloaded);
        }
        
        protected override void OnShown()
        {
            StoryDot currentStoryDot = GetCurrentStoryDot();
            if (currentStoryDot is ImageStoryDot imageDot)
            {
                ShowImage(imageDot.FileId);
            }
            else
            {
                throw new Exception($"ImageQuestionView: Current story dot is not image, {currentStoryDot}");
            }
        }

        private void ShowImage(int fileId)
        {
            Image.sprite = GetSprite(fileId);
            ImageBorder.sprite = Image.sprite;
        }
        
        private Sprite GetSprite(int fileId)
        {
            Sprite sprite = NoImageSprite;

            if (MasterFilesRepository.Has(fileId))
            {
                byte[] bytes = MasterFilesRepository.GetBytes(fileId);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _pendingFileId = null;
            }
            else
            {
                _pendingFileId = fileId;
                Debug.LogWarning($"Don't have loaded file, fileId '{fileId}'");
            }
            return sprite;
        }

        private void OnClientFileDownloaded(int fileId)
        {
            if (!IsActive)
                return;

            if (_pendingFileId == fileId)
                ShowImage(fileId);
        }
    }
}