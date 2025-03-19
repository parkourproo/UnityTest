# UnityTest


# Game Update Log  

## 📂 Resources Management  
- Renamed the `Prefabs` folder to `PrefabsOld`.  
- Created a new `Prefabs` folder and copied all files from `PrefabsOld` to `Prefabs`.  
- Changed the sprites of `itemNormal` to fish accordingly.  

## 🛠 Buffer System & Item Selection  
- Created `BufferController` to control the tray containing selected items.  
- Implemented logic to manage item movement within the tray.  
- Removed the old game logic in `BoardController`.  
- Modified the item selection logic: when selected, the item moves to the tray.  

## 🎮 UI & Navigation  
- Added **buttons** to navigate between different game modes.  
- Added the **match-3 game win condition**.  
- Created `UIPanelGameWin` and implemented win logic.  

## 🤖 Auto Selection & Match-3 Logic  
- Added **functions in `Board.cs`** to support:  
  - Automatic item picking.  
  - Picking a set of three identical items.  
- Updated `BoardController` with:  
  - **Auto-lose function** (random selection).  
  - **Autoplay function** (automatically picks a set of three identical items).  
