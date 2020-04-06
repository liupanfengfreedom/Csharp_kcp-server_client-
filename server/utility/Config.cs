using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace ChatServer
{
    public class Config
    {
        public struct Configinfor
        {
            public String ipaddress;
            public int tccipport;
            public int ttcipport;

        }
        public Configinfor configinfor;
        public Config()
        {
            try {

                String ss = File.ReadAllText("./config.ini");
                configinfor =  JsonConvert.DeserializeObject<Configinfor>(ss);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
