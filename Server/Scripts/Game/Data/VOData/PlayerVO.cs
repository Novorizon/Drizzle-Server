using DataBase;
using Net;

namespace Game
{
    public class PlayerVO
    {
        public TCPClientState client;
        public int id;                                        // 
        public string name;                                     // 
        public string modelPath;                                     // 
        public int modelId;                                       // 

        public int gender;                                       // 等级
        public int level;                                // 

        public int attack;                                      // 
        public int defence;                                      // 

        public float speed;                                      // 
        public int health;                                       // 
        public int age;                                      // 

        public int strength;                         // 
        public int agility;                                  // 
        public int intelligence;                           // 


        public Protobuffer.Vector3 position;                                       // 等级
        public Protobuffer.Vector3 forward;                                // 
        public PlayerVO()
        {
           
        }


        public PlayerVO Clone()
        {
            PlayerVO newClone = new PlayerVO();
            return newClone;
        }
    }
}