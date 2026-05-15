# GADS Part 2: LLM-Integrated Survival Game

## Overview
This project is a standalone survival game prototype developed for Game Design 3A at Emeris. It integrates a local Large Language Model (LLM) via Ollama to drive a core gameplay mechanic: interrogating daily visitors. 

## Gameplay Loop & Structure
The game follows a repeating day-night cycle where the player's decisions during the day directly influence the danger and outcomes of the night. 

During the day phase, the player remains around a small forest cabin located near a hiking trail. Various NPCs arrive at the cabin, each presenting different situations. The player interacts with these NPCs through AI-generated dialogue, deciding whether to trust them or turn them away while completing simple maintenance tasks. 

Each NPC has a hidden nature (safe or dangerous), and the player must make decisions based on limited information. At night, the game shifts into a survival phase where events are triggered based on earlier choices. The player may need to investigate disturbances, escape danger, and secure the cabin under time pressure.

### Game Structure Loop
1. **Start of Day:** Safe environment
2. **Day Phase:** NPC interaction and decisions
3. **Transition:** Atmosphere shifts
4. **Night Phase:** Events triggered
5. **Survival Phase:** Secure cabin
6. **Outcome:** Success or reset

### Encounter Types (NPCs)
1. Injured hiker asking for help
2. Lost tourist separated from group
3. Quiet, suspicious traveler
4. Panicked individual claiming danger
5. Friendly couple requesting shelter
6. Contradictory or suspicious lone individual

### Night Survival Mechanics & Triggers
If dangerous choices were made, night events may include knocking or banging at the gate, voices calling for help, strange forest noises, or shadowy figures near the cabin. Survival mechanics involve returning to the cabin quickly, locking all doors, closing and securing windows, and surviving until morning under time pressure.

---

## Installation Instructions (Final Build)
This project has been pre-built for easy playtesting without requiring the Unity Editor.

1. Download and install **Ollama** from [ollama.com](https://ollama.com).
2. Open your command prompt/terminal and run: `ollama run phi3`. Keep this terminal open in the background.
3. Download the provided `Final_Build` folder.

## How to Play
1. **Launch the Game:** Double-click the executable file inside the `Final_Build` folder. 
2. **Gameplay:** Complete your daily resource tasks using the Interaction Range helper (Hold E/F).
3. **Interrogation:** At **13:00 (1:00 PM)**, an NPC will spawn. Wait for them to reach the cabin door, then press **T** to initiate the interrogation.
4. **Decide:** Ask up to 3 questions, then press **Y** to Trust or **N** to Turn Away. Survive 3 days to win!

## Dependencies & Tools
* **Game Engine:** Unity
* **Local LLM Server:** Ollama
* **LLM Model:** `phi3` (chosen for low memory footprint alongside Unity)
* **AI Coding Assistants:** Cursor IDE, Google Gemini (used for debugging syntax, refactoring C# interactions, and optimizing API calls).

## Credits
* **Developers:** Xiluva Maluleke & Latita Mvunelo
