using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private RawImage backgroundImage;
        
        public void SetScore(int score) => scoreText.text = score.ToString();

        public void SetBackgroundColor(Color color) => backgroundImage.color = color;

        public void SetTextColor(Color color) => scoreText.color = color;
    }
}
