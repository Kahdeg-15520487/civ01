# Artifact Materials & Balance System

> "The vessel shapes the flow. Choose poorly, and the vessel breaks."

## Overview

This document defines the material system, component costs, and trade-offs that prevent artifacts from becoming overpowered. Every material has strengths and weaknesses; every component has a cost.

---

## Material System

### Material Attributes

| Attribute | Description | Game Impact |
|-----------|-------------|-------------|
| **Conductivity** | Max Qi amplitude per tick | Limits circuit power |
| **Capacity** | Max complexity budget | Limits node count |
| **Affinity** | Elemental bias | Reduces cost for matched elements |
| **Durability** | Physical health | Damage threshold |
| **Heat Dissipation** | Cooling rate | Limits heat-generating circuits |
| **Weight** | Mass | Affects movement speed, stamina drain |
| **Poly Cost** | Geometry multiplier | Affects polygon budget |
| **Special** | Unique properties | Emergent behaviors |
| **Drawbacks** | Negative effects | Risk/reward trade-offs |

---

## Material Database

### Mortal Tier Materials

#### Spirit Stone (灵石)

The baseline material for all artifacts.

```
Conductivity: 15
Capacity: 20
Affinity: None
Durability: 50
Heat Dissipation: 10
Weight: 2.5 kg/L
Poly Cost Multiplier: 1.0x
Special: Passive Qi regeneration (+1 Qi/min)
Drawbacks: None

Availability: Common
Price: 100 spirit coins
Tier: Mortal
```

**Use Cases**: Heating stones, lighting, practice artifacts

---

#### Spirit Wood (灵木)

Living wood that conducts Qi naturally.

```
Conductivity: 10 (Low)
Capacity: 30 (High)
Affinity: Wood
Durability: 30 (Low)
Heat Dissipation: 5 (Very Low)
Weight: 0.8 kg/L (Very Light)
Poly Cost Multiplier: 0.8x
Special: Self-repair (regenerates 1 durability/hour)
Drawbacks: Flammable (Fire circuits deal 2x damage)

Availability: Common
Price: 80 spirit coins
Tier: Mortal
```

**Use Cases**: Training weapons, temporary artifacts, containers

---

### Spirit Tier Materials

#### Cold Iron (寒铁)

Standard metal for cultivator weapons.

```
Conductivity: 30
Capacity: 40
Affinity: Metal
Durability: 70
Heat Dissipation: 25 (High)
Weight: 7.8 kg/L (Heavy)
Poly Cost Multiplier: 1.2x
Special: Efficient heat conduction (+20% cooling)
Drawbacks: Heavy (-15% movement speed when equipped)

Availability: Uncommon
Price: 500 spirit coins
Tier: Spirit
```

**Use Cases**: Flying swords, defensive artifacts, standard weapons

---

#### Crimson Copper (赤铜)

Fire-aspected metal with amplifying properties.

```
Conductivity: 50 (High)
Capacity: 35
Affinity: Fire
Durability: 45
Heat Dissipation: 15
Weight: 8.9 kg/L (Very Heavy)
Poly Cost Multiplier: 1.3x
Special:
  - +20% Fire amplitude
  - +50% Fire circuit efficiency
Drawbacks:
  - Unstable: +15% backfire risk with Fire circuits
  - -10% efficiency for non-Fire elements
  - Melts at high temperature (80% heat)

Availability: Rare
Price: 1200 spirit coins
Tier: Spirit
```

**Use Cases**: Fire weapons, explosives, aggressive combat artifacts

---

#### Jade Essence (玉精)

Water-aspected stone with isolating properties.

```
Conductivity: 25
Capacity: 50 (High)
Affinity: Water
Durability: 60
Heat Dissipation: 8
Weight: 3.2 kg/L
Poly Cost Multiplier: 1.5x
Special:
  - Trace isolation: Ignores adjacent trace noise
  - +30% Water circuit efficiency
  - Stable: -50% Qi deviation risk
Drawbacks:
  - Brittle: Shatters on critical hit (instant break)
  - Cannot be repaired, only replaced
  - -20% Fire circuit efficiency

Availability: Rare
Price: 1500 spirit coins
Tier: Spirit
```

