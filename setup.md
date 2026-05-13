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
- Ensure the `OllamaController` GameObject in the scene has the `OllamaIntegration.cs` script attached.
- The **Model Name** field in the Inspector MUST be set to exactly: `phi3`.
- The default local API endpoint is configured to `http://localhost:11434/api/generate`.

## 3. How to Play/Test
- Press **Play** in the Unity Editor.
- When an NPC approaches the cabin, use the on-screen input field to type questions.
- Press **Enter** to ask the question. Wait for the LLM to process and respond.
- You have a strict limit of 3 questions before night falls.
- Press **Y** to Trust the NPC, or **N** to Turn them Away.