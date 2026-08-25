# Stencil 

Stencil Shader가 적용된 오브젝트가 플레이어 또는 마우스 주변을 가리지 않도록 

Mask 영역을 생성하고 위치를 제어하는 클래스의 모음입니다. 

 

## ✔ StencilMask 

Stencil Mask에 공통으로 사용되는 Mask 크기를 설정하는 부모 클래스입니다. 

StencilPlayerMask, StencilMouseMask에서 상속해 사용합니다. 

 

## ✔ StencilPlayerMask 

플레이어 주변의 Stencil Mask 위치를 관리해 해당 영역의 Stencil Shader가 적용된 오브젝트가 플레이어를 가리지 않도록 하는 클래스입니다. 

 

## ✔ StencilMouseMask 

마우스 주변의 Stencil Mask 위치를 관리해 해당 영역의 Stencil Shader가 적용된 오브젝트가 보이지 않도록하는 클래스입니다. 

InputSystem을 통해 마우스 위치를 받고, Mask 위치를 업데이트합니다. 