**Use Cases**: Precision artifacts, medical tools, defensive formations

---

### Earth Tier Materials

#### Mithril-Steel Alloy (秘银钢)

Advanced metallurgy combining lightness and strength.

```
Conductivity: 60
Capacity: 70
Affinity: Metal/Wood (Dual)
Durability: 85
Heat Dissipation: 35
Weight: 4.2 kg/L (Moderate)
Poly Cost Multiplier: 1.8x
Special:
  - Self-sharpening: Edge weapons maintain sharpness
  - +25% all Metal circuits
  - Can support dual-element circuits
Drawbacks:
  - Requires special forging (mini-game)
  - Cannot be melted down (permanent)
  - 10% chance of rejection during bonding

Availability: Epic
Price: 5000 spirit coins
Tier: Earth
Requirement: Forging Skill 50+
```

---

#### Void Crystal (虚晶)

Material from the boundary between realms.

```
Conductivity: 40
Capacity: 90 (Very High)
Affinity: None (Universal)
Durability: 55
Heat Dissipation: 20
Weight: 1.5 kg/L (Very Light)
Poly Cost Multiplier: 2.0x
Special:
  - Stores excess Qi (acts as buffer)
  - Can change affinity (takes 24 hours)
  - +50% storage capacity
Drawbacks:
  - Random Qi leakage (-5 Qi/min random)
  - Attracts void creatures when active
  - Cannot use stabilizers (interference)

Availability: Epic (Dungeon drop)
Price: 10,000 spirit coins
Tier: Earth
Requirement: Cultivation Level 200+
```

---

### Heaven Tier Materials

#### Star Steel (星钢)

Metal forged from meteorites in the upper atmosphere.

```
Conductivity: 81 (Extreme)
Capacity: 100 (Max)
Affinity: Lightning
Durability: 95
Heat Dissipation: 50 (Extreme)
Weight: 6.0 kg/L
Poly Cost Multiplier: 2.0x
Special:
  - Zero resistance: Lightning has no decay
  - Superconducts: +100% Lightning efficiency
  - Immune to Lightning damage
  - Absorbs ambient Lightning Qi
Drawbacks:
  - Grounds wielder: +50% lightning damage taken
  - Cannot use non-Lightning power sources
  - Requires constant Lightning Qi flow or becomes inert

Availability: Legendary
Price: 50,000 spirit coins
Tier: Heaven
Requirement: Cultivation Level 500+, Lightning Body
```

---

#### Dragon Bone (龙骨)

Biological material from ancient spirit beasts.

```
Conductivity: 70
Capacity: 85
Affinity: All (Adaptive)
Durability: 90
Heat Dissipation: 30
Weight: 3.5 kg/L
Poly Cost Multiplier: 2.5x
Special:
  - Adapts to user's cultivation style
  - Grows stronger with wielder (+1% stats per level)
  - Self-repairing (regenerates 10% durability/day)
  - Can host spirit consciousness
Drawbacks:
  - Requires blood bonding (permanent)
  - Transmits pain feedback (50% damage to wielder)
  - Will reject if wielder kills dragons

Availability: Legendary (Dragon hunt)
Price: Priceless
Tier: Heaven
Requirement: Dragon Slayer achievement
```

---

## Component Database

### Power Sources

#### Spirit Stone Socket

```
Type: Power Source
Poly Cost: 50
Capacity Impact: +5
Conductivity Impact: 0
Heat Generation: 0
Required: Yes (all artifacts need power)

Parameters:
  - slots: 1-5 (number of stones)
  - size: small/medium/large

Description: Accepts Spirit Stone fuel sources
Tier Available: Mortal+
Max Per Artifact: 1
```

---

#### Cultivator Link

```
Type: Power Source
Poly Cost: 200
Capacity Impact: +20
Conductivity Impact: +10
Heat Generation: 5
Required: No

Parameters:
  - bandwidth: 10-100 (Qi per tick)
  - range: touch/short/long

Description: Draws Qi directly from cultivator's reserves
Tier Available: Spirit+
Max Per Artifact: 1

Special: Can be used indefinitely but drains user's stamina
Drawback: User takes damage if Qi exceeds capacity
```

---

### Output Components

#### Effect Emitter

