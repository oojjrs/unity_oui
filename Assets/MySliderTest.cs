using UnityEngine;
using UnityEngine.UI;

public class MySliderTest : MonoBehaviour, oojjrs.oui.MySlider.CallbackInterface, oojjrs.oui.MySlider.InitializerInterface
{
    [SerializeField]
    private Text _valueText;

    float oojjrs.oui.MySlider.InitializerInterface.InitialValue => 50f;

    void oojjrs.oui.MySlider.CallbackInterface.OnValueChanged(float value)
    {
        _valueText.text = value.ToString("0.##");
    }
}
