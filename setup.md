# Technical Setup Guide: Local LLM Integration

## 1. System Requirements & Setup
To run this project, you must have Ollama installed locally.
1. Download and install Ollama from [ollama.com](https://ollama.com).
2. Open your command prompt or terminal.
3. Run the following command to download the required lightweight model:
   `ollama run phi3`

## 2. Unity Configuration
- Open the Unity project.
- Ensure the `GameManager` GameObject is configured with the `OllamaManager.cs` script attached.
- The **Model Name** field in the Inspector MUST be set to exactly: `phi3`.
- The default local API endpoint is configured to `http://localhost:11434/api/generate`.

## 3. Gameplay Flow & Testing
1. **Daily Tasks:** Complete daily resource tasks (logs, mushrooms, water, fence repair) using the Interaction Range helper (Hold E/F).
2. **The Arrival:** At **13:00 (1:00 PM)** in-game time, an NPC will spawn in the forest. You *cannot* interact with them while they are walking. 
3. **The Interrogation:** Once the NPC reaches the cabin door, a prompt will appear on your screen. Press **T** to initiate the interrogation. In-game time will pause.
4. **Investigation:** Type questions in the UI to figure out their intent. You are strictly limited to **3 questions**.
5. **Walking Away:** Press **X** at any time to close the UI, unfreeze time, and finish your tasks. You can return and press **T** to resume without losing the conversation's memory.
6. **The Decision:** Press **Y** to Trust the NPC, or **N** to Turn them Away. 
7. **Win/Loss Conditions:** If you trust a dangerous NPC, the game will automatically load the `Lose` scene. Survive 3 days of interrogations to trigger the `Win` scene.