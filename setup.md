# Technical Setup Guide: Local LLM Integration

## 1. System Requirements & Setup
To run this project, you must have Ollama installed locally.
1. Download and install Ollama from [ollama.com](https://ollama.com).
2. Open your command prompt or terminal.
3. Run the following command to download the required lightweight model:
   `ollama run phi3`
   *(Note: We chose phi3 to minimize computational load and ensure Unity runs smoothly alongside local inference without crashing).*

## 2. Unity Configuration
- Open the Unity project.
- Ensure the `GameManager` GameObject is configured with the `OllamaManager.cs` script attached.
- The **Model Name** field in the Inspector MUST be set to exactly: `phi3`.
- The default local API endpoint is configured to `http://localhost:11434/api/generate`.

## 3. How to Play
- Press **Play** in the Unity Editor.
- Complete daily resource tasks (logs, mushrooms, water, fence repair).
- At **13:00 (1:00 PM)** in-game time every day, an NPC will spawn in the forest and walk to the cabin door.
- Walk up to the NPC and press **T** to initiate the interrogation. In-game time will pause.
- Type questions in the UI to figure out their intent. You are limited to 3 questions.
- Press **X** at any time to walk away, resume time, and finish your tasks. You can return to the NPC and press **T** to resume the conversation without losing memory.
- Press **Y** to Trust the NPC, or **N** to Turn them Away. 
- **Win Condition:** Survive 3 days. Trusting a dangerous NPC results in instant death.