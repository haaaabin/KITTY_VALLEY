# KITTY VALLEY
<a name="readme-top"></a>

<p>
  Unity 2D RPG 농장 생활 시뮬레이션 게임
</p>

![title](https://github.com/user-attachments/assets/c899b047-01b7-4f7f-b0ea-0be394a1c65f)
<br/>

<!-- TABLE OF CONTENTS -->

## 목차

1. [프로젝트 개요](#Intro)
2. [게임 기능](#Features)
3. [게임 플레이](#Play)
4. [핵심 기능](#CoreFeatures)
<br/>

<a name="Intro"></a>
## 프로젝트 개요
- 프로젝트 기간 : 2024.10 ~ 2024.11
- 개발 엔진 및 언어 : Unity & C#
- 플랫폼 : PC

<br/>

<a name="Features"></a>
## 게임 기능
1. 플레이어 캐릭터 조작
    - W / A / S / D로 플레이어를 이동하고, 마우스를 사용하여 상호작용합니다.
      
2. 농작물 관리 시스템
    - 땅을 갈고 씨앗을 심어 다양한 농작물을 재배합니다.
    - 일정 시간이 지나면 농작물은 성장하고, 수확 가능합니다.
      
3. 아이템 관리 및 인벤토리 시스템
    - 다양한 아이템을 수집하고 인벤토리에서 확인할 수 있습니다.
    - 툴바에서 아이템을 빠르게 선택하여 사용할 수 있습니다.
      
4. 상점과 아이템 판매
    - 판매 상자를 통해 수확한 농작물을 판매할 수 있습니다.
    - 상점에서 씨앗을 구매할 수 있습니다.
      
4. 저장 / 로드 시스템
    - 게임 진행 상황(날짜, 돈, 툴바/인벤토리의 저장된 아이템, 식물 성장 단계)을 저장하고, 다시 로드하여 이어서 플레이 가능합니다.

<br/>

<a name="Play"></a>
## 게임 플레이

![Tree-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/4eb5383d-9b33-4093-98c6-539f1de4a0bf)
![ItemBox-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/ea50d89d-af3f-4b14-a8f4-59d0cf22aab4)
![Inventory-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/eebd03e8-8ed2-4d68-b263-6b0919d09028)
![Drop-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/b3056266-edd1-45dc-b7d6-3e4219514c54)
![OnDayEnd-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/d8cd3d1c-a4d2-4ba5-85b6-cf9f5ed0a107)
![Shop-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/7ef89135-b101-45d4-bdd9-6c82a6117b93)
![Seed-ezgif com-video-to-gif-converter (1)](https://github.com/user-attachments/assets/daceb67b-afc3-4b5d-9099-e7f3cc0c56c2)
![Harvest-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/c7731736-6ee0-42b2-90a8-224095f02e6f)
![Save-ezgif com-video-to-gif-converter (1)](https://github.com/user-attachments/assets/5ab81b3a-c02f-4457-bfc0-52af5845fd55)
<br/>

<a name="CoreFeatures"></a>
## 핵심 기능
이 게임의 핵심 기능은 인벤토리 시스템, 농작물 관리 시스템, 저장/로드 시스템입니다.

- 인벤토리 시스템
    - 아이템 데이터는 아이템 이름, 아이콘, 가격, 최대 개수, 판매 가능 여부 정보를 관리하는 ScriptableObject로 구성했습니다.
    - [Inventory](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Inventory.cs) : 게임 내 아이템을 저장하는 슬롯 시스템을 관리하며, 아이템의 추가, 삭제, 이동 등의 기능을 제공합니다.
    - [InventoryManager](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/InventoryManager.cs) : 게임 내 다양한 인벤토리(Backpack, Toolbar)를 관리합니다.
    - [InventoryBase](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/InventoryBase.cs) : 인벤토리 UI와 관련된 기본적인 기능을 담당합니다. (인벤토리 UI 갱신, 드래그 앤 드롭 기능)
    - [Inventory_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Inventory_UI.cs) : InventoryBase를 상속받아, 인벤토리(Backpack) UI를 담당합니다.
    - [Toolbar_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Toolbar_UI.cs) : InventoryBase를 상속받아, 인벤토리(Toolbar) UI를 담당합니다.
    - [Slot_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Slot_UI.cs) : 인벤토리의 각 슬롯의 UI를 담당합니다.

<br/>
 
- 농작물 관리 시스템 [코드](https://github.com/haaaabin/KITTY_VALLEY/blob/main/Assets/Scripts/Plant/PlantGrowthManager.cs)
    - 식물 데이터는 식물의 이름, 프리팹, 성장 단계별 타일 리스트, 성장 시간 리스트 정보를 관리하는 ScriptableObject로 구성했습니다.
    - 타일맵과 상호작용하는 기능이 핵심으로, 타일의 상태를 실시간으로 관리하고 옵저버 패턴으로 타일의 상태를 변경합니다.
    - 현재 심어진 식물의 상태를 PlantSaveData 리스트에 저장하고, 게임 로드 시 해당 데이터를 복원합니다.

<br/>
 
-  저장/로드 시스템 [코드](https://github.com/haaaabin/KITTY_VALLEY/blob/main/Assets/Scripts/SaveData.cs)
    - 인벤토리, 식물, 플레이어의 데이터(돈, 날짜)를 JSON 파일로 직렬화와 역직렬화를 통해 저장하고 로드합니다.
