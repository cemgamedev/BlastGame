# Blast Game

Blast Game is a mobile game project developed using the Unity game engine. The project follows modern game development practices and adheres to the **SOLID** principles to ensure a well-structured architecture.

## Technologies Used

### Main Technologies
- **Unity Game Engine**: The core game engine for the project
- **C#**: The programming language used
- **DOTween**: For animations and transitions
- **UniTask**: For asynchronous programming
- **Dependency Injection**: Utilized to eliminate dependencies using a container
- **UniRx**: For reactive programming
- **TextMesh Pro**: For advanced text rendering and UI
- **Odin Inspector**: For improved Unity Inspector functionality

### Architecture and Design Patterns
- **Dependency Injection**: For dependency management
- **SOLID Principles**: For organized and maintainable code
- **Scriptable Objects**: For data management
- **Interface-based Programming**: For modularity and testability

## Project Architecture

### Folder Structure
```
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
```
![image](https://github.com/user-attachments/assets/c7ac8662-68fc-4d4b-9708-204de58f8912)
![image](https://github.com/user-attachments/assets/74ec144f-9451-47fc-b935-e01d09113569)

## Getting Started

### Prerequisites
- Unity (Recommended version: **2022.x** or higher)
- .NET SDK (for C# development)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/cemgamedev/BlastGame.git
   ```
2. Open the project in Unity.
3. Install the required dependencies via Unity's Package Manager:
   - **DOTween**
   - **UniTask**
   - **UniRx**
   - **Odin Inspector**
4. Build and run the project to start playing.

## Contribution
Contributions are welcome! If you'd like to improve Stick Blast, please submit a pull request or open an issue.

## License
This project is licensed under the [MIT License](LICENSE).

