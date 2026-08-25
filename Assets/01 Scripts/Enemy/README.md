# Enemy
### FSM(Finite State Machine)와 Navemesh를 활용해 적 AI를 구현했습니다. 


[State Machine 폴더](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Enemy/State%20Machine)


## ✔ Enemy

적 AI의 데이터를 관리합니다. 

> Enemy Data, Gun Data 

현재 상태(State)를 관리하며 상태 전환을 담당합니다. 

> Dictionary<Enum, EnemyState(class)>를 사용하여 State를 빠르게 조회합니다. 

퍼사드(Facade) 패턴 사용 

> 서브 컴포넌트와 State 클래스 간의 직접적인 참조를 줄이고, Enemy를 통해 필요한 기능에 접근하도록 구성했습니다.
 

## ✔ EnemyDetection

플레이어를 감지할 때 사용됩니다. 

- 시야로 플레이어 감지

  1. 플레이어와의 거리가 적의 시야거리 안에 들어있는지 체크 

  2. 플레이어가 시야각 안에 들어있는지 체크 

  3. 적과 플레이어 사이에 장애물이 존재하는지 Raycast로 체크 

- 소리로 플레이어 감지

플레이어와의 거리와 방향 계산을 담당합니다. 



## ✔ EnemyHealth

적의 HP 회복과 죽음을 관리합니다. 

적이 죽은 자리에 전리품 상자를 Object Pool에서 가져와 생성합니다. 

 

## ✔ EnemySound

적과 관련된 효과음들을 관리합니다. 

 

## ✔ EnemyUI 

적의 World Space Canvas를 관리합니다. 

플레이어를 발견하거나 놓쳤을 때 보이는 아이콘의 활성화 및 비활성화를 담당합니다. 

플레이어의 시야에 따라 Canvas의 Render를 활성화 또는 비활성화합니다. 
