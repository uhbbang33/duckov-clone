# Stat - Player 

플레이어가 사용하는 스탯 관련 클래스를 모아둔 폴더입니다. 

#### [TimeDecreasingStat](https://github.com/uhbbang33/duckov-clone/tree/main/Assets/01%20Scripts/Stat/Player/TimeDecreasingStat)

- 시간에 따라 줄어드는 스탯을 관리하는 클래스를 모아둔 폴더입니다.

## ✔ PlayerHealthPoint 

플레이어의 체력 포인트(HP)를 관리하는 클래스입니다. 

HealthPoint를 상속받습니다. 

 

### [주요 기능]

> 플레이어 데이터를 불러와 HP 정보를 초기화 
> 
> 점점 줄어드는 HP를 코루틴을 사용해 구현 
>
> HP 상태에 따른 Slider UI 갱신 


## ✔ StaminaPoint 

플레이어의 스태미나 포인트(SP)를 관리하는 클래스입니다. 

 

### [주요 기능]

> 플레이어 데이터를 불러와 SP 정보 초기화 
>
> 달리기, 구르기 등에 따른 SP 소모 관리 
>
> 일정 시간 후 SP 자동 회복 및 수분 상태에 따른 SP 회복량 조절 
>
> SP 상태에 따른 Slider UI 갱신 
