# Misc
> Utility나 Define, Scene Load 등의 기능을 하는 기타 스크립트를 모아둔 폴더입니다. 

## ✔ CameraShakeController 

총기 발사 시 총기의 반동에 따라 카메라 흔들림을 발생시키는 클래스입니다. 

플레이어가 총기를 발사할 때 ShakeOnFire()를 호출하여 카메라 흔들림을 발생시킵니다. 

흔들림의 방향과 강도를 계산 후 반동을 적용합니다. 


## ✔ Define 

매직 넘버와 하드코딩을 줄이기 위해 프로젝트 전반에서 공통으로 사용하는 상수값을 한 곳에서 관리합니다. 
 

## ✔ Enums 

프로젝트 전반에서 사용하는 상태 및 유형을 enum으로 정의하여 관리합니다. 
 

## ✔ SaveUtility 

저장 시스템에서 필요한 슬롯 정보를 반환하는 정적 유틸리티 클래스입니다. 

정적 클래스로 구현해 별도의 인스턴스 생성없이 전역에서 사용 가능합니다. 

 

ISaveableContainer에서 슬롯 정보를 가져와 저장에 필요한 데이터로 변환합니다. 

슬롯의 ID, AmmoCount, Quantity, Durability 데이터를 List<int> 형태로 반환합니다. 


## ✔ SceneInitializer 

씬 내 다른 객체들의 초기화가 완료된 후 이벤트(OnSceneReady)를 호출하여 씬 초기화 완료 시점을 전달하는 클래스입니다. 

SceneLoader에서 이 이벤트를 구독해 씬 초기화가 완료된 이후에 저장 데이터를 적용하는 작업을 수행합니다. 

[DefaultExecutionOrder(1000)] 을 적용하여 다른 MonoBehaviour보다 늦게 실행되도록 구현했습니다. 


## ✔ SceneLoader 

씬 전환 과정과 로딩 UI를 관리하는 씬 로딩 클래스입니다. 

 

싱글톤으로 구현했으며 DontDestroyOnLoad를 사용해 씬 전환 과정에서 하나의 인스턴스만 유지합니다. 

Coroutine을 사용해 씬 전환 중 중복 로딩을 방지합니다. 

LoadSceneAsync()를 사용해 비동기로 씬을 로딩합니다. 

씬 로딩 중 입력을 비활성화하고 Loading UI의 FadeIn() / FadeOut()를 호출합니다. 

SceneInitializer.OnSceneReady를 통해 씬 내 초기화가 완료된 후 저장 데이터를 적용합니다. 


## ✔ SingletonMonoBehaviour<T> 

MonoBehaviour를 상속하는 클래스에서 싱글톤 패턴을 사용할 수 있도록 구현한 제네릭 추상 클래스입니다. 

Awake()에서 인스턴스를 등록, 중복 생성된 객체는 제거합니다. 

OnDestroy()에서 현재 인스턴스가 제거될 경우 참조를 초기화합니다. 

상속받은 클래스는 별도의 싱글톤 구현 없이 Instance를 통해 접근이 가능합니다.  


## ✔ SortUtility 

아이템 컨테이너의 아이템을 병합, 정렬하는 정적 유틸리티 클래스입니다. 

ISortableContainer를 통해 정렬 대상의 슬롯을 가져와 처리합니다. 

동일한 Stackable 아이템을 하나로 병합하고 최대 Stack 크기에 맞게 다시 분할합니다. 

정렬 과정에서 QuickSlot 연결 정보를 유지합니다. 

정렬 완료 후 ISortableContainer의 OnSortCompleted()를 호출해 정렬 결과를 전달합니다. 

 

정렬 우선순위 

1. ItemType 우선순위 오름차순 

  - Gun -> Ammo -> Medicine -> Food -> Etc 

2. 아이템 무게 내림차순 

3. 아이템 내구도 오름차순 
