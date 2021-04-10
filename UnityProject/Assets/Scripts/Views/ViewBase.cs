using UnityEngine;

namespace Victorina
{
    public class ViewBase : MonoBehaviour
    {
        public GameObject Content;

        public bool IsActive => Content.activeSelf;
        
        protected virtual void OnShown() { }
        protected virtual void OnHide() { }

        public void Show()
        {
            // if (Static.BuildMode == BuildMode.Development)
            //     Debug.Log($"Show '{name}' view, {Time.time}");
            
            Content.SetActive(true);
            OnShown();
        }

        public void Hide()
        {
            // if (Static.BuildMode == BuildMode.Development)
            //     Debug.Log($"Hide '{name}' view, {Time.time}");
            
            Content.SetActive(false);
            OnHide();
        }

        protected void SwitchTo(ViewBase otherView)
        {
            Hide();
            otherView.Show();
        }

        protected void ClearChild(RectTransform root)
        {
            while (root.childCount > 0)
                DestroyImmediate(root.GetChild(0).gameObject);
        }
    }
}