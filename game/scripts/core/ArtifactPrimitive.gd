class_name ArtifactPrimitive extends Resource

## Data model for 3D artifact primitives used in the Artifact Modeler.
## Represents a single 3D shape with transforms and CSG operation settings.

@export var id: String
@export var display_name: String
@export var description: String = ""

## Types of 3D primitives available
enum PrimitiveType {
	CUBE,
	SPHERE,
	CYLINDER,
	CONE,
	TORUS
}
@export var type: PrimitiveType = PrimitiveType.CUBE

## CSG operations for combining primitives
enum CSGOperation {
	UNION,		## Add to parent (green outline)
	DIFFERENCE,	## Subtract from parent (red outline)
	INTERSECTION ## Intersect with parent (blue outline)
}
@export var csg_op: CSGOperation = CSGOperation.UNION

## Transform in grid coordinates
@export var position: Vector3i = Vector3i.ZERO
@export var rotation: Vector3 = Vector3.ZERO  ## Euler angles in degrees
@export var scale: Vector3 = Vector3.ONE

## Primitive-specific parameters
@export var size: Vector3 = Vector3(2, 2, 2)  ## For cube
@export var radius: float = 1.0  ## For sphere, cylinder, cone, torus
@export var height: float = 2.0  ## For cylinder, cone
@export var tube_radius: float = 0.3  ## For torus (tube thickness)

## CSG relationship - empty string means root level (no parent)
## Otherwise, this primitive applies its CSG operation to the parent primitive
@export var parent_id: String = ""

## Color for visualization (can be overridden per instance)
@export var color: Color = Color(0.7, 0.7, 0.8)

## Get the string name of the primitive type
func get_type_name() -> String:
	match type:
		PrimitiveType.CUBE: return "cube"
		PrimitiveType.SPHERE: return "sphere"
		PrimitiveType.CYLINDER: return "cylinder"
		PrimitiveType.CONE: return "cone"
		PrimitiveType.TORUS: return "torus"
		_: return "unknown"

## Get the string name of the CSG operation
func get_csg_op_name() -> String:
	match csg_op:
		CSGOperation.UNION: return "union"
		CSGOperation.DIFFERENCE: return "difference"
		CSGOperation.INTERSECTION: return "intersection"
		_: return "union"

## Get the color for the CSG operation (for visualization)
func get_csg_color() -> Color:
	match csg_op:
		CSGOperation.UNION: return Color(0.2, 0.8, 0.2)  # Green
		CSGOperation.DIFFERENCE: return Color(0.8, 0.2, 0.2)  # Red
		CSGOperation.INTERSECTION: return Color(0.2, 0.4, 0.8)  # Blue
		_: return Color.WHITE

## Create a duplicate of this primitive with a new ID
func duplicate_with_id(new_id: String) -> ArtifactPrimitive:
	var dupe = duplicate(true)
	dupe.id = new_id
	return dupe

## Get a dictionary representation for serialization
func to_dict() -> Dictionary:
	return {
		"id": id,
		"display_name": display_name,
		"type": type,
		"csg_op": csg_op,
		"position": position,
		"rotation": rotation,
		"scale": scale,
		"size": size,
		"radius": radius,
		"height": height,
		"tube_radius": tube_radius,
		"parent_id": parent_id,
		"color": color
	}

## Load from dictionary representation
static func from_dict(data: Dictionary) -> ArtifactPrimitive:
	var prim = ArtifactPrimitive.new()
	prim.id = data.get("id", "")
	prim.display_name = data.get("display_name", "Unknown")
	prim.type = data.get("type", PrimitiveType.CUBE)
	prim.csg_op = data.get("csg_op", CSGOperation.UNION)
	prim.position = data.get("position", Vector3i.ZERO)
	prim.rotation = data.get("rotation", Vector3.ZERO)
	prim.scale = data.get("scale", Vector3.ONE)
	prim.size = data.get("size", Vector3(2, 2, 2))
	prim.radius = data.get("radius", 1.0)
	prim.height = data.get("height", 2.0)
	prim.tube_radius = data.get("tube_radius", 0.3)
	prim.parent_id = data.get("parent_id", "")
	prim.color = data.get("color", Color(0.7, 0.7, 0.8))
	return prim
