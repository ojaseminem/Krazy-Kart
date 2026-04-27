# KRAZY KART — FLOOR LD PLANS
> Reference for level designers and Claude Code. All 5 detailed floors.

---

## B1 — BASEMENT PARKING (Tutorial)
**MC: 0–140 | Cops: None | Vertical: Minimal**

Paths:
- A: Spawn ramp → Open drift bay → Pillar maze → Exit spiral
- B: Spawn ramp → Open drift bay → Cargo ramp (platform) → Exit shortcut

| Module         | Type              | MC       | Notes                                      |
|----------------|-------------------|----------|--------------------------------------------|
| Spawn ramp     | Aisle (tutorial)  | 0        | 3s safe. Teaches boost + steer.            |
| Open drift bay | Open drift arena  | 20–60    | Parked cars = destructible. Style MC lines.|
| Pillar maze    | Skill corridor    | 30–80    | Shortcut. No-hit streak → combo multiplier.|
| Cargo ramp     | Vertical ramp     | 40–80    | Forklift pallet. Platform above = shortcut.|
| Exit spiral    | Transition entry  | —        | C-shape to F1.                             |

Tone: Concrete. Oil stains. Fluorescent flicker. Physics tutorial disguised as atmosphere.

---

## F1 — FOOD COURT
**MC: 80–280 | Cops: Low→Medium | Vertical: Mezzanine**

Paths:
- A: Entry → NPC crowd → Kitchen → Cop lane → Exit bridge
- B: Entry → NPC crowd → Mezzanine ramp (skips cop lane) → Upper exit

| Module          | Type              | MC       | Notes                                           |
|-----------------|-------------------|----------|-------------------------------------------------|
| NPC food court  | NPC zone          | 40–120   | Tables, trays, crowds. Two drift lines through. |
| Kitchen cluster | Destruction cluster| 60–120  | Fryers, trolleys. Chain hit = grease fire 2x MC.|
| Mezzanine ramp  | Vertical ramp     | 30–60    | Tilted escalator. Skips cop lane entirely.      |
| Cop ambush lane | Dynamic obstacle  | 0/60     | First cops here. Narrow. Bypass via mezzanine.  |

Signature: Tray tower in centre. One hit → cascade → kitchen → 5s double MC if chain reaches kitchen.

---

## F2 (F4–5) — ELECTRONICS & APPLIANCES
**MC: 120–380 | Cops: Medium | Vertical: Display towers**

Paths:
- A: TV corridor → Appliance aisle → Exit
- B: Appliance aisle → Display towers → Server cage → Exit

| Module          | Type              | MC         | Notes                                            |
|-----------------|-------------------|------------|--------------------------------------------------|
| TV wall corridor| Skill corridor    | 30–90      | Shattering without slowing = Precision Bonus.    |
| Appliance aisle | Destruction cluster| 60–180    | Drift sideways → cascade. Path opens as they fall.|
| Display towers  | Vertical ramp     | 50–110     | Multi-tower mid-air hit extends combo.            |
| Server cage     | Combo zone        | x2 mult    | No-wall-contact = streak maintained. Cop-free.  |

Tone: Sterile showroom → spark-shower chaos. The contrast IS the joke.
Cop note: 3rd cop spawns if player triggers 6+ chain destructions. Server cage cop-inaccessible.

---

## F3 (F6) — SPORTS & TOYS
**MC: 180–500 | Cops: Medium→High | Vertical: Half-pipe + Trampolines**

Paths:
- A: Sports arena → Half-pipe → Ball pit → Exit lift
- B: Sports arena → RC car track → Toy aisle → Exit lift

| Module        | Type              | MC       | Notes                                              |
|---------------|-------------------|----------|----------------------------------------------------|
| Sports arena  | Open drift arena  | 40–100   | Widest zone. Equipment trolleys as movable hazards.|
| Half-pipe     | Vertical stunt    | 80–160   | Carving walls = continuous Style MC.               |
| Ball pit chaos| Physics toy zone  | 60–140   | Balls spray out. Roll into adjacent modules. Cops get stuck here.|
| RC car track  | Dynamic obstacle  | 50–120   | Autonomous RC cars on oval. Redirect into cops.    |

Signature: Half-pipe → ball pit combo. Biggest single-jump MC reward in the game. Ball pit = guaranteed cop-free zone.

---

## F4 (F7) — HOME FURNITURE
**MC: 200–600 | Cops: High + Roadblock | Vertical: Crate mountain**

Paths:
- A: Showroom alley → Crate mountain → Glass partitions → Boss transition
- B: Showroom alley → Sofa maze (flank cops) → Glass partitions → Boss transition

| Module           | Type               | MC       | Notes                                               |
|------------------|--------------------|----------|-----------------------------------------------------|
| Showroom alley   | Open drift arena   | 50–120   | Room setups = chicanes. Home Tour Combo for sequence.|
| Crate mountain   | Vertical ramp      | 100–200  | Tallest jump. Roof landing = secret bonus.          |
| Sofa maze        | Destruction cluster| 80–160   | Chain bounce. Room Wrecker bonus for full cascade.  |
| Glass partitions | Skill corridor     | 60–120   | 20 panels. Momentum carries into boss entry.        |
| Cop roadblock    | Mandatory encounter| varies   | Only mandatory cop fight. 3-kart formation. 3 bypasses possible.|

Tone: IKEA sterile → red emergency lighting as player nears boss. Mood shift starts here.
Boss handoff: speed through glass corridor carries directly into boss opening sequence. Full boost = cinematic entry.

---

## TRANSITION CORRIDORS
C-shaped drift spirals between every floor. ~15s. No obstacles. Streams next floor.

| Corridor      | Visual Theme          |
|---------------|-----------------------|
| B1 → F1       | Parking spiral        |
| F1 → F2       | Service corridor      |
| F2 → F3       | Delivery ramp         |
| F3 → F4       | Glass elevator shaft  |
| F5 → F6       | Maintenance tunnel    |
| F7 → Boss     | Emergency stairwell   |

Design rule: Clean arc through spiral = MC bonus. No collisions = boost chain preserved.
