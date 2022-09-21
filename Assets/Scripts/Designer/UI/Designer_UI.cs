using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Designer_UI : MonoBehaviour
{
    [Header("Designer Manager")]
    public DesignerScript m_designerScript;


    [Header("UI Elements")]
    [SerializeField] GameObject m_partListContent;
    [SerializeField] GameObject m_itemPrefab;
    [SerializeField] InputField m_nameImputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetList(List<Part> _parts)
    {
        ClearList();

        AddToList(_parts);
    }

    public void AddToList(List<Part> _parts)
    {
        _parts.ForEach(part =>
        {
            {
                Item_UI buffer = Instantiate(m_itemPrefab, m_partListContent.transform).GetComponent<Item_UI>();
                buffer.m_itemName.text = part.name;
                buffer.m_button.onClick.AddListener(() => m_designerScript.AddComponent(part));
            }
        });
    }

    public void ClearList()
    {
        for (int i = 0; i < m_partListContent.transform.childCount; ++i)
        {
            Destroy(m_partListContent.transform.GetChild(i).gameObject);
        }
    }


    public void OnSave_Click()
    {
        if (m_nameImputField.text == "")
        {
            Debug.Log("Entrez un nom");
        }
        else
        {
            m_designerScript.m_name = m_nameImputField.text;
	        //m_designerScript.SaveUnitToScriptableObject();
	        
	        m_designerScript.SerializeUnit();
        }
    }
    
	public void OnLoad_Click(){
		if (m_nameImputField.text == "")
		{
			Debug.Log("Entrez un nom");
		}
		else
		{
			SerializationManager.Instance().DeserializeUnit(m_nameImputField.text + ".txt");
		}
	}
}
