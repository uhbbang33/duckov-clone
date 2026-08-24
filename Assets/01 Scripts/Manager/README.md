# Manager

> 매니저 클래스를 모아둔 폴더입니다.
> 
> 모든 매니저클래스는 싱글톤을 사용하여 관리됩니다. 


## ✔ BunkerManager

BunkerScene을 관리하는 클래스입니다. 

Unity Editor에서 Shop, Storage Slot 오브젝트를 BunkerManager에 등록하고, 각 Slot 정보를 Shop, Storage 클래스에 전달합니다. 


## ✔ DataManager 

Data와 Data List를 관리하는 클래스입니다. 

ID 및 타입을 기반으로 데이터를 조회하거나 랜덤 아이템을 생성합니다. 

DontDestroyOnLoad를 사용해 씬 전환 후에도 데이터를 유지합니다. 

 

### [데이터 관리] 

> Gun, Ammo, UsableItem(Food, Medicine), EtcItem, Enemy, Player, Shop 데이터를 관리합니다. 
>
> 모든 아이템 데이터를 Dictionary<int, ItemData>에 저장하여 ID를 통해 빠른 조회가 가능하도록 구현 했습니다. 

 

### [랜덤 아이템 생성] 

> 아이템의 타입별로 랜덤한 아이템을 반환하는 메서드를 구현했습니다. 
>
> 아이템의 가중치(WeightValue)를 기반으로 아이템을 반환합니다. 

 

### [씬 별 데이터 저장 및 로드] 

> 현재 씬에 따라 데이터를 저장하고 로드합니다. 
> 
> - Bunker 씬: 시간, 플레이어 스탯, 인벤토리, 창고 
> 
> - Field 씬: 시간, 플레이어 스탯, 인벤토리 



## ✔ FieldManager

FieldScene을 관리하는 클래스입니다. 

Unity Editor에서 Box Slot 오브젝트를 FieldManager에 등록하고, Slot 정보를 Box 클래스에 전달합니다. 

현재 플레이어와 가장 가까운 상자와 현재 열려있는 상자를 관리합니다. 

적 간 충돌이 일어나지 않도록 Enemy AI priority를 관리합니다. 


## ✔ GameManager 

게임 전반에서 공통으로 사용하는 객체와 상태를 관리합니다. 

씬 전환과 입력 시스템을 제어합니다. 

 

### [게임 전역 데이터 관리] 

> Inventory, PlayerObject, DayNightCycle, 현재 씬 정보를 관리합니다. 
> 
> DontDestroyOnLoad를 사용해 씬 전환 후에도 상태를 유지합니다. 

 

### [인벤토리 초기화]  

> Unity Editor에서 설정한 슬롯 수와 최대 무게를 기반으로 인벤토리를 초기화합니다. 

 

### [Input System 관리] 

> InputActions를 생성합니다. 
> 
> Input Action을 활성화 및 비활성화하는 메서드를 구현해 일시정지 등의 상황에 사용합니다. 

 

### [게임 종료] 

> 게임 종료 전 DataManager를 통해 현재 씬의 게임 데이터를 저장합니다. 
> 
> Unity Editor와 빌드 환경을 구분해 게임을 종료합니다. 


## ✔ GameResources 

게임 전역에서 사용하는 Resource만 따로 모아놓은 클래스입니다. 

Resources폴더에 GameResources 오브젝트를 생성해 GameResources.cs를 컴포넌트로 등록해 사용합니다. 

 

## ✔ PauseManager 

게임 일시정지 상태와 Pause UI를 관리하는 클래스입니다. 

Time.timeScale을 조절해 게임을 일시정지하고 재개합니다. 

일시정지 중 플레이어 조작을 제한하고, 오디오를 일시정지합니다. 

### [Pause UI 관리] 

> Pause UI의 활성화 및 비활성화를 관리합니다. 
> 
> 인터페이스 IUICloseable을 구현하여 UIManager에서 UI를 공통적으로 닫을 수 있도록 구성했습니다. 

 

### [Pause 메뉴 기능] 

> 게임으로 돌아가기 
> 
> 타이틀 화면으로 돌아가기 
> 
> 설정 
>
> 게임 종료 
 

## ✔ PoolManager 

오브젝트 풀링을 통해 반복적으로 생성 및  삭제되는 게임 오브젝트를 관리하는 클래스입니다. 

Dictionary를 통해 ID로 Pool을 빠르게 검색하고, Queue를 사용해 생성된 오브젝트를 순차적으로 관리하도록 구현했습니다. 

 

### [Object Pool 생성] 

> Unity Inspector에서 ID, Pool Size, Prefab을 등록합니다. 
> 
> 게임 시작 시 등록된 Prefab을 지정된 개수만큼 미리 생성하고 비활성화해 Pool을 구성합니다. 
> 
> Dictionary<uint, Queue<GameObject>> _poolDictionary 로 ID별 Pool을 관리합니다. 

 

### [Get Object 메서드] 

> ID를 통해 Pool에서 비활성화된 오브젝트를 반환합니다. 
> 
> Pool에 사용 가능한 오브젝트가 없을 경우 Prefab을 새로 생성합니다. 
>
> 부모를 변경하거나 Transform을 설정할 수 있습니다. 

 

### [Return Object 메서드] 

> 사용이 끝난 오브젝트를 ID에 따라 해당 Pool의 Queue에 반환하고 비활성화합니다. 
 

