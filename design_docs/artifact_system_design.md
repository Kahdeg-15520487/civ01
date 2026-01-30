# Artifact & Formation System Design

> Defines the physical "hardware" and "chassis" that host runic programs.

---

## 1. Overview

While **Runes** are the software (logic/flow), **Artifacts** and **Formations** are the hardware.
- **Artifacts (法宝)**: Portable, object-oriented devices. Single unified grid or assembly of parts.
- **Formations (阵法)**: Stationary, area-of-effect systems. Network of distinct nodes connected by ley lines.

---

## 2. Artifact Anatomy

An artifact is composed of **Materials**, **Structure**, and **Engraving Surface**.

### 2.1 Artifact Stats

| Stat | Chinese | Description | Determined By |
|------|---------|-------------|---------------|
| **Conductivity** | 导灵性 | Max Qi Amplitude allowed per tick without damaging the item. | Material Grade |
| **Capacity** | 承载力 | Max "Complexity Cost" of runes (Rune count / advanced nodes). | Surface Area / Size |
| **Affinity** | 相性 | Elemental bias. Reduces cost/decay for specific elements. | Material Type |
| **Durability** | 耐久度 | Physical health. Reduced by Overrunning Release or damage. | Material Hardness |

### 2.2 Materials (The PCB)

Materials dictate the base stats and physical limits.

| Material | Affinity | Conductivity | Heat Dissipation | Rigidity | Stability | Special Property |
|----------|----------|--------------|------------------|----------|-----------|------------------|
| **Spirit Wood** | Wood | Low (10) | Low | Low | High | Self-Healing (Passive repair) |
| **Cold Iron** | Metal | Med (30) | High | High | Low | Conducts Heat efficiently |
| **Crimson Copper**| Fire | High (50) | Med | Med | Unstable | Amplifies Fire amplitude |
| **Jade Essence** | Water | Med (25) | Low | High | V. High | Isolates interference |
| **Star Steel** | Lightning| Extreme (81)| V. High | Extreme | Med | Superconductor (0 resistance) |

*Conductivity = Max Amplitude. Heat Dissipation = Cooling rate. Rigidity = Physical durability.*

### 2.3 Structure: Parts & Assembly

Simple artifacts are one piece. Complex artifacts are assemblies.

**Example: Flying Sword**
1.  **Blade (Body)**: Main rune grid. High conductivity. Contains *Effect Emitters* (Sharpness).
2.  **Hilt (Core)**: Power interface. Contains *Spirit Stone Socket* or *Cultivator Link*.
3.  **Guard (Control)**: Logic processing. Contains *Yin-Yang Gates* for steering.

*The player engrave each part individually, then assembles them. Ports must align physically between parts.*

---

## 3. Formation Anatomy

Formations are **distributed systems**. They don't have a single grid but a set of **Nodes**.

### 3.1 Formation Architecture

- **Array Base (阵盘)**: The central controller (Server). Holds the main logic runes.
- **Flags/Pillars (阵旗/阵柱)**: Remote nodes. Act as Sources or Emitters.
- **Ley Lines (灵脉)**: Wireless connections between nodes. Distance matters.

### 3.2 Environmental Interaction

Formations draw from or affect the environment.
- **Ambient Qi:** Can pull Qi from the terrain (e.g., placing Water Formation in a lake provides passive Water Qi input).
- **Environment Modification:** A "Drought Formation" might output Heat to the environment, changing the biome over time.

---

## 4. The Crafting Loop

How the player builds an artifact.

### Phase 1: Material Processing (Refining)
*Mini-game or automated process.*
- Keep impurities low to increase **Conductivity**.
- Mix materials to adjust **Affinity**.

### Phase 2: Shaping (The Chassis)
- **Vector Editor**: Player draws the physical 2D shape of the component (e.g., blade outline).
- **Physics Simulation**:
  - **Center of Qi**: Components requiring constant Qi flow must physically touch the energy source (Handle/Core).
  - **Force Balance**: Emitters generating force must be balanced or the artifact will fly erratically.
- **Grids**: The drawn shape determines the available runic grid layout.

### Phase 3: Engraving (Circuit Layout)
Incorporates physical circuit design principles:
- **Trace Width**: Wider traces = higher Qi bandwidth (Amplitude) but consume more space.
- **Interference**: Traces too close together cause signal noise (Qi Deviation risk).
- **Vias (Through-Holes)**: Connect components or cross layers (requires drilling).
- **Thermal Management**: High-amp runes generate Heat. Must place **Heat Sinks** or leave air gaps for cooling.
- **Grounding**: Lightning-based runes must connect to a **Grounding Rod** component to prevent shocks.

### Phase 4: Assembly (Linking)
- Connect parts together.
- Verify that *Output Port A* aligns with *Input Port B* on the connecting part.
- Test the full system.

---

## 5. Artifact Classes/Tiers

| Tier | Name | Max Grid Size | PCB Features |
|------|------|---------------|--------------|
| **Mortal (凡)** | 5x5 | Single Layer | Basic traces, no vias. |
| **Spirit (灵)** | 10x10 | Dual Layer | Simple vias, basic grounding. |
| **Earth (地)** | Flexible | Multi-Layer (4) | Complex vias, interference shielding, flexible shapes. |
| **Heaven (天)** | Infinite | 3D Topology | Spatial folding, wireless internal links. |

---

## 7. Automation Machinery (Manufactories)

> "To forge a thousand swords requires one mold, not a thousand hammers."

Introduced in Chapter 2, these are large-scale stationary systems designed for mass production.

### 7.1 Architecture
- **Assembler Core**: The large grid (20x20+) where the "blueprint" rune is inscribed.
- **Input Hoppers**: Feed raw materials (Spirit Wood, Iron) into the system.
- **Conveyors / Levitation Arrays**: Move items between machines.
- **Output Bins**: Collect finished products.

### 7.2 Logic Difference
Unlike standard artifacts which process *Qi*, Automata process *Matter* using Qi as fuel.
- **Logic**: IF `Iron Ingot` detected AND `Qi > 50` THEN `Stamp Shape`.

## 8. Open Questions

- [ ] How do we visualize "Layered Grids" for Earth-tier items?
- [ ] Should assembling parts require a separate mini-game?
- [ ] Detail the "Ambient Qi" mechanic for formations.
- [ ] Define the specific "Instruction Set" for Matter processing vs Qi processing.

