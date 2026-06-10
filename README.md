<h1 align="center">
  🎮 Pixel Odyssey
</h1>

<p align="center">
  <strong>Un juego de plataformas 2D estilo pixel art desarrollado en Unity 6</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.0.42f1-black?style=for-the-badge&logo=unity&logoColor=white" alt="Unity Version"/>
  <img src="https://img.shields.io/badge/C%23-Scripting-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/Platform-Windows%20%7C%20WebGL-blue?style=for-the-badge&logo=windows&logoColor=white" alt="Platform"/>
  <img src="https://img.shields.io/badge/Render%20Pipeline-URP%2017.0.4-purple?style=for-the-badge" alt="URP"/>
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture-green?style=for-the-badge" alt="Architecture"/>
</p>

---

## 📖 Descripción

**Pixel Odyssey** es un juego de plataformas 2D con estética pixel art desarrollado en **Unity 6**, en el que el jugador atraviesa escenarios desafiantes llenos de enemigos, trampas y jefes finales. El proyecto destaca por su arquitectura limpia (*Clean Architecture*), el uso de patrones de diseño avanzados y un sistema robusto de persistencia de datos por perfiles.

El juego fue diseñado como proyecto de portafolio para demostrar habilidades en **programación de videojuegos**, **arquitectura de software** y **diseño de sistemas de juego**.

---

## ✨ Características Principales

| Característica | Descripción |
|---|---|
| 🏃 **Sistema de Movimiento Avanzado** | Movimiento fluido con soporte para salto, dash y animaciones sincronizadas usando el patrón **State Machine** |
| ❤️ **Sistema de Salud con Escudo** | El jugador cuenta con puntos de vida y escudo que absorbe el daño primero, con eventos C# para actualización reactiva de la UI |
| 🧠 **IA de Enemigos** | Comportamiento modular con tres estados: **Patrullar**, **Perseguir** y **Atacar**, implementados como comportamientos intercambiables |
| 👾 **Variedad de Enemigos** | 5 tipos de enemigos únicos: **Goblin**, **Mushroom**, **Ogre**, **Skeleton** y **Demon Fly**, cada uno con mecánicas propias |
| 👹 **Jefe Final (Boss)** | El Slime Demonio cuenta con una **secuencia de transformación**, múltiples patrones de ataque (melee, lanzallamas, salto) y una fase de combate épica |
| 💾 **Persistencia por Perfiles** | Sistema de guardado que soporta **múltiples perfiles** de jugador, almacenando posición, escena actual, salud y escudo |
| 🎬 **Escenas y Flujo de Juego** | Pantalla de inicio, intro animada, tutorial, 2 niveles completos, escena final y créditos con transiciones suaves |
| 🎵 **Gestión de Audio** | AudioManager centralizado con soporte para música de fondo y efectos de sonido posicionales |
| ⏸️ **Menú de Pausa** | Pausa completa del juego con opciones de continuar, guardar y regresar al menú principal |

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue los principios de **Clean Architecture**, separando el código en tres capas bien definidas para maximizar la mantenibilidad y la extensibilidad:

```
Assets/src/Scripts/
│
├── 📁 Player/
│   ├── domain/          → Entidades del jugador (Player, HealthPlayer, IHealth)
│   ├── application/     → Lógica de negocio (MovementPlayer, atackPlayer, States/)
│   └── Infrastructure/  → Adaptadores de UI (LifeBar, ShieldBar)
│
├── 📁 Enemies/
│   ├── Domain/
│   │   ├── Entities/    → Goblin, Mushroom, Ogre, Skeleton, RangeEnemy, HitEnemy
│   │   ├── Factories/   → Creación de instancias de enemigos
│   │   └── Interfaces/  → Contratos (IMeleeEnemy, IRangeEnemy, IEnemyAnimator)
│   ├── Application/     → Comportamientos (PatrolBehavior, ChaseBehavior, AttackBehavior, EnemyHealth)
│   └── Infrastructure/  → Controladores (EnemyAnimatorController, EnemyPhysics, EnemySpawner)
│
├── 📁 BossSlime/
│   ├── domain/          → BossEnemy, IAttackBehavior
│   ├── application/     → Ataques del Boss (Melee, Flamethrower, Jump)
│   └── Infrastructure/  → Adaptadores de animación y física del Boss
│
├── 📁 Persistence/
│   ├── Domain/          → Modelo PlayerData, contratos de repositorio
│   ├── Application/     → Casos de uso (SaveGame, LoadGame)
│   └── Infrastructure/  → PersistenceController, GameRepository (lectura/escritura en disco)
│
├── 📁 Audio/
│   ├── domain/          → Interfaz IAudioManager
│   ├── application/     → AudioManager (singleton)
│   └── infrastructure/  → PlayerAudioHandler
│
├── 📁 UI/
│   ├── Menu/            → MainMenu
│   ├── Profiles/        → ProfileManager, ProfileSlotUI, ProfileButtonUI
│   ├── Pause/           → PauseManager
│   ├── Tutorial/        → TutorialUI
│   ├── Intro/           → IntroUI
│   ├── Credits/         → CreditsUI
│   └── Scenas/          → SceneTransition helpers
│
├── 📁 OnySingleton/     → Singletons de cámara (CamaraManagerSingleton, MainCamara, VirtualCamara)
├── 📁 Objects/          → Potion, PotionShield
└── GameManager.cs       → Orquestador principal del juego (Singleton)
```

---

## 🎮 Patrones de Diseño Implementados

### 🔄 State Pattern — Movimiento del Jugador
El movimiento se gestiona mediante un `PlayerMovementContext` que delega el comportamiento en estados concretos:

```csharp
// MovementPlayer.cs
movementContext = new PlayerMovementContext(this);
movementContext.TransitionTo(new NormalMovementState());
```

Los estados disponibles son:
- `NormalMovementState` — Movimiento estándar, salto, control de dirección
- `DashMovementState` — Dash rápido con cooldown e inmunidad temporal

---

### 🧩 Strategy Pattern — Comportamiento de Enemigos
Los enemigos delegan su comportamiento en estrategias intercambiables en tiempo de ejecución:

```csharp
// Goblin.cs - Transición dinámica entre comportamientos
if (distanceToPlayerX > VisionRange)
    SetBehavior(new PatrolBehavior());
else if (distanceToPlayerX > AttackRange)
    SetBehavior(new ChaseBehavior());
else
    SetBehavior(new AttackBehavior());
```

---

### 🔂 Unit of Work — Persistencia de Datos
El sistema de guardado encapsula las operaciones de repositorio dentro de una unidad atómica:

```csharp
// PersistenceController.cs
var unitOfWork = new UnitOfWork();
saveGame = new SaveGame(unitOfWork);
loadGame = new LoadGame(unitOfWork);
```

---

### 📣 Observer Pattern — Sistema de Salud
Los cambios de salud y escudo se propagan mediante eventos C# para actualizar la UI de forma desacoplada:

```csharp
// HealthPlayer.cs
public event Action OnDeath;
public event Action<float> OnHealthChanged;
public event Action<float> OnShieldChanged;
```

---

### 🔒 Singleton Pattern
Los managers críticos usan el patrón Singleton con `DontDestroyOnLoad`:
- `GameManager` — Orquestación de escenas y estados de partida
- `AudioManager` — Control centralizado de audio
- `PersistenceController` — Acceso único al sistema de guardado
- `ProfileManager` — Gestión del perfil activo

---

## 🗺️ Escenas del Juego

| Escena | Descripción |
|---|---|
| `Intro_Scene` | Cinemática de introducción con Timeline |
| `MainMenu` | Menú principal con opciones de nuevo juego y perfiles |
| `Profiles` | Selección y gestión de hasta 3 perfiles de guardado |
| `Loading` | Pantalla de carga entre transiciones de nivel |
| `FirstScene` | Escena bootstrap principal |
| `Tutorial` | Tutorial interactivo para nuevos jugadores |
| `Level1` | Primer nivel completo con enemigos y puzzles |
| `Level2` | Segundo nivel con mayor dificultad y nuevos enemigos |
| `Final` | Escena del jefe final con música épica y transformación |
| `Credits` | Pantalla de créditos al completar el juego |

---

## 👾 Enemigos

### Enemigos Estándar

| Enemigo | Tipo | Comportamiento |
|---|---|---|
| **Goblin** | Melee | Patrulla, persigue y ataca cuerpo a cuerpo con cooldown y stun |
| **Mushroom** | Melee | Variante con velocidad y daño únicos |
| **Ogre** | Melee | Tanque de alta resistencia y gran alcance de ataque |
| **Skeleton** | Melee | Comportamiento agresivo con mecánicas especiales |
| **Demon Fly** | Rango | Enemigo volador con ataque a distancia |

### Jefe Final — Slime Demonio

El jefe final cuenta con:
- ✅ **Fase 0 — Dormante**: Absorbe el primer golpe sin recibir daño
- ✅ **Secuencia de Transformación**: Animación cinematográfica, knockback al jugador y cambio de collider
- ✅ **Fase de Combate**: Persecución activa con 3 tipos de ataque

| Ataque | Descripción |
|---|---|
| ⚔️ **BossAttackMelee** | Golpe cuerpo a cuerpo con hitbox precisa |
| 🔥 **BossAttackFlamethrower** | Ataque de área con proyectiles de llama |
| 💥 **BossAttackJump** | Salto de área que daña al aterrizar |

---

## 💾 Sistema de Persistencia

El juego soporta **3 perfiles independientes** que guardan automáticamente:

```json
{
  "ProfileId": 1,
  "ProfileName": "Perfil 1",
  "CurrentScene": "Level2",
  "SpawnPointName": "Checkpoint_2",
  "PositionX": 45.3,
  "PositionY": 2.0,
  "PositionZ": 0.0,
  "Health": 75.0,
  "Shield": 30.0
}
```

El guardado se activa automáticamente al pasar por **portales** y **checkpoints**. Al cargar una partida, el `GameManager` restaura la escena, posición, salud y escudo del jugador.

---

## 📦 Paquetes Unity Utilizados

| Paquete | Versión | Uso |
|---|---|---|
| `Universal Render Pipeline` | 17.0.4 | Renderizado 2D con efectos de iluminación |
| `Input System` | 1.13.1 | Sistema de entrada moderno y configurable |
| `Cinemachine` | 3.1.3 | Cámara dinámica con seguimiento suave del jugador |
| `2D Feature Pack` | 2.0.1 | Tilemaps, Sprite Atlas, Physics 2D |
| `Timeline` | 1.8.7 | Cinemáticas y secuencias animadas |
| `TextMesh Pro` | (incluido) | UI de texto con alto rendimiento |
| `Visual Scripting` | 1.9.5 | Soporte para scripts visuales |
| `Test Framework` | 1.4.6 | Pruebas unitarias del proyecto |

---

## 🚀 Cómo Abrir el Proyecto

### Requisitos Previos
- **Unity Hub** instalado
- **Unity 6000.0.42f1** (versión exacta recomendada)
- **Visual Studio 2022** con el workload `Unity Game Development`

### Pasos

```bash
# 1. Clona el repositorio
git clone https://github.com/DavidGR21/Pixel-Odyssey.git

# 2. Abre Unity Hub

# 3. Selecciona "Add project from disk"

# 4. Navega a la carpeta:
#    Pixel-Odyssey/Pixel Odyssey Final/

# 5. Abre el proyecto con Unity 6000.0.42f1
```

Una vez abierto:
1. Ve a **File → Build Settings**
2. Asegúrate de que todas las escenas estén en el Build Order (en el orden listado arriba)
3. Presiona **Play** desde la escena `MainMenu` para iniciar el juego

---

## 🗂️ Estructura de Carpetas

```
Pixel-Odyssey/
└── Pixel Odyssey Final/
    ├── Assets/
    │   ├── src/
    │   │   ├── Scripts/         → Código fuente del juego (Clean Architecture)
    │   │   ├── Scenes/          → Todas las escenas del juego
    │   │   ├── Art/             → Sprites, tilesets, backgrounds y fuentes
    │   │   ├── Audio/           → Música y efectos de sonido
    │   │   ├── Animations/      → Clips de animación
    │   │   ├── AnimatorControllers/ → Controladores de animación
    │   │   ├── Prefabs/         → Prefabs de personajes, enemigos y objetos
    │   │   ├── Materials/       → Materiales URP
    │   │   └── Background/      → Fondos parallax
    │   └── TextMesh Pro/        → Assets de TMP
    ├── Packages/
    │   └── manifest.json        → Dependencias del proyecto
    └── ProjectSettings/         → Configuración del proyecto Unity
```

---

## 🎨 Arte y Estilo Visual

- **Estética**: Pixel Art con paleta de colores vibrante
- **Resolución de sprites**: Optimizada para 2D con filtro Point (sin anti-aliasing)
- **Personajes incluidos**: Hero principal, Goblin, Mushroom, Ogre, Skeleton, DemonFly, SlimeDemon (Boss)
- **Entornos**: Tilesets de exterior, interiores y escenarios de jefe
- **Efectos**: Parallax scrolling en fondos, efectos de partículas en ataques

---

## 👨‍💻 Autor

**David García** — *Estudiante de Ingeniería en Sistemas · 7mo Semestre*

> Proyecto desarrollado como parte del portafolio académico para demostrar habilidades en desarrollo de videojuegos con Unity, arquitectura limpia y patrones de diseño en C#.

---

## 📄 Licencia

Este proyecto es de uso académico y de portafolio personal. Todos los derechos sobre los assets artísticos pertenecen a sus respectivos autores originales.

---

<p align="center">
  Hecho con ❤️ y Unity 6 · <strong>Pixel Odyssey</strong>
</p>
