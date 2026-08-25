# State Machine 

### 적은 FSM(Finite State Machine)을 사용해 구현했습니다. 

<img width="796" height="468" alt="image" src="https://github.com/user-attachments/assets/6c416098-af43-49ac-8750-f278f3391504" />


## FSM 상태 클래스 

## ✔ EnemyStateBase

모든 적 상태(State) 클래스의 부모 추상 클래스입니다. 

추상 메서드 Enter, Exit, Update를 통해 상태 진입, 종료, 업데이트 로직을 정의합니다. 

 

## ✔ AttackState 

적의 공격 상태를 담당합니다. 

플레이어 조준 및 연사, 공격 쿨타임, 재장전, ChaseState 전환을 처리합니다. 

총알(Bullet) 오브젝트를 Object Pool에서 가져와 생성합니다. 

 

## ✔ ChaseState 

적의 추적 상태를 담당합니다. 

NavmeshAgent를 활용해 플레이어 위치에 따라 주기적으로 목적지를 갱신합니다. 

플레이어를 시야에서 놓칠 경우, 마지막 확인 위치로 Agent 목적지를 설정합니다. 

플레이어가 공격 범위에 진입하면 AttackState로 전환합니다. 

추적 중 적의 스폰 위치와의 거리가 일정 거리(ChaseRange)를 초과하면 ReturnState로 전환합니다. 

일정 시간 동안 플레이어를 찾지 못할 경우에도 ReturnState로 전환합니다. 

 

 

## ✔ DeathState.cs 

적의 죽음 상태를 담당합니다. 

적의 hp가 0이 될 경우 DeathState로 상태가 전환됩니다. 

Enemy.cs의 EnemyDeath()를 호출합니다. 

 

## ✔ FleeState.cs 

적의 도망 상태를 담당합니다. 

적의 hp가 20% 일경우 FleeState로 상태가 전환됩니다. 

적이 Spawn point로 달려갑니다. 

 

## ✔ IdleState.cs 

적의 대기 상태를 담당합니다. 

플레이어를 감지할 경우 ChaseState로 전환됩니다. 

적의 HP가 Max HP가 아닐 때, HP가 Max HP까지 서서히 회복됩니다. 

일정 시간 이후 PatrolState로 전환됩니다. 

 

## ✔ PatrolState.cs 

적의 순찰 상태를 담당합니다. 

기존에 지정된 순찰 목적지를 PatrolState의 Enter() 호출마다 순차적으로 Agent의 목적지로 설정합니다. 

플레이어를 감지하면 ChaseState로 전환됩니다. 

목적지에 도착할 경우 IdleState로 전환됩니다. 

 

## ✔ ReturnState.cs 

적의 복귀 상태를 담당합니다. 

Agent의 목적지를 적의 Spawn Point로 지정합니다. 

Spawn point 도달 시 IdleState로 전환됩니다. 

플레이어 감지 시 ChaseState로 전환됩니다. 

ReturnState로 전환 후 일정 시간은 ChaseState으로의 전환을 막습니다. 
