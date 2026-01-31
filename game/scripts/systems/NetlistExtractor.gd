class_name NetlistExtractor
extends RefCounted

## Analyzes the visual grid (runes and traces) to build a logical graph dictionary.

var grid_system: GridSystem

func _init(grid: GridSystem) -> void:
	grid_system = grid

## Main entry point. Returns the dictionary expected by SimulationManager.BuildGraph.
func extract(board_state: Dictionary, traces: Array[Dictionary]) -> Dictionary:
	print("NetlistExtractor: Starting extraction...")
	
	var nodes_list: Array = []
	var connections_list: Array = []
	
	# 1. Identify Nodes and Map Ports
	# Map: global_grid_pos -> { "node_id": str, "port_name": str }
	var port_map: Dictionary = {}
	
	# We need to map visual Runes to unique logical IDs
	# var visited_runes: Dictionary = {} # rune_instance_ref -> unique_id
	var rune_counter: int = 0
	
	for grid_pos in board_state:
		var entry = board_state[grid_pos]
		# Only process the "center" or "main" entry to avoid duplicates
		if entry.has("main_pos") and entry["main_pos"] != grid_pos:
			continue
			
		var rune_res = entry["rune"] as Rune
		# Unique ID generation
		rune_counter += 1
		var node_id = "%s_%d" % [rune_res.id, rune_counter]
		
		# Add to Nodes List
		# Mapped C# Type: Rune resource should have 'simulation_classes' (e.g. SpiritStoneSource)
		# We assume the Resource 'simulation_classes' property holds the Type name (e.g. "SpiritStoneSource")
		# Or we extract the last part of "Civ.Emulator.SpiritStoneSource"
		var sim_type = rune_res.simulation_classes.split(".")[-1]
		
		nodes_list.append({
			"id": node_id,
			"type": sim_type,
			"pos": grid_pos
		})
		
		# Map Ports
		# io_definition: { "out": Vector2i(1, 0) } relative to center
		if rune_res.io_definition:
			for port_name in rune_res.io_definition:
				var rel_pos = rune_res.io_definition[port_name]
				# Check rotation? (Not implemented yet, assuming 0 rotation)
				var abs_pos = grid_pos + rel_pos
				
				if not port_map.has(abs_pos):
					port_map[abs_pos] = []
				
				port_map[abs_pos].append({
					"node_id": node_id,
					"port_name": port_name
				})
				print("  - Port Mapped: %s.%s at %s" % [node_id, port_name, abs_pos])

	# 2. Build Trace Connectivity Graph (Adjacency List)
	# Graph Nodes = Grid Cells
	# Graph Edges = Trace segments
	var trace_adj: Dictionary = {} # grid_pos -> [neighbor_grid_pos]
	
	for t in traces:
		var visual = t["visual"]
		if not is_instance_valid(visual): continue
		
		var world_points = visual.line_2d.points
		if world_points.size() < 2: continue
		
		# Convert Polyline to grid segments
		for i in range(world_points.size() - 1):
			var p1_world = world_points[i]
			var p2_world = world_points[i + 1]
			var p1 = grid_system.world_to_grid(p1_world)
			var p2 = grid_system.world_to_grid(p2_world)
			
			# Add Edge p1-p2
			_add_adj(trace_adj, p1, p2)
			_add_adj(trace_adj, p2, p1)
			
			# Walk the line if it spans multiple cells (Manhattan assumed)
			# If traces are direct lines, intermediate cells are also conductive
			var cursor = p1
			var direction = sign(Vector2(p2 - p1)) # (1,0) or (0,1) etc
			
			while cursor != p2:
				var next = cursor + Vector2i(direction)
				_add_adj(trace_adj, cursor, next)
				_add_adj(trace_adj, next, cursor)
				cursor = next

	# 3. Resolve Nets (Connected Components)
	var visited_cells: Dictionary = {}
	var nets: Array = [] # Array of Arrays of grid_points
	
	for start_cell in trace_adj:
		if visited_cells.has(start_cell): continue
		
		# BFS to find component
		var component: Array = []
		var queue: Array = [start_cell]
		visited_cells[start_cell] = true
		
		while queue.size() > 0:
			var curr = queue.pop_front()
			component.append(curr)
			
			if trace_adj.has(curr):
				for neighbor in trace_adj[curr]:
					if not visited_cells.has(neighbor):
						visited_cells[neighbor] = true
						queue.push_back(neighbor)
		
		nets.append(component)

	# 4. Resolve Connections from Nets
	for net in nets:
		# Find all ports connected to this net
		var connected_ports: Array = [] # [ {node_id, port_name} ]
		
		for cell in net:
			if port_map.has(cell):
				connected_ports.append_array(port_map[cell])
		
		# Create connections between found ports
		# Note: RuneGraph is directed.
		# A Net might connect Output of A to Input of B, and Input of C.
		# We need to distinguish Source (Output) vs Target (Input) ports?
		# Actually SimulationManager/RuneGraph.Connect takes (Port, Port).
		# Typical flow: Output -> Input.
		# If we have [OutA, InB, InC], we create A->B and A->C.
		# If we have [OutA, OutB], that's a short circuit (Simulator handles logic or error).
		
		var sources: Array = []
		var targets: Array = []
		
		for p in connected_ports:
			# Heuristic: "out" in name implies output, "in" implies input
			if "out" in p["port_name"].to_lower():
				sources.append(p)
			else:
				targets.append(p)
				
		# Generate Connections
		for src in sources:
			for tgt in targets:
				# Avoid connecting a node to itself via same port (unlikely but possible)
				connections_list.append({
					"from_node": src["node_id"],
					"from_port": src["port_name"],
					"to_node": tgt["node_id"],
					"to_port": tgt["port_name"]
				})
				print("  - Connection Found: %s.%s -> %s.%s" %
					[src["node_id"], src["port_name"], tgt["node_id"], tgt["port_name"]])

	return {
		"nodes": nodes_list,
		"traces": connections_list
	}

func _add_adj(adj: Dictionary, p1: Vector2i, p2: Vector2i) -> void:
	if not adj.has(p1): adj[p1] = []
	if not adj[p1].has(p2): adj[p1].append(p2)
