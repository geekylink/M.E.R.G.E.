using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBaseShip : BaseShip {
	public int squadId;
	public GameObject currTarget;
	
	// Use this for initialization
	public override void Start () {
		squadId = 0;
		base.Start();	
	}
	
	public override void Die() {
		SquadManager.S.RemoveEnemy (this, squadId);
		base.Die();		
	}
}
