using System;
using System.Numerics;

namespace WebServer
{
    public class Player
    {
        public int ID;

        public string Username;

        public Vector3 Position;
        public Quaternion Rotation;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            ID = _id;
            Username = _username;
            Position = _spawnPosition;
            Rotation = Quaternion.Identity;
        }
    }
}
