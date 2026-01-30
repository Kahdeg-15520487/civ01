# Runic Programming Language Specification

> Technical design for the visual language and internal representation

---

## 1. Overview

The runic programming language is a **visual, node-based system** inspired by SHENZHEN I/O's component model. Players connect **Rune Nodes** together with **Qi Channels** to create artifacts and formations.

### Design Goals
- Fully deterministic and reproducible execution
- Step-through debugging support
- Visual clarity matching the xinxia aesthetic
- Emergent complexity from simple primitives

---

## 2. Element System

### 2.1 Primary Elements (五行)

The five classical elements form the foundation of all Qi manipulation.

| Element | Chinese | Generates | Overcomes | Color |
|---------|---------|-----------|-----------|-------|
| Wood (木) | Mù | Fire | Earth | Green |
| Fire (火) | Huǒ | Earth | Metal | Red |
| Earth (土) | Tǔ | Metal | Water | Yellow |
| Metal (金) | Jīn | Water | Wood | White |
| Water (水) | Shuǐ | Wood | Fire | Blue |

**Generation Cycle (相生)**: Wood → Fire → Earth → Metal → Water → Wood
**Overcoming Cycle (相克)**: Wood → Earth → Water → Fire → Metal → Wood

### 2.2 Auxiliary Elements (辅元)

Two elements exist outside the five-element cycle but interact with them.

| Element | Chinese | Properties | Interactions |
|---------|---------|------------|--------------|
| **Lightning (雷)** | Léi | Speed, sudden discharge | **Amplifies** Fire (→Plasma); **Conducts** through Water (→Storm); **Attracted** to Metal (→Magnetism); **Shatters** Earth (→Quartz); **Burns** Wood (→Thorn Storm) |
| **Wind (风)** | Fēng | Movement, dispersion | **Fans** Fire (→Wildfire); **Erodes** Earth (→Dust); **Chills** Metal (→Blade Wind); **Diffuses** Water (→Mist); **Scatters** Wood (→Spore) |

### 2.3 Sub-Elements (衍生元素)

Created by combining elements. Sub-elements are **unstable** and only exist during active Qi flow.

#### Primary + Primary Combinations (10)

| Sub-Element | Chinese | Derivation | Base TTL | Decays To | Properties |
|-------------|---------|------------|----------|-----------|------------|
| **Charcoal (炭)** | Tàn | Wood + Fire | 4 | Fire | Slow burn, fuel source, absorption |
| **Fertile Soil (沃)** | Wò | Wood + Earth | 5 | Earth | Growth acceleration, nutrient rich |
| **Splinter (刺)** | Cì | Wood + Metal | 3 | Wood | Piercing, fragmentation |
| **Life Sap (液)** | Yè | Wood + Water | 4 | Water | Life essence, healing base |
| **Magma (熔)** | Róng | Fire + Earth | 4 | Earth | Transformation, melting, slow flow |
| **Slag (渣)** | Zhā | Fire + Metal | 3 | Metal | Reshaping, extreme heat forging |
| **Steam (蒸)** | Zhēng | Fire + Water | 3 | Water | Pressure, expansion, obscuring |
| **Ore (矿)** | Kuàng | Earth + Metal | 6 | Metal | Refinement potential, dense storage |
| **Mud (泥)** | Ní | Earth + Water | 5 | Earth | Binding, slowing, shaping |
| **Ice (冰)** | Bīng | Metal + Water | 5 | Water | Preservation, stillness, brittleness |

#### Primary + Auxiliary Combinations (10)

| Sub-Element | Chinese | Derivation | Base TTL | Decays To | Properties |
|-------------|---------|------------|----------|-----------|------------|
| **Spore (孢)** | Bāo | Wood + Wind | 2 | Wood | Spreading, seeding, dispersion |
| **Thorn Storm (荆)** | Jīng | Wood + Lightning | 2 | Wood | Rapid piercing, chain strikes |
| **Wildfire (焰)** | Yàn | Fire + Wind | 2 | Fire | Rapid spread, uncontrolled expansion |
| **Plasma (离)** | Lí | Fire + Lightning | 2 | Fire | Extreme heat, ionization |
| **Dust (尘)** | Chén | Earth + Wind | 2 | Earth | Dispersion, infiltration, erosion |
| **Quartz (晶)** | Jīng | Earth + Lightning | 4 | Earth | Energy storage, crystallization |
| **Blade Wind (刃)** | Rèn | Metal + Wind | 2 | Metal | Cutting currents, sharp dispersal |
| **Magnetism (磁)** | Cí | Metal + Lightning | 3 | Metal | Attraction, repulsion, field effects |
| **Mist (雾)** | Wù | Water + Wind | 3 | Water | Diffusion, concealment, cooling |
| **Storm (暴)** | Bào | Water + Lightning | 2 | Water | Chaotic discharge, area saturation |