## ✔ QuickSlotManager 

인벤토리 아이템과 퀵슬롯의 연결을 관리하는 클래스입니다. 

 

### [퀵슬롯 등록 관리] 

> ‘Key: ID, Value: 퀵슬롯 위치’ 인 Dictionary를 통해 퀵슬롯 등록 정보를 관리합니다. 
> 
> 하나의 아이템이 여러 퀵슬롯에 중복 등록되지 않도록 구현했습니다. 
> 
> 이미 다른 퀵슬롯에 등록된 아이템을 새로운 퀵슬롯에 등록할 경우 기존 연결을 해제하고 새로운 퀵슬롯에 연결합니다. 

 

### [퀵슬롯 연결 해제] 

> 아이템 ID를 기반으로 퀵슬롯 등록 정보를 제거합니다. 
>
> 연결된 Inventory Slot UI와의 참조도 해제합니다. 

 

### [인벤토리 슬롯 정보 관리] 

> List<int> 형태로 각 퀵슬롯에 연결된 인벤토리 슬롯의 Index를 반환합니다. 
>
> 이는 퀵슬롯 저장에 사용합니다. 


## ✔ SaveAndLoadManager 

게임 데이터를 JSON 파일로 저장하고 로드하며, 저장 파일을 관리하는 클래스입니다. 

각 JSON 파일 단위로 저장, 로드, 삭제가 가능합니다. 

 

### [데이터 저장] 

> JsonUtility를 사용해 게임 데이터를 JSON으로 직렬화하고 저장합니다. 
> 
> 제네릭 Save<T>()를 사용해 다양한 데이터 타입을 공통된 로직으로 저장할 수 있도록 구성했습니다. 

 

### [데이터 로드] 

> 저장된 JSON파일을 읽어 데이터 타입으로 역직렬화합니다. 
>
> 제네릭 Load<T>()를 사용해 다양한 데이터 타입을 공통된 로직으로 로드할 수 있도록 구성했습니다. 
>
> 저장 파일이 존재하지 않을 경우 초기 데이터를 설정할 수 있도록 구성했습니다. 

 

 

 

## ✔ SoundManager 

BGM과 SFX를 통합 관리하고 재생하는 클래스입니다. 

 

### [BGM 관리] 

> Title, Bunker등 씬별 BGM을 관리하고 재생합니다. 

 

### [SFX 관리] 

> 게임 전반에서 사용하는 SFX를 Dictionary에 등록하여 이름으로 관리합니다. 
> 
> SFX의 이름은 SFXName이라는 정적 클래스를 통해 상수로 관리됩니다. 
>
> 걷기와 달리기를 구분, 이동 속도에 따라 발소리를 다르게 재생합니다. 

 

### [Audio Mixer 연동] 

> Audio Mixer의 PitchShifter 파라미터를 조절하여 효과음의 Pitch를 조정했습니다. 
>
> 일시정지한 경우에도 특정 AudioSource의 재생을 유지할 수 있도록 ignoreListenerPause를 설정했습니다. 

 

 

 

## ✔ TitleManager 

TitleScene의 게임 시작 및 세이브 데이터 관리를 담당하는 클래스입니다. 

 

타이틀 화면 진입 시 Cursor를 표시하고 Title BGM을 재생합니다. 

새 게임, 불러오기, 설정, 게임 종료 버튼을 관리합니다. 

 

### [게임 시작 버튼] 

> 새 게임과 불러오기를 구분하여 BunkerScene으로 이동합니다. 
>
> ‘새 게임’ 선택 시 기존 저장 데이터가 존재하면 초기화 여부를 확인하는 팝업을 표시합니다. 
>
> ‘새 게임’을 확정하면 기존 저장 데이터를 삭제한 후 새로운 게임을 시작합니다. 
>
> ‘불러오기’ 선택 시 기존 저장 데이터를 유지한 상태로 게임을 시작합니다. 

 

 

 

## ✔ UIManager 

게임 내 UI의 상태와 UI 간 상호작용을 관리하는 클래스입니다. 

 

### [UI 상태 관리] 

> 인벤토리, 상자, 창고, 상점 GameOver의 UI 활성화 상태를 관리합니다. 
> 
> Player HUD, Crosshair, Cursor의 표시 상태를 관리합니다. 

 

### [Item Slot Menu 관리] 

> Item Slot 우클릭 시 아이템 타입과 슬롯 타입에 따라 사용할 수 있는 버튼을 동적으로 활성화합니다. 
> 
> Equip, Unload, Use, Split, Discard 버튼을 활성화합니다. 
>
> (예) 아이템 타입이 Gun, 슬롯 타입이 EQUIP일 경우: Equip, Unload 버튼만 활성화 

 

 

### [UI Stack 관리] 

> Stack<IUICloseable>을 사용하여 현재 열린 UI를 관리합니다. 
>
> UI를 Stack에 Push/Pop 하고 플레이어가 UI를 닫는 버튼을 누를 경우, 최상단 UI를 닫을 수 있도록 구현했습니다. 

 

### [UI 정보 갱신] 

> 인벤토리와 창고의 아이템 개수, 무게, 소지금 등을 갱신합니다. 
>
> 플레이어 HP, Hunger, Hydration을 Slider를 통해 표시합니다. 
>
> Hunger와 Hydration 상태에 따라 Slider Background의 색상을 변경합니다. 
