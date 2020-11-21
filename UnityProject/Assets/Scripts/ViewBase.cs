using UnityEngine;

namespace Victorina
{
    public class ViewBase : MonoBehaviour
    {
        public GameObject Content;

        public void Show()
        {
            Content.SetActive(true);
        }

        public void Hide()
        {
            Content.SetActive(false);   
        }
    }
}