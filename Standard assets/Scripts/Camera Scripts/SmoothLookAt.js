var target : Transform;
var damping = 6.0;
var smooth = true;

@script AddComponentMenu("Camera-Control/Smooth Look At")

function LateUpdate () {
	if (target) {
		if (smooth)
		{
			// Look at and dampen the rotation
			var rotation = Quaternion.LookRotation(target.position - transform.position);
			//没什么好说的，和LookAt类似
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
			//球面插值---------
			//首先，roatation = 并不是一个value赋值，也不是一个普通的引用赋值，而是4元数赋值，（能理解为什么面试题问题都很SB了吧）
			//所谓4元数赋值，博主也没搞清楚是什么，但是这个赋值代表的是一个旋转过程，也就是Camera（摄像机）的旋转过程
			//其次，球面插值，意识就是非线性插值，也非抛物线插值，整个旋转过程快慢就是和一个球面相对应
			//如果把Camera(摄像机)比作人的眼睛，球面插值，也就是物体从远处来，镜头转得慢，物体越靠近中间，镜头转动越快，物理从另一端远处离开，镜头又慢下来
			//球面插值就是实现了这么一个过程
		}
		else
		{
			// Just lookat
		    transform.LookAt(target);
		    //不做球面插值，直接LookAt
		}
	}
}

function Start () {
	// Make the rigid body not change rotation
   	if (rigidbody)
		rigidbody.freezeRotation = true;
}