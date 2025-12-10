using UnityEngine;
using UnityEngine.UI;

public class SimpleTransactionPieChart : MonoBehaviour
{
    public float chartSize = 300f;
    public Sprite circleSprite;
    
    void Start()
    {
        CreateTransactionPieChart();
    }
    
    void CreateTransactionPieChart()
    {
        // Clear old
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        
        // From your screenshot:
        // 1. cash sales: Income • Sales = +$2,000.00 (GREEN)
        // 2. gas: Expense • Utilities = $500.00 (RED)
        
        float[] values = { 2000f, 500f };    // $2000 cash sales, $500 gas
        string[] names = { "Cash Sales", "Gas" };
        Color[] colors = { Color.green, Color.red };
        
        float total = 2000f + 500f; // $2500 total
        
        // Create pie slices
        float currentAngle = 0;
        
        for (int i = 0; i < values.Length; i++)
        {
            float percent = values[i] / total;
            
            GameObject slice = new GameObject(names[i]);
            slice.transform.SetParent(transform, false);
            
            Image img = slice.AddComponent<Image>();
            img.sprite = circleSprite;
            img.color = colors[i];
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Radial360;
            img.fillAmount = percent;
            img.fillOrigin = 0;
            
            RectTransform rt = slice.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(chartSize, chartSize);
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.Euler(0, 0, currentAngle);
            
            // Add label
            CreateLabel(names[i], values[i], percent, i);
            
            currentAngle -= percent * 360;
        }
        
        Debug.Log("Pie Chart Created:");
        Debug.Log("- Cash Sales: $2000.00 (80%)");
        Debug.Log("- Gas: $500.00 (20%)");
        Debug.Log("- Total: $2500.00");
    }
    
    void CreateLabel(string name, float amount, float percent, int index)
    {
        GameObject label = new GameObject(name + "_Label");
        label.transform.SetParent(transform, false);
        
        Text text = label.AddComponent<Text>();
        text.text = $"{name}\n${amount:F2}\n{(percent*100):F0}%";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 40; // Slightly larger for bold
        text.fontStyle = FontStyle.Bold; // ← ADDED BOLD FONT STYLE
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        
        RectTransform rt = label.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 60);
        
        // Position labels on opposite sides
        if (index == 0) // Cash Sales - left side
            rt.localPosition = new Vector3(-chartSize * 0.5f, 0, 0);
        else // Gas - right side
            rt.localPosition = new Vector3(chartSize * 0.5f, 0, 0);
        
        // Add background for readability
        GameObject bg = new GameObject("LabelBG");
        bg.transform.SetParent(label.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.sizeDelta = new Vector2(110, 70);
        bgRt.SetAsFirstSibling();
    }
}