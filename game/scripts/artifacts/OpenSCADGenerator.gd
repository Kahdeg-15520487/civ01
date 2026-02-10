class_name OpenSCADGenerator extends RefCounted

## Generates OpenSCAD script from placed ArtifactPrimitives.

func generate(placed_primitives: Dictionary) -> String:
	var script = ""
	var root_primitives = []

	# Separate root-level primitives from children
	for prim_id in placed_primitives:
		var entry = placed_primitives[prim_id]
		var prim = entry["data"] as ArtifactPrimitive
		if prim.parent_id.is_empty():
			root_primitives.append(prim)

	# Generate script for each root primitive
	for prim in root_primitives:
		script += _generate_primitive_tree(prim, placed_primitives, 0) + "\n"

	return script

func _generate_primitive_tree(prim: ArtifactPrimitive, placed_primitives: Dictionary, indent: int) -> String:
	var result = ""
	var indent_str = "".lpad(indent * 4, " ")

	# Find children of this primitive
	var children = []
	for prim_id in placed_primitives:
		var entry = placed_primitives[prim_id]
		var child_prim = entry["data"] as ArtifactPrimitive
		if child_prim.parent_id == prim.id:
			children.append(child_prim)

	if children.is_empty():
		# Simple primitive without children
		result += indent_str + _generate_single_primitive(prim)
	else:
		# Primitive with children - wrap in CSG operation
		var csg_name = prim.get_csg_op_name()
		result += indent_str + "%s() {\n" % csg_name
		result += indent_str + "    " + _generate_single_primitive(prim)

		# Recursively add children
		for child in children:
			result += _generate_primitive_tree(child, placed_primitives, indent + 1)

		result += indent_str + "}\n"

	return result

func _generate_single_primitive(prim: ArtifactPrimitive) -> String:
	var result = ""

	# Add transforms
	result += _generate_transforms(prim)

	# Generate primitive shape
	match prim.type:
		ArtifactPrimitive.PrimitiveType.CUBE:
			result += "cube([%s, %s, %s], center=true);\n" % [prim.size.x, prim.size.y, prim.size.z]
		ArtifactPrimitive.PrimitiveType.SPHERE:
			result += "sphere(r=%s);\n" % prim.radius
		ArtifactPrimitive.PrimitiveType.CYLINDER:
			result += "cylinder(h=%s, r=%s, center=true);\n" % [prim.height, prim.radius]
		ArtifactPrimitive.PrimitiveType.CONE:
			result += "cylinder(h=%s, r1=%s, r2=0, center=true);\n" % [prim.height, prim.radius]
		ArtifactPrimitive.PrimitiveType.TORUS:
			result += "rotate_extrude(angle=360) translate([%s, 0]) circle(r=%s);\n" % [prim.radius, prim.tube_radius]
		_:
			result += "cube([1, 1, 1], center=true); // Unknown type\n"

	return result

func _generate_transforms(prim: ArtifactPrimitive) -> String:
	var result = ""

	# Only add transforms if they're not default
	if prim.position != Vector3i.ZERO:
		result += "translate([%s, %s, %s]) " % [prim.position.x, prim.position.y, prim.position.z]

	if prim.rotation != Vector3.ZERO:
		result += "rotate([%s, %s, %s]) " % [prim.rotation.x, prim.rotation.y, prim.rotation.z]

	if prim.scale != Vector3.ONE:
		result += "scale([%s, %s, %s]) " % [prim.scale.x, prim.scale.y, prim.scale.z]

	return result

func _get_csg_op_name(op: ArtifactPrimitive.CSGOperation) -> String:
	match op:
		ArtifactPrimitive.CSGOperation.UNION: return "union"
		ArtifactPrimitive.CSGOperation.DIFFERENCE: return "difference"
		ArtifactPrimitive.CSGOperation.INTERSECTION: return "intersection"
		_: return "union"

## Generate a simple test script for verification
func generate_test_script() -> String:
	return """
union() {
    translate([5, 5, 0]) cube([10, 10, 2], center=true);
    translate([5, 5, 2]) sphere(r=1.5);
}
"""
