using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/*
 消息协议基类，所有协议类都继承自此类
 */
namespace NetFramework
{
    public class MsgBase
    {
        //协议名
        public string protoName = "";

        //编码,将对象转为json
        public static byte[] Encode(MsgBase msgBase)
        {
            string json = JsonConvert.SerializeObject(msgBase);
            return Encoding.UTF8.GetBytes(json);
        }

        //解码,将json转为对象
        public static MsgBase Decode(string protoNmae,byte[] bytes,int offset,int count)
        {
            string s = Encoding.UTF8.GetString(bytes, offset, count);
            MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s,Type.GetType(protoNmae));
            return msgBase;
        }

        //编码协议名
        public static byte[] EncodeNmae(MsgBase msgBase)
        {
            //将协议名转为 字节数组
            byte[] nameBytes = Encoding.UTF8.GetBytes(msgBase.protoName);

            //获取协议名的长度
            Int16 len = (Int16)nameBytes.Length;

            //申请 bytes 数组,因为需要使用 长度计数法，所以需要 2 字节来存储长度
            byte[] bytes = new byte[2 + len];

            //组装2字节的长度信息 将名字长度以小端编码的形式放置在 nameBytes 前面
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);

            //组装名字 bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);

            return bytes;
        }

        //解码协议名
        public static string DecodeName(byte[] bytes,int offset,out int count)
        {
            count = 0;

            //必须大于2字节
            if (offset + 2 > bytes.Length)
            {
                return "";
            }

            //读取长度
            Int16 len = (Int16)((bytes[offset + 1] << 8 ) | bytes[offset] );

            //解析
            count = 2 + len;
            string name = Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }
    }
}
