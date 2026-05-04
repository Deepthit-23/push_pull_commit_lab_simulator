
# Chemistry_Lab_Simulator
> An immersive Virtual Reality training application for laboratory safety procedures built for Meta Quest 2/3.

---

## Table of Contents

- [Project Overview](#project-overview)
- [System Requirements](#system-requirements)
- [Installation Guide](#installation-guide)
- [Project Structure](#project-structure)
- [Scene Breakdown](#scene-breakdown)
- [Core Scripts](#core-scripts)
- [XR Setup and Configuration](#xr-setup-and-configuration)
- [Training Scenarios](#training-scenarios)
- [UI System](#ui-system)
- [Audio System](#audio-system)
- [Build and Deployment](#build-and-deployment)
- [Troubleshooting](#troubleshooting)
- [Future Improvements](#future-improvements)
- [Credits and License](#credits-and-license)

---

## Project Overview

### Description
VR Lab Safety Trainer is an immersive Virtual Reality training application designed to teach proper laboratory safety procedures. Built for Meta Quest 2/3 headsets, it provides hands-on training in three critical areas:
- Personal Protective Equipment (PPE) usage
- Fire extinguisher operation
- Hazard identification

### Purpose
- Provide a safe, risk-free environment for safety training
- Reduce training costs compared to physical equipment
- Enable repeatable, consistent training experiences
- Prepare students/workers for real laboratory environments

### Target Platform
| Target | Details |
|--------|---------|
| Primary | Meta Quest 2, Meta Quest 3 |
| Development | Unity Editor with XR Device Simulator |
| Distribution | Standalone APK via SideQuest |

### Technical Stack
| Component | Technology |
|-----------|-----------|
| Engine | Unity 2022.3 LTS |
| Render Pipeline | Universal Render Pipeline (URP) |
| XR Framework | XR Interaction Toolkit 3.4.0 |
| Platform | Android (API Level 29+) |
| Scripting | C# .NET Standard 2.1 |

---

## System Requirements

### Development — Hardware
| Component | Minimum |
|-----------|---------|
| CPU | Intel i5 / AMD equivalent (6th gen+) |
| RAM | 16GB (32GB recommended) |
| GPU | NVIDIA GTX 1060 / AMD RX 580 |
| Storage | 10GB free space |
| USB | USB 3.0 port for Quest connection |

### Development — Software
| Software | Notes |
|----------|-------|
| Unity 2022.3 LTS | Required |
| Android Build Support | Unity Module |
| Android SDK & NDK Tools | Unity Module |
| OpenJDK | Unity Module |
| GitHub Desktop | Optional, version control |
| SideQuest | Quest deployment |
| Android Platform Tools (ADB) | Device management |

### Runtime Requirements (Quest)
- Meta Quest 2 or Meta Quest 3
- 500MB free storage
- Developer Mode enabled
- Guardian boundary set up

---

## Installation Guide

### For Developers

#### Step 1 — Install Unity
1. Download Unity Hub from https://unity.com
2. Install Unity 2022.3 LTS
3. Add the **Android Build Support** module including:
   - Android SDK & NDK Tools
   - OpenJDK

#### Step 2 — Clone the Repository

**Option A — GitHub Desktop**
1. Open GitHub Desktop
2. File → Clone Repository
3. Enter URL: `https://github.com/YourUsername/VR-Lab-Safety-Trainer`
4. Choose local path → Click **Clone**

**Option B — Command Line**
```bash
git clone https://github.com/YourUsername/VR-Lab-Safety-Trainer.git
cd VR-Lab-Safety-Trainer
```

**Option C — Download ZIP**
1. Go to the repository URL
2. Code → Download ZIP
3. Extract to desired location

#### Step 3 — Open in Unity
1. Open Unity Hub
2. Click **Add** (or **Open**)
3. Navigate to the cloned/extracted folder
4. Select folder → Click **Open**
5. Wait for initial import (5–10 minutes)

#### Step 4 — Import Required Packages
`Window → Package Manager → Packages: In Project`

| Package | Status |
|---------|--------|
| XR Interaction Toolkit (3.4.0+) | ✅ Required |
| XR Plugin Management | ✅ Required |
| Oculus XR Plugin | ✅ Required |
| Universal RP | ✅ Required |
| TextMeshPro | ✅ Required |

If any are missing: **Window → Package Manager → Unity Registry** → search and install.

#### Step 5 — Import Starter Assets
1. Window → Package Manager
2. Find **XR Interaction Toolkit**
3. Expand **Samples**
4. Import **Starter Assets**
5. Import **XR Device Simulator**

#### Step 6 — Configure XR Settings
`Edit → Project Settings → XR Plug-in Management`

| Platform | Setting |
|----------|---------|
| PC/Mac/Linux Standalone | ✅ XR Device Simulator |
| Android | ✅ Oculus &nbsp;&nbsp; ❌ OpenXR (disable) |

#### Step 7 — Test in Editor
1. Open `Scenes/MainMenu`
2. Press **Play**
3. XR Device Simulator should appear
4. Test navigation with keyboard/mouse

---

### For End Users (Quest Installation)

#### Step 1 — Enable Developer Mode
1. Create a Meta Developer Account at [developer.oculus.com](https://developer.oculus.com)
2. Open the **Meta Quest app** on your phone
3. Menu → Devices → Your Quest → **Developer Mode → ON**
4. Wait ~5 minutes for activation

#### Step 2 — Install SideQuest
1. Download from [sidequestvr.com/setup-howto](https://sidequestvr.com/setup-howto)
2. Install the application
3. **Windows only:** Install Oculus ADB Drivers from [developer.oculus.com/downloads](https://developer.oculus.com/downloads)

#### Step 3 — Connect Quest
1. Connect Quest to PC via USB-C cable
2. Put on the Quest headset
3. Accept the **Allow USB Debugging** popup
4. Check **Always allow from this computer** → OK
5. SideQuest should show a green dot (connected)

#### Step 4 — Install the APK

**Via SideQuest**
1. Download `VRLabSafety.apk` from Releases
2. Open SideQuest
3. Drag the APK file into the SideQuest window
4. Wait for **"Successfully installed"**

**Via ADB**
```bash
adb install VRLabSafety.apk
```

#### Step 5 — Launch the App
1. Put on Quest headset
2. Press the Oculus button
3. Apps → Library → Filter: **Unknown Sources**
4. Find **VR Lab Safety Trainer** → Launch

---

## Project Structure

```
VRLabSafetyTrainer/
│
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity              # Entry scene with menu UI
│   │   └── LaboratoryScene.unity       # Main training environment
│   │
│   ├── Scripts/
│   │   ├── SceneController.cs          # Scene loading/management
│   │   ├── MenuUIManager.cs            # Menu panel switching
│   │   ├── TrainingFlowManager.cs      # Scenario sequencing
│   │   ├── AudioManager.cs             # Audio playback system
│   │   ├── FireExtinguisherController.cs
│   │   ├── FireController.cs
│   │   ├── HazardQuiz.cs
│   │   └── PPE/
│   │       ├── SafetyGogglesPPE.cs
│   │       ├── LabCoatPPE.cs
│   │       └── SafetyGlovesPPE.cs
│   │
│   ├── Prefabs/
│   │   ├── SafetyEquipment/
│   │   │   ├── Safety_Goggles.prefab
│   │   │   ├── Lab_Coat.prefab
│   │   │   └── Safety_Gloves.prefab
│   │   └── Props/
│   │       ├── Fire_Extinguisher.prefab
│   │       ├── Fire_Hazard.prefab
│   │       └── Chemical_Bottle.prefab
│   │
│   ├── Audio/
│   │   ├── UI/
│   │   │   ├── button_click.wav
│   │   │   └── success_sound.wav
│   │   ├── Interactions/
│   │   │   ├── spray_sound.wav
│   │   │   └── grab_sound.wav
│   │   └── Ambient/
│   │       └── lab_ambience.wav
│   │
│   └── Settings/
│       └── URP-MobileVR.asset          # Mobile-optimized URP
│
├── Packages/
│   └── manifest.json                   # Package dependencies
│
├── ProjectSettings/
│   ├── ProjectSettings.asset
│   └── QualitySettings.asset
│
├── .gitignore
└── README.md
```

---

## Scene Breakdown

### MainMenu Scene
**Purpose:** Entry point, scenario selection, settings

```
MainMenu
├── XR Origin (VR)
│   └── Camera Offset
│       ├── Main Camera
│       ├── LeftHand Controller
│       └── RightHand Controller
│
├── MainMenu_Canvas (World Space)
│   ├── MainMenuPanel
│   │   ├── TitleText
│   │   ├── StartButton
│   │   ├── ScenarioSelectButton
│   │   ├── InstructionsButton
│   │   └── QuitButton
│   ├── ScenarioSelectPanel
│   │   ├── Scenario1Button (PPE)
│   │   ├── Scenario2Button (Fire)
│   │   ├── Scenario3Button (Quiz)
│   │   ├── FullTrainingButton
│   │   └── BackButton
│   └── InstructionsPanel
│       ├── InstructionsText
│       └── BackButton
│
├── EventSystem
├── MenuManager
│   ├── SceneController
│   └── MenuUIManager
└── Directional Light
```

**Key Settings**
| Property | Value |
|----------|-------|
| Canvas Position | (0, 1.5, 0) |
| Canvas Scale | (0.01, 0.01, 0.01) |
| Canvas Render Mode | World Space |
| XR Origin Position | (0, 0, -3) |
| Tracking Origin Mode | Device |
| Locomotion | ❌ None |

---

### LaboratoryScene
**Purpose:** Main training environment with all scenarios

```
LaboratoryScene
├── XR Origin (VR)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── LeftHand Controller
│   │   └── RightHand Controller
│   ├── Character Controller
│   ├── Continuous Move Provider
│   ├── Snap Turn Provider
│   └── Locomotion System
│
├── Environment
│   ├── Floor / Walls / Ceiling
│   ├── Lab_Tables
│   └── Lighting
│
├── PPE_Equipment
│   ├── Safety_Goggles
│   ├── Lab_Coat
│   └── Safety_Gloves
│
├── Fire_Equipment
│   ├── Fire_Extinguisher
│   └── Fire_Hazard
│
├── Quiz_Objects
│   ├── Chemical_Bottle_1
│   ├── Chemical_Bottle_2
│   └── Chemical_Bottle_3
│
├── UI_Canvases
│   ├── PPE_InstructionPanel
│   ├── Quiz_Panel
│   └── CompletionPanel
│
└── Managers
    ├── TrainingFlowManager
    ├── AudioManager
    └── SceneController
```

**Key Settings**
| Property | Value |
|----------|-------|
| XR Origin Position | (0, 0, 1.5) |
| Character Controller Height | 1.8 |
| Character Controller Radius | 0.25 |
| Character Controller Center | (0, 0.9, 0) |
| Step Offset | 0.1 *(low — prevents climbing tables)* |
| Move Speed | 2.0 |
| Use Gravity | ✅ |
| Camera Offset Position | (0, 1.6, 0) |

---

## Core Scripts

### SceneController.cs
**Purpose:** Manages scene loading and transitions  
**Location:** `Assets/Scripts/SceneController.cs`

```csharp
public void LoadMainMenu()          // Load main menu scene
public void LoadLaboratoryScene()   // Load training scene
public void LoadScenario1()         // PPE only
public void LoadScenario2()         // Fire only
public void LoadScenario3()         // Quiz only
public void LoadFullTraining()      // All scenarios in sequence
public void QuitApplication()       // Exit app
```

> **Note:** Do NOT use `DontDestroyOnLoad` — it causes button duplication issues.  
> Uses `PlayerPrefs` to communicate the selected scenario to the lab scene.

---

### MenuUIManager.cs
**Purpose:** Controls menu panel visibility  
**Location:** `Assets/Scripts/MenuUIManager.cs`

```csharp
// Inspector Fields
public GameObject mainMenuPanel;
public GameObject instructionsPanel;
public GameObject scenarioSelectPanel;

// Methods
public void ShowMainMenu()
public void ShowInstructions()
public void ShowScenarioSelect()
```

---

### TrainingFlowManager.cs
**Purpose:** Manages scenario progression and completion callbacks  
**Location:** `Assets/Scripts/TrainingFlowManager.cs`

```csharp
// Scenario control
public void StartPPEScenario()
public void StartFireScenario()
public void StartQuizScenario()

// Completion callbacks
public void OnPPEComplete()
public void OnFireExtinguished()
public void OnQuizComplete()

public void ReturnToMenu()
```

**Workflow**
```
Start() reads PlayerPrefs.GetInt("StartScenario")
  0 = Full training (all scenarios)
  1 = PPE only
  2 = Fire only
  3 = Quiz only
  ↓
Enables appropriate scenario
  ↓
Waits for completion callback
  ↓
If full training → advances to next scenario
  ↓
Shows completion panel when done
```

---

### SafetyGogglesPPE.cs / LabCoatPPE.cs / SafetyGlovesPPE.cs
**Purpose:** Handles PPE grab and auto-wear mechanics  
**Location:** `Assets/Scripts/PPE/`

```csharp
// SafetyGogglesPPE
public Transform headAttachPoint;
public bool isWorn = false;

// LabCoatPPE
public Transform torsoAttachPoint;  // Child of Camera Offset at (0, -0.3, 0.2)

// SafetyGlovesPPE
public Transform handAttachPoint;   // LeftHand or RightHand Controller
```

| PPE Item | Attach Point | Detection Range |
|----------|-------------|-----------------|
| Safety Goggles | Main Camera | 1.0 units |
| Lab Coat | Camera Offset child | 1.0 units |
| Safety Gloves | Hand Controllers | 1.0 units |

> Items auto-attach when released within 1.0 units of their attach point.

---

### FireExtinguisherController.cs
**Purpose:** Handles grab, trigger-spray, and fire detection  
**Location:** `Assets/Scripts/FireExtinguisherController.cs`

```csharp
[Header("Components")]
public ParticleSystem sprayParticles;
public Transform sprayOrigin;
public AudioSource spraySound;

[Header("Settings")]
public float sprayRange = 15f;
public float extinguishRate = 25f;
```

**Required GameObject Setup**
```
Fire_Extinguisher
├── ✅ Rigidbody
├── ✅ Capsule Collider
├── ✅ XR Grab Interactable  (Use Activate Action: ✅)
├── ✅ FireExtinguisherController
├── ✅ Audio Source
└── Nozzle/
    └── SprayOrigin
        └── Spray_Particles (ParticleSystem)
```

> ⚠️ **Use Activate Action must be CHECKED** on the XR Grab Interactable or trigger input will not work.

---

### FireController.cs
**Purpose:** Manages fire intensity and visual feedback  
**Location:** `Assets/Scripts/FireController.cs`

```csharp
public float maxFireIntensity = 100f;
public float currentFireIntensity = 100f;
public ParticleSystem fireParticles;
public Light fireLight;
public bool isExtinguished = false;
```

| Property | Value |
|----------|-------|
| Starting Intensity | 100 |
| Extinguish Rate | 25 / second |
| Time to Extinguish | ~4 seconds |

**Required GameObject Setup**
```
Fire_Hazard
├── ✅ Sphere Collider  (Is Trigger: ✅, Radius: 0.8)
├── ✅ FireController
├── Fire_Particles (ParticleSystem)
└── Fire_Light (Light)
```

---

### HazardQuiz.cs
**Purpose:** Chemical bottle placement quiz  
**Location:** `Assets/Scripts/HazardQuiz.cs`

```csharp
public GameObject[] chemicalBottles;
public Transform[] correctPositions;
public float placementThreshold = 0.5f;
public GameObject quizPanel;
public TextMeshProUGUI feedbackText;
```

**Correct Placements**
| Bottle | Chemical Type | Storage Location |
|--------|--------------|-----------------|
| Bottle 1 | Flammable | Red safety cabinet |
| Bottle 2 | Corrosive | Yellow corrosive cabinet |
| Bottle 3 | General | White storage shelf |

---

### AudioManager.cs
**Purpose:** Centralized singleton audio system  
**Location:** `Assets/Scripts/AudioManager.cs`

```csharp
public void PlayButtonClick()
public void PlaySuccessSound()
public void PlayGrabSound()
public void PlayPlaceSound()
public void PlaySound(AudioClip clip)
```

> Singleton with `DontDestroyOnLoad` — persists across all scenes.  
> Uses separate Audio Sources for UI (2D) and effects (2D/3D).

---

## XR Setup and Configuration

### Required Packages
| Package | Version |
|---------|---------|
| XR Interaction Toolkit | 3.4.0+ |
| XR Plugin Management | Latest |
| Oculus XR Plugin | Latest |

**Samples to import:**
- ✅ Starter Assets
- ✅ XR Device Simulator

### XR Plugin Management
| Platform | Setting |
|----------|---------|
| PC/Mac/Linux | ✅ XR Device Simulator |
| Android | ✅ Oculus &nbsp;&nbsp; ❌ OpenXR |

### XR Origin Configuration
```
XR Origin (VR)
  Scale: (1, 1, 1)  ⚠️ MUST be exactly 1,1,1

  ✅ XR Origin
     Tracking Origin Mode: Device

  ✅ Character Controller
     Height: 1.8  |  Radius: 0.25
     Center: (0, 0.9, 0)  |  Step Offset: 0.1

  ✅ Continuous Move Provider
     Move Speed: 2.0  |  Use Gravity: ✅
     Forward Source: Main Camera  ← MUST be assigned

  ✅ Snap Turn Provider
     Turn Amount: 45°

  ✅ Input Action Manager
     Action Assets: XRI Default Input Actions
```

### Main Camera
```
Main Camera
  ✅ Camera
     Allow HDR: ❌  |  Allow MSAA: ❌
     Near: 0.1  |  Far: 1000

  ✅ Audio Listener

  ✅ Tracked Pose Driver (Input System)  ← CRITICAL
     Tracking Type: Rotation And Position
     Update Type: Update And Before Render
```
> ⚠️ Without Tracked Pose Driver, head tracking will not work in VR.

### Controller Configuration
Each controller needs **both** interactor types:

| Component | Purpose |
|-----------|---------|
| XR Direct Interactor | Grabbing nearby objects |
| XR Ray Interactor | Clicking UI from a distance |

```
LeftHand / RightHand Controller
  ✅ XR Controller (Action-based)
  ✅ Tracked Pose Driver (Input System)
  ✅ XR Direct Interactor
  ✅ XR Ray Interactor
     Enable Interaction with UI: ✅  ← CRITICAL
  ✅ XR Interactor Line Visual
  ✅ Sphere Collider  (Is Trigger: ✅, Radius: 0.1)
```

### World Space Canvas (VR UI)
```
Canvas
  Render Mode: World Space      ← never Screen Space
  Event Camera: Main Camera     ← MUST be assigned
  Layer: UI                     ← required for raycast
  Scale: (0.01, 0.01, 0.01)
  
  ✅ Graphic Raycaster          ← CRITICAL

Buttons:
  ✅ Image → Raycast Target: ✅
  ✅ Button → Interactable: ✅
```

---

## Training Scenarios

### Scenario 1 — PPE Training

**Objective:** Wear all three PPE items correctly.

**Workflow**
1. PPE instruction panel appears on wall
2. Player grabs each item from the equipment table
3. Release near the correct body part → item auto-attaches
4. All 3 items worn → success sound → scenario complete

| Item | Release Near | Auto-attaches To |
|------|-------------|-----------------|
| Safety Goggles | Head | Main Camera transform |
| Lab Coat | Torso | Camera Offset child |
| Safety Gloves | Hands | Controller transforms |

---

### Scenario 2 — Fire Extinguisher

**Objective:** Extinguish the laboratory fire.

**Workflow**
1. Fire is active (particles + light)
2. Player grabs the fire extinguisher
3. Pull trigger while pointing at fire → spray emits
4. Fire intensity decreases while spray hits it
5. Intensity reaches 0 → fire out → scenario complete

| Property | Value |
|----------|-------|
| Starting Intensity | 100 |
| Extinguish Rate | 25 / second |
| Spray Range | 15 metres |
| Time to Extinguish | ~4 seconds |

---

### Scenario 3 — Hazard Quiz

**Objective:** Sort 3 chemical bottles into correct storage locations.

**Workflow**
1. Bottles start in wrong positions
2. Player grabs and moves each bottle to the correct cabinet
3. Bottle within 0.5 units of target = correctly placed
4. All 3 placed correctly → scenario complete

| Bottle | Type | Correct Location |
|--------|------|-----------------|
| Bottle 1 | Flammable | Red safety cabinet |
| Bottle 2 | Corrosive | Yellow corrosive cabinet |
| Bottle 3 | General | White storage shelf |

---

### Full Training Mode
When `PlayerPrefs "StartScenario" = 0`, all three scenarios run in sequence:

```
Main Menu → Start Training
    ↓
Scenario 1: PPE Training
    ↓  (on complete)
Scenario 2: Fire Extinguisher
    ↓  (on complete)
Scenario 3: Hazard Quiz
    ↓  (on complete)
Completion Panel → Return to Menu
```

> Enforces the correct real-world safety order — PPE always before handling hazards.

---

## UI System

### Button Connections

| Button | Method |
|--------|--------|
| StartButton | `SceneController.LoadFullTraining()` |
| ScenarioSelectButton | `MenuUIManager.ShowScenarioSelect()` |
| InstructionsButton | `MenuUIManager.ShowInstructions()` |
| QuitButton | `SceneController.QuitApplication()` |
| Scenario1Button | `SceneController.LoadScenario1()` |
| Scenario2Button | `SceneController.LoadScenario2()` |
| Scenario3Button | `SceneController.LoadScenario3()` |
| BackButton | `MenuUIManager.ShowMainMenu()` |

### Lab UI Positions
| Panel | Position | Rotation |
|-------|----------|---------|
| PPE Instruction Panel | (-4.5, 2, 0) | (0, 90, 0) |
| Quiz Panel | (3, 2.2, -2) | (0, -90, 0) |
| Completion Panel | (0, 2, 0) | (0, 0, 0) |

### VR UI Best Practices

**Do ✅**
- World Space canvas
- Event Camera assigned to Main Camera
- Layer set to UI
- Position at eye level (Y: 1.5–2.5)
- Scale small (0.005–0.01)
- Large buttons (easy to point at)
- High contrast text
- TextMeshPro for crisp rendering

**Avoid ❌**
- Screen Space canvas (doesn't render in VR)
- Tiny buttons
- Low contrast colours
- Canvas placed behind the player

---

## Audio System

### Audio Clips
| Clip | Category | Notes |
|------|----------|-------|
| button_click.wav | UI | Short click |
| success_sound.wav | UI | Completion chime |
| grab_sound.wav | Interaction | Pick up objects |
| spray_sound.wav | Interaction | Looping, on extinguisher |
| lab_ambience.wav | Ambient | Background loop |

### AudioManager Setup
```
AudioManager GameObject
  ✅ AudioManager script   (Singleton, DontDestroyOnLoad)
  ✅ Audio Source — UI     (Spatial Blend: 0, Volume: 0.7)
  ✅ Audio Source — Effects (Spatial Blend: 0, Volume: 0.8)
```

### From Scripts
```csharp
AudioManager.instance.PlayButtonClick();
AudioManager.instance.PlaySuccessSound();
AudioManager.instance.PlayGrabSound();
AudioManager.instance.PlaySound(myClip);
```

### Spray Sound (3D Spatial)
```csharp
// On the Fire_Extinguisher GameObject itself — not AudioManager
void StartSpray()
{
    spraySound.loop = true;
    spraySound.Play();
}

void StopSpray()
{
    spraySound.Stop();
}
```

> ⚠️ Drag the **Audio Source component** into the Spray Sound field — not the GameObject.

---

## Build and Deployment

### Build Settings
`File → Build Settings`

| Setting | Value |
|---------|-------|
| Platform | Android |
| Scene [0] | Scenes/MainMenu |
| Scene [1] | Scenes/LaboratoryScene |
| Texture Compression | ASTC |

### Player Settings (Android)
| Setting | Value |
|---------|-------|
| Scripting Backend | IL2CPP |
| Target Architecture | ARM64 only |
| Minimum API Level | Android 10.0 (API 29) |
| Graphics API | OpenGLES3 only (remove Vulkan) |
| Multithreaded Rendering | ✅ |

### URP Asset Settings (Mobile-Optimised)
| Setting | Value |
|---------|-------|
| HDR | ❌ DISABLED |
| MSAA | Disabled |
| Soft Shadows | ❌ DISABLED |
| Main Light | Per Vertex |
| Cast Shadows | ❌ Disabled |
| SRP Batcher | ✅ Enabled |

> ⚠️ HDR and Soft Shadows **must** be disabled on Quest — they cause white spots and green tint.

### Build Process
1. ✅ Verify scenes in Build Settings (MainMenu = 0, LaboratoryScene = 1)
2. ✅ XR Settings: Android → Oculus ✅, OpenXR ❌
3. ✅ Save all scenes (`Ctrl+S`)
4. ✅ File → Build Settings → **Build**
5. ✅ Name file `VRLabSafety.apk`
6. ✅ Wait 5–15 minutes

### Deploy via SideQuest
1. Connect Quest via USB → accept USB debugging prompt
2. Open SideQuest → verify green dot
3. Drag `VRLabSafety.apk` into SideQuest
4. Wait for **"Successfully installed"**
5. In headset: Apps → Unknown Sources → VR Lab Safety Trainer

### Deploy via ADB
```bash
adb devices                               # verify connection
adb install VRLabSafety.apk              # install
adb install -r VRLabSafety.apk           # force reinstall
adb uninstall com.yourname.vrlabsafety   # uninstall
adb logcat -s Unity                       # view logs
adb shell screencap /sdcard/screen.png   # screenshot
adb pull /sdcard/screen.png
adb shell screenrecord /sdcard/demo.mp4  # record video (Ctrl+C to stop)
adb pull /sdcard/demo.mp4
```

### Post-Deploy Testing Checklist
- [ ] App appears in Unknown Sources
- [ ] Launches without crashing
- [ ] Main Menu visible and correctly positioned
- [ ] All buttons clickable with controller ray
- [ ] Head tracking works
- [ ] Controller tracking works
- [ ] Locomotion works (walk + snap turn)
- [ ] PPE items grabbable and auto-wear
- [ ] Fire extinguisher spray works
- [ ] Fire extinguishes when sprayed
- [ ] Chemical bottles grabbable
- [ ] Quiz detects correct placement
- [ ] Audio plays correctly
- [ ] No white spots or visual artifacts
- [ ] Framerate smooth (72+ FPS)
- [ ] Return to menu works

---

## Troubleshooting

### Visual Issues
| Issue | Cause | Fix |
|-------|-------|-----|
| White spots on surfaces | HDR + Soft Shadows on mobile GPU | Disable HDR and Soft Shadows in URP asset |
| Green tint on everything | Linear colour space + HDR on mobile | Disable HDR in URP asset |
| Head tracking not working | Missing Tracked Pose Driver | Add Tracked Pose Driver (Input System) to Main Camera |
| Controllers not tracked | Missing input actions | Assign XRI Default Input Actions to XR Origin |

### Interaction Issues
| Issue | Cause | Fix |
|-------|-------|-----|
| Buttons don't respond | Missing Event Camera | Canvas → Event Camera: assign Main Camera |
| Can't grab objects | Missing components | Object needs Rigidbody + Collider + XR Grab Interactable |
| Floating / stuck on tables | Step Offset too high | Character Controller → Step Offset: 0.1, enable Use Gravity |
| App starts at wrong scene | Wrong scene order | MainMenu must be index 0 in Build Settings |
| Height wrong in VR | Tracking Origin Mode | XR Origin → Tracking Origin Mode: Device |

### Build Issues
| Issue | Fix |
|-------|-----|
| IL2CPP build error | Edit → Preferences → External Tools → verify NDK/SDK paths |
| Gradle build failed | Use embedded Gradle in External Tools |
| APK too large (>2GB) | Enable Split APKs, use ASTC compression, remove unused assets |

### Audio Issues
| Issue | Fix |
|-------|-----|
| No sound in VR | Check Quest volume, verify Audio Listener on Main Camera |
| Spray sound missing | Drag the Audio Source **component** (not the GameObject) into Spray Sound field |

### Performance Issues
| Issue | Fix |
|-------|-----|
| Low framerate / stuttering | Disable shadows, reduce particles, use URP-MobileVR, fewer Rigidbodies |
| Battery drains quickly | Target 72 Hz, lower texture quality, disable unused particle systems |

---

## Future Improvements

### Training Enhancements
- [ ] More scenarios (chemical spill, emergency shower, eye wash)
- [ ] Scoring system with leaderboards
- [ ] Time limits and difficulty levels (Easy / Medium / Hard)
- [ ] Multiplayer training sessions
- [ ] Voice instructions / narration
- [ ] Hand gesture recognition (hand tracking)

### Technical Improvements
- [ ] Save progress between sessions
- [ ] Analytics (completion rates, time per scenario)
- [ ] Localisation (multiple languages)
- [ ] Accessibility features (subtitles, colorblind modes)

### Platform Expansion
- [ ] PCVR support (SteamVR, Oculus Rift)
- [ ] Pico headset support
- [ ] WebXR version

### Known Limitations
| Limitation | Detail |
|-----------|--------|
| Single player only | No multiplayer |
| No save system | Progress resets each session |
| 3 scenarios only | No spill/shower modules yet |
| Placeholder models | Geometric shapes for PPE |
| No voice instructions | Text only |
| English only | No localisation |
| Quest only | No PCVR support |

### Suggested Refactoring
```csharp
// Base class for all PPE items (reduces duplication)
public abstract class BasePPE : MonoBehaviour
{
    protected Transform attachPoint;
    protected bool isWorn;
    public virtual void Wear() { }
    public virtual void Remove() { }
}

// Enum instead of hard-coded ints
public enum TrainingScenario
{
    FullTraining,
    PPEOnly,
    FireOnly,
    QuizOnly
}

// Static event system
public class TrainingEvents
{
    public static event Action OnPPEComplete;
    public static event Action OnFireComplete;
    public static event Action OnQuizComplete;
}
```

---

## Credits and License

### Project Info
| Field | Details |
|-------|---------|
| Project | VR Lab Safety Trainer |
| Engine | Unity 2022.3 LTS |
| XR Framework | XR Interaction Toolkit 3.4.0 |
| Platform | Meta Quest (Oculus XR Plugin) |
| Language | C# (.NET Standard 2.1) |
| Year | 2026 |

### Acknowledgements
- Unity Technologies — XR Interaction Toolkit
- Meta — Quest platform and developer documentation
- SideQuest team — deployment tooling



### Contact
| | |
|-|-|
| GitHub |(https://github.com/Deepthit-23/VR_Lab_Safety_Trainer2.0) |
| Email | deepthit.126@gmail.com |

---

*Version 1.0 — January 2026*
```
