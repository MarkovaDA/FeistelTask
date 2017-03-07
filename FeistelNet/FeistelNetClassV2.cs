using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeistelNet
{
    class FeistelNetClassV2
    {
        private const int BLOCK_SIZE = 4;
        private const UInt16 SECRET = 65535;
        private const int ROUNDS = 5;

        //шифрация каждог блока данных
        public static uint[] FeistelEncrypt(uint[] plainText)
        {
            var cipherBlocks = new uint[plainText.Count()];
            for (int i = 0; i < plainText.Count(); i++)
            {
                cipherBlocks[i] = encryptBlock(plainText[i]);
            }
            return cipherBlocks;
        }
        
        //дешифраця блоков данных
        public static uint[] FeistelDecrypt(uint[] encryptedText)
        {
            var cipherBlocks = new uint[encryptedText.Count()];
            for (int i = 0; i < encryptedText.Count(); i++)
            {
                cipherBlocks[i] = decryptBlock(encryptedText[i]);
            }
            return cipherBlocks;
        }
        
        //функция шифрации  блока данных
        public static uint encryptBlock(uint originalBlock)
        {
            var bytes = GetBytes(originalBlock);
            //выделение левого блока
            UInt16 leftPart = ToUInt16(bytes.Take(2).ToArray());
            //выделение правого блока
            UInt16 rightPart = ToUInt16(bytes.Skip(2).Take(2).ToArray());
            //прогонка раундов
            for (int i = 0; i < ROUNDS; i++)
            {
                var f = RoundF(leftPart);
                UInt16 result = (UInt16)(f ^ rightPart);
                if (i < ROUNDS - 1)
                {
                    rightPart = leftPart;
                    leftPart = result;
                }
                else
                {
                    rightPart = result;
                }
            }

            var cipherBytes = new byte[BLOCK_SIZE]
        	{
            	BitConverter.GetBytes(leftPart)[1], BitConverter.GetBytes(leftPart)[0],
				BitConverter.GetBytes(rightPart)[1],BitConverter.GetBytes(rightPart)[0]
			};
            return ToUInt32(cipherBytes);
        }

        //функция дешифрации данных
        public static uint decryptBlock(uint originalBlock)
        {
            var bytes = GetBytes(originalBlock);
            //выделение левого блока
            UInt16 leftPart = ToUInt16(bytes.Take(2).ToArray());
            //выделение правого блока
            UInt16 rightPart = ToUInt16(bytes.Skip(2).Take(2).ToArray());
            //прогонка раундов
            for (int i = ROUNDS - 1; i >= 0 ; i--)
            {
                var f = RoundF(leftPart);
                UInt16 result = (UInt16)(f ^ rightPart);
                if (i > 0)
                {
                    rightPart = leftPart;
                    leftPart = result;
                }
                else
                {
                    rightPart = result;
                }
            }
            var cipherBytes = new byte[BLOCK_SIZE]
        	{
            	BitConverter.GetBytes(leftPart)[1], BitConverter.GetBytes(leftPart)[0],
				BitConverter.GetBytes(rightPart)[1],BitConverter.GetBytes(rightPart)[0]
			};
            return ToUInt32(cipherBytes);
        }
        
        public static UInt16 RoundF(UInt16 originalBlock)
        {
            UInt16 result = (UInt16)(originalBlock ^ 65535);
            result = (UInt16)(result << 5);
            return result;
        }

        public static UInt16 ToUInt16(byte[] bytes)
        {
            UInt16 result = 0;
            result += (ushort)(bytes[0] * 256 + bytes[1]);
            return result;
        }
        
        public static UInt32 ToUInt32(byte[] bytes)
        {
            UInt32 result = 0;
            for (int i = 0; i < 4; i++)
            {
                result += (UInt32)Math.Pow(256, 3 - i) * bytes[i];
            }
            return result;
        }

        //выделение из строки блока данных
        public static uint[] GetBlocks(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            var blocksCount = (int)Math.Ceiling(bytes.Count() / (double)BLOCK_SIZE);
            var result = new uint[blocksCount];
            for (int i = 0; i < blocksCount; i++)
            {
                result[i] = ToUInt32(bytes.Skip(i * BLOCK_SIZE).Take(BLOCK_SIZE).ToArray());
            }
            return result;
        }

        public static byte[] GetBytes(UInt32 originalBlock)
        {
            return BitConverter.GetBytes(originalBlock).Reverse().ToArray();
        }
    }
}
