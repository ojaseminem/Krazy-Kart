# KRAZY KART — STRUCTURE PLAN
> Session start: read CONTEXT.md first, then this file. Check off tasks as completed.

---

## PHASE STATUS
| Phase | Name              | Status      |
|-------|-------------------|-------------|
| 0     | Foundation        | ✅ DONE     |
| 1     | Floor Systems     | 🔲 NEXT     |
| 2     | Cop System        | 🔲 PENDING  |
| 3     | MC & Scoring      | 🔲 PENDING  |
| 4     | Procedural Layout | 🔲 PENDING  |
| 5     | Boss Floor        | 🔲 PENDING  |
| 6     | Polish & VFX      | 🔲 PENDING  |
| 7     | UI & Leaderboard  | 🔲 PENDING  |

---

## PHASE 0 — FOUNDATION ✅
- [x] Kart physics feel established
- [x] GDD written and refined (v1.0)
- [x] Floor LD plans (5 floors documented)
- [x] AI context files created (this session)

---

## PHASE 1 — FLOOR SYSTEMS 🔲 ← ACTIVE NEXT
**Goal:** One playable floor end-to-end. B1 → Transition → F1 (Food Court).

### 1A — Scene Architecture
- [ ] Create `MallFloor` base MonoBehaviour (floor ID, MC budget, cop threshold)
- [ ] Create `FloorManager` singleton — tracks current floor, MC total, cop count
- [ ] Create `TransitionCorridor` component — triggers floor load, plays drift spiral
- [ ] Define scene naming convention: `Floor_B1`, `Floor_F1`, etc.
- [ ] Setup additive scene loading pipeline

### 1B — Prop System
- [ ] Create `Prop` base class: health, MC value, destruction VFX slot, ragdoll toggle
- [ ] Create `PropCluster` component: tracks group destruction for chain bonus
- [ ] Implement chain destruction detection (proximity + time window: 0.8s)
- [ ] Prop types to build first: shelf unit, food tray tower, parked car
- [ ] Create prop prefab folder structure: `Assets/Prefabs/Props/{Floor}/`

### 1C — B1 Basement (Tutorial Floor)
- [ ] Block out B1 geometry (see LD plan)
- [ ] Place: Spawn ramp · Open drift bay (parked cars) · Pillar maze · Cargo ramp · Exit spiral
- [ ] Ensure 2 navigable paths: (A) Bay→Maze→Exit  (B) Bay→Cargo ramp→Exit
- [ ] No cop spawns on B1
- [ ] Validate kart feel on B1 geometry

### 1D — F1 Food Court
- [ ] Block out F1 geometry (see LD plan)
- [ ] Place: NPC crowd zone · Kitchen cluster · Mezzanine ramp · Cop ambush lane · Exit bridge
- [ ] Implement tray tower chain reaction (hit tower → cascade → kitchen → double MC 5s)
- [ ] NPC ragdoll system (basic): scatter on hit, play reaction animation

### 1E — B1→F1 Transition Corridor
- [ ] Build parking spiral geometry (~15s at target speed)
- [ ] Add drift line rewards (clean arc = bonus MC)
- [ ] Trigger next floor load at midpoint of corridor

---

## PHASE 2 — COP SYSTEM 🔲
**Goal:** Cops feel threatening but fair. Player has clear counter-options.

### 2A — Cop Kart AI
- [ ] Create `CopKart` AI controller (extends base vehicle or separate)
- [ ] Implement behaviours: patrol → pursuit → box-in → ram
- [ ] State machine: Idle → Alert → Chase → Attack → Blocked → Reset
- [ ] Formation logic for 2+ cops (flanking, pincer)

### 2B — Spawn System
- [ ] `CopSpawnManager`: listens to FloorManager destruction count
- [ ] Spawn triggers: 3 props → 1 cop, 6 → 2 cops, 10 → 3-kart formation
- [ ] Pre-baked spawn zones per floor (not random positions)
- [ ] Hard cap: 4 active cops maximum
- [ ] Despawn on floor exit

### 2C — Counter Systems
- [ ] Collision redirect detection (cop hit by player into prop → MC reward)
- [ ] Verticality escape: tag ramp volumes as cop-inaccessible
- [ ] Cop-on-cop collision → both briefly stunned

---

## PHASE 3 — MC & SCORING SYSTEM 🔲
**Goal:** MC feels satisfying to earn. Clear feedback on every action.

### 3A — MC Manager
- [ ] `MCManager` singleton: running total, floor subtotal, conversion function
- [ ] MC → Escape Time formula: implement table as AnimationCurve (tunable in Inspector)
- [ ] Events: `OnMCEarned(int amount, MCSource source)` — drives UI + audio
- [ ] Floor subtotal resets on floor transition (carry total accumulates)

### 3B — Scoring Sources
- [ ] Prop destroy: base value on Prop scriptable object
- [ ] Chain bonus: PropCluster fires chain event
- [ ] Drift proximity: `DriftScorer` — raycast near-miss detection, style MC per second
- [ ] Combo multiplier: timer-based streak (resets after 2s without action)
- [ ] Stunt: air time > 0.5s = stunt. Score scales with hang time.
- [ ] Cop evade: cop loses pursuit → MC reward
- [ ] Transition clean run: no wall contact in corridor → bonus

### 3C — Scriptable Objects
- [ ] `PropData` SO: name, base MC, chain multiplier, VFX prefab
- [ ] `FloorData` SO: floor ID, MC budget, cop thresholds, module list
- [ ] `ScoringConfig` SO: all tunable values in one place (no magic numbers in code)

---

## PHASE 4 — PROCEDURAL LAYOUT 🔲
**Goal:** Same floor type, different layout each run.

### 4A — Module System
- [ ] Define `FloorModule` base: entry connector, exit connector, bounds, type enum
- [ ] Build module prefabs:
  - [ ] `Mod_Aisle` (linear, 2 lane widths)
  - [ ] `Mod_Corner` (90° turn)
  - [ ] `Mod_VertRamp` (angled + landing zone)
  - [ ] `Mod_DestructionCluster` (dense props, contained)
  - [ ] `Mod_OpenArena` (wide, minimal structure)
  - [ ] `Mod_SkillCorridor` (narrow, combo zone)

### 4B — Layout Graph
- [ ] `LayoutGraph`: nodes = module slots, edges = valid connections
- [ ] Generation rules:
  - Always: 1 open zone + 1 destruction cluster + exit connector
  - Optional: 1 vertical ramp module (placed if RNG > 0.4)
  - Guarantee: 2 distinct paths from entry to exit
- [ ] Seed system: run seed stored → same seed = same layout (for leaderboard fairness)
- [ ] Validation pass: pathfinding check before layout is confirmed

### 4C — Object Density Scaling
- [ ] Prop density parameter per FloorData SO
- [ ] Scales B1 (sparse) → F7 (dense) linearly
- [ ] Cop spawn zone density also scales with floor number

---

## PHASE 5 — BOSS FLOOR 🔲
**Goal:** Tense, readable, climactic. Ragdoll exit = memorable every time.

### 5A — Phase System
- [ ] `BossPhaseManager`: 4 phases, driven by remaining escape time
- [ ] Phase 1 (first 8s): clear floor, shutters visible
- [ ] Phase 2: shutter lerp animation + bollard deploy
- [ ] Phase 3: cleaning robot NavMesh agents + rotating door timing puzzles
- [ ] Phase 4: glass runway opens, boost ramp activates

### 5B — Escape Time Countdown
- [ ] Pull MC total from MCManager on entry
- [ ] Convert via AnimationCurve → escape seconds
- [ ] Countdown drives phase transitions AND boss UI
- [ ] Time-zero: force fail state (kart ragdolls, run ends, show MC summary)

### 5C — Cinematic Exit
- [ ] Glass wall trigger: on kart contact at boost threshold speed
- [ ] Slow-mo: Time.timeScale → 0.2 over 0.3s, hold 2s, restore
- [ ] Ragdoll: disable kart controller, enable ragdoll physics, apply exit velocity
- [ ] Camera: switch to cinematic follow cam, pull back
- [ ] Fade to run summary screen

---

## PHASE 6 — POLISH & VFX 🔲
- [ ] Prop destruction VFX (chunky, readable, satisfying)
- [ ] MC earn VFX (floating +number, colour by source type)
- [ ] Cop chase VFX (siren light, screech effects)
- [ ] Drift smoke trail
- [ ] Chain reaction visual feedback
- [ ] Ragdoll exaggeration tuning
- [ ] Transition corridor atmosphere (each one unique visual theme)
- [ ] Boss floor: lighting shifts red on Phase 3

---

## PHASE 7 — UI & LEADERBOARD 🔲
- [ ] HUD: MC counter (top right), minimap (bottom left), combo streak (centre)
- [ ] Boss HUD: full-screen countdown becomes dominant element
- [ ] Run summary screen: MC total, time, playstyle tag, leaderboard deltas
- [ ] 6 leaderboard categories (see CONTEXT.md)
- [ ] Session persistence: save run data between sessions

---

## UNITY FOLDER STRUCTURE (proposed)
```
Assets/
├── _AI/                  ← this folder. context files live here.
├── _Game/
│   ├── Scripts/
│   │   ├── Core/         ← FloorManager, MCManager, GameState
│   │   ├── Kart/         ← KartController (DONE), KartCamera
│   │   ├── Cops/         ← CopKart, CopSpawnManager
│   │   ├── Props/        ← Prop, PropCluster, ChainDetector
│   │   ├── Procedural/   ← FloorModule, LayoutGraph, LayoutValidator
│   │   ├── Boss/         ← BossPhaseManager, EscapeSequence
│   │   └── UI/           ← HUD, RunSummary, Leaderboard
│   ├── ScriptableObjects/
│   │   ├── PropData/
│   │   ├── FloorData/
│   │   └── ScoringConfig/
│   ├── Prefabs/
│   │   ├── Kart/
│   │   ├── Props/{Floor}/
│   │   ├── Modules/
│   │   └── Cops/
│   ├── Scenes/
│   │   ├── Floor_B1
│   │   ├── Floor_F1
│   │   ├── Floor_F2
│   │   ├── Floor_F3
│   │   ├── Floor_F4
│   │   ├── Floor_F5
│   │   ├── Floor_F6
│   │   ├── Floor_Boss
│   │   └── Corridor_{X}to{Y}
│   └── VFX/
└── Plugins/
```

---

## NAMING CONVENTIONS
| Type             | Convention          | Example                  |
|------------------|---------------------|--------------------------|
| MonoBehaviour    | PascalCase          | `FloorManager.cs`        |
| ScriptableObject | PascalCase + Data   | `PropData.cs`            |
| Prefab           | Cat_Name            | `Prop_ShelfUnit.prefab`  |
| Scene            | Context_ID          | `Floor_B1.unity`         |
| Event            | On + PastTense      | `OnMCEarned`             |
| Const/Magic num  | NEVER in code       | → ScoringConfig SO       |

---

## CURRENT NEXT ACTION
**Phase 1A — Scene Architecture.** Start with `FloorManager.cs` and `MallFloor.cs`. Establish the data flow skeleton before any geometry or props.
