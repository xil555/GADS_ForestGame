# Ollama Integration Plan

* **Model Choice:** `phi3` - Chosen for its high performance/low memory footprint ratio on consumer hardware, ensuring the game frame rate remains stable during runtime inference.
* **Inference Timing:** Runtime. The game actively pauses the `TimeController` clock while waiting for the HTTP POST request to return the JSON response from `http://localhost:11434/api/generate`.
* **Data Flow:**
  1. Internal clock hits 13:00; `GameManager` instantiates the daily NPC prefab.
  2. NPC reaches cabin waypoint; global interaction prompt is triggered.
  3. Unity compiles the specific NPC's `conversationHistory` string.
  4. JSON payload sent via `UnityWebRequest`.
  5. Ollama processes the request locally.
  6. JSON response received, appended to the background memory string, and displayed purely as recent dialogue in the TextMeshPro UI.
* **Risks & Mitigations:** - *Risk:* API timeout from long prompts. *Mitigation:* Character limits on input fields.
  - *Risk:* Hallucinations breaking character. *Mitigation:* System prompts strictly enforce "Reply in one short sentence" to keep the LLM focused on immediate physical needs rather than grand backstories.
  - *Risk:* UI text clutter. *Mitigation:* Separated the payload memory from the UI display, showing only the most recent response to the player.