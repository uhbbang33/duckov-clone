using UnityEngine;

[DefaultExecutionOrder(-10)]
public class JsonParsing : MonoBehaviour
{
    private void Start()
    {
        DataManager dataManager = DataManager.Instance;

        TextAsset jsonText = LoadDataList("JsonData/GunItemData");
        dataManager.GunDatas = JsonUtility.FromJson<GunDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/AmmoItemData");
        dataManager.AmmoDatas = JsonUtility.FromJson<AmmoDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/UsableItemData");
        dataManager.UsableItemDatas = JsonUtility.FromJson<UsableItemDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/EtcItemData");
        dataManager.EtcItemDatas = JsonUtility.FromJson<EtcItemDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/EnemyData");
        dataManager.EnemyDatas = JsonUtility.FromJson<EnemyDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/PlayerBaseData");
        dataManager.PlayerBaseList = JsonUtility.FromJson<PlayerBaseDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/PlayerMoveData");
        dataManager.PlayerMoveList = JsonUtility.FromJson<PlayerMoveDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/PlayerSoundData");
        dataManager.PlayerSoundList = JsonUtility.FromJson<PlayerSoundDataList>(jsonText.text);


        Debug.Log(dataManager.PlayerSoundList.PlayerSoundDatas[0].DefaultSoundLevel);
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
