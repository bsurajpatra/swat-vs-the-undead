using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    public Image damageImage;
    public float flashSpeed = 5f; // how fast the red fades away
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // semi-transparent red

    private bool isFlashing = false;

    void Awake()
    {
        if (damageImage != null)
            damageImage.color = Color.clear; // start transparent
    }

    void Update()
    {
        if (damageImage == null) return;

        if (isFlashing)
        {
            // Gradually fade from red to transparent
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

            // Once almost clear, stop flashing
            if (damageImage.color.a < 0.01f)
            {
                damageImage.color = Color.clear;
                isFlashing = false;
            }
        }
    }

    public void TriggerFlash()
    {
        if (damageImage != null)
        {
            damageImage.color = flashColor; // make it red
            isFlashing = true; // start fading out
        }
    }
}
