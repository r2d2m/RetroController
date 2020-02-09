using UnityEngine;
using UnityEngine.UI;

namespace vnc.Samples
{
    public class SampleUI : MonoBehaviour
    {
        public static SampleUI Instance { get; private set; }

        public Text m_textMessage;
        public Image m_textBackground;
        public CanvasGroup m_helpPanel;

        [Header("Settings")]
        public float m_speed = 4;
        public float m_delayPerWord = 0.06f;
        public float m_additionalDelay = 2f;

        float timer;
        string displayText;
        bool helpPanelEnabled;

        public void Awake()
        {
            Instance = this;
            m_helpPanel.blocksRaycasts = false;
            m_helpPanel.interactable = false;
            m_helpPanel.alpha = 0f;
        }

        private void Update()
        {
            var scale = m_textBackground.rectTransform.localScale;

            if (Time.time > timer)
            {
                m_textBackground.rectTransform.localScale = Vector3.Lerp(scale, new Vector3(0, 1, 1), m_speed * Time.deltaTime);
                m_textMessage.text = string.Empty;
            }
            else
            {
                m_textBackground.rectTransform.localScale = Vector3.Lerp(scale, Vector3.one, m_speed * Time.deltaTime);
                if (m_textBackground.rectTransform.localScale.x > 0.99)
                    m_textMessage.text = displayText;
            }

            if (Input.GetKeyDown(KeyCode.F1))
                helpPanelEnabled = !helpPanelEnabled;

            m_helpPanel.alpha = helpPanelEnabled ? 1f : 0f;
        }

        public void Write(string text)
        {
            displayText = text.ToUpperInvariant();
            timer = Time.time + m_additionalDelay + text.Length * m_delayPerWord;

            m_textMessage.text = string.Empty;
            m_textBackground.rectTransform.localScale = new Vector3(0, 1, 1);
        }
    }
}