#pragma strict

var speed = 6.0;
var jumpSpeed = 8.0;
var gravity = 20.0;

public var joystickPrefab : GameObject;

private var moveDirection = Vector3.zero;
private var grounded : boolean = false;
private var joystickLeft : Joystick;
private var joystickRight : Joystick;
private var joystickLeftGO : GameObject;
private var joystickRightGO : GameObject;

private var screenMovementSpace : Quaternion;
private var screenMovementForward : Vector3;
private var screenMovementRight : Vector3;

private var mainCamera : Camera;
private var mainCameraTransform : Transform;



private var sensitivityX : float = 2.5F;
private var sensitivityY : float = 2.5F;

private var minimumX : float = -360F;
private var maximumX : float = 360F;

private var minimumY : float = -60F;
private var maximumY : float = 60F;

private var rotationX : float = 0F;
private var rotationY : float = 0F;
private var originalRotation : Quaternion;

function Awake () {	

	mainCamera = Camera.main;
	mainCameraTransform = mainCamera.transform;

	#if UNITY_IPHONE || UNITY_ANDROID
	if (joystickPrefab) {
			// Create left joystick
			joystickLeftGO = Instantiate (joystickPrefab) as GameObject;
			joystickLeftGO.name = "Joystick Left";
			joystickLeft = joystickLeftGO.GetComponent.<Joystick> ();
			
			// Create right joystick
			joystickRightGO = Instantiate (joystickPrefab) as GameObject;
			joystickRightGO.name = "Joystick Right";
			joystickRight = joystickRightGO.GetComponent.<Joystick> ();			
		}
	#endif
}

function Start () {

#if UNITY_IPHONE || UNITY_ANDROID
		// Move to right side of screen
		var guiTex : GUITexture = joystickRightGO.GetComponent.<GUITexture> ();
		guiTex.pixelInset.x = Screen.width - guiTex.pixelInset.x - guiTex.pixelInset.width;	
	
		originalRotation = transform.localRotation;		
	#endif	

	if (rigidbody)
			rigidbody.freezeRotation = true;

	screenMovementSpace = Quaternion.Euler (0, mainCameraTransform.eulerAngles.y, 0);
	screenMovementForward = screenMovementSpace * Vector3.forward;
	screenMovementRight = screenMovementSpace * Vector3.right;	
}

function FixedUpdate() {
	if (grounded) {
		// We are grounded, so recalculate movedirection directly from axes
		#if UNITY_IPHONE || UNITY_ANDROID
		moveDirection = new Vector3(joystickLeft.position.x, 0, joystickLeft.position.y);//(joystickLeft.position.x * screenMovementRight) + (joystickLeft.position.y * screenMovementForward);
		if (moveDirection.sqrMagnitude > 1)
			moveDirection.Normalize();
		#elif UNITY_EDITOR
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		#endif
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		
		if (Input.GetButton ("Jump")) {
			moveDirection.y = jumpSpeed;
		}
	}
	
	#if UNITY_IPHONE || UNITY_ANDROID
	// Read the mouse input axis
		rotationX += joystickRight.position.x * sensitivityX;
		rotationY += joystickRight.position.y * sensitivityY;

		rotationX = ClampAngle (rotationX, minimumX, maximumX);
		rotationY = ClampAngle (rotationY, minimumY, maximumY);
		
		var xQuaternion : Quaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
		var yQuaternion : Quaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
		
		transform.localRotation = originalRotation * xQuaternion * yQuaternion;
	#endif

	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	var flags = controller.Move(moveDirection * Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
}

static function ClampAngle (angle:float, min:float, max:float) : float
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}

@script RequireComponent(CharacterController)