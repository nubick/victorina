using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerWidget : MonoBehaviour
    {
        public Text PlayerName;

        public void Bind(string playerName)
        {
            PlayerName.text = playerName;
        }
    }
}