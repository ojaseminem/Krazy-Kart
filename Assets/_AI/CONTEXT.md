# KRAZY KART — AI CONTEXT
> Read this first. Always. Then read STRUCTURE_PLAN.md for task-specific detail.

---

## WHAT THIS PROJECT IS
Unity arcade runner. Kart ascends a mall (B1 → 8 floors → Boss). Stylish destruction earns Mall Credit (MC) → converts to escape time in boss sequence. ~15 min sessions.

**Engine:** Unity (version TBD)  
**Status:** Pre-production. Kart physics DONE. Everything else: not started.  
**Design lead:** Human. Claude handles: design tasks, code scaffolding, documentation, LD plans.

---

## CORE MECHANIC (memorize this)
```
Destroy / Drift / Stunt → earn MC → MC converts to escape seconds → Boss uses those seconds
Low MC = brutal boss. High MC = forgiving boss. Player self-selects difficulty.
```

## MC → TIME TABLE
| MC    | Escape Sec |
|-------|-----------|
| 0     | 12s       |
| 200   | 20s       |
| 500   | 35s       |
| 900   | 55s       |
| 1400  | 85s       |

---

## FLOORS (bottom → top)
| ID   | Name                  | MC Range    | Key Feature              |
|------|-----------------------|-------------|--------------------------|
| B1   | Basement Parking      | 0–140       | Tutorial. No cops.       |
| F1   | Food Court            | 80–280      | NPC crowds. Tray tower.  |
| F2   | Fashion & Clothing    | 100–300     | Mannequins. Escalator.   |
| F3   | Supermarket           | 100–260     | Trolleys. Shelf collapse.|
| F4-5 | Electronics           | 120–380     | TV wall. Server cage.    |
| F6   | Sports & Toys         | 180–500     | Half-pipe. Ball pit.     |
| F7   | Home Furniture        | 200–600     | Crate mountain. Glass.   |
| BOSS | Manager's Office      | uses all MC | 4-phase hostile escape.  |

---

## 3 PLAYSTYLES (no explicit difficulty slider)
- **Rush** → low MC → brutal boss → speedrun leaderboard
- **Destroy** → high MC → easy boss → slow but safe
- **Style** → optimal MC/sec → combo chains → best all-round

---

## SCORING SOURCES (key values)
| Action              | MC          |
|---------------------|-------------|
| Destroy prop        | +5–20       |
| Chain destruction   | +30–80      |
| Drift near object   | +10–25      |
| NPC hit             | +15–40      |
| Combo multiplier    | x1.5→x4    |
| Stunt/air time      | +40–120     |
| Evade cop           | +50–150     |
| Knock cop into prop | +80–200     |

---

## COP SYSTEM
- Spawn trigger: 3 props destroyed → 1 cop. 6+ → 2 cops. 10+ → 3-kart formation.
- Max active: 4 karts. Despawn on floor transition.
- Behaviours: box-in, ram, lane-block, herd, ambush.
- Counter: outmaneuver, env knock, ramp escape, cop-on-cop redirect.

---

## FLOOR LAYOUT RULES (procedural)
Every floor guarantees: 2 paths to exit · 1 open zone · 1 destruction cluster · 1 optional vertical route.  
Module types: Aisle · Corner · Vertical Ramp · Destruction Cluster · Open Drift Arena · Narrow Skill Corridor.  
Transition corridors between floors: C-shaped drift spiral. ~15s. No obstacles. Streams next floor.

---

## BOSS FLOOR (4 phases)
1. Clean entry (8s free)
2. Shutters + bollards deploy
3. Cleaning robots + rotating doors (slippery)
4. Glass runway → full boost → slow-mo ragdoll exit

---

## WHAT IS DONE
- [x] Kart physics (feel established)
- [ ] Everything else

---

## OPEN VARIABLES (need playtesting)
| Variable              | Current Estimate |
|-----------------------|-----------------|
| Floor size (time)     | 60–90s casual   |
| Destruction sweet spot| ~40% props      |
| MC per floor target   | 150–200         |
| Boss base time        | 12s             |
| Modules per floor     | 4–6             |
| Vertical route freq   | 1 guaranteed    |
| Transition length     | ~15s            |
| Max cop karts         | 4               |

---

## FILE INDEX (this folder)
| File                  | Purpose                              |
|-----------------------|--------------------------------------|
| `CONTEXT.md`          | THIS FILE. Read first every session. |
| `STRUCTURE_PLAN.md`   | Full build plan, task breakdown, phases. |
| `GDD_visual.html`     | Full styled GDD with all design detail. |
| `LD_floors.md`        | 5 floor LD plans (detailed).         |

---

## HOW TO USE THIS PROJECT
**Every Claude Code session must:**
1. Read `CONTEXT.md` (this file) — always first
2. Read `STRUCTURE_PLAN.md` — for current phase and next tasks
3. Check which tasks are checked off in STRUCTURE_PLAN.md
4. Proceed only with tasks in the active phase

**Claude Design sessions (claude.ai):** Read CONTEXT.md → ask human what design output is needed → produce → save artifact to this folder.
