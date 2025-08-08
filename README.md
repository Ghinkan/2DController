# 2D Character Controller with State Machine

This project implements a **scalable 2D character controller** in Unity, powered by a **finite state machine (FSM)**.  
The main goal is to provide a **modular and easily extendable architecture** for various movement types and abilities, avoiding large monolithic scripts.

## ✨ Current Features

- **Decoupled FSM architecture** supporting:
  - States (`IState`)
  - Transitions (`Transition<T>`)
  - Predicates (`IPredicate`, `FuncPredicate`, `ActionPredicate`)
    
- **Two FSM implementations**:
  1. **Code-based FSM** – located in `Scripts/Controllers`  
     - Each movement type has its own state class inside the `States` folder.  
     - `PlayerControllerStates` defines the main FSM logic, transitions, and references to movement data. 
  2. **ScriptableObject-based FSM** – located in `Scripts/ScriptableStateMachine`  
     - States are implemented as ScriptableObjects inside the `States` folder.  
     - Predicates are also ScriptableObjects, allowing configurable transitions in the Unity Inspector.  
     - The main controller is `PlayerController`.

- **Fully functional 2D character movement**
  - Ground movement (`IdleState`, `RunState`)
  - Falling (`FallState`)
  - Wall sliding and wall jumping (`WallSlide`, `WallJumpState`)
  - Dashing from any state (`DashState`)
  - Jumping with *coyote time* and *input buffering*
  - Sensor system (`BoxSensorOverlap2D`) for ground and wall detection
  - Automatic character facing direction based on input
  - **Reusable timers** for action buffers and time windows

## 🚀 Roadmap

 Future development plan includes:
 **Node Graph Editor**  
   - Visual interface to create and connect states, similar to Animator Controller or Behaviour Trees.
   - Drag-and-drop nodes to define transitions and conditions.

These improvements will make the controller **fully configurable within Unity** without changing the source code, allowing faster design iteration.

## 💡 Motivation

The system is inspired by high-precision 2D platformers like *Celeste* and *Hollow Knight*, aiming for:
- **Scalability** – easily add new abilities without breaking the core.
- **Maintainability** – avoid “spaghetti code” in the main controller.
- **Flexibility** – reuse states and conditions across different characters.
