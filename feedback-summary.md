# Playtester Feedback Summary - Joburg Game Dev Meetup

## 1. Raw Playtester Feedback List
* "With doing tasks and stuff, it would be easier to understand with a UI bar that fills in while you're doing it."
* "The concept of figuring out the monster/entity by asking questions is cool, but maybe if there was a bit more guidance or story on the questions you can ask. Otherwise, I'm just asking 'what is an apple, bro?'"
* "The controls could be a bit more responsive."
* "I really like the charm, the animations, and the creative direction."
* "I want more interaction with the environment. Like, I want to burn if I stand in the fire."
* "When I pick up mushrooms, I don't know where they go. There should be a floating animation or something to show I've collected it."
* "The player guide being separate threw me off. I wish it was shown in-game instead of reading a separate thing and trying to remember what I read."
* "The last part threw me off because I just got killed out of nowhere. What did I do wrong? The death screen needs more clarity."
* "The juggling mechanics are fun, and the fact that you are talking directly to an AI is a really cool direction."

## 2. Feedback Categorisation
### LLM Integration & Narrative
* Request for explicit guardrails or guidance regarding NPC dialogue options (Playtester 1).
* Positive reception toward the core mechanical implementation of local LLM conversation (Playtester 3).

### Gameplay & Environment
* Desired addition of environmental hazards, such as active fire damage (Playtester 2).
* Missing player feedback loop for resource gathering/item inventory tracking (Playtester 2).

### UI & UX
* Need for active progress indicators (loading bars) for physical tasks (Playtester 1).
* Request for an integrated, contextual tutorial system to replace external reading material (Playtester 3).
* Lack of diagnostic/interpretive information on the Game Over screen explaining the failure state (Playtester 3).

### Technical Performance
* Input latency or responsiveness issues noted during standard player movement and interaction (Playtester 2).

## 3. Recurring Themes
* **Onboarding & Guidance Limitations:** Multiple testers experienced friction knowing exactly what to do or say next, highlighting a structural need for contextual tutorial elements.
* **Lack of Visual Feedback:** Testers felt a clear disconnect during active states, whether performing an action, picking up a mushroom, or dying, due to minimal UI feedback.

## 4. Initial Designer Reactions
* **Surprise:** It was surprising that the raw novelty of an LLM-driven NPC wasn't enough to carry the narrative framework completely; players felt lost without prompt examples.
* **Agreement:** The critique regarding the separate player guide and abrupt death loop is entirely valid. Our current state machine simply cuts the game thread without setting a data string for the UI text to read.
* **Concern:** Tuning movement responsiveness in Unity's Physics2D system might require altering fixed update cycles, which could destabilize asset tracking loops if not managed carefully.

