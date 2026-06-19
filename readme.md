# GADS Part 3: LLM-Integrated Survival Game (Refined Build)

## Overview
This project is a standalone survival game prototype developed for Game Design 3A at Emeris. It integrates a local Large Language Model (LLM) via Ollama to drive a core gameplay mechanic: interrogating daily visitors. This version has been fully refined using player experience feedback gathered at the Joburg Game Dev Meetup.

## Gameplay Loop & Structure
The game follows a repeating day-night cycle where the player's decisions during the day directly influence the danger and outcomes of the night. 

During the day phase, the player remains around a small forest cabin located near a hiking trail. Various NPCs arrive at the cabin, each presenting different situations. The player interacts with these NPCs through AI-generated dialogue, deciding whether to trust them or turn them away while completing simple maintenance tasks. 

Each NPC has a hidden nature (safe or dangerous), and the player must make decisions based on limited information. At night, the game shifts into a survival phase where events are triggered based on earlier choices. 

### Game Structure Loop
1. **Start of Day:** Safe environment
2. **Day Phase:** NPC interaction and decisions
3. **Transition:** Atmosphere shifts
4. **Night Phase:** Events triggered
5. **Survival Phase:** Secure cabin or experience failure states
6. **Outcome:** Success or reset

### Encounter Types (NPCs)
1. Injured hiker asking for help (Dangerous)
2. Lost tourist separated from group (Safe)
3. Quiet, suspicious traveler (Dangerous)
4. Panicked individual claiming danger (Safe)
5. Friendly wanderer requesting shelter (Dangerous)
6. Contradictory or suspicious lone individual

### Refined Night Survival & UI Feedback Mechanics
* **Responsive Controls & Movement Smoothing:** Character physics have been optimized using input polling and frame interpolation to eliminate movement lag reported by playtesters.
* **Task Progress Bar UI:** Physical activities now feature a visual progress indicator slider that fills up dynamically while interacting, letting players know an action is actively processing.
* **Diagnostic Game Over Screen:** If dangerous choices are made and a failure state triggers, the death screen pulls active variables from the event manager to explicitly inform players how they died rather than abruptly cutting the game thread.

---

## Installation Instructions (Final Build)
This project has been pre-built for easy playtesting without requiring the Unity Editor.

1. Download and install **Ollama** from [ollama.com](https://ollama.com).
2. Open your command prompt/terminal and run: `ollama run phi3`. Keep this terminal open in the background.
3. Download the provided `Final_Build` folder.

## How to Play
1. **Launch the Game:** Double-click the executable file inside the `Final_Build` folder. 
2. **Gameplay:** Complete your daily resource tasks using the Interaction Range helper (Hold E/F). Monitor your progress via the new interactive loading bars.
3. **Interrogation:** At **13:00 (1:00 PM)**, an NPC will spawn. Wait for them to reach the cabin door, then press **T** to initiate the interrogation.
4. **Decide:** Ask up to 3 questions, then press **Y** to Trust or **N** to Turn Away. Survive 3 days to win!

## Dependencies & Tools
* **Game Engine:** Unity
* **Local LLM Server:** Ollama
* **Model Name:** `phi3` (chosen for low memory footprint alongside Unity)
* **AI Coding Assistants:** Cursor IDE, Google Gemini (used for debugging syntax, resolving PromptManager compiler issues, and optimizing physics loops).

## Credits
* **Developers:** Xiluva Maluleke & Latita Mvunelo