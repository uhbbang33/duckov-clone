# UI Interactable 

상호작용 가능한 오브젝트의 UI를 관리하는 클래스를 모아둔 폴더입니다.

## ✔ InteractableStateUI 

상호작용 가능한 오브젝트의 UI 상태와 상호작용 기능을 관리하는 클래스입니다. 

모든 상호작용 UI 클래스의 부모 클래스입니다. 

###### 상호작용 UI 예시 사진

<img width="495" height="203" alt="image" src="https://github.com/user-attachments/assets/094677b0-ff97-47dc-a86c-483eb151350b" />


상호작용 UI의 선택 상태를 관리합니다. 

Selected(), Deselected(), OnInteract() 메서드를 virtual로 선언하여 상속 클래스에서 확장 가능하도록 설계했습니다. 

> Selected(): 플레이어와 가장 가까운 상호작용 오브젝트일 경우 PlayerInteract에 전달하여 플레이어 상호작용 대상으로 설정합니다. 
> 
> Deselected(): 선택이 해제되면 호출합니다. 
> 
> OnInteract(): 상호작용할 경우 호출됩니다. 

 

 

## ✔ InteractableBoxUI 

상호작용 가능한 상자의 UI를 관리하는 클래스입니다. 

InteractableStateUI를 상속받습니다. 

한 번이라도 상호작용했을 경우 상태 이미지가 달라지도록 구현했습니다. 

###### 상호작용 하지 않았을 경우

<img width="129" height="109" alt="image" src="https://github.com/user-attachments/assets/5b6476ab-b387-4e94-97e6-7ff5c5851617" />

###### 상호작용 했을 경우

<img width="134" height="99" alt="image" src="https://github.com/user-attachments/assets/539f5d4b-8824-4818-b951-f4ac50c7e23f" />


 

## ✔ InteractableDoorUI 

상호작용 가능한 문의 UI를 관리하는 클래스입니다. 

InteractableStateUI를 상속받습니다. 

OnInteract()를 오버라이드하였으며 상호작용할 경우 씬이 전환됩니다. 

 

 

## ✔ InventoryContainerUI

오브젝트와 상호작용할 경우, 인벤토리를 같이 열기 위해 사용하는 클래스입니다. 

InteractableStateUI를 상속받습니다. 

상호작용 시 인벤토리의 OnInventoryOpenWithInteractable() 메서드를 호출해 인벤토리를 엽니다. 


## ✔ DropItemUI 

InteractableStateUI를 상속받습니다. 

제일 가까운 상호작용 오브젝트일 경우 상태 이미지를 변경합니다. 

상호작용할 경우 연결된 DroppedItem의 OnInteract()메서드를 호출합니다. 
