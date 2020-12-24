using UnityEngine;

namespace Victorina
{
    public class ViewBase : MonoBehaviour
    {
        public GameObject Content;

        public bool IsActive => Content.activeSelf;
        
        protected virtual void OnShown() { }

        public void Show()
        {
            Content.SetActive(true);
            OnShown();
        }

        public void Hide()
        {
            Content.SetActive(false);   
        }

        protected void SwitchTo(ViewBase otherView)
        {
            Hide();
            otherView.Show();
        }
    }
}