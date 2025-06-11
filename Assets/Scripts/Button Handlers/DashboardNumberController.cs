using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardNumberController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider householdSizeSlider;
    public Slider savingsToggleSlider;
    public TextMeshProUGUI kwhText;
    public TextMeshProUGUI chfText;
    public TextMeshProUGUI solarText;
    public TextMeshProUGUI waterText;

    [Header("Datentabelle")]
    [SerializeField] private int[,] consumptionTable = new int[,]
    {
        // Haushalte 1-4, [vor, nach] für kWh, CHF, Solar, Wasser
        { 800, 600, 240, 180, 400, 544, 100, 145 }, // 1 Person
        { 1200, 800, 360, 240, 600, 544, 180, 145 }, // 2 Personen  
        { 1800, 1200, 540, 360, 800, 700, 250, 200 }, // 3 Personen
        { 2400, 1600, 720, 480, 1000, 900, 320, 250 } // 4 Personen
    };

    void Start()
    {
        SetupSliders();
        UpdateDisplay();
    }

    void SetupSliders()
    {
        householdSizeSlider.SetValueWithoutNotify(2);
        savingsToggleSlider.SetValueWithoutNotify(1);
        
        householdSizeSlider.onValueChanged.AddListener(_ => UpdateDisplay());
        savingsToggleSlider.onValueChanged.AddListener(_ => UpdateDisplay());
    }

    void UpdateDisplay()
    {
        int household = (int)householdSizeSlider.value - 1;
        int savings = (int)savingsToggleSlider.value;
        
        kwhText.text = consumptionTable[household, savings].ToString();
        chfText.text = consumptionTable[household, 2 + savings].ToString();
        solarText.text = consumptionTable[household, 4 + savings].ToString();
        waterText.text = consumptionTable[household, 6 + savings].ToString();
    }
}
