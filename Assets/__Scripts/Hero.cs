﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero    S; 

    [Header("Set in Inspector")]
    public float        speed = 30;
    public float        rollMult =-45;
    public float        pitchMult=30;
    public float        gameRestartDelay=2f;
    public GameObject   projectilePrefab;
    public float        projectileSpeed =40;

    [Header("Set Dynamically")]
    [SerializeField]

    private float       _shieldLevel =1;
    private GameObject  lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate    fireDelegate;

    void Awake() {
        if (S ==null){
            S=this;
        }else{
            Debug.LogError("Hero.Awake()-Attempted to assign second Hero.S!");
        }
        // fireDelegate+= TempFire;
    }
    // Start is called before the first frame update
 

    // Update is called once per frame
    void Update()
    {
        float xAxis =Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis*pitchMult,xAxis*rollMult,0);

        if(Input.GetAxis("Jump") == 1 && fireDelegate != null){
            fireDelegate();

       }
    }

    void TempFire(){
        GameObject projGO = Instantiate <GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    

        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    void OnTriggerEnter(Collider other) {
        
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // print("triggered: "+go.name);
        if(go == lastTriggerGo){
            return;
        }

        lastTriggerGo = go;

        if(go.tag =="Enemy"){
            shieldLevel--;
            Destroy(go);
        }else if (go.tag == "PowerUp"){
            AbsorbPowerUp(go);

            }else{
            print("Triggerd by non enemy : "+go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go){
        PowerUp pu = go.GetComponent<PowerUp>();
        switch(pu.type){

        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel{
        get{
            return(_shieldLevel);
        }
        set{
            _shieldLevel = Mathf.Min(value,4);
            if (value<0){
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
