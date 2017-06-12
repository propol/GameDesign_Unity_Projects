using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

	// This static List holds all Boid instances and is shared amongst them
	static public List<Boid> boids;

	//Note: This code does NOT use a Rigidbody. It handles velocity directly
	public Vector3 velocity; //the current velocity
	public Vector3 newVelocity; //the velocity for next frame
	public Vector3 newPosition;//the position for next frame

	public List<Boid> neighbors; //all nearby Boids
	public List<Boid> collisionRisks; //all Boids that are too close
	public Boid closest; //the single closest Boid

	//Initialize this Boid on Awake()
	void Awake () {
		
		//Define the boids List if it still null
		if (boids == null) {
			boids = new List<Boid> ();
		}

		//add this Boid to boids
		boids.Add(this);

		//Give this Boid instance a random position and velocity
		Vector3 randPos=Random.insideUnitSphere*BoidSpawner.S.spawnRadius;
		randPos.y = 0; //flatten the boid to only move in the XZ plane
		this.transform.position=randPos;
		velocity = Random.onUnitSphere;
		velocity *= BoidSpawner.S.spawnVelocity;

		//Initialize the two Lists
		neighbors=new List<Boid>();
		collisionRisks=new List<Boid>();

		//make this.transform a child of the Boids GameObject
		this.transform.parent = GameObject.Find("Boids").transform;

		//give the Boid a random color, but make sure it's not too dark
		Color randColor=Color.black;
		while (randColor.r + randColor.g + randColor.b < 1.0f) {
			randColor = new Color (Random.value, Random.value, Random.value);
		}

		Renderer[] rends = gameObject.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in rends) {
			r.material.color = randColor;
		}

		
	}
	
	// Update is called once per frame
	void Update () {
		//get the list of nearby Boids (this Boid's neighbors)
		List<Boid> neighbors = GetNeighbors(this);

		//Initialize newVelocity and newPosition to the current values
		newVelocity=velocity;
		newPosition = this.transform.position;

		//velocity matching: this sets the velocity of the boid to be similar to that of its neighbors
		Vector3 neighborVel = GetAverageVelocity(neighbors);
		//utilizes the fields set on the BoidSpawner.S singleton
		newVelocity+= neighborVel*BoidSpawner.S.velocityMatchingAmt;

		//Flock Centering: move toward middle of neighbors
		Vector3 neighborCenterOffset=GetAveragePosition(neighbors) - this.transform.position;
		newVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;

		Vector3 dist;
		if (collisionRisks.Count > 0) {
			Vector3 collisionAveragePos = GetAveragePosition (collisionRisks);
			dist = collisionAveragePos - this.transform.position;
			newVelocity += dist * BoidSpawner.S.collisionAvoidanceAmt;
		}

		//Mouse Attraction - Move toward the mouse no matter how far away
		dist = BoidSpawner.S.mousePos-this.transform.position;
		if (dist.magnitude > BoidSpawner.S.mouseAvoidanceDist) {
			newVelocity += dist * BoidSpawner.S.mouseAttractionAmt;
		} else {
			//if the mouse is too close, move away quickly!
			newVelocity-=dist.normalized*BoidSpawner.S.mouseAvoidanceDist*BoidSpawner.S.mouseAvoidanceAmt;
		}

		//newVelocity and newPosition are ready but wait until LateUpdate()
		//to set them so that this Boid doesn't move before other have
		//had a chance to calculate their new values
	}

	//By allowing all Boids to Update() themselves before any boids move
	// we avoid race conditions that could be caused by some Boids
	// moving before other have decided where to go.

	void LateUpdate(){
		//Adjust the current velocity based on newVelocity using a linear interpolation (Appendix B)
		velocity=(1-BoidSpawner.S.velocityLerpAmt)*velocity+BoidSpawner.S.velocityLerpAmt*newVelocity;
		if (velocity.magnitude > BoidSpawner.S.maxVelocity) {
			velocity = velocity.normalized * BoidSpawner.S.maxVelocity;
		}
		if (velocity.magnitude < BoidSpawner.S.minVelocity) {
			velocity = velocity.normalized * BoidSpawner.S.minVelocity;
		}

		//decide on the newPosition
		newPosition= this.transform.position + velocity*Time.deltaTime;
		//keep everything in the XZ plane
		newPosition.y=0;
		//Look from the old position at the newPosition to orient the model
		this.transform.LookAt(newPosition);
		//actually move to the newPosition
		this.transform.position=newPosition;

	}

	//find which Boids are near enough to be considered neighbors
	//boi is BoidOfInterest, the Boid on which we are focusing

	public List<Boid> GetNeighbors(Boid boi){
		float closestDist = float.MaxValue;
		Vector3 delta;
		float dist;
		neighbors.Clear ();
		collisionRisks.Clear ();

		foreach (Boid b in boids) {
			if (b == boi)
				continue;
			delta = b.transform.position - boi.transform.position;
			dist = delta.magnitude;
			if (dist < closestDist) {
				closestDist = dist;
				closest = b;
			}
			if (dist < BoidSpawner.S.nearDist) {
				neighbors.Add (b);
			}
			if (dist < BoidSpawner.S.collisionDist) {
				collisionRisks.Add (b);
			}

		}
		if (neighbors.Count == 0) {
			neighbors.Add (closest);
		}
		return(neighbors);
	}

	//get the average position of the Boids in a List<Boid>
	public Vector3 GetAveragePosition(List<Boid> someBoids){
		Vector3 sum = Vector3.zero;
		foreach (Boid b  in someBoids) {
			sum += b.transform.position;
		}
		Vector3 center = sum / someBoids.Count;
		return(center);
	}

	//get the average velocity of the Boids in a List<Boid>
	public Vector3 GetAverageVelocity(List<Boid> someBoids){
		Vector3 sum = Vector3.zero;
		foreach (Boid b  in someBoids) {
			sum += b.velocity;
		}
		Vector3 avg = sum / someBoids.Count;
		return(avg);
	}



}
