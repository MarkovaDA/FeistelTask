using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeistelNet
{
    class FeistelNetClassV3
    {
        private const int BLOCK_SIZE = 8;//8 байт = 64 бита
        private const int ROUNDS = 5;

        //шифрация блоков данных итеративно
        public static UInt64[] FeistelEncrypt(UInt64[] plainText)
        {
            var encryptedBlocks = new UInt64[plainText.Count()];
            for (int i = 0; i < plainText.Count(); i++)
            {   
                encryptedBlocks[i] = encryptBlock(plainText[i]);
            }
            return encryptedBlocks;
        }
        //дешифраця блоков данных итеративно
        public static UInt64[] FeistelDecrypt(UInt64[] encryptedText)
        {
            var decryptedBlocks = new UInt64[encryptedText.Count()];
            for (int i = 0; i < encryptedText.Count(); i++)
            {
                decryptedBlocks[i] = decryptBlock(encryptedText[i]);
            }
            return decryptedBlocks;
        }
        //шифрация блока данных
        public static UInt64 encryptBlock(UInt64 originalBlock)
        {
            byte[] bytes = GetBytes(originalBlock);
            //выделение левого блока
            UInt32 leftPart = ToUInt32(bytes.Take(4).ToArray());
            //выделение правого блока
            UInt32 rightPart = ToUInt32(bytes.Skip(4).Take(4).ToArray());
            //прогонка раундов
            for (int i = 0; i < ROUNDS; i++)
            {
                UInt32 f = RoundF(leftPart);
                UInt32 result = (UInt32)(f ^ rightPart);
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
            //слитие закодированных 32-битных блоков назад в 64-битных блок
            var cipherBytes = new byte[8]
        	{
            	BitConverter.GetBytes(leftPart)[3], BitConverter.GetBytes(leftPart)[2], BitConverter.GetBytes(leftPart)[1], BitConverter.GetBytes(leftPart)[0],
				BitConverter.GetBytes(rightPart)[3], BitConverter.GetBytes(rightPart)[2], BitConverter.GetBytes(rightPart)[1], BitConverter.GetBytes(rightPart)[0]
			};
            return ToUInt64(cipherBytes);
        }
        
        //дешифрация блока данных
        public static UInt64 decryptBlock(UInt64 originalBlock)
        {
            byte[] bytes = GetBytes(originalBlock);
            //выделение левого блока
            UInt32 leftPart = ToUInt32(bytes.Take(4).ToArray());
            //выделение правого блока
            UInt32 rightPart = ToUInt32(bytes.Skip(4).Take(4).ToArray());
            //прогонка раундов
            for (int i = ROUNDS - 1; i >=0; i--)
            {
                UInt32 f = RoundF(leftPart);
                UInt32 result = (UInt32)(f ^ rightPart);
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
            //слитие закодированных 32-битных блоков назад в 64-битных блок
            var cipherBytes = new byte[8]
        	{
            	BitConverter.GetBytes(leftPart)[3], BitConverter.GetBytes(leftPart)[2], BitConverter.GetBytes(leftPart)[1], BitConverter.GetBytes(leftPart)[0],
				BitConverter.GetBytes(rightPart)[3], BitConverter.GetBytes(rightPart)[2], BitConverter.GetBytes(rightPart)[1], BitConverter.GetBytes(rightPart)[0]
			};
            return ToUInt64(cipherBytes);
        }
        
        //разбитие строки на блоки данных длиною в 64 бит
        public static UInt64[] GetBlocks(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            var blocksCount = (int)Math.Ceiling(bytes.Count() / (double)BLOCK_SIZE);
            var result = new UInt64[blocksCount];
            for (int i = 0; i < blocksCount; i++)
            {
                result[i] = ToUInt64(bytes.Skip(i * BLOCK_SIZE).Take(BLOCK_SIZE).ToArray());
            }
            return result;
        }
        //набор байт переводим в 64-битное число
        public static UInt64 ToUInt64(byte[] bytes)
        {
            UInt64 result = 0;
            for (int i = 0; i < 8; i++)
            {
                result += (UInt64)Math.Pow(256, 8 - i) * bytes[i];
            }
            return result;
        }
        //набор байт переводим в 32-битное число
        public static UInt32 ToUInt32(byte[] bytes)
        {
            UInt32 result = 0;
            for (int i = 0; i < 4; i++)
            {
                result += (UInt32)Math.Pow(256, 3 - i) * bytes[i];
            }
            return result;
        }
        //набор байт переводим в 16-битное число
        public static UInt16 ToUInt16(byte[] bytes)
        {
            UInt16 result = 0;
            result += (ushort)(bytes[0] * 256 + bytes[1]);
            return result;
        }
        //воздействующая функция
        public static UInt32 RoundF(UInt32 originalBlock)
        {
            UInt32 result = (UInt32)(originalBlock ^ 65535);
            result = (UInt32)(result << 5);
            return result;
        }
        //число переводим в байты
        public static byte[] GetBytes(UInt64 originalBlock)
        {
            return BitConverter.GetBytes(originalBlock).Reverse().ToArray();
        }
    }
}
