#define MODE1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ChatServer
{
    enum DataType
    {
        GUID,
        EXITGAME,
        DATA,
        LOCALPLAYERJOINROOM,
        LOCALPLAYERJOINOK,
        PROXYREPORT,
        PROXYREPORTOK,
        ORDERPROXYREPORT,
        PING,
    }
    struct FDataPackage
    {
        public DataType MT;
        public string PayLoad;
        public FDataPackage(string s)
        {
            MT = DataType.GUID; 
            PayLoad = "";
        }
    }

    class Room<T>
    {
        static Dictionary<String, Room<T>> roommap = new Dictionary<string, Room<T>>();
        public static Room<T> JoinClientroom(String roomid, T mtc)
        {         
            bool b = roommap.ContainsKey(roomid);
            if (!b)
            {
                roommap.Add(roomid, new Room<T>(roomid));
            }
            Room<T> room;
            roommap.TryGetValue(roomid,out room);
            room.Add(mtc);
            return room;
        }
        public static T getclientfromroommap(string roomid,Func<T,bool> func)
        {
            Room<T> temproom;
            bool b = roommap.TryGetValue(roomid,out temproom);
            if (b)
            {
               return temproom.findmemberfromroom(func);               
            }
            return default;
        }
        public static Room<T> getroomfromroommap(String roomid)
        {
            Room<T> temproom;
            bool b = roommap.TryGetValue(roomid, out temproom);
            return temproom;
        }

        public readonly object memberchangeandsendLock = new object();
        public string roomid;
        List<T> mPeopleinroom;
        
        Thread RoomkillerThread;

        public Room(String roomid)
        {
            this.roomid= roomid;
            mPeopleinroom = new List<T>();

            RoomkillerThread = new Thread(new ThreadStart(shouldclosthisroom));
            RoomkillerThread.IsBackground = true;
            RoomkillerThread.Start();
        }
        ~Room()
        {

            Console.WriteLine("Room<T> In destructor.");
        }
        public void Add(T mtc)
        {
           bool b = mPeopleinroom.Contains(mtc);
            if (b)
            {

            }
            else
            { 
              mPeopleinroom.Add(mtc);
            }
        }
        public T findmemberfromroom(Func<T, bool> func)
        {
            foreach (T var in mPeopleinroom)
            {
                if (func(var))
                {
                    return var;
                }
            }
            return default;
        }
        public void Remove(T mtc)
        {
            mPeopleinroom.Remove(mtc);
            int len = mPeopleinroom.Count;
            Console.WriteLine("Room<T> :" + len.ToString());
        }
        public List<T> GetAllMember()
        {
            return mPeopleinroom;
        }

        void destroythisroom()
        {
            Room<T>.roommap.Remove(roomid);
            Console.WriteLine("listroom :"+ Room<T>.roommap.Count.ToString());
        }
        void shouldclosthisroom()
        {

            while (true)
            {
                if (mPeopleinroom.Count == 0)
                {
                    destroythisroom();
                    break;
                }
                Thread.Sleep(5000);
            }

        }
    }
}