#### Auxiliary + Auxiliary Combination (1)

| Sub-Element | Chinese | Derivation | Base TTL | Decays To | Properties |
|-------------|---------|------------|----------|-----------|------------|
| **Tempest (雷暴)** | Léibào | Lightning + Wind | 2 | Wind | Max chaos. **Special:** Long-range comms if tuned with Grounding Rod + Sky Antenna. |

#### Combination Matrix Reference

```
              │ Wood  │ Fire  │ Earth │ Metal │ Water │ Lightning │ Wind
──────────────┼───────┼───────┼───────┼───────┼───────┼───────────┼──────
Wood          │   -   │ Charcoal│Fertile│Splinter│ Sap  │Thorn Storm│ Spore
Fire          │   -   │   -   │ Magma │Molten │ Steam │  Plasma   │Wildfire
Earth         │   -   │   -   │   -   │  Ore  │  Mud  │  Quartz   │ Dust
Metal         │   -   │   -   │   -   │   -   │  Ice  │ Magnetism │Blade Wind
Water         │   -   │   -   │   -   │   -   │   -   │   Storm   │ Mist
Lightning     │   -   │   -   │   -   │   -   │   -   │     -     │Tempest
```

### 2.4 Qi Merging Rules

When Qi flows of the **same element** merge (multiple channels into one junction):
- **Magnitudes are additive**: 3 Wood + 5 Wood = 8 Wood
- **TTL takes the maximum**: Steam(TTL=2) + Steam(TTL=4) = Steam(TTL=4, magnitude combined)

When **different elements** merge:
- Requires a **Combiner (合流)** node
- May produce sub-elements or trigger generation/overcoming effects
- Incompatible elements without proper handling cause **Qi Deviation**

---

## 3. Visual Language

### 3.1 Rune Nodes (符文节点)

Each node:
- Has a **type** defining its behavior
- Has **input/output ports** with **amplitude requirements**
- Can contain internal **state**
- May have **configurable parameters**

**Port Timing:**
- **Input ports**: Contents are read at the **beginning** of the tick
- **Output ports**: Contents are cleared at the **end** of the tick
- **Connection rule**: Cannot connect output→input with incompatible amplitude configuration

```
┌─────────────────────┐
│   [Node Name]       │
│   节点名称           │
├─────────────────────┤
│ ●─ input_1 [3-10]  │  ← Input port with amplitude range
│ ●─ input_2 [5+]    │  ← Minimum amplitude requirement
├─────────────────────┤
│      [State]       │  ← Internal state display
├─────────────────────┤
│         output_1 ─● │
└─────────────────────┘
```

### 3.2 Qi Channels (气道)

Channels connecting nodes:
- Carry **typed Qi** with element and amplitude
- **Amplitude cap: 81** (maximum displayable/processable)
- Display element color-coding
- Show flow animation with amplitude visualization (thicker = higher amplitude)
- **Can merge** same-element flows (additive)
- Different elements cannot share a channel

### 3.3 Routing (气路)

Routing is physical and structural:
- **Merging**: Connecting multiple outputs to one input sums their amplitude (if same element).
- **Crossing**: Channels can cross without interacting (planar graph rules apply only if "layers" exist, otherwise visual crossing allowed).
- **Loops**: Creating feedback loops is a valid pattern for iteration or accumulation. There is no explicit "loop variable" - the flowing Qi itself is the state.

### 3.3 Canvas Regions

- **Input Region (左)**: Power sources (spirit stones, cultivator connection)
- **Work Region (中)**: Player's runic circuit
- **Output Region (右)**: Functional outputs (weapon strike, shield projection, etc.)

---

## 4. Node Types

### 4.1 Source Nodes (源节点) — Power Sources

Different artifacts require different power sources based on their intended use.

#### Spirit Stone Sources (灵石源)
For artifacts powered by consumable spirit stones.

| Node | Shape | Ports | Description |
|------|-------|-------|-------------|
| **Spirit Stone Socket (灵石槽)** | 2x2 | `→ out` | Holds 1 Stone. Stable flow. |
| **Stone Array (灵石阵)** | 3x3 | `→ out` | Holds up to 5 Stones. High amplitude. |

**Stone Grades & Sizes:**
| Grade | Amplitude | Base Duration | Size | Capacity Multiplier |
|-------|-----------|---------------|------|---------------------|
| Low (下品) | 1-3 | 100 cycles | Small (碎) | ×1 |
| Medium (中品) | 4-7 | 80 cycles | Standard (整) | ×5 |
| High (上品) | 8-12 | 60 cycles | Large (块) | ×20 |
| Supreme (极品) | 13-20 | 40 cycles | Massive (晶) | ×100 |

*Total Qi available = Amplitude × Base Duration × Capacity Multiplier*

#### Cultivator Interface (修士接口)
For artifacts powered by the user's personal Qi.

| Node | Ports | Description |
|------|-------|-------------|
| **Cultivator Link (灵脉接口)** | `→ out` | Variable amplitude based on user's power |
| **Tuned Resonator (调谐器)** | `in → out` | Calibrates to specific user's Qi signature |
| **Amplitude Regulator (幅度调节)** | `in → out` | Accepts range of amplitudes, normalizes output |

> **Design Note**: Cultivator-powered artifacts need amplitude tolerance. A Qi Deviation Realm cultivator might output amplitude 5-8, while a Nascent Soul might output 50-80. Artifacts must either be tuned to a specific user OR include regulation circuits.

#### Capacitor Systems (储能系统)
For artifacts requiring massive momentary output.

| Node | Ports | Description |
|------|-------|-------------|
| **Qi Capacitor (蓄灵器)** | `in → out, full` | Accumulates Qi over time, releases on command |
| **Burst Trigger (爆发触发)** | `capacitor, trigger → out` | Releases stored Qi instantly |

### 4.2 Sink Nodes (汇节点)

#### Functional Outputs
| Node | Ports | Description |
|------|-------|-------------|
| **Qi Receptacle (灵匣)** | `in →` | Validates output against expected (puzzle use) |
| **Effect Emitter (效果释放)** | `in →` | Converts Qi to external effect (heat, force, light) |

#### Void Drains & Side Effects

| Node | Ports | Side Effect | Description |
|------|-------|-------------|-------------|
| **Void Drain (虚渊)** | `in →` | Harmless | Safely dissipates excess Qi |
| **Heat Sink (散热器)** | `in →` | Harmless | Converts to ambient heat (warming effect) |
| **Grounding Rod (接地符)** | `in →` | Harmless | Disperses Lightning safely into earth |
| **Unstable Vent (泄灵口)** | `in →` | Harmful | Quick disposal but damages surroundings |
| **Backlash Node (反噬节点)** | `in →` | Harmful | Excess Qi reflects to user (self-damage) |
| **Corruption Seep (浊气渗)** | `in →` | Harmful | Pollutes local spiritual environment |

### 4.3 Container Nodes (容器节点)

### 4.3 Container Nodes (容器节点)

| Node | Shape | Ports | Description |
|------|-------|-------|-------------|
| **Spirit Vessel (灵器)** | 1x1 | `in → out` | Stores one Qi value |
| **Dual Vessel (双灵器)** | 1x2 | `in1, in2 → out` | Stores two values separately |
| **Elemental Pool (元池)** | 2x2 | `in → out` | Accumulates large Qi buffer |
| **Qi Capacitor (蓄灵器)** | 2x1 | `in → out, full` | Releases only when full |

### 4.4 Operation Nodes (运算节点)

| Node | Shape | Ports | Description |
|------|-------|-------|-------------|
| **Transmuter (化符)** | 1x1 | `in → out` | Converts element (follows generation cycle) |
| **Amplifier (增幅)** | 2x1 | `primary, catalyst → out` | **Generation Cycle**: `primary` generates `catalyst`. Mag = `primary` mag × `catalyst` mag. Condition: `primary` mag > `catalyst` mag. Result is `catalyst` element. |
| **Dampener (克制)** | 2x1 | `target, suppressor → out` | **Overcoming Cycle**: `suppressor` overcomes `target`. Mag = `target` mag - `suppressor` mag. Condition: `target` mag ≥ `suppressor` mag. Result is `target` element. |
| **Attenuator (减幅)** | 1x2 | `in → out, excess` | Reduces to target amplitude; excess goes to second port |
| **Splitter (分流)** | 1x1 | `in → out1, out2` | Divides magnitude between outputs |
| **Combiner (合流)** | 1x1 | `in1, in2 → out` | Merges elements → may create sub-element. **Constraint**: Inputs must have **equal magnitude**. Unequal = Qi Deviation. |
| **Attenuator (减幅)** | 1x2 | `in → out, excess` | Reduces amplitude by a factor (0.0-1.0). Excess Qi is ejected via second port (Conservation of Qi). |

> **Amplifier Example**: Input 5 Fire (primary) + 2 Earth (catalyst). Result = 10 Earth.
> **Dampener Example**: Input 8 Water (target) + 3 Earth (suppressor). Result = 5 Water.
> **Combiner Example**: Input 4 Water + 4 Fire = 8 Steam. (Input 4 Water + 3 Fire = Qi Deviation).

### 4.5 Control Nodes (控制节点)

#### Yin-Yang Gate (阴阳门)
Routes Qi flow based on a boolean condition.

```
         ┌──────────────┐
 in ────▶│              │───▶ true_out   (when cond is positive/Yang)
         │  Yin-Yang    │
cond ───▶│    Gate      │───▶ false_out  (when cond is negative/Yin)
         └──────────────┘
```

**Condition evaluation:**
- Magnitude > 0 = Yang = routes to `true_out`
- Magnitude 0 = Yin = routes to `false_out`
- (Runecrafting does not recognize negative Qi)
- Can also check element type with variant nodes

#### Threshold Gate (阈门)
Routes based on whether magnitude meets a threshold.

```
         ┌──────────────┐
         │  Threshold   │
 in ────▶│    Gate      │───▶ pass  (if magnitude ≥ threshold)
         │  [threshold] │───▶ block (if magnitude < threshold)
         └──────────────┘
```

**Parameters:**
- `threshold`: Minimum magnitude to pass
- Configurable per-node

#### Element Filter (元筛)
Routes based on element type matching.

```
         ┌──────────────┐
         │   Element    │
 in ────▶│    Filter    │───▶ match (if element matches configured type)
         │   [Fire]     │───▶ other (all non-matching elements)
         └──────────────┘
```

**Parameters:**
- `targetElement`: Element type to filter for
- Can chain filters for multiple element handling

> **Design Note**: Explicit "Loop Nodes" (Ascending/Descending Array) have been removed. Iteration is achieved by routing Qi output back to inputs, using Gates to control exit conditions, effectively creating "while" loops with circuit topology.

### 4.7 Interface Nodes (接口节点)

These nodes interface with the game's evaluation system or physical world.

| Node | Shape | Ports | Description |
|------|-------|-------|-------------|
| **Stable Emitter (稳释)** | 1x1 | `in → out` | Standard output for validation |
| **Overrun Emitter (奔溢)** | 2x2 | `in → out, feedback` | High capacity emitter. Risk of explosion. |
| **Fizzle Emitter (消散)** | 1x1 | `in → out` | Momentary pulse interface. |
| **Sky Antenna (通天针)** | 3x3 | `in → out, signal` | Massive wireless tower. Requires Tempest input. |

---

## 5. Execution Model

### 5.1 Cycle-Based Execution

1. **Propagate Phase**: Qi moves along edges, same-element flows merge additively
2. **Execute Phase**: Nodes with inputs meeting **amplitude requirements** execute
3. **Emit Phase**: Nodes push Qi to output ports
4. **Decay Phase**: Sub-elements decrement TTL; expired elements decay
5. **Check Phase**: Validate outputs, detect errors

### 5.2 Amplitude Requirements

- Each input port has an **amplitude requirement** (min, max, or range)
- Node executes only when **all required inputs have Qi with sufficient amplitude**
- Under-amplitude: Node blocks, Qi accumulates until threshold met
- Over-amplitude (if max specified): Excess may cause instability or overflow effects

### 5.3 Sub-Element Decay

Sub-elements decay over time. TTL management is critical for their use.

**Base Decay:**
- TTL decrements by 1 each cycle while in transit or storage
- At TTL = 0, element decays to its **dominant parent**
  - Steam → Water (water is denser)
  - Magma → Earth
  - Ice → Water
  - Plasma → Fire

**Decay Modifiers:**

