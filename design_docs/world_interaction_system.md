# World Interaction System

> "The cultivator shapes the world, and the world shapes the cultivator."

## Overview

This document defines how player-created artifacts and formations interact with the game world. The core philosophy: **player actions have lasting environmental consequences**.

---

## Two Forms of Power

### Artifacts (法宝) - Portable Power

```
Definition: Physical objects that house runic circuits
Scale: Personal (affects user and immediate surroundings)
Portability: Carried by cultivator
Examples: Flying swords, storage bags, defensive talismans

Characteristics:
- Limited by material and size
- Powered by Spirit Stones or cultivator's Qi
- Can be used anywhere
- Degrade with use
- Can be lost, stolen, or destroyed
```

### Formations (阵法) - Stationary Power

```
Definition: Large-scale runic circuits etched into the environment
Scale: Regional (affects areas, buildings, terrain)
Portability: Fixed in place (once deployed)
Examples: Sect defense arrays, treasure vault protections, agricultural formations

Characteristics:
- Scale limited only by resources
- Powered by ambient Qi, ley lines, or massive spirit stone banks
- Only work at installation site
- Persistent (last until destroyed)
- Can be linked into networks
- Can tap ambient Qi
```

---

## Deployment vs Carrying

### Artifact Usage

```
Carried: Always available, instant activation
- Flying sword: Summon to hand, mount and fly
- Storage bag: Access inventory anywhere
- Defensive talisman: Auto-activate on hit

Worn: Passive effects
- Qi-boosting ring: +X% regeneration
- Element-protecting amulet: Resist element
- Experience-gathering pendant: +Y% XP

Active: Manual activation
- Combat: Emitter weapons
- Utility: Lighting, heating
- Emergency: Teleportation, shield
```

### Formation Deployment

```
Temporary: Deployed for specific purpose
- Battle formation: Set up before fight, retrieve after
- Exploration formation: Light/scan as you move
- Camp defense: Protect during rest

Permanent: Fixed installation
- Sect defense: Always active, massive scale
- Resource gathering: Continuous production
- Territory control: Assert dominance

Deployment Process:
1. Design formation (circuit editor)
2. Engrave on flag/pillar/surface
3. Place at location
4. Activate (connection to power source)
5. Formation influences area
```

---

## Formation Architecture

### Components

```
Array Base (阵盘) - Central Controller
- The "brain" of the formation
- Houses main logic circuits
- Connected to all nodes
- Destroying base disables formation
- Size: 1x1m to 10x10m (varies by tier)

Nodes (阵点) - Remote Units
- Flag (阵旗): Portable, temporary
- Pillar (阵柱): Permanent, durable
- Spirit Stone (灵石): Power source
- Effect Emitter: Output point

Connections:
- Ley Lines (灵脉): Wireless Qi channels
- Physical Traces: Engraved lines (more reliable)
- Vortex Links: Spatial connections (Heaven-tier)
```

### Formation Tiers

```
Mortal Formation (1-10 nodes):
- Range: 10m radius
- Power: Low (affects mortals)
- Cost: 100-1000 coins
- Duration: 1 hour (temporary) to 1 day (permanent)
- Examples: Lighting array, basic heating

Spirit Formation (10-100 nodes):
- Range: 100m radius
- Power: Medium (affects Spirit cultivators)
- Cost: 10,000-100,000 coins
- Duration: 1 day (temp) to 1 year (perm)
- Examples: Sect perimeter defense, garden irrigation

Earth Formation (100-1000 nodes):
- Range: 1km radius
- Power: High (affects Earth cultivators)
- Cost: 1M-10M coins
- Duration: 1 year (temp) to 100 years (perm)
- Examples: City-scale weather control, mountain sealing

Heaven Formation (1000+ nodes):
- Range: Continental
- Power: Extreme (affects reality)
- Cost: Priceless
- Duration: Centuries to millennia
- Examples: The Grand Formation itself, realm boundaries
```

---

## Environmental Interaction

### Ambient Qi Tapping

