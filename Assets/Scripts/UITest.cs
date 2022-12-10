using UnityEngine;
using TMPro;

public class UITest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FrameText = null;

    private float frames = 0f;
    private float frameTime = 0f;
    private float timeElaps = 0f;

    // Update is called once per frame
    private void Update()
    {
        ++frames;
        timeElaps += Time.unscaledDeltaTime;

        if (timeElaps > 1f)
        {
            frameTime = timeElaps / frames;
            timeElaps -= 1f;
            UpdateText();
            frames = 0f;
        }
    }

    private void UpdateText()
    {
        FrameText.text = string.Format("FPS : {0}, FrameTime : {1:F2} ms", frames, frameTime * 1000f);
    }
}
