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
            Debug.Log($"Show '{name}' view");
            Content.SetActive(true);
            OnShown();
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
    }
}