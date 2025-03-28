1. Project Overview
Stick Blast is a mobile game project developed using the Unity game engine. The project follows modern game development practices and adheres to the SOLID principles to ensure a well-structured architecture.

2. Technologies Used
2.1 Main Technologies
Unity Game Engine: The core game engine for the project

C#: The programming language used

DOTween: For animations and transitions

UniTask: For asynchronous programming

Dependency Injection: Utilized to eliminate dependencies using a container

UniRx: For reactive programming

TextMesh Pro: For advanced text rendering and UI

Odin Inspector: For improved Unity Inspector functionality

2.2 Architecture and Design Patterns
Dependency Injection: For dependency management

SOLID Principles: For organized and maintainable code

Scriptable Objects: For data management

Interface-based Programming: For modularity and testability

3. Project Architecture
3.1 Folder Structure
Assets
├── Art/                  # Visual assets
├── Audio/                # Audio files
├── Prefabs/              # Unity prefabs
├── Resources/            # Resources to be loaded at runtime
├── Scenes/               # Game scenes
├── Scripts/              # C# script files
   ├── Core/             # Core systems
   ├── Grid/             # Game grid system
   ├── Effects/          # Visual effects
   ├── Implementation/   # Concrete implementations
   ├── Interfaces/       # Interfaces
   ├── Utility/           # Utility functions
   └── ScriptableObjects  # Scriptable objects
├── TextMesh Pro/         # TMPro assets
└── ThirdParty/           # Third-party libraries
