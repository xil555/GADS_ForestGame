# Ollama Integration Plan

* **Model Choice:** `phi3` - Chosen for its high performance/low memory footprint ratio on consumer hardware, ensuring the game frame rate remains stable during runtime inference.
* **Inference Timing:** Runtime. The game actively pauses the `TimeController` clock while waiting for the HTTP POST request to return the JSON response from `http://localhost:11434/api/generate`.
* **Data Flow:**
  1. Internal clock hits 13:00; `GameManager` instantiates daily NPC prefab.
  2. NPC walks to door; player proximity triggers interaction prompt.
  3. Unity compiles specific NPC `conversationHistory` string.
  4. JSON payload sent via `UnityWebRequest`.
  5. Ollama processes locally.
  6. JSON response received, appended to background memory string, and displayed purely as recent dialogue in TextMeshPro UI.
* **Risks & Mitigations:** - *Risk:* API timeout from long prompts. *Mitigation:* Character limits on input fields.
  - *Risk:* Hallucinations breaking character. *Mitigation:* System prompts strictly enforce "Reply in one short sentence" to keep the LLM focused.
  - *Risk:* UI text clutter. *Mitigation:* Separated the payload memory from the UI display, showing only the most recent response.