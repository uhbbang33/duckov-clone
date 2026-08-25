# TimeDecreasingStat

## ✔ TimeDecreasingStat

시간에 따라 줄어드는 스탯을 관리하는 클래스입니다. 

playerMove의 OnRun과 OnRunCancel 이벤트를 구독해 플레이어가 달릴 때 감소값이 2배 적용됩니다. 

스탯값이 0일 때와 0에서 회복됐을 때 각각 OnEnterZeroStat(), OnExitZeroStat()을 실행할 수 있도록 제공합니다. 


## ✔ Hunger 

플레이어의 배고픔을 관리하는 클래스입니다. 

TimeDecreasingStat를 상속받습니다. 

OnEnterZeroHunger와 OnExitZeroHunger 이벤트를 통해 배고픔이 0이 되었을 때 플레이어 HP가 지속적으로 감소하고, 배고픔이 회복되면 HP 감소를 중지합니다. 


## ✔ Hydration 

플레이어의 수분을 관리하는 클래스입니다. 

TimeDecreasingStat를 상속받습니다.

OnEnterZeroHydration과 OnExitZeroHydration 이벤트를 통해 수분이 0이 되었을 때 플레이어의 이동속도를 절반으로 감소시키고, 수분이 회복되면 원래 속도로 복구합니다. 
