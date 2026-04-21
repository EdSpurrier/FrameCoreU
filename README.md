# FrameCore-U

### Lightweight Runtime Framework for Unity

---

## Overview

**FrameCore-U** is a lightweight, reusable runtime framework designed to provide the foundational systems required for Unity applications and games.

It is not a game framework or gameplay system.

It is the **core infrastructure layer** that handles how systems run, communicate, and execute — allowing higher-level systems to be built cleanly on top.

---

## Purpose

FrameCore-U exists to:

* Provide stable, reusable core systems across projects
* Reduce repeated boilerplate setup
* Standardize common patterns (events, pooling, timing, etc.)
* Keep low-level systems separate from gameplay logic

---

## Architecture Position

FrameCore-U is the **lowest layer** in the overall architecture:

```
Game / App Layer
       ↑
     Foundry
       ↑
   FrameCore-U
```

* **FrameCore-U** → runtime infrastructure
* **Foundry** → reusable gameplay/building systems
* **Game/App** → project-specific logic

---

## Core Philosophy

FrameCore-U follows a strict design philosophy:

### 1. System-Level Only

FrameCore-U should only contain systems that are useful across **all projects**.

### 2. No Game Logic

No gameplay, domain logic, or project-specific behaviour should exist here.

### 3. Stability Over Expansion

Once stable, systems should rarely change.
Avoid adding features “just in case.”

### 4. Clarity Over Abstraction

Prefer simple, clear systems over overly abstract or flexible ones.

---

## Core Systems

### FrameCore

The central entry point that connects and validates all core systems.

* Ensures only one instance exists
* Connects subsystems automatically
* Provides runtime validation

---

### Frame (Static Access)

Global access layer for core systems.

```csharp
Frame.Events
Frame.Pools
Frame.Sound
Frame.Scenes
Frame.Debouncer
```

This allows systems to be accessed without tight coupling.

---

### Event System

A flexible, data-driven event pipeline:

```
EventManager → FrameCoreEvent → FrameAction(s)
```

#### EventManager

* Triggers sequences of events
* Supports queuing, looping, and single-use behaviour

#### FrameCoreEvent

* Represents a single event
* Supports delay/queueing
* Executes:

    * UnityEvents
    * FrameActions

#### FrameAction

* Abstract base for actions
* Implemented externally (typically in Foundry)

---

### EventCore

Handles timed and queued event execution.

* Processes delayed events
* Maintains runtime event timing

---

### PoolCore

Object pooling system for efficient spawning.

* Preloads objects
* Reuses instances instead of instantiating
* Supports lazy loading / pool boosting

---

### SoundCore

Centralized audio system.

* Plays sounds via SoundBanks
* Handles priorities and playback logic

---

### SceneCore

Manages scene transitions and loading.

---

### Timing / Debouncer

Utility systems for:

* Rate limiting
* Delayed execution
* Controlled triggering

---

## What Belongs in FrameCore-U

Only include systems that are:

* Required in nearly every project
* Infrastructure-level
* Independent of gameplay meaning

### Examples

✔ Event framework
✔ Pooling
✔ Audio core
✔ Scene management
✔ Timing utilities
✔ Shared low-level utilities

---

## What Does NOT Belong in FrameCore-U

Avoid placing the following here:

✘ Gameplay systems

✘ Triggers / interactions

✘ AI logic

✘ Inventory systems

✘ Combat systems

✘ UI flows

✘ Project-specific logic


These belong in higher layers (Foundry or Game/App).

---

## Dependency Rules

Dependencies must flow **one way only**:

```
Game/App → Foundry → FrameCore-U
```

### Rules

* FrameCore-U must NOT depend on Foundry
* FrameCore-U must NOT depend on Game/App code
* Foundry can depend on FrameCore-U
* Game/App can depend on both

---

## Usage Example

Triggering an event:

```csharp
Frame.Events.ActivateTimedEvent(myEvent);
```

Spawning an object:

```csharp
Frame.Pools.SpawnObject(prefab, position, rotation);
```

---

## Project Structure (Suggested)

```
FrameCoreU/
├── Core/
├── Events/
├── Pooling/
├── Audio/
├── Scene/
├── Timing/
├── Unity/
├── Utils/
└── Data/
```

---

## Development Guidelines

### Keep It Small

If a system is not clearly reusable across projects, it does not belong here.

### Avoid Premature Abstraction

Do not design for hypothetical future needs.

### Promote Later

If something proves reusable across multiple projects, move it into FrameCore-U later.

### Prefer Stability

Once a system works reliably, avoid refactoring it unnecessarily.

---

## Definition of “Done”

FrameCore-U is considered “done enough” when:

* Core systems are stable and predictable
* It supports at least one real project without friction
* New features are no longer being added regularly
* Changes are mostly bug fixes or minor improvements

---

## Summary

* **FrameCore-U = How things run**
* Provides stable, reusable runtime systems
* Should remain small, focused, and reliable

---
