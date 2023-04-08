using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    ������ͨ���У����� byet[] ����
 */
namespace NetFramework
{
    public class ByteArray
    {
        //Ĭ�ϴ�С
        const int DEFAULT_SIZE = 1024;

        //��ʼ��С
        int initSize = 0;

        //������
        public byte[] bytes;

        //��дλ��
        public int readIndex = 0;
        public int writeIndex = 0;

        //����
        private int capacity = 0;

        //ʣ��ռ�
        public int Remain 
        { 
            get
            {
                return capacity - writeIndex;
            }
        }

        //���ݳ���
        public int Length
        {
            get 
            { 
                return writeIndex - readIndex; 
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="size">��С</param>
        public ByteArray(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIndex = 0;
            writeIndex = 0;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="defaultBytes">�ⲿbyte[]</param>
        public ByteArray(byte[] defaultBytes)
        {
            bytes = defaultBytes;
            capacity = defaultBytes.Length;
            initSize = defaultBytes.Length;
            readIndex = 0;
            writeIndex = defaultBytes.Length;
        }

        /// <summary>
        /// ��������ߴ�
        /// </summary>
        /// <param name="size"></param>
        public void ReSize(int size)
        {
            if (size < Length)
            {
                return;
            }

            if (size < initSize)
            {
                return;
            }

            int n = 1;

            while(n < size)
            {
                n = n * 2;
            }

            capacity = n;

            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIndex, newBytes, 0, writeIndex - readIndex);
            bytes = newBytes;
            writeIndex = Length;
            readIndex = 0;
        }

        /// <summary>
        /// ��鲢�ƶ�����
        /// </summary>
        public void CheckAndMoveBytes()
        {
            if (Length < 8)
            {
                MoveBytes();
            }
        }

        /// <summary>
        /// �ƶ�����
        /// </summary>
        private void MoveBytes()
        {
            Array.Copy(bytes, readIndex, bytes, 0, Length);
            writeIndex = Length;
            readIndex = 0;
        }

        /// <summary>
        /// д������
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Write(byte[] bs,int offset,int count)
        {
            if (Remain < count)
            {
                ReSize(Length + count);
            }

            Array.Copy(bs, offset, bytes,writeIndex,count);
            writeIndex += count;
            return count;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] bs,int offset,int count)
        {
            count = Math.Min(count, Length);
            Array.Copy(bytes, 0, bs, offset, count);
            readIndex += count;
            CheckAndMoveBytes();
            return count;
        }
        public Int16 Readintl6()
        {
            if (Length < 2) return 0;
            Int16 ret = (Int16)((bytes[1] << 8) | bytes[0]);
            readIndex += 2;
            CheckAndMoveBytes();
            return ret;
        }
            //��ȡInt32
            public Int32 Readint32()
            {
                if (Length < 4) return 0;
                Int32 ret = (Int32)((bytes[3] << 24)|
               (bytes[2] << 16)|
               (bytes[1] << 8)|
                bytes[0] ) ;
                readIndex += 4;
                CheckAndMoveBytes();
                return ret;
            }
        }
}

