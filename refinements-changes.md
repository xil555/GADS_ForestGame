# Scope Changes, Refinements, & AI-Assisted Decisions Log

## Core Structural Evolutions
* **Shift from Static to Dynamic Dialogue:** Initially planned to use randomized dialogue arrays. Upgraded to a dynamic API bridge using `UnityWebRequest` to parse JSON from Ollama in real-time to meet POE functional integration requirements.
* **Model Optimization:** Started with `llama3` but encountered severe memory overhead when running alongside the Unity Editor. Switched the system to `phi3` for faster inference, lower latency, and better hardware compatibility.
* **Tension Mechanics (The 3-Question Limit):** Added an interrogation mechanic where the player can only ask 3 questions before the UI locks out. This forces a high-stakes decision before nightfall and prevents the player from endlessly pinging the API.
* **Interaction System Overhaul:** Replaced standard Unity `OnTriggerEnter` calls with a custom `InteractionRangeHelper` to ensure reliable proximity detection with the `CharacterController`. 
* **Strict Waypoint Execution:** Modified the `NPC.cs` script so players cannot intercept NPCs in the forest. The NPC must fully reach their cabin door waypoint before the global prompt allows interaction, building tension and preserving immersion.
* **Memory Persistence:** Separated the `conversationHistory` string from the UI text. This allows the player to walk away from the conversation and return later, with the LLM remembering the exact context of the interrogation.
* **State Management & Scene Loading:** Removed earlier pause menu concepts to preserve the integrity of the day/night cycle. Upgraded the Win/Loss states to evaluate the NPC's `isDangerous` boolean, which now dynamically transitions to dedicated `Win` or `Lose` scenes via the `SceneManager`.

## Community Feedback Refinements (Meetup-Driven Changes)
Following playtesting feedback from developers at the Joburg Game Dev Meetup, our group implemented the following crucial changes for clear design and UX reasons:

* **Movement Feedback and Smoothing Loop:** Refactored the character controller to poll inputs within `Update()` and execute movement changes inside `FixedUpdate()`. We explicitly enabled Rigidbody2D interpolation to smooth out input latency and character responsiveness.
* **Visual Task Indicators (Loading Bars):** Implemented a dynamic UI `Slider` tracked directly by our physical interaction script. This provides an active visual progress bar when players harvest resources or perform cabin tasks, resolving player disconnect.
* **Diagnostic Defeat System:** Overhauled the failure state loading sequence. Instead of abruptly transitioning scenes, the `ShowGameOver` routine now queries our state manager to print a specific cause-of-death string to the canvas, giving players essential contextual diagnostic clarity on how they failed.
* **Codebase Compilation Maintenance:** Cleaned up legacy UI code within `CabinDoorInteract.cs`, removing outdated property definitions (such as `PromptManager.PriorityDoor`) to restore completely stable compilation parameters across our active namespace.

## Scrapped Features & Technical Constraints
* **Environmental Damage Sub-routines:** Playtesters requested active environmental tiles (such as taking fire damage). Our group officially chose to scrap this feature due to strict time constraints. Re-engineering the core player controller code to integrate health pooling routines fell completely outside our execution loop for this milestone.
* **Integrated In-Game Tutorial Database:** Ideas for moving the player guidebook directly into the interactive runtime environment were rejected. Restructuring the canvas assets and menu layers within our remaining schedule presented a high risk of feature creep, so it was cut to protect project scope.