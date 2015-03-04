﻿using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;
	Vector2 dest = Vector2.zero;
	Vector2 dir = Vector2.right;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public PointSprites points;

    public static int killstreak = 0;

	// script handles
	private GameGUINavigation GUINav;
	private GameManager GM;
    private ScoreManager SM;

	private bool deadPlaying = false;

	// Use this for initialization
	void Start () 
	{
		GM = GameObject.Find ("Game Manager").GetComponent<GameManager>();
	    SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
		GUINav = GameObject.Find ("UI Manager").GetComponent<GameGUINavigation>();
		dest = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		switch(GameManager.gameState)
		{
		case GameManager.GameState.Game:
			readInputAndMove();
			animate ();
			break;

		case GameManager.GameState.Dead:
			if(!deadPlaying)	
				StartCoroutine("PlayDeadAnimation");
			break;
		}

	
	}

	IEnumerator PlayDeadAnimation()
	{
		deadPlaying = true;
		GetComponent<Animator>().SetBool("Die", true);
		yield return new WaitForSeconds(1);
		GetComponent<Animator>().SetBool("Die", false);
		deadPlaying = false;

	    if (GameManager.lives <= 0)
	    {
            Debug.Log("Treshold for High Score: " + SM.LowestHigh());
	        if (GameManager.score >= SM.LowestHigh())
	            GUINav.getScoresMenu();
	        else
	            GUINav.H_ShowGameOverScreen();
	    }

		else			           
            GM.ResetScene();
	}

	void animate()
	{
		Vector2 dir = dest - (Vector2)transform.position;
		GetComponent<Animator>().SetFloat("DirX", dir.x);
		GetComponent<Animator>().SetFloat("DirY", dir.y);
	}

	bool valid(Vector2 dir)
	{
		// cast line from 'next to pacman' to pacman
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos+dir, pos);
		return hit.collider.name == "pacdot" || (hit.collider == collider2D);
	}

	public void resetDestination()
	{
		dest = new Vector2(15f, 11f);
		GetComponent<Animator>().SetFloat("DirX", 1);
		GetComponent<Animator>().SetFloat("DirY", 0);
	}

	void readInputAndMove()
	{
		// move closer to destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		rigidbody2D.MovePosition(p);
		
		// Check for Input if not moving
		if ((Vector2)transform.position == dest) {
			if (Input.GetAxis("Vertical") > 0 && valid(Vector2.up))
			{
				dest = (Vector2)transform.position + Vector2.up;
				dir = Vector2.up;
			}
			if (Input.GetAxis("Horizontal") > 0 && valid(Vector2.right))
			{
				dest = (Vector2)transform.position + Vector2.right;
				dir = Vector2.right;
			}
			if (Input.GetAxis("Vertical") < 0 && valid(-Vector2.up))
			{
				dest = (Vector2)transform.position - Vector2.up;
				dir = -Vector2.up;
			}
			if (Input.GetAxis("Horizontal") < 0 && valid(-Vector2.right))
			{
				dest = (Vector2)transform.position - Vector2.right;
				dir = -Vector2.right;
			}
		}
	}

	public Vector2 getDir()
	{
		return dir;
	}

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;
    
        Instantiate(points.pointSprites[killstreak-1], transform.position, Quaternion.identity);
        GameManager.score += (int)Mathf.Pow(2, killstreak)*100;

    }
}
