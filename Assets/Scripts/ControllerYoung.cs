﻿using UnityEngine;
using System.Collections;

public class ControllerYoung : MonoBehaviour
{
    public Sprite[] brother;
    private Color32 lineColor;
    private float distance,
                  lineWidth,
                  moveSpeed,
                  movementH,
                  movementV;
    private GameObject oldBrother;
    private int temp;
    private LineRenderer lr;

    private Animator anim;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
    private SpriteRenderer sr;

    private bool isCrying = false;

    private int patate;

    void Start()
    {
		anim = GetComponent<Animator>();
        anim.SetLayerWeight(0, 1f);
        boxCollider = GetComponent <BoxCollider2D> ();
		rb2D = GetComponent <Rigidbody2D> ();
        sr = GetComponent<SpriteRenderer>();

		oldBrother = Player.instance.GetComponentInChildren<Controller>().gameObject; //TEMP!!
        moveSpeed = 5f;
        temp = 0;
        lr = GetComponent<LineRenderer>();
        lineColor.a = 0;
        lineColor.b = 0;
        lineWidth = 0.2f;
        StartCoroutine(testLine());
    }

    void Update()
    {
        distance = getDistance();
        anim.SetInteger("Dir", patate);
        
        if (distance < 4)
        {
            isCrying = false;
            movementH = Input.GetAxis("HorizontalYoung");
            movementV = Input.GetAxis("VerticalYoung");
            transform.Translate(Vector2.right * moveSpeed * Input.GetAxis("HorizontalYoung") * Time.deltaTime);
            transform.Translate(Vector3.down * moveSpeed * Input.GetAxis("VerticalYoung") * Time.deltaTime);
            if (Mathf.Abs(movementV) < 0.75f)
            {
                if (movementH < 0)
                    patate = 4;
                else if (movementH > 0)
                    patate = 3;
            }
            else
            {
                if (movementV > 0)
                    patate = 1;
                else
                    patate = 2;
            }
        }
        else if (!isCrying)
        {
            movementH = 0;
            movementV = 0;
            cry ();
            Debug.Log("CRY");
            isCrying = true;
            StartCoroutine(helpMeSister());
        }

        if (movementH == 0 && movementV == 0)
        {
            if (isCrying)
                patate = 5;
            else
                patate = 0;
        }
        
        

        lr.SetPosition(0, this.transform.position);
        lr.SetPosition(1, new Vector3(oldBrother.transform.position.x, oldBrother.transform.position.y, oldBrother.transform.position.z));
    }

    float getDistance()
    {
        return Mathf.Sqrt(Mathf.Abs(oldBrother.transform.position.x - this.transform.position.x) + Mathf.Abs(oldBrother.transform.position.y - this.transform.position.y));
    }

    IEnumerator testLine()
    {
        while(true)
        {
            if (temp % 6 < 3)
                lineWidth += 0.02f;
            else
                lineWidth -= 0.02f;
            temp++;

            if (distance > 0 && distance < 3)
                lineColor.a = 0;
            else if (distance > 3 && distance < 4)
                lineColor.a = (byte)((distance - 3) * 230);
            
            lineColor.r = 255;

            lr.SetWidth(lineWidth, lineWidth);

            lr.SetColors(lineColor, lineColor);
            yield return new WaitForSeconds(0.05f);
        }
    }

	private void OnTriggerEnter2D (Collider2D patate)
	{
		Player player = GetComponentInParent<Player>();
		if (patate.tag == "Zone") {
			if (Player.instance.CurrentRoom == null) {
				Player.instance.CurrentRoom = patate.GetComponent<Room> ();
				Player.instance.showRoomName ();
			}
			Debug.Log ("Entering " + Player.instance.CurrentRoom.nameOfRoom());
			patate.GetComponent<Room> ().ControllersInside++;
		} else if (patate.tag == "Note") {
			Debug.Log ("Note");
			player.Note++; 
			patate.gameObject.GetComponent<AudioSource> ().Play();
			Destroy (patate.gameObject, patate.gameObject.GetComponent<AudioSource> ().clip.length);
		} else if (patate.tag == "Apple") {
			Debug.Log ("Pomme");
			player.Health += 10; 
			patate.gameObject.GetComponent<AudioSource> ().Play();
			Destroy (patate.gameObject, patate.gameObject.GetComponent<AudioSource> ().clip.length);
		}
	}

	private void OnTriggerExit2D (Collider2D patate)
	{
		if (patate.tag == "Zone") {
			if (Player.instance.CurrentRoom != null) {
				Debug.Log ("Exiting " + Player.instance.CurrentRoom.nameOfRoom ());
				Player.instance.CurrentRoom = null;
			}
			patate.GetComponent<Room> ().ControllersInside--;
		}
	}

	void cry () {
        AudioSource audio = GetComponent<AudioSource> ();
		if (!audio.isPlaying) {
			audio.Play ();
		}
	}

    IEnumerator helpMeSister()
    {
        GameObject instanciatedObject = (GameObject) Instantiate(Resources.Load("HelpMeSis"), new Vector3(this.transform.position.x, this.transform.position.y + 3, this.transform.position.z), Quaternion.identity);
        yield return new WaitWhile(() => isCrying);
        Destroy(instanciatedObject);
    }
		
}