```
Type: Output
Poly Cost: 100
Capacity Impact: +5
Conductivity Impact: -5
Heat Generation: 10 (depends on effect)
Required: No (most artifacts need at least 1)

Parameters:
  - emission_type: determines effect
  - intensity: 1-100
  - range: 0.1-10 meters

Emission Types:
  - Sharpness: Weapon edge enhancement
  - Heat: Thermal damage
  - Light: Illumination
  - Force: Kinetic push
  - Shield: Protective barrier
  - Flight: Propulsion
  - etc.

Tier Available: Mortal+
Max Per Artifact: 4 (tier-dependent)
```

**Heat Generation by Type**:
- Sharpness: 5
- Heat: 20
- Light: 3
- Force: 15
- Shield: 25
- Flight: 30

---

#### Void Vent

```
Type: Output (Emergency)
Poly Cost: 150
Capacity Impact: +10
Conductivity Impact: 0
Heat Generation: -50 (removes heat)
Required: No

Description: Safely vents excess Qi to prevent explosion
Tier Available: Spirit+
Max Per Artifact: 2

Special: Prevents catastrophic failure
Drawback: 10% chance of drawing void creature attention
```

---

### Thermal Components

#### Heat Sink

```
Type: Thermal
Poly Cost: 80
Capacity Impact: +3
Conductivity Impact: 0
Heat Generation: -30 (removes heat)
Required: Conditional

Parameters:
  - capacity: 10-100 (heat dissipated per tick)
  - fin_count: 4-16 (more fins = better cooling)

Description: Passive thermal dissipation
Tier Available: Mortal+
Max Per Artifact: No limit (diminishing returns)

Balance: Each additional heat sink is 50% effective
```

---

#### Cooling Chamber

```
Type: Thermal (Modifier)
Poly Cost: 200
Capacity Impact: +10
Conductivity Impact: -5
Heat Generation: -60
Required: No

Description: Active cooling using Ice or Water Qi
Tier Available: Earth+
Max Per Artifact: 1

Special: Can convert heat to usable Water/Ice Qi
Drawback: Requires constant supply of Water/Ice Qi
```

---

### Stability Components

#### Stabilizer Node

```
Type: Modifier
Poly Cost: 120
Capacity Impact: +10
Conductivity Impact: -10
Heat Generation: 5
Required: No

Description: Reduces Qi deviation and backfire risk
Tier Available: Spirit+
Max Per Artifact: 3

Effect: -50% deviation chance per stabilizer
Balance: Diminishing returns (50%, 25%, 12.5%)
```

---

#### Grounding Rod

```
Type: Safety
Poly Cost: 60
Capacity Impact: 0
Conductivity Impact: +20 (Lightning only)
Heat Generation: 0
Required: Conditional (for Lightning circuits)

Description: Prevents Lightning feedback damage
Tier Available: Mortal+
Max Per Artifact: No limit

Special: Required for Lightning above amplitude 50
Drawback: Adds weight (+0.5 kg each)
```

---

### Flight Components

#### Gyro Stabilizer

```
Type: Flight Control
Poly Cost: 200
Capacity Impact: +15
Conductivity Impact: 0
Heat Generation: 15
Required: For flying artifacts

Description: Improves flight stability and maneuverability
Tier Available: Spirit+
Max Per Artifact: 2

Effect: +30% handling, -20% stamina drain
```

---

## Balance Calculations

### Artifact Complexity Score

```
Complexity = Base Node Cost + Component Costs + Material Multiplier

Base Node Cost:
- Source nodes: 10 points each
- Processor nodes: 15 points each
- Storage nodes: 20 points each
- Control nodes: 25 points each
- Output nodes: 30 points each

Component Costs:
- From component database (above)

Material Multiplier:
- Mortal: 1.0x
- Spirit: 1.5x
- Earth: 2.0x
- Heaven: 3.0x

Final Complexity = Sum × Material Multiplier
```

**Limits**:
- Mortal: Max 100 complexity
- Spirit: Max 500 complexity
- Earth: Max 2000 complexity
- Heaven: Unlimited

---

### Heat Balance

