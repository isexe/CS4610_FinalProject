using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DungeonGenerator : MonoBehaviour
{
    public struct Map {
        private GameObject room;
        private GameObject leftWall;
        private GameObject rightWall;
        private GameObject topWall;
        private GameObject bottomWall;

        public Map(GameObject room, GameObject leftWall, GameObject rightWall, GameObject topWall, GameObject bottomWall){
            this.room = room;
            this.leftWall = leftWall;
            this.rightWall = rightWall;
            this.topWall = topWall;
            this.bottomWall = bottomWall;
        }

        public GameObject Room{
            get{ return room;}
            set{ room = value;}
        }

        public GameObject LeftWall{
            get{ return leftWall;}
            set{ leftWall = value;}
        }

        public GameObject RightWall{
            get{ return rightWall;}
            set{ rightWall = value;}
        }

        public GameObject TopWall{
            get{ return topWall;}
            set{ topWall = value;}
        }

        public GameObject BottomWall{
            get{ return bottomWall;}
            set{ bottomWall = value;}
        }
    }

    public List<Map> map;

    // Start is called before the first frame update
    void Start(){
        map = new List<Map>();
        Map room = new Map(null, null, null, null, null);
    }


    // Update is called once per frame
    void Update()
    {
        foreach (Map area in map)
        {
            if(area.Room != null){
                // TODO Add room generation, any wall with null needs to be random room
            }
        }
    }
}
