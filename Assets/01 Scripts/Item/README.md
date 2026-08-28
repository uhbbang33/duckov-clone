### [ItemContainer](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Item/ItemContainer)

여러 아이템 슬롯을 관리하는 컨테이너 클래스를 모아둔 폴더입니다. 

### [ItemSlot](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Item/ItemSlot)

아이템을 담는 ItemSlot 클래스와 ItemSlot를 상속받는 클래스를 모아둔 폴더입니다. 

### [Items](http://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Item/Items)

아이템 기본정보를 담는 Item 클래스와 Item을 상속받는 클래스를 모아둔 폴더입니다.

 
## ✔ DroppedItem

플레이어가 버린(드랍한) 아이템 오브젝트를 관리합니다. 

플레이어가 아이템을 드랍 시 아이템 ID을 기준으로 ItemSpriteDictionary에서 Sprite를 가져와 SpriteRenderer에 적용합니다. 

드랍 아이템의 위치를 결정합니다.

> 플레이어 앞쪽 부채꼴 영역에 우선적으로 배치될 수 있도록 시도합니다. 
> 
> 배치에 실패했다면, 부채꼴 외 나머지 영역에 배치 시도합니다. 

아이템이 땅 바로 위에 배치되도록 구현했습니다.

> TrySetPosition에서 지정된 위치에서 아래로 ray를 쏜 후, 땅(groundLayer)에 ray가 맞은 부분에 아이템을 배치합니다. 

아이템이 다른 아이템과 겹치지 않도록 구현했습니다.

드랍 아이템과 상호작용을 구현했습니다.

상호작용 시 인벤토리의 TryAddItem()을 호출합니다. 

> 인벤토리에 등록 성공 시 드랍 아이템 오브젝트를 삭제합니다. 

 
## ✔ ItemSpriteDictionary

GameResources에 저장되어 있는 Sprite들을 불러와 Dictionary<int, Sprite>에 Id와 함께 저장합니다. 

싱글톤으로 구현했으며, GetItemSprite(int id)로 Sprite를 불러올 수 있도록 구현했습니다. 
 

## ✔ ItemTypeWeight.cs 

상자의 아이템 타입 가중치를 정할 때 사용하는 클래스입니다. 
