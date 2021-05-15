using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CrafterQuestionWidget : MonoBehaviour
    {
        public Text Price;

        public void Bind(Question question)
        {
            Price.text = question.Price.ToString();
        }
    }
}