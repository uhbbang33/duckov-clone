# UI

UI 관련 클래스를 모아둔 폴더입니다.

[UI Controller](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/UI/Controller)

[UI Interactable](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/UI/Interactable)

> 상호작용 가능한 오브젝트의 UI를 관리하는 클래스를 모아둔 폴더입니다.

[UI ItemSlot](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/UI/ItemSlot)
> ItemSlotUI와 이를 상속 받는 클래스, Default HUD Slot UI 관련 클래스들을 모아둔 폴더입니다.

[UI Text](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/UI/Text)

[UI TitleScene](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/UI/TitleScene) 

> Title 씬에서만 사용되는 UI 클래스 폴더입니다.



## ✔ CanvasLookAtCamera 

캔버스가 항상 카메라를 바라보도록 구현한 클래스입니다. 

렌더 모드가 World Space인 캔버스에 사용합니다.

###### 플레이어 HP Bar등에 적용

<img width="140" height="115" alt="image" src="https://github.com/user-attachments/assets/c2f9182c-df1b-4ab1-8617-00074ebe100f" />



## ✔ SetWorldSpaceCanvas 

렌더 모드가 World Space인 캔버스의 크기를 조절하기 위해 구현한 클래스입니다. 

 

## ✔ GameOverUI 

GameOver UI를 관리하는 클래스입니다. 

FadeController를 상속받아 게임 오버 시 FadeIn() 메서드를 사용합니다. 

Button이벤트로 “계속하기” 버튼을 클릭 시 벙커씬을 로드합니다. 

###### Game Over UI

<img width="288" height="180" alt="image" src="https://github.com/user-attachments/assets/6c4c313d-206b-4d65-9069-ce729220b886" />


## ✔ ItemInfoUI 

아이템의 정보를 표시하는 UI를 관리하는 클래스입니다. 

InfoUI가 마우스를 따라가도록 구현했습니다. 

아이템 타입별로 표시되는 정보가 달라지도록 구현했습니다. 

###### 약품 아이템 Info UI

<img width="323" height="209" alt="image" src="https://github.com/user-attachments/assets/d7c428b0-0873-4e0a-9dc1-225257d3ebe0" />

###### 총 아이템 Info UI

<img width="323" height="304" alt="image" src="https://github.com/user-attachments/assets/13c152d4-16bc-41df-bc23-1b7a3462e694" />


## ✔ ItemSplitUI 

아이템을 분할할 때 표시되는 UI를 관리하는 클래스입니다. 

<img width="514" height="251" alt="image" src="https://github.com/user-attachments/assets/d1559afe-3022-4084-a5be-a901cd71a6be" />

 

## ✔ LoadingUI 

로딩 UI를 관리하는 클래스입니다. 

싱글톤 패턴을 사용하였습니다. 

게임 실행 시 한 번 생성되며 DontDestroyOnLoad로 설정됩니다. 

FadeController 클래스를 상속받아 FadeIn, FadeOut 메서드를 사용합니다. 

SceneLoader 클래스에서 사용합니다. 

 

## ✔ TimeUI 

시간 UI를 관리하는 클래스입니다. 

밤낮(DayNightCycle)의 현재 시간(currentTime)에 따라 시간 Text를 업데이트합니다. 

###### Time UI

<img width="89" height="51" alt="image" src="https://github.com/user-attachments/assets/c4c0b43c-b122-4bc4-b0c3-05e40f5eb3cc" />
