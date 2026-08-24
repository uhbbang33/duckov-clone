# Map

## ✔ BuildingMouseDetector 

플레이어가 마우스로 건물 안쪽을 볼 수 있도록 마우스 위치에 있는 건물을 감지하고, 건물의 지붕 Renderer를 제어하는 클래스입니다. 

Field Scene의 main Camera에 컴포넌트로 등록하여 사용합니다.

Camera.main에서 마우스 위치를 기준으로 Ray 생성, Physics.Raycast()를 사용해 건물을 감지합니다. 

RendererHider.MouseHover() 를 호출해 건물의 지붕 Renderer를 비활성화합니다. 


## ✔ RendererHider 

건물 오브젝트에 컴포넌트로 등록해 플레이어의 건물 내부 진입 및 마우스 Hover 상태에 따라서 지정된 Renderer의 활성화 상태를 제어하는 클래스입니다. 

플레이어가 건물 내부에 있거나 마우스를 건물에 Hover한 경우 지정된 Renderer를 비활성화합니다. 

OnTriggerEnter와 OnTriggerExit를 통해 플레이어가 건물 내부로 진입했는지 판단합니다. 


## ✔ DayNightCycle 

시간을 관리하고, 시간에 따라 Field Scene의 태양의 방향과 색상을 변경하는 클래스입니다. 

하루를 24시간으로 설정했으며, Gradient를 사용해 시간대별 태양의 색상을 설정했습니다. 

현재 시간을 TimeSaveData로 변환해 저장하고 로드합니다. 
