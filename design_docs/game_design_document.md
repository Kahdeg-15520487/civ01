# Rune Engraver (铭文师) - Game Design Document

> A Zachtronics-style programming puzzle game set in a Chinese xinxia cultivation world

---

## 1. Overview

### Concept
Players take on the role of a **Rune Engraver (铭文师)** in a cultivation sect, learning to inscribe mystical runes that power artifacts and formations. The game teaches programming concepts through the lens of runic alchemy, progressing from simple inscriptions to complex artifact systems.

### Target Audience
- Players familiar with Zachtronics games, especially **SHENZHEN I/O** (primary inspiration due to component crafting similarity)
- Fans of programming puzzles seeking a fresh thematic wrapper
- Cultivation/xinxia enthusiasts interested in a unique game format

### Platform & Engine
- **Platform:** Steam (Windows, Mac, Linux)
- **Engine:** Godot 4
  - Free and open-source
  - Excellent 2D and UI support
  - GDScript for rapid development
  - GodotSteam plugin for Steam integration

---

## 2. Core Mechanics

### Programming ↔ Runic System Mapping

> **Design Principle:** The system must be fully consistent and reproducible, allowing easy debugging by stepping through rune programs instruction-by-instruction.

| Programming Concept | Xinxia Equivalent | Description |
|---------------------|-------------------|-------------|
| Variables | **Spirit Containers (灵器)** | Vessels holding Qi of different elemental types |
| Functions | **Rune Patterns (符文阵)** | Reusable inscription templates |
| Conditionals | **Yin-Yang Gates (阴阳门)** | Routes Qi (True/False) based on positive/zero magnitude |
| Loops | **Circuit Topology** | Emergent loops created by routing output back to input |
| Data Types | **Elements (五行 + 辅元)** | 5 Primary (Metal/Wood/Water/Fire/Earth) + 2 Auxiliary (Lightning/Wind) |
| Composition | **Formation Nesting (阵中阵)** | Reuse formations within grander formations; combine artifact components |
| Power Sources | **Spirit Stones / Cultivator Qi** | Stable (Stones) vs Variable (Human input) amplitude sources |

### Five Elements & Auxiliary Systems
**7 Primary Elements:** Metal, Wood, Water, Fire, Earth + Lightning, Wind.
**21 Sub-Elements:** Complex unstable elements created by mixing primary interactions (e.g., Magma, Plasma, Mist).

*(See `runic_language_spec.md` for full interaction matrix)*

### Qi Release States

Three possible output states for any runic construct:

| State | Chinese | Behavior | Use Case |
|-------|---------|----------|----------|
| **Stable Release** | 稳释 (Wěn Shì) | Consistent, predictable output | Standard operations |
| **Overrunning Release** | 奔溢 (Bēn Yì) | Amplifies Qi input, escalating power | Power multiplication, chain reactions |
| **Fizzling Release** | 消散 (Xiāo Sàn) | Brief burst then dissipates | Momentary effects, triggers, signals |

### Failure States

| Failure | Runic Equivalent | Cause |
|---------|------------------|-------|
| Syntax Error | Rune Malformation | Invalid rune structure |
| Logic Error | Qi Deviation (走火入魔) | Artifact produces wrong output |
| Infinite Loop | Demonic Corruption | Uncontrolled energy spiral |
| Stack Overflow | Formation Collapse | Nested patterns exceed capacity |

### Optimization Metrics (Zachtronics-style)

- **Qi Consumption (灵力消耗)** - Energy cost per cycle
- **Rune Count (符文数量)** - Total inscriptions used
- **Stability Rating (稳定度)** - Resistance to edge cases
- **Execution Cycles (运转周期)** - Time to complete

---

## 3. Progression Structure

### Chapter 1: "The Novice's First Stroke" (入门第一笔)
**Theme:** Learning fundamentals

- Join Azure Peak Sect as outer disciple
- Learn basic runes: input, output, simple operations
- Tutorial puzzles: repair broken talismans, create storage pouches
- **Inciting Incident:** Discover a corrupted ancient artifact in storage

### Chapter 2: "Ink and Intent" (墨意相通)
**Theme:** Automation & Formations

- Promoted to inner disciple
- **New Feature: Automation Machinery**: Design runic assembly lines to mass-produce low-level components (e.g., standard Qi containers, simple sword blanks).
  - *Goal*: Automate tedious crafting to focus on complex, high-concept designs.
- **New Mechanic: Formations**: Introduction to stationary, multi-node arrays.
- Learn: Functions, composition, Five Elements interactions.
- Quest system: Fulfill **bulk orders** using your automation lines.
- **Conflict:** Ironheart Pavilion pressures sect for artifact blueprints

### Chapter 3: "The Broken Formation" (残阵之谜)
**Theme:** Reverse engineering legacy systems

