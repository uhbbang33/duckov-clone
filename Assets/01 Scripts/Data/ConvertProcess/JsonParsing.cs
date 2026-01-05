using UnityEngine;

public class JsonParsing : MonoBehaviour
{
    private void Start()
    {
        TextAsset jsonText = LoadDataList("JsonData/GunItemData");
        DataManager.Instance.GunDatas = JsonUtility.FromJson<GunDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/AmmoItemData");
        DataManager.Instance.AmmoDatas = JsonUtility.FromJson<AmmoDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/UsableItemData");
        DataManager.Instance.UsableItemDatas = JsonUtility.FromJson<UsableItemDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/EtcItemData");
        DataManager.Instance.EtcItemDatas = JsonUtility.FromJson<EtcItemDataList>(jsonText.text);
    }

    private TextAsset LoadDataList(string dataAddress)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(dataAddress);

        if(jsonText == null)
        {
            Debug.LogError("json Data not found");
        }

        return jsonText;
    }
}
