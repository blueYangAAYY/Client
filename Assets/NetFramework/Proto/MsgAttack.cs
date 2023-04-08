using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramework
{
    public class MsgAttack : MsgBase
    {
        //描述
        public string desc = "127.0.0.1:6543";

        public MsgAttack()
        {
            protoName = "MsgAttack";
        }
    }
}
