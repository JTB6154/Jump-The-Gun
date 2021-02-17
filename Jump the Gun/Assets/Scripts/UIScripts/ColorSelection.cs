using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelection : MonoBehaviour
{

    GameObject crosshairImage;

    Slider redSlider;
    Slider greenSlider;
    Slider blueSlider;

    Text redText;
    Text greenText;
    Text blueText;

    // Start is called before the first frame update
    void Start()
    {
        crosshairImage = GameObject.Find("CrosshairImage");

        redSlider = GameObject.Find("RedSlider").GetComponent<Slider>();
        greenSlider = GameObject.Find("GreenSlider").GetComponent<Slider>();
        blueSlider = GameObject.Find("BlueSlider").GetComponent<Slider>();

        redSlider.value = GameStats.Instance.redColor * 255;
        greenSlider.value = GameStats.Instance.greenColor * 255;
        blueSlider.value = GameStats.Instance.blueColor * 255;

        redText = GameObject.Find("RedNumText").GetComponent<Text>();
        greenText = GameObject.Find("GreenNumText").GetComponent<Text>();
        blueText = GameObject.Find("BlueNumText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        redText.text = redSlider.value.ToString();
        greenText.text = greenSlider.value.ToString();
        blueText.text = blueSlider.value.ToString();

        crosshairImage.GetComponent<Image>().color = new Color(redSlider.value/255, greenSlider.value/255, blueSlider.value/255);
        GameStats.Instance.redColor = redSlider.value / 255;
        GameStats.Instance.greenColor = greenSlider.value / 255;
        GameStats.Instance.blueColor = blueSlider.value / 255;
    }
}