| Modifier | Effect | Mechanism |
|----------|--------|-----------|
| **Stabilizer Node (稳定符)** | +2 TTL per pass | Reinforces sub-element bonds |
| **Cooling Chamber (冷却室)** | +50% TTL | Slows decay for cold-related sub-elements |
| **Heating Chamber (加热室)** | +50% TTL | Slows decay for heat-related sub-elements |
| **Catalyst Node (催化符)** | -2 TTL per pass | Accelerates decay (useful for controlled release) |
| **Void Proximity** | -1 TTL per cycle | Being near void drains destabilizes |
| **High Amplitude** | -1 TTL if amp > 10 | High power destabilizes complex elements |

**Decay Effects:**
- When sub-element decays, magnitude is **partially preserved** (80%)
- Released energy can trigger secondary effects
- Example: Plasma (mag: 10) → Fire (mag: 8) + Lightning burst

### 5.4 Debugging Features

- **Pause/Resume**: Stop execution at any cycle
- **Step**: Execute one cycle at a time
- **Breakpoints**: Pause when Qi reaches specific edge/node
- **Amplitude Monitor**: Display real-time amplitude at any point
- **Trace**: Highlight path a specific Qi value took
- **TTL Viewer**: Track sub-element remaining lifetime

---

## 6. Internal Representation

### 6.1 Graph Structure

```typescript
interface RuneGraph {
  nodes: Map<NodeId, RuneNode>;
  edges: Edge[];
  metadata: GraphMetadata;
}

interface RuneNode {
  id: NodeId;
  type: NodeType;
  position: { x: number; y: number };
  params: Record<string, ParamValue>;
  inputRequirements: Map<PortName, AmplitudeRequirement>;
  state: NodeState;
}

interface AmplitudeRequirement {
  min?: number;
  max?: number;
  elementType?: ElementType;
}

interface Edge {
  id: EdgeId;
  from: { nodeId: NodeId; port: PortName };
  to: { nodeId: NodeId; port: PortName };
  elementType: ElementType | null;
}
```

### 6.2 Qi Value Representation

```typescript
interface QiValue {
  element: ElementType;
  magnitude: number;          // Amplitude/strength
  stability: Stability;
  ttl?: number;               // For sub-elements
  origin?: NodeId;            // For tracing
}

enum ElementType {
  // Primary (stable)
  METAL = "金", WOOD = "木", WATER = "水", FIRE = "火", EARTH = "土",
  
  // Auxiliary (stable)
  LIGHTNING = "雷", WIND = "风",
  
  // Sub-elements (unstable)
  STEAM = "蒸", MAGMA = "熔", ICE = "冰", DUST = "尘", 
  PLASMA = "离", MIST = "雾",
  
  // Special
  VOID = "虚",
}
```

### 6.3 Execution State

```typescript
interface ExecutionState {
  cycle: number;
  nodeStates: Map<NodeId, NodeState>;
  pendingQi: Map<EdgeId, QiValue[]>;  // Multiple Qi can be in transit
  completed: boolean;
  error: ExecutionError | null;
}

interface NodeState {
  registers: QiValue[];
  active: boolean;
  waitingFor: PortName[];
  accumulatedInput: Map<PortName, QiValue>;  // For amplitude accumulation
}
```

---

## 7. Formation Nesting (阵中阵)

```typescript
interface FormationBlueprint {
  id: BlueprintId;
  name: string;
  graph: RuneGraph;
  exposedInputs: PortDefinition[];
  exposedOutputs: PortDefinition[];
  cost: ResourceCost;
}
```

When placed as a node:
- Appears as a single node with exposed ports
- Can be "opened" to view internal graph
- Execution expands inline

---

## 8. Example: Capacitor-Powered Strike

**Goal**: Accumulate Qi over 10 cycles, release as powerful burst.

```
┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
│ Spirit   │     │    Qi    │     │  Burst   │     │  Effect  │
│  Stone   │────▶│ Capacitor│────▶│ Trigger  │────▶│ Emitter  │
│  (Fire)  │     │          │     │          │     │ (Strike) │
│  amp: 5  │     │ cap: 50  │     │          │     │          │
└──────────┘     └──────────┘     └──────────┘     └──────────┘
                      ▲
                      │ full signal when 50 reached
```

---

## 9. Open Questions

- [ ] Complete sub-element combination matrix
- [ ] Interaction rules for Lightning/Wind with all primary elements
- [ ] Visual glyph design for node types
- [ ] Sound design for Qi flow
- [ ] Serialization format

---

*Specification Version: 0.2 — Refined with amplitude and decay systems*