```
Natural Sources:
- Ley Lines: Invisible rivers of Qi
- Dragon Veins: Major geological features
- Spirit Wells: Natural concentration points
- Elemental Nodes: Local element dominance

Formation Tapping:
- Divert ambient Qi to formation
- Strength depends on formation quality
- Over-tapping depletes area (permanent damage)

Tap Efficiency:
- Basic formation: 1% of ambient Qi
- Optimized formation: 10%
- Masterwork: 50%
- Heaven-tier: 90% (dangerous)
```

### Environmental Modification

```
Formations CAN change the environment:

Climate Control:
- Heating formation: Area becomes hotter
- Rain summoning: Area becomes wetter
- Drought formation: Area becomes drier
- Wind calming: Reduces wind speed

Permanent Changes (after long-term use):
- Desert creation: Fire over-use (100 years)
- Forest creation: Wood over-use
- Lake formation: Water accumulation
- Mountain raising: Earth reinforcement

Reversibility:
- Minor changes: Reverse in 1 year
- Major changes: Reverse in 100 years
- Extreme changes: Permanent (requires Heaven-tier to fix)
```

### Elemental Zones

```
Natural Zones:
- Volcanic region: Fire dominant
- Frozen tundra: Water/Ice dominant
- Lightning peak: Lightning dominant
- Dense forest: Wood dominant
- Rich mines: Metal dominant
- Open plains: Earth dominant

Formation-Created Zones:
- Player can create elemental dominance
- Requires massive formation
- Takes years to establish
- Affects local spawns, resources, NPCs

Example: Fire Dominant Zone
- Spawns: Fire spirits, magma slimes
- Resources: Fire crystals, sulfur
- NPCs: Fire cultivators attracted
- Plants: Fire-resistant flora grows
```

---

## World Persistence

### The Change Tracker

```
Every tile (100x100m) tracks:
- Ambient Qi levels (by element)
- Formation activity history
- Environmental modifications
- Biodiversity changes
- Resource depletion

Data Structure:
Tile {
  position: (x, y)
  biome: "forest"
  ambient_qi: {
    metal: 10,
    wood: 50,
    water: 30,
    fire: 5,
    earth: 25,
    lightning: 0,
    wind: 15
  }
  formations: [list of active formations]
  modifications: [history of changes]
  depletion: 0-100% (0 = pristine, 100 = dead)
}
```

### Player Impact Tracking

```
Per-Player Statistics:
- Total Qi consumed
- Formations deployed
- Environmental changes caused
- Resources harvested
- Creatures killed

Reputation:
- World-benefactor: Improved areas
- World-destroyer: Damaged areas
- Local-hero: Helped specific region
- Local-villain: Harmed specific region
```

### Recovery Systems

```
Natural Recovery:
- Rate: 1% per year (unmodified)
- Accelerated by: Life formations, spirit stones
- Stopped by: Death formations, pollution

Player-Assisted Recovery:
- Restoration formation: +10% recovery rate
- Planting spirit trees: +5% recovery rate
- Removing pollution: +20% recovery rate
- Heaven-tier intervention: Instant (with cost)
```

---

## Location-Specific Mechanics

### The Central Plain (Dusty Expanse)

```
Ambient: Metal dominant, Earth secondary
Hazards: Data dust (scratches artifacts)
Resources: Ancient ruins, scavenging

Formation Interactions:
- Metal formations: +20% efficiency
- Wood formations: -20% efficiency (hostile)
- Dust protection formation: Prevents artifact damage

Special:
- The Whispering Canyon: Audio loops
  - Formation opportunity: Echo communication
  - Danger: Infinite recursion feedback
```

### The Southern Volcanic Belt

```
Ambient: Fire extreme, Earth moderate
Hazards: Ash storms, ground instability
Resources: Fire crystals, sulfur, iron

Formation Interactions:
- Fire formations: +50% efficiency, unstable
- Water formations: -50% efficiency, evaporate
- Heat protection formation: Required

Special:
- Active volcanoes: Can tap for massive Fire Qi
- Risk: Eruption destroys formations
```

