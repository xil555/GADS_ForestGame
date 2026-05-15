# GADS Part 2: LLM-Integrated Survival Game

## Overview
This project is a standalone survival game prototype developed for Game Design 3A at Emeris. It integrates a local Large Language Model (LLM) via Ollama to drive a core gameplay mechanic: interrogating daily visitors. The player must manage daily survival tasks (collecting water, chopping trees, repairing fences) while evaluating the hidden intentions of procedurally generated NPCs.

## Installation Instructions
1. Download and install **Ollama** from [ollama.com](https://ollama.com).
2. Open your command prompt/terminal and run: `ollama run phi3`. Keep this terminal open in the background.
3. Clone this repository and open the project in **Unity**.
4. In Unity, go to **File > Build Settings** and ensure the following scenes are in your Build list:
   - `MainMenu`
   - `Game Scene`
   - `Lose`
   - `Win`
5. Open `MainMenu` and press **Play**!

## Dependencies & Tools
* **Game Engine:** Unity
* **Local LLM Server:** Ollama
* **LLM Model:** `phi3` (chosen for low memory footprint alongside Unity)
* **Assets:** "City People Free Samples" (3D character models and animations)
* **AI Coding Assistants:** Cursor IDE, Google Gemini (used for debugging syntax, refactoring C# interactions, and optimizing API calls).

## Credits
* **Developer:** Latita Mvunelo
* **Co-Developer:** [Partner's Name - Add their name here] (Developed the Daily Objective Manager, Time Controller, and Interaction Systems).