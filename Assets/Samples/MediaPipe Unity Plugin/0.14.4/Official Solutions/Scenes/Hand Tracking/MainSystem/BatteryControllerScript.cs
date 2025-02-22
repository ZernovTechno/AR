using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BatteryControllerScript : MonoBehaviour
{
    [SerializeField] private RectTransform level;
    [SerializeField] private TextMeshProUGUI level_text;
    [SerializeField] private Image Color;
    [SerializeField] private Image Arrow;
    
    private Color Above03 = new(0, 0, 0);
    private Color Down03 = new(0, 0, 0);
    private float bat_level;
    // Start is called before the first frame update
    public void Start()
    {
        InvokeRepeating("UpdateBatteryLevel", 1, 5);
    }

    // Update is called once per frame
    public void UpdateBatteryLevel()
    {
        bat_level = SystemInfo.batteryLevel;
        level_text.text = bat_level * 100 + "%";
        level.offsetMax = new Vector2(level.offsetMax.x, -(26 - bat_level * 25));
        if (bat_level >= 0.3f)
        {
            Above03.r = 1.3f - bat_level;
            Above03.g = 230;
            Above03.b = 0;
            Color.color = Above03;
        }
        else if (bat_level < 0.3f)
        {
            Down03.r = 255;
            Down03.g = bat_level * 3;
            Down03.b = 0;
            Color.color = Down03;
        }
        if (SystemInfo.batteryStatus == BatteryStatus.Charging) Arrow.gameObject.SetActive(true);
        else Arrow.gameObject.SetActive(false);
    }
}   