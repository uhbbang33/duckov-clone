### [ConvertProcess](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Data/ConvertProcess)
- Excel파일을 Json으로 변환하거나 Json을 파싱할때 사용하는 클래스를 모아둔 폴더입니다. 

### [SaveData](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Data/SaveData)
- 정보를 직렬화하기 위한 데이터 클래스를 모아둔 폴더입니다. 

 

## ItemData.cs 

모든 아이템의 공통 데이터를 관리하는 클래스입니다. 

아이템 데이터 클래스(AmmoData, EtcItemData, GunData, UsableItemData)는 ItemData클래스를 상속받아 필요한 속성만 추가합니다. 

ToItem()으로 데이터를 Item객체로 변환할 수 있습니다. 
