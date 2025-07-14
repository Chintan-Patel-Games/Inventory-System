# 🧿 Inventory & Shop System in Unity

## 🎮 Overview
This is a Unity-based 2D project that features a fully functional **Inventory and Shop System**. The system supports:
- Item gathering with increasing rarity
- Buying and selling via drag-and-drop with confirmation popups
- Weight management and gold currency
- UI-based interactivity with clean transitions and feedback
- Modular and extensible architecture

---

## 🧰 Features
- Inventory panel with slot-based item handling
- Shop panel for transactions
- Confirmation & quantity selection UI
- Tooltip system
- Gold and weight system
- Raycast blocker for modal behavior
- Toast and popup messages
- Sound and BGM system

---

## 🧱 Architecture Overview
![Architecture Diagram](https://github.com/Chintan-Patel-Games/Inventory-System/blob/Project-Setup/Assets/Sprites/Inventory%20System%20Project%20Architecture.jpg)

---

## 🧩 Design Patterns & Principles Used
| Pattern/Principle      | Usage Location                        |
|------------------------|----------------------------------------|
| **Singleton**          | `SoundManager` for global sound access |
| **Observer**           | UI event subscriptions (e.g. `OnHide`, `OnConfirmPopup`) |
| **ScriptableObject**   | `ItemData` for flexible item definition |
| **Separation of Concerns** | `UIManager`, `ShopController`, `InventoryManager` handle distinct responsibilities |
| **Event-Driven Design**| Popup confirmations and transaction callbacks |
| **Dependency Injection (Manual)** | UI references injected via Inspector instead of hard references |
| **Open/Closed Principle** | Systems like `ItemData`, `TooltipUI` are extensible |
| **DRY Principle**      | Shared methods for UI blocking, messaging, popup control |

---

## 🎥 Gameplay Video
[![Watch the video]

---

## 🧪 How to Run
1. Clone this repo.
2. Open in Unity 2022.3 or later.
3. Open `Scenes/Main.unity`.
4. Press Play and interact with the UI.
