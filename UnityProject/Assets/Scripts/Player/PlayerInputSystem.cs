using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayerInputSystem : MonoBehaviour
    {
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerAnswerSystem.OnAnyKeyDown();
            }
        }
    }
}