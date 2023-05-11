using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCarController : MonoBehaviour
{
	
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque;
	public float maxSteeringAngle;
	
	public static MyCarController cc;
	
	public float carMaxSpeed = 100;
	public float carCurrentSpeed = 0;
	
	public float brakeTorque = 1000;
	
	Rigidbody rb;
	
	bool isbraking;
	float arrowHorizontal;
	float arrowVertical;
	int goingLR;
	
	
	public void Start(){
		cc = this;
		rb = GetComponent<Rigidbody>();
		arrowHorizontal = 0f;
		arrowVertical = 0f;
		goingLR = 0;
	}
	
	public void ApplyLocalPositionToVisuals(WheelCollider collider){
		if(collider.transform.childCount == 0){
			return;
		}
		Transform visualWheel = collider.transform.GetChild(0);
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

    public void FixedUpdate()
    {
		
		if(goingLR == 1){
			if(arrowHorizontal < 1f)
				arrowHorizontal+= 2f * Time.deltaTime;
		}else if(goingLR == -1){
			if(arrowHorizontal > -1f)
				arrowHorizontal-= 2f * Time.deltaTime;
		}else if(goingLR == 0){
			if(arrowHorizontal > 0){
				arrowHorizontal-= 2f * Time.deltaTime;
			}else if(arrowHorizontal < 0){
				arrowHorizontal+= 2f * Time.deltaTime;
			}
		}
		
        float motor = maxMotorTorque * (Input.GetAxis("Vertical") + arrowVertical);
		float steering = maxSteeringAngle * (Input.GetAxis("Horizontal") + arrowHorizontal);
		
		if(isbraking)
			motor = 0;
		
		foreach(AxleInfo axleInfo in axleInfos){
			if(axleInfo.steering){
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if(axleInfo.motor){
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
				
				carCurrentSpeed = (rb.velocity.magnitude * 3.6f) / carMaxSpeed;
			}
			
			if(isbraking){
				axleInfo.leftWheel.brakeTorque = brakeTorque;
				axleInfo.rightWheel.brakeTorque = brakeTorque;
			}else{
				axleInfo.leftWheel.brakeTorque = 0;
				axleInfo.rightWheel.brakeTorque = 0;
			}
			
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
			
		}
    }
	
	public void CarIsBraking(bool isit){
		if(isit){
			isbraking = true;
		}else{
			isbraking = false;
		}
	}
	
	public void ArrowPressedVertical(float arVal){
		arrowVertical = arVal;
	}
	
	public void GoingLeftRight(int arVal){
		goingLR = arVal;
	}
}

[System.Serializable]
public class AxleInfo{
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}