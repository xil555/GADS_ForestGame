# Ollama Integration Plan

* **Model Choice:** `phi3` - Chosen for its high performance/low memory footprint ratio on consumer hardware, ensuring the game frame rate remains stable during runtime inference.
* **Inference Timing:** Runtime. The game actively pauses the day/night cycle while waiting for the HTTP POST request to return the JSON response from `http://localhost:11434/api/generate`.
* **Data Flow:**
  1. Player triggers Day Phase event.
  2. Unity compiles `conversationHistory` string + `NPC.systemPrompt`.
  3. JSON payload created and sent via `UnityWebRequest`.
  4. Ollama processes locally.
  5. JSON response received, parsed, and displayed in TextMeshPro UI.
* **Risks:** - *Risk:* Player types extremely long prompts causing API timeout. *Mitigation:* Implement character limit on input field.
  - *Risk:* Hallucinations breaking character. *Mitigation:* System prompts strictly enforce "Reply in one short sentence" to keep the LLM focused and prevent over-sharing.