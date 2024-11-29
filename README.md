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




## 핵심 기능
이 게임의 핵심 기능은 인벤토리 시스템, 농작물 관리 시스템, 저장/로드 시스템입니다.

- 인벤토리 시스템
    - [Inventory](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Inventory.cs) : 게임 내 아이템을 저장하는 슬롯 시스템을 관리하며, 아이템의 추가, 삭제, 이동 등의 기능을 제공합니다.
    - [InventoryManager](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/InventoryManager.cs) : 게임 내 다양한 인벤토리(Backpack, Toolbar)를 관리합니다.
    - [InventoryBase](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/InventoryBase.cs) : 인벤토리 UI와 관련된 기본적인 기능을 담당합니다. (인벤토리 UI 갱신, 드래그 앤 드롭 기능)
    - [Inventory_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Inventory_UI.cs) : InventoryBase를 상속받아, 인벤토리(Backpack) UI를 담당합니다.
    - [Toolbar_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Toolbar_UI.cs) : InventoryBase를 상속받아, 인벤토리(Toolbar) UI를 담당합니다.
    - [Slot_UI](https://github.com/haaaabin/Valley/blob/main/Assets/Scripts/Inventory/Slot_UI.cs) : 인벤토리의 각 슬롯의 UI를 담당합니다.

