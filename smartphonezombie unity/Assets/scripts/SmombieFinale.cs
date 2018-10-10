using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmombieFinale : MonoBehaviour {


    public GameObject dog;
    public GameObject friends;
    public GameObject nofriends;
    public TriggerChecker poinOfNoReturn;
    public TriggerChecker reachFinale;
    public bool dogIsFriend = false;
    public bool friendsPresent = true;
    public delegate void voidDelegate();
    public voidDelegate onReachedPointOfNoReturn;
    public voidDelegate onReachedFinale;
	// Use this for initialization
	void Start () {
        Reset();
	}

    /// <summary>
    /// reset the finale at gamestart
    /// </summary>
    public void Reset()
    {
        dogIsFriend = false;
        friendsPresent = true;
        poinOfNoReturn.onTrigger = reachedPointOfNoReturn;
        reachFinale.onTrigger = reachedFinale;
        update();
    }

    void reachedFinale()
    {
        onReachedFinale();
    }

    void reachedPointOfNoReturn()
    {
        onReachedPointOfNoReturn();
    }

    /// <summary>
    /// run when dog becomes new friend
    /// </summary>
    public void dogNewFriend()
    {
        dogIsFriend = true;
        update();
    }

    /// <summary>
    /// run when friends leave
    /// </summary>
    public void friendsLeave()
    {
        friendsPresent = false;
        update();
    }

     void update()
    {
        dog.SetActive(dogIsFriend);
        friends.SetActive(friendsPresent);
        nofriends.SetActive(!friendsPresent);
    }

    // Update is called once per frame
    private void Update ()
    {
        update();
    }
}
