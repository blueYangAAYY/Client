using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramework
{
    public class MsgMove : MsgBase
    {
        //坐标
        public int x = 0;
        public int y = 0;
        public int z = 0;

        public MsgMove()
        {
            protoName = "MsgMove";
        }
    }
}