```
Net Heat = Heat Generated - Heat Dissipated

Heat Generated:
- Sum of all component heat generation
- Circuit heat based on amplitude (amplitude² / 100)

Heat Dissipated:
- Material base dissipation
- Sum of all cooling components

Thresholds:
- < 50%: Safe (green)
- 50-80%: Warning (yellow)
- 80-100%: Danger (red)
- > 100%: Failure (explosion/breakdown)
```

---

### Conductivity Check

```
Max Circuit Amplitude ≤ Material Conductivity

If exceeded:
- 110-150%: -20% efficiency, +50% heat
- 150-200%: -50% efficiency, +100% heat, 10% break chance/tick
- > 200%: Instant material failure
```

---

## Trade-off Examples

### Example 1: Flying Sword

**Option A: Speed Build**
```
Material: Crimson Copper (Fire affinity)
Components:
  - High-power emitter (flight)
  - No heat sink
  - No stabilizer

Stats:
  - Speed: Very Fast
  - Heat: Critical (85%)
  - Break Risk: 15% per combat
  - Efficiency: +20% (Fire)

Verdict: Glass cannon - devastating but fragile
```

**Option B: Balanced Build**
```
Material: Cold Iron (Metal affinity)
Components:
  - Medium-power emitter (flight)
  - 2x Heat Sink
  - 1x Stabilizer
  - 1x Gyro Stabilizer

Stats:
  - Speed: Fast
  - Heat: Safe (40%)
  - Break Risk: 1% per combat
  - Efficiency: Baseline

Verdict: Reliable daily driver
```

**Option C: Tank Build**
```
Material: Jade Essence (Water affinity)
Components:
  - Low-power emitter (flight)
  - 3x Heat Sink
  - 2x Stabilizer
  - Shield emitter

Stats:
  - Speed: Slow
  - Heat: Minimal (15%)
  - Break Risk: 0.1% per combat
  - Efficiency: -10% (non-Water)

Verdict: Defensive support weapon
```

---

### Example 2: Heating Stone

**Option A: Disposable**
```
Material: Spirit Wood (cheap)
Components:
  - Single socket
  - Basic emitter

Stats:
  - Heat Output: 50
  - Duration: 1 hour
  - Cost: 80 coins

Verdict: Practice/training use
```

**Option B: Reusable**
```
Material: Spirit Stone (baseline)
Components:
  - Triple socket
  - Quality emitter
  - Heat sink

Stats:
  - Heat Output: 80
  - Duration: 4 hours
  - Cost: 300 coins

Verdict: Household item
```

**Option C: Premium**
```
Material: Cold Iron (durable)
Components:
  - Cultivator Link (unlimited power)
  - Dual emitter
  - 2x Heat sink
  - Stabilizer

Stats:
  - Heat Output: 120
  - Duration: Unlimited (while user has Qi)
  - Cost: 2000 coins

Verdict: Sect equipment, master-level item
```

---

## Anti-Exploit Measures

### 1. Diminishing Returns

```
Multiple identical components:
- 1st: 100% effective
- 2nd: 50% effective
- 3rd: 25% effective
- 4th: 12.5% effective
- etc.

Prevents stacking unlimited emitters/heat sinks
```

### 2. Element Conflicts

```
Fire + Water without container = Explosion risk
Lightning without ground = Feedback damage
Metal + Wood affinity on same item = Efficiency penalty (-20% each)
```

### 3. Component Interference

```
Too many components in small space = Interference penalty
- Components within 5mm of each other: -10% efficiency each
- Traces too close: Qi noise (random amplitude fluctuation)
```

### 4. Material Incompatibility

```
Certain materials reject certain components:
- Organic materials (wood, bone) reject Metal components
- Fire materials reject Water cooling
- Lightning materials reject non-conductive mounts
```

### 5. Tier Gating

```
Components locked by tier:
- Mortal: Basic sockets, simple emitters
- Spirit: Amplifiers, transmuters, stabilizers
- Earth: Multi-element processors, cooling chambers
- Heaven: Spatial components, void tech
```

---

## Open Questions

- [ ] How to handle material degradation over time?
- [ ] Should materials have hidden attributes discovered through use?
- [ ] Player-crafted materials (alloys, composites)?
- [ ] Material fusion/sacrifice systems?
