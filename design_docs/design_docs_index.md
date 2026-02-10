# Design Documents Index

This directory contains all design specifications for the **Rune Engraver (铭文师)** project.

---

## Core Game Design

### [game_design_document.md](./game_design_document.md)
*Original game concept document*
- Game overview and vision
- Core gameplay loop
- Target audience
- Unique selling points

### [project_roadmap.md](./project_roadmap.md)
*Development timeline and milestones*
- Phase breakdown
- Feature priorities
- Target dates

---

## Programming & Language

### [runic_language_spec.md](./runic_language_spec.md)
*The Runic runic programming language*
- Syntax and semantics
- Node types and behaviors
- Example programs

### [runic_hdl_design.md](./runic_hdl_design.md)
*Runic HDL (Hardware Description Language) specification*
- Formation definitions
- Port system
- Synthesis to node graphs

### [rhdl_implementation_plan.md](./rhdl_implementation_plan.md)
*RunicHDL compiler architecture*
- Parser design
- Semantic analysis
- Code generation

---

## Artifact System (NEW - Session Documentation)

### [artifact_modeling_system.md](./artifact_modeling_system.md)
**OpenSCAD-based 3D artifact modeling**
- Progression levels (prefab stitching → custom CSG)
- OpenSCAD integration (binary bundling, CLI execution)
- Runtime mesh loading (OBJ parser)
- Prefab library structure
- Error handling and validation

### [artifact_blueprint_format.md](./artifact_blueprint_format.md)
**Complete blueprint file specification**
- JSON schema for artifacts
- Physical definition (OpenSCAD)
- Circuit definition (RHDL)
- Component placement and binding
- Validation rules
- Cache system

### [artifact_materials_balance.md](./artifact_materials_balance.md)
**Material stats and component balancing**
- Material database (all tiers)
- Component costs and limitations
- Balance calculations
- Anti-exploit measures
- Trade-off examples

---

## World & Systems (NEW - Session Documentation)

### [housing_system.md](./housing_system.md)
**Player housing and base building**
- Cave dwelling concept (cave as circuit)
- Housing tiers (Meditation Cave → Heaven Domain)
- Room types and functions
- Ambient Qi system
- Formation grid for buildings
- Defense and security

### [world_interaction_system.md](./world_interaction_system.md)
**How artifacts and formations affect the world**
- Artifacts vs Formations (portable vs stationary)
- Formation architecture and deployment
- Environmental interaction and modification
- World persistence and player impact
- Glitch zones (broken reality areas)
- Formation puzzles and dungeons
- Multiplayer territory control

---

## Lore & Setting

### [worldbuilding_bible.md](./worldbuilding_bible.md)
*Tianwen Continent setting*
- Historical timeline (5 eras)
- Regional gazetteer
- Sect and NPC rosters
- Philosophy of code (Dao)
- Slang and terminology

### [artifact_system_design.md](./artifact_system_design.md)
*Original artifact/formation system concept*
- Artifact anatomy
- Formation architecture
- Crafting loop
- Automation machinery

---

## Additional Design

### [quest_system_design.md](./quest_system_design.md)
*Quest and mission structure*

### [godot_implementation_plan.md](./godot_implementation_plan.md)
*Godot 4.x implementation details*

### [task.md](./task.md)
*Current development tasks*

---

## Design Session Summary (2025-01-16)

### Topics Discussed

1. **Artifact Modeling System**
   - OpenSCAD integration for 3D geometry
   - 5-level progression (prefabs → from scratch)
   - OBJ runtime loading in Godot
   - Bundling OpenSCAD binary (GPL compliance)

2. **Blueprint Format**
   - Unified JSON structure
   - Physical (OpenSCAD) + Circuit (RHDL) separation
   - Component binding system
   - Validation and caching

3. **Materials & Balance**
   - Material stats per tier
   - Component costs
   - Trade-off systems
   - Anti-exploit mechanisms

4. **Housing System**
   - Cave as formation concept
   - Tier progression
   - Room specializations
   - Ambient Qi collection

5. **World Interaction**
   - Artifacts (portable) vs Formations (stationary)
   - Environmental modification
   - World persistence
   - Glitch zones as puzzles

### Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| OpenSCAD via CLI (not linked) | Keeps game proprietary while using GPL tool |
| OBJ format for runtime | Native Godot support, simple parsing |
| Prefab-only progression (early) | Low barrier to entry, teaches concepts |
| Separate OpenSCAD + RHDL | Physical form independent of circuit logic |
| Material limitations | Prevent overpowered builds |
| Environmental persistence | Player actions have consequences |
| Cave-as-circuit | Unifies housing and core gameplay |

### Architecture Summary

```
Player Input
    ↓
┌─────────────────┬─────────────────┐
│   Visual Editor │   Text Editor   │
│   (prefabs)     │   (OpenSCAD)    │
└────────┬────────┴────────┬────────┘
         ↓                 ↓
    OpenSCAD         RHDL Compiler
    Compiler         (C# Internal)
         ↓                 ↓
      OBJ Mesh      Circuit Graph
         ↓                 ↓
         └───────┬─────────┘
                 ↓
         Godot Artifact Node
         (Mesh + Circuit + Stats)
```

---

## Next Steps

### Immediate
- [ ] Implement OpenSCAD bridge (C#)
- [ ] Create OBJ parser for Godot
- [ ] Design basic prefab library
- [ ] Implement material database

### Short-term
- [ ] Build visual editor UI (Godot)
- [ ] Create blueprint loader/saver
- [ ] Implement component visualizer
- [ ] Design first tier of materials

### Long-term
- [ ] Implement housing system
- [ ] Create world persistence
- [ ] Build formation deployment
- [ ] Design environmental interaction

---

## Open Questions Across All Systems

| System | Question | Priority |
|--------|----------|----------|
| Modeling | LOD generation method? | Medium |
| Materials | Hidden material attributes? | Low |
| Housing | Multiplayer co-op housing? | High |
| World | Real-time vs game-time for changes? | High |
| World | Server persistence limits? | High |
| All | Cross-server compatibility? | Medium |

---

## Document Conventions

### Naming
- All files use `snake_case.md`
- Descriptive names, no abbreviations
- System-specific prefixes for related docs

### Format
- Markdown for all documents
- Tables for structured data
- Code blocks for examples (C#, GDScript, OpenSCAD, RHDL)
- Section headers with `###` for h3

### Version Control
- Each document is standalone
- Edit history in git, not in file
- Major revisions: Create new file with `_v2` suffix

### Status Markers
- `(NEW)` - Created in design session
- `(UPDATED)` - Modified in design session
- `(DEPRECATED)` - Outdated, kept for reference

---

## Quick Reference

### Want to design an artifact?
→ Start with [artifact_modeling_system.md](./artifact_modeling_system.md)
→ Then [artifact_blueprint_format.md](./artifact_blueprint_format.md)
→ Check [artifact_materials_balance.md](./artifact_materials_balance.md) for stats

### Want to build a house?
→ Read [housing_system.md](./housing_system.md)

### Want to understand the world?
→ Start with [worldbuilding_bible.md](./worldbuilding_bible.md)
→ Then [world_interaction_system.md](./world_interaction_system.md)

### Want to implement a feature?
→ Check [godot_implementation_plan.md](./godot_implementation_plan.md)
→ Review relevant design docs

### Want to write Runic code?
→ [runic_language_spec.md](./runic_language_spec.md)
→ [runic_hdl_design.md](./runic_hdl_design.md)

---

*Last Updated: 2025-01-16 (Design Session)*
*Contributors: Claude Code & User*
