# ItemSlotUI

ItemSlotUI와 이를 상속 받는 클래스, Default HUD Slot UI 관련 클래스들을 모아둔 폴더입니다. 

## ✔ ItemSlotUI 

아이템 슬롯의 UI를 관리하는 클래스입니다. 

각 아이템 슬롯에 ItemSlotUI가 할당됩니다. 

드래그 앤 드롭을 통해 아이템을 다른 슬롯으로 옮기거나, 다른 슬롯 아이템과 교환(swap)할 수 있습니다. 

> IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler 인터페이스 사용 

더블클릭을 통해 현재 열려있는 다른 컨테이너의 슬롯으로 아이템을 옮길 수 있습니다. 

> IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 인터페이스 사용

ItemSlot의 아이템의 상태가 변경되면 수량, 내구도 수치 등의 UI를 갱신합니다. 

아이템이 들어있는 슬롯위에 마우스를 올리면 아이템 정보(_infoUI)를 표시합니다.

###### Info UI

<img width="406" height="248" alt="image" src="https://github.com/user-attachments/assets/8ecad3f4-68db-43c8-bab1-05fa78fa83b0" />


아이템이 들어있는 슬롯을 우클릭하면 아이템의 타입에 따라 사용, 버리기 등의 슬롯 메뉴를 표시됩니다. 

###### Usable Item
<img width="185" height="124" alt="image" src="https://github.com/user-attachments/assets/dcfb43e5-58fc-4791-bf1a-f7b14f883289" />

###### Gun Item
<img width="186" height="161" alt="image" src="https://github.com/user-attachments/assets/c70a832a-f300-4cb7-88d7-bb45e1fb8422" />


 

 

## ✔ BoxSlotUI 

상자의 아이템 슬롯의 UI를 관리하는 클래스입니다. 

ItemSlotUI를 상속받습니다. 

더블클릭 시 인벤토리로 아이템 이동을 시도합니다. 

 

 

## ✔ EquipSlotUI 

인벤토리 장비 슬롯의 UI를 관리하는 클래스입니다. 

ItemSlotUI를 상속받습니다. 

Default HUD의 Equip Default HUD SlotUI와 아이템 이름, 탄약 수량, 이미지가 연동됩니다. 

###### 장비 슬롯 UI

<img width="398" height="209" alt="image" src="https://github.com/user-attachments/assets/653481d8-488b-425e-9d81-c2770f244679" />

 

 

## ✔ InventorySlotUI 

인벤토리 아이템 슬롯의 UI를 관리하는 클래스입니다. 

ItemSlotUI를 상속받습니다. 

###### 인벤토리 슬롯 UI

<img width="415" height="480" alt="image" src="https://github.com/user-attachments/assets/faa233e2-30e2-406c-a521-28dd778f5314" />

퀵슬롯 연결을 관리합니다. 

ItemSlotUI의 OnDrop, CombineItem 메서드를 오버라이드해 퀵슬롯 연결 관련 코드를 추가했습니다. 

###### 퀵슬롯 UI

<img width="493" height="157" alt="image" src="https://github.com/user-attachments/assets/e01183f7-d55d-4ee5-8046-bd22766fc4e4" />

인벤토리 아이템끼리 교환(swap)시, 기존 퀵슬롯 연결이 해제되지 않도록 구현했습니다. 

ItemSlotUI의 OnDoubleClick을 오버라이드하여 열려있는 컨테이너의 타입(상점, 창고, 박스)에 따라 더블클릭시 서로 다른 상호작용을 구현했습니다. 

> 창고, 상자: 아이템 이동 
> 
> 상점: 아이템 판매 



## ✔ StorageSlotUI 

창고 아이템 슬롯의 UI를 관리하는 클래스입니다. 

ItemSlotUI를 상속받습니다. 

ItemSlotUI의 OnDrop()을 오버라이드하여, 인벤토리에서 창고로 아이템 이동 시 기존 퀵슬롯 연결을 해제합니다. 

###### 창고 UI

<img width="429" height="514" alt="image" src="https://github.com/user-attachments/assets/74daeb2b-f1f2-4e14-8c9c-6929f539f8df" />

 

 

## ✔ ShopSlotUI 

상점 아이템 슬롯의 UI를 관리하는 클래스입니다. 

ItemSlotUI를 상속받습니다. 

ID로 상점 아이템을 설정합니다. 

ItemSlotUI의 OnDoubleClick()을 오버라이드해 더블클릭시 상점 아이템을 구매할 수 있도록 구현했습니다. 

ItemSlotUI의 드래그앤드롭관련 코드를 오버라이드해 드래그앤드롭 작동을 막았습니다. 

###### 상점 UI

<img width="466" height="463" alt="image" src="https://github.com/user-attachments/assets/2ed61c81-096e-4135-9a93-3ebd68118148" />


 
## ✔ DefaultHUDSlotUI 

HUD의 슬롯 UI에 표시할 아이템 정보를 설정하고 UI를 갱신합니다. 

HUD의 슬롯에는 장비 슬롯과 퀵슬롯이 표시됩니다.

슬롯당 하나씩 할당됩니다.

###### Default HUD Slot

<img width="665" height="170" alt="image" src="https://github.com/user-attachments/assets/822c3805-da2b-4664-a340-4a9c8cda4a2b" />

 

## ✔ EquipDefaultHUDSlotUI 

HUD의 장비 슬롯의 UI를 관리합니다. 

DefaultHUDSlotUI를 상속받습니다. 

슬롯이 선택될 시 아웃라인과 장비 정보가 활성화됩니다. 

> 장비 정보 중 탄약 갯수는 인벤토리에서 가져와 표시합니다.