### The Eastern Forests

```
Ambient: Wood extreme, Water moderate
Hazards: Aggressive flora, spores
Resources: Spirit herbs, wood, beast cores

Formation Interactions:
- Wood formations: +30% efficiency
- Fire formations: Risk of forest fire (banned)
- Life formations: Boost growth

Special:
- The Living Tree: Can carve formations into it
- Root network: Natural ley lines
```

### The Northern Glaciers

```
Ambient: Water/Ice extreme, Wind moderate
Hazards: Cold, aurora interference
Resources: Ice crystals, preserved artifacts

Formation Interactions:
- Water/Ice formations: +40% efficiency
- Fire formations: -30% efficiency, melt ice
- Heat formation: Essential for survival

Special:
- Aurora Borealis: Transmits data
  - Formation opportunity: Communication network
- Frozen Fleet: Ancient ships (scavenge)
```

### The Western Archipelago

```
Ambient: Lightning extreme, Wind extreme
Hazards: Constant lightning, magnetic interference
Resources: Lightning crystals, storm glass

Formation Interactions:
- Lightning formations: +50% efficiency, lethal
- Metal formations: Attract lightning (danger)
- Grounding formation: Essential

Special:
- Magnetic anomalies: Compass fails, needs formation navigation
- Tomb of Li Qing: Legendary formation puzzle
```

---

## Glitch Zones (Broken Reality)

### What Are Glitches?

```
Origin: Remnants of the Great Calamity
Nature: Laws of physics broken locally
Persistence: Permanent (hard to fix)

Player Opportunities:
- Study glitches (gain insights)
- Harvest glitch materials (unique)
- Solve glitches (quest rewards)
- Exploit glitches (risky)

Player Dangers:
- Reality instability (death)
- Glitch beasts (aggressive)
- Corruption (permanent debuffs)
```

### Known Glitch Zones

#### The Whispering Canyon
```
Glitch Type: Audio loop
Effect: Sound repeats infinitely
Player Opportunity: Echo-based communication arrays
Danger: Infinite recursion (brain damage)
Fix: Heaven-tier sound cancellation formation
```

#### The Glass Sea
```
Glitch Type: Light refraction break
Effect: Light bends randomly
Player Opportunity: Laser weapons, invisibility
Danger: Spontaneous combustion (focused light)
Fix: Heaven-tier light restoration formation
```

#### The Null Wastes
```
Glitch Type: Qi void
Effect: No Qi exists
Player Opportunity: Test mortal-tech, Qi-free artifacts
Danger: Cultivators weaken, mortals unaffected
Fix: Cannot be fixed (permanent dead zone)
```

#### The Time Fractures
```
Glitch Type: Temporal loop
Effect: Areas repeat 5-second loop
Player Opportunity: Time-based puzzles
Danger: Get trapped in loop, age rapidly
Fix: Heaven-tier time stabilization formation
```

#### The Spatial Tears
```
Glitch Type: Dimensional breach
Effect: Space doesn't connect properly
Player Opportunity: Short-range teleport
Danger: Teleport into solid object, lost forever
Fix: Heaven-tier space mending formation
```

---

## Formation Puzzles

### Dungeon Formations

```
Ancient formations protect treasure
Players must solve to access

Puzzle Types:
1. Pattern Matching: Activate in correct sequence
2. Element Balancing: Supply correct elements
3. Timing: Activate at precise moments
4. Position: Stand on specific tiles
5. Logic: Solve runic programming challenge

Rewards:
- Ancient artifacts
- Formation blueprints
- Rare materials
- Knowledge (skill XP)

Difficulty Scales:
- Mortal: Simple patterns
- Spirit: Programming basics
- Earth: Complex logic
- Heaven: Optimization challenges
```

### Dynamic Formations

```
Some formations change over time:
- Rotation pattern (daily cycle)
- Seasonal activation
- Responsive to player actions
- Multi-stage puzzles

Example: The Seven-Star Formation
- Align with actual stars
- Changes position nightly
- Must solve within time limit
- Reward: Star map technique
```

