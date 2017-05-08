/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
var distance = 10.0;
// the height we want the camera to be above the target
var height = 5.0;
// How much we 
var heightDamping = 2.0;
var rotationDamping = 3.0;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Smooth Follow")


function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;
	
	// Calculate the current rotation angles
	var wantedRotationAngle = target.eulerAngles.y;//以y轴旋转作为欧拉角度，除非你做盗梦空间那种游戏，在游戏中，一般y轴就是一指向天的竖轴
	var wantedHeight = target.position.y + height; //height是摄像机的高度，甚至runtime（游戏运行过程中）可调
		
	var currentRotationAngle = transform.eulerAngles.y;	//同上，唯一不同的是，上是目标的y轴，这里是摄像机的y轴，所以目的是后面比较2者角度的差异
	var currentHeight = transform.position.y;			//同上
	
	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	//js代码有些恶心，实际上currentRotationAngle 是float类型，所以Mathf.LerpAngle的输出是一个值，不是四元数，所以不懂四元数的同学还暂时不用操心
	//据博主所知， lerp一般是旋转的意思，unity3d里面还带动态的
	//博主也不懂英文，不知道歪国人current好wanted的意思，但是这么写，你应该更清楚LerpAngle是做什么用的了吧？
	//float angle= Mathf.LerpAngle(minAngle, maxAngle, Time.time);  
	//所以这个currentRotationAngle就是用来同步角度的（整个类是作用smoothfollow嘛，很好理解）
	
	// Damp the height
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	//同上，但这个是同步高度
	//明显地(很多书本都用这个词，其实很不明显，只是博主个人推测)，Mathf.Lerp是用于计算当前帧的高度（从currentHeigth到wantedHeight，当前时间3个参数）
	//所以明显的，Time.deltaTime是当前时间
	//LastUpdate这个方法熟悉unity3d机制，或者其他游戏机制都程序员都应该知道，LastUpdate时间一直在变化的，所以 Time.daltaTime = 当前时间这个也合理
	//但是博主在另外一个框架的camera脚本，类似这个smooth脚本，但是不是放在LastUpdate，也还是使用Time.deltaTime作为当前时间，也没什么大问题
	//所以这个Time.deltaTime可以广泛使用，但需要多留意
	//
	
	// Convert the angle into a rotation（balabala，在说什么捏）
	var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
	// Set the position of the camera on the x-z plane to:（balabala，在说什么捏）
	// distance meters behind the target
	transform.position = target.position;
	transform.position -= currentRotation * Vector3.forward * distance;
	//来了，来了，四元数来了。。。。。。。。。。。。。。。。。。。。。。
	//position减等于的理解，博主也不知道自己理解是不是对，反正就是按2D的矢量运算理解即可，好像一直没错
	//

	// Set the height of the camera（又是balabala，在说什么捏）
	transform.position.y = currentHeight;
	
	// Always look at the target
	transform.LookAt (target);
	//其实前面搞那么多都是花招，关键还是最后这个LookAt方法，(关键是Camera的定位，LookAt反而是小事)
	//camera.transform.LookAt(target)
	//也是unity3d内置的方法，所以还是unity3d这个框架叼，封装了不少有用的API
	//其实这个方法的实现也没有多难，就是把camera的前向量指向target，主要问题是人家好像玩儿似的，做3D的转换随便做
}