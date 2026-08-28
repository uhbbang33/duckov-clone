# UI Controller

## ✔ CrosshairController

크로스헤어 UI와 사격에 따른 반동, 흔들림을 관리하는 클래스입니다. 

크로스헤어는 마우스를 따라가며, 인벤토리를 열거나 상호작용을 할 경우엔 비활성화됩니다. 

사격 시 십자선들의 거리를 넓히고, 크로스헤어가 좌우로 흔들립니다. 

조준 시 십자선들과 조준점의 거리를 좁히고, 조준하지 않을 경우 십자선들이 원래 위치로 돌아옵니다. 

###### 기본 크로스헤어

<img width="123" height="105" alt="image" src="https://github.com/user-attachments/assets/d46a8fb8-bfbd-47a1-ade7-97c8d3356444" />

###### 사격시

<img width="178" height="143" alt="image" src="https://github.com/user-attachments/assets/c1082aeb-79ad-47f2-9c86-0e52b504ace1" />

###### 조준
<img width="86" height="82" alt="image" src="https://github.com/user-attachments/assets/49846cd2-f719-4bdb-a96f-8a1a9f076804" />

 

### [반동 구현] 

> 사격 시 화면 중앙에서 마우스 위치까지의 방향을 기준으로 반동 방향을 계산합니다. 
> 
> Lerp를 통해 크로스헤어 위치에 부드럽게 반동을 적용합니다. 
> 
> 이후 크로스헤어를 원래 위치로 되돌립니다. 


## ✔ FadeController 

지정한 CanvasGroup을 Fade In, Fade Out 하기 위해 사용하는 클래스입니다. 

GameOverUI와 LoadingUI에서 상속받아 사용합니다. 

Mathf.Lerp으로 alpha값을 조절해 구현했습니다. 
 

## ✔ MapUIController

맵을 열고 닫는데에 사용되는 클래스입니다. 

Input System을 사용해 맵을 열고 닫을 수 있도록 했습니다. 

맵 위에 카메라를 추가해 플레이어 아이콘 등의 UI, 지형과 건물만 보이도록 설정했습니다. 

맵을 열 때마다 플레이어 아이콘을 플레이어 위치로 업데이트합니다. 

###### Map
<img width="501" height="488" alt="image" src="https://github.com/user-attachments/assets/85721c53-1873-4bce-b46d-391dff19edc4" />