---

## Player vs Environment

### Harvesting Resources

```
Natural Regeneration:
- Standard rate: 1% per day
- Formation boosted: 10% per day
- Player planted: 100% per harvest

Over-Harvesting:
- 80% depletion: Slows regeneration
- 90% depletion: Resource extinct locally
- 100% depletion: Dead zone (permanent)

Responsible Farming:
- Leave 20%: Sustainable
- Replant: Accelerates regeneration
- Formation care: Maximizes yield
```

### Beast Interactions

```
Wild Spirit Beasts:
- Roam based on Qi availability
- Attracted to formations (food source)
- Can be hunted (materials)
- Can be tamed (allies)

Formation Effects:
- Food formation: Attracts beasts (farmable)
- Defense formation: Deters beasts (safe area)
- Bait formation: Lures specific beasts

Beast Evolution:
- Low Qi: Weak beasts
- High Qi: Powerful bosses
- Glitch areas: Mutated beasts
```

### NPC Interactions

```
NPCs React to Formations:
- Benefit: NPCs move near (towns grow)
- Harm: NPCs avoid (ghost towns)
- Trade: NPCs pay for services
- Conflict: Compete for resources

Examples:
- Irrigation formation: Farmers settle nearby
- Defense formation: Merchants feel safe
- Industrial formation: Workers hired
- Experimental formation: Scholars study
```

---

## Multiplayer Interactions

### Territory Control

```
Formations Claim Territory:
- Overlap detection: Conflicting formations
- Strength contest: Stronger formation wins
- Diplomacy: Can share access

Sect Wars:
- Destroy enemy formations
- Capture territory
- Steal resources
- Defend your formations

Formation PvP:
- Direct battle: Formation vs formation
- Sabotage: Infiltrate and weaken
- Hacking: Redirect power
- Overload: Cause catastrophic failure
```

### Cooperative Formations

```
Multiple Players, One Formation:
- Shared design effort
- Split costs
- Combined power
- All operators can control

Example: Grand Sect Defense
- 100 disciples each contribute 1 node
- Combined: 100-node formation (Earth-tier power)
- Requires coordination
- Single weak point: Sabotage one node, all fail
```

### Formation Marketplace

```
Buy/Sell Formations:
- Purchase blueprint: Learn formation
- Hire formation: Temporary use
- Lease space: Allow others to use your formation

Formation Services:
- Charging station: Pay to recharge Qi
- Fast travel: Pay to teleport
- Training: Pay for XP buff
- Protection: Pay for defense
```

---

## Progression Integration

### Early Game (Mortal)

```
Player discovers:
- Artifacts: Basic utility items
- Formations: Don't exist yet
- Environment: Mostly static

Focus:
- Craft personal artifacts
- Learn circuit basics
- Survive in world
```

### Mid Game (Spirit)

```
Player unlocks:
- Formations: Small-scale deployment
- Ambient Qi: Can tap weak sources
- Environment: Minor influence

Focus:
- Deploy camp formations
- Establish small base
- Start affecting environment
```

### Late Game (Earth)

```
Player masters:
- Formations: Large-scale networks
- Ambient Qi: Major tapping
- Environment: Significant changes

Focus:
- Build sect headquarters
- Control territory
- Manage resources sustainably
```

### End Game (Heaven)

```
Player transcends:
- Formations: Reality-scale
- Ambient Qi: Can create sources
- Environment: Can reshape regions

Focus:
- Challenge Grand Formation
- Access Terminal of Origin
- Rewrite local laws of physics
```

---

## Open Questions

- [ ] How long does "long-term" environmental change take? (Game time vs real time)
- [ ] Should formations persist when player is offline?
- [ ] Can multiple players' formations overlap? (Co-op or conflict)
- [ ] Server limitations for persistent world state?
- [ ] How to handle abandoned formations?
- [ ] Should world have limited total Qi (zero-sum game)?