- Discover fragments of the ancient Grand Formation
- Learn: complex pattern analysis, modular construction
- Puzzle type: reconstruct ancient artifacts from partial blueprints
- **Conflict:** Multiple sects race to recover Grand Formation knowledge

### Chapter 4: "Tournament of Inscriptions" (铭文大比)
**Theme:** Optimization and competition

- Regional competition between sects
- **Async PvP:** Player artifacts compete against others' solutions
- Leaderboards: "Dao Ranking" by efficiency metrics
- **Stakes:** Winner gains access to restricted ancient archives

### Chapter 5: "Sect War" (宗门之战)
**Theme:** Large-scale system design

- Ironheart Pavilion declares war on Azure Peak
- Create defensive formations protecting sect grounds
- Design artifacts for combat, logistics, communication
- **Climax:** Activate restored Grand Formation to turn the tide

---

## 4. Worldbuilding

### The World: Tianwen Continent (天纹大陆)
*"Heaven's Inscription Continent"*

#### Creation Myth
In the primordial era, the **Dao Ancestor (道祖)** inscribed the first rune upon the void, and from that single stroke, reality unfolded. All natural laws are merely "runes written upon existence." Cultivators who understand these runes can rewrite reality itself.

### Historical Eras

| Era | Chinese | Translation | Key Events |
|-----|---------|-------------|------------|
| Ancient Era | 太古纪 | Taigu Ji | Dao Ancestor inscribes reality. First runes discovered in nature. |
| Founding Era | 开宗纪 | Kaizong Ji | Seven Great Sects established. Rune classification system created. |
| Golden Age | 盛世纪 | Shengshi Ji | Peak of runic knowledge. Grand Formation protected entire continent. |
| Calamity Era | 劫变纪 | Jiebian Ji | Grand Formation collapsed catastrophically. Ancient knowledge lost. Sects weakened. |
| Current Era | 重铸纪 | Chongzhu Ji | "Era of Reforging" - Sects rebuild, rediscover lost techniques. *Player's story begins here.* |

### Major Sects

#### Azure Peak Sect (青峰宗) — *Player's Sect*
- **Specialty:** Balanced runic arts, preservation of ancient texts
- **Philosophy:** "Understanding before power"
- **Assets:** Fragments of Grand Formation blueprints, extensive library
- **Reputation:** Scholarly, respected but declining in power
- **Colors:** Blue and white

#### Ironheart Pavilion (铁心阁) — *Primary Antagonist*
- **Specialty:** Weapon artifacts, combat formations
- **Philosophy:** "Strength through refinement"
- **Assets:** Finest spiritual weapons on the continent
- **Reputation:** Militaristic, aggressive expansion
- **Colors:** Iron gray and crimson

#### Flowing Cloud Valley (流云谷) — *Neutral Power*
- **Specialty:** Utility artifacts, transportation, communication runes
- **Philosophy:** "Adapt and endure"
- **Assets:** Teleportation formation network, controls trade routes
- **Reputation:** Mercantile, neutral in conflicts
- **Colors:** Sky blue and silver

#### Crimson Lotus Temple (赤莲寺) — *Healer Faction*
- **Specialty:** Healing artifacts, purification runes
- **Philosophy:** "Harmony restores all"
- **Assets:** Techniques to cleanse Qi deviation artifacts
- **Reputation:** Benevolent publicly, secretly hoarding knowledge
- **Colors:** Red and gold

#### Obsidian Forge Sect (玄铁门) — *Resource Controller*
- **Specialty:** Material refinement, vessel creation
- **Philosophy:** "The foundation determines the peak"
- **Assets:** Raw material monopoly, mining territories
- **Reputation:** Isolationist, greedy
- **Colors:** Black and bronze

---

## 5. Multiplayer & Social Features

### Async Competitive
- **Dao Ranking:** Global leaderboards per puzzle
- **Artifact Duels:** Submit artifacts that compete against others
- **Tournament Events:** Seasonal ranked competitions

### Community Content
- **Sect Exchange (Steam Workshop):** Share custom puzzles
- **Solution Viewing:** Watch how top players solved puzzles
- **Challenge Mode:** Player-created puzzles with ratings

---

## 6. Art Direction (Placeholder Phase)

Current phase uses placeholder art. Final direction TBD.

**Aesthetic Notes for Later:**
- Calligraphy-inspired UI elements
- Ink wash painting (水墨画) influence
- Clean workspace for rune inscription (similar to Opus Magnum's aesthetic)
- Animated Qi flow visualization

---

## 7. Open Questions

- [ ] Define exact runic programming syntax/visual language
- [ ] Design the artifact workspace UI
- [ ] Detail the Five Elements interaction system
- [ ] Create named characters (sect elders, rivals, mentors)
- [ ] Design the quest/order fulfillment system
- [ ] Plan monetization (base game, DLC chapters, cosmetics?)

---

*Document Version: 0.1 — Initial Brainstorm*
