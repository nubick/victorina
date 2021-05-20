using System.Collections;
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
            Content.SetActive(true);
            OnShown();
        }

        public IEnumerator ShowAndWaitForFinish()
        {
            Show();
            while (IsActive)
                yield return null;
        }

        public void Hide()
        {
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

        protected void ClearChild(RectTransform root, GameObject ignore)
        {
            int remaining = 0;
            while (root.childCount > remaining)
            {
                GameObject child = root.GetChild(remaining).gameObject;
                if (child == ignore)
                    remaining++;
                else
                    DestroyImmediate(child);
            }
        }
    }
}