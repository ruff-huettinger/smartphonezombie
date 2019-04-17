﻿using UnityEngine;
using System.Collections;

public class BenjasMath : MonoBehaviour{

	

	public float restrict (float x, float limit_1, float limit_2){
		x = Mathf.Max(x, Mathf.Min(limit_1,limit_2));// x >= the lower limit
		x = Mathf.Min(x, Mathf.Max(limit_1,limit_2));// x <= the higer limit
		
		return x;
	}
	
	public Vector3 delayed_repositioning(Vector3 origin, Vector3 target, float delay){
		//delays the Movement from origin to target  to a maximum of "delay"
		
		if (delay >= 0) {
			target -= origin; //make target the vector from origin to target
					  
			
			if(target.magnitude > delay){
				return origin + delay * target.normalized;
			}
			else{
				return origin + target;
			}
		}
		return target;
	}

	public Vector3 VectorComponentProduct (Vector3 A ,Vector3 B )
	{
		A.x *= B.x;
		A.y *= B.y;
		A.z *= B.z;
		
		return A;
	}

	/*
       ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀        █▀▀▀  █     ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀ 
       ▀■■▄  █▀▄▀█  █  █  █  █    █          █■■   █     █  █  █■■█    █   
       ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █          █     █▄▄▄  ▀▄▄▀  █  █    █   
	*/


public class smoothFloat{

	float[] array;
	
	public smoothFloat(int iterations, float startValue){
		
		if (iterations>0)
		{
			array = new float[iterations];
			for(int i = 0; i< iterations; i++){
				array[i]=startValue;
			}
		}
	}
	
	public float smooth(float newValue)
	{
	//use tis function to get the value smoothed
	
	
		if(array!=null){
			int iMax = array.Length-1;
			float temp = newValue;
			for (int i=0; i<iMax; i++) {
				temp += array[i];
				array[i]=array[i+1];
			}
			temp += array[iMax];
			array[iMax]=newValue;
			return temp /(iMax+2);
		}
		
		return newValue;
	}
}

public class smoothFloatFaster{
	
	float currentValue;
	int iteration;
	
	public smoothFloatFaster(int iterations, float startValue){
		currentValue = startValue;
		iteration = iterations;
	}
	
	public float smooth(float newValue)
	{
		//use tis function to get the value smoothed
		
		
		if(iteration>0){
				currentValue += (newValue-currentValue)/iteration;
			return currentValue;
		}
		
		return newValue;
	}
}

/*
 ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀  █  █        █  █  █▀▀▀  ▄▀▀▄  ▀▀█▀▀  ▄▀▀▄  █▀▀▄ 
 ▀■■▄  █▀▄▀█  █  █  █  █    █    █■■█        █ █   █■■   █       █    █  █  █▀▀▄ 
 ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █    █  █         █    █▄▄▄  ▀▄▄▀    █    ▀▄▄▀  █  █ 
*/



public class smoothVector3{
	
	Vector3 currentValue;
	int iteration;
	
	public smoothVector3(int iterations, Vector3 startValue){
		currentValue = startValue;
		iteration = iterations;
	}
	
	public Vector3 smooth(Vector3 newValue)
	{
		//use tis function to get the value smoothed
		
		if(iteration>0){
			//currentValue = (currentValue*iteration+newValue)*1f/(iteration+1);
			currentValue += (newValue - currentValue)*1f/(iteration);
			return currentValue;
		}
		
		return newValue;
	}
}

/*
 ▄▀▀■  █▄ ▄█  ▄▀▀▄  ▄▀▀▄  ▀▀█▀▀  █  █        █▀▀▄  ▄▀▀▄  ▀▀█▀▀  ▄▀▀▄  ▀▀█▀▀  ▀█▀  ▄▀▀▄  █▄ █ 
 ▀■■▄  █▀▄▀█  █  █  █  █    █    █■■█        █▀▀▄  █  █    █    █■■█    █     █   █  █  █▀▄█ 
 ■▄▄▀  █ █ █  ▀▄▄▀  ▀▄▄▀    █    █  █        █  █  ▀▄▄▀    █    █  █    █    ▄█▄  ▀▄▄▀  █ ▀█ 
*/

	public class smoothAngle{
		
		float currentValue;
		int iteration;
		
		public smoothAngle(int iterations, float startValue){
			currentValue = startValue;
			iteration = iterations;
		}
		
		public float smooth(float newValue)
		{
			//use tis function to get the value smoothed
			
			if(iteration>0){
				currentValue += Mathf.DeltaAngle(currentValue,newValue)/iteration;
				return currentValue;
			}
			
			return newValue;
		}
	}
	
	public class smoothRotation{
		
		Vector3 currentValue;
		int iteration;
		
		public smoothRotation(int iterations, Vector3 startValue){
			currentValue = startValue;
			iteration = iterations;
		}
		
		public Vector3 smooth(Vector3 newValue)
		{
			//use tis function to get the value smoothed
			
			if(iteration>0){
				currentValue.x += Mathf.DeltaAngle(currentValue.x,newValue.x)*1/iteration;
				currentValue.y += Mathf.DeltaAngle(currentValue.y,newValue.y)*1/iteration;
				currentValue.z += Mathf.DeltaAngle(currentValue.z,newValue.z)*1/iteration;
				return currentValue;
			}
			
			return newValue;
		}
	}

	public static Vector3 angularLerp(Vector3 from, Vector3 to, float t)
	{
		return new Vector3(	Mathf.LerpAngle(from.x,to.x,t),
							Mathf.LerpAngle(from.y,to.y,t),
							Mathf.LerpAngle(from.z,to.z,t));
	}

	public static float smooth01(float t)
	{
		return Mathf.Clamp01(Mathf.SmoothStep(0,1,t));
	}

	public static float easeInOut(float t)
	{
        //uses cosinus shape between, input 0..1, output 0..1
        //return Mathf.Clamp01((1-Mathf.Cos(t*Mathf.PI))/2);
        t = Mathf.Clamp01(t);
        return Mathf.InverseLerp(1,-1, Mathf.Cos(t*Mathf.PI));
	}

    public static float easeIn(float t)
    {
        t = Mathf.Clamp01(t);
        return Mathf.Sin(t * Mathf.PI/2);
    }

    public static float easeOut(float t)
    {
        t = Mathf.Clamp01(t);
        return  1- Mathf.Cos(t * Mathf.PI/2);
    }

    public int[] randomIntArray(int min, int max, int count)
	{
		if(count>max-min)
		{
			Debug.LogError("randomIntArray Error, count of elements is bigger than Interval: "+count+">"+max+"-"+min+", just gave back array from min to max-1");
			count = max-min-1;
		}

		int[] ints = new int[count];
		for(int i=0;i<ints.Length;i++)
		{
			ints[i]=Random.Range(min,max-(count-i)+1);
			min=ints[i]+1;
			Debug.Log("randomInt count: "+count+" int nr "+i +" "+ints[i]);
		}
		return ints;
	}

    public static void randomizeArray(int[] Array)
    {
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < Array.Length; t++)
            {
                int tmp = Array[t];
                int r = Random.Range(t, Array.Length);
                Array[t] = Array[r];
                Array[r] = tmp;
            }
        }

    }

    public static int[] intArray(int min, int maxPlusOne,bool randomized=false)
    {
        int[] newArray = new int[maxPlusOne - min];
        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = i + min;
        }
        if (randomized) randomizeArray(newArray);
        return newArray;
    }

    public static int[] repeatArray(int[] Array, int newLength)
    {
        if (newLength < 1)
            return null;
        int[] newArray = new int[newLength];
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        int i = 0;
        for (int t = 0; t < newArray.Length; t++)
        {
            newArray[t] = Array[i];
            i++;
            if (i >= Array.Length)
                i = 0;
        }
        return newArray;
    }

    public static void randomizeArray(GameObject[] Array)
	{
	    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
			for (int t = 0; t < Array.Length; t++ )
	        {
					GameObject tmp = Array[t];
					int r = Random.Range(t, Array.Length);
					Array[t] = Array[r];
					Array[r] = tmp;
	        }
	    }
		
	}

    public static int cycle(int value, int max, int min = 0)
    {
        if(value==max) return min;
        return value++;
    }

    public static int cycle(ref int value, int max, int min = 0)
    {
        value++;
        if (value > max) value = min;
        else if (value < min) value = min;
        return value;
    }

    public static void randomizeArray(ArrayList Array)
    {
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < Array.Count; t++)
            {
                var tmp = Array[t];
                int r = Random.Range(t, Array.Count);
                Array[t] = Array[r];
                Array[r] = tmp;
            }
        }
    }

    static public GameObject[] cleanUpArray(GameObject[] Array)
	{
		
		int j=0;
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i]!=null)
			{
				if(j<i)
				{
					Array[j]=Array[i];
					Array[i]=null;
				}
				j++;
			}
		}
		GameObject[] Output = new GameObject[j];
		for(int i = 0;i<Output.Length;i++)
		{
			Output[i]=Array[i];
		}
		return Output;
	}

    public static float map(float value, float valMin, float valMax, float outMin, float OutMax, bool clamp = false)
    {
        value = Mathf.InverseLerp(valMin, valMax, value);
        if (clamp) value = Mathf.Clamp01(value);
        return Mathf.Lerp(outMin, OutMax, value);
    }

    public static float map(float value, float minIn, float maxIn, float minOut, float maxOut, float minClamp, float maxClamp)
    {
        return Mathf.Clamp(map(value, minIn, maxIn, minOut, maxOut, false), minClamp, maxClamp);
    }

    public static float keepAngle0to360(float Angle)
    {
        while(Angle<0)
        { Angle += 360; }
        while (Angle > 360)
        { Angle -= 360; }
        return Angle;
    }

    public static float keepAngleBetween(float Angle, float Min, float Max)
    {
        float temp = Mathf.Lerp(Min, Max, 0.5f);
        while (Angle < Min-360)
        { Angle += 360; }
        while (Angle >  Max + 360)
        { Angle -= 360; }
        return Angle;
    }

    public static Vector3 ClosestPointToIntersect( Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2){

		float a = Vector3.Dot(lineVec1, lineVec1);
		float b = Vector3.Dot(lineVec1, lineVec2);
		float e = Vector3.Dot(lineVec2, lineVec2);
 
		float d = a*e - b*b;
 
		
		if(d == 0.0f)
		{
			//lines are parallel, return center between line points
			return (linePoint1+linePoint2)*0.5f;
		}
		else
		{
 
			Vector3 r = linePoint1 - linePoint2;
			float c = Vector3.Dot(lineVec1, r);
			float f = Vector3.Dot(lineVec2, r);
 
			float s = (b*f - c*e) / d;
			float t = (a*f - c*b) / d;
 
			return ((linePoint1 + lineVec1 * s)+(linePoint2 + lineVec2 * t))*0.5f;
		}
	}

    /// <summary>
    /// use ref for currentTime, will be reduced by time since last frame
    /// returns true when zero is reached, false if not
    /// </summary>
    /// <param name="currentTime"></param>
    /// <returns>true when countdown at zero</returns>
    public static bool countdownToZero(ref float currentTime)
    {
        if (currentTime == 0)
        {
            //make it fast if it's done, no uneccessary calculations
            return true;
        }
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            //clamp it to minimum of 0
            currentTime = 0;
            return true;
        }
        // keep going
        return false;
    }

    /// <summary>
    /// use ref for currentTime, will be changed
    /// </summary>
    /// <param name="currentTime"></param>
    /// <returns>float between 0 and 1 </returns>
    public static float timer(ref float currentTime, float maxTime, bool pausing = false)
    {
        if(!pausing) currentTime = Mathf.Min(currentTime + Time.deltaTime,maxTime);
        return Mathf.InverseLerp(0f, maxTime, currentTime);
    }

}