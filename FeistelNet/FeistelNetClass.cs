using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeistelNet
{   //учесть влияние и генерацию ключа в функции
    class FeistelNetClass
    {   //КЛЮЧ 64 БИТА, ИСПОЛЬЗУЮТСЯ ТОЛЬКО 32 БИТА, БЛОКИ по 16 бит (Продумать размеры блоков по 32 бита)
        private const int BLOCK_SIZE = 8;//8 байт = 64 бита
        private const int ROUNDS = 5;
        private const UInt64 startKey = 12345;//начальный 64-битовый ключ
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
            //два блока длиною по 32 бита
            UInt32 leftPart = ToUInt32(bytes.Take(4).ToArray());
            UInt32 rightPart = ToUInt32(bytes.Skip(4).Take(4).ToArray());
            //каждый из блоков еще разбиваем на два подблока длиною в 16 бит
            UInt16 onePart = ToUInt16(GetBytes(leftPart).Take(2).ToArray());
            UInt16 twoPart = ToUInt16(GetBytes(leftPart).Skip(2).Take(2).ToArray());
            UInt16 threePart = ToUInt16(GetBytes(rightPart).Take(2).ToArray());
            UInt16 fourPart = ToUInt16(GetBytes(rightPart).Skip(2).Take(2).ToArray());
            //прогонка раундов
            for (int i = 0; i < ROUNDS; i++)
            {   UInt32 roundKey =  ror64(startKey, i*8);
                UInt16 f = RoundF(onePart, roundKey);
                if (i < ROUNDS - 1)
                {
                    fourPart = onePart;
                    onePart = (UInt16)(f ^ twoPart);
                    twoPart = (UInt16)(f ^ threePart);
                    threePart = (UInt16)(f ^ fourPart);
                }
                else
                {
                    twoPart ^=f;
                    threePart ^= f;
                    fourPart ^= f;
                }
            }
            //в leftPart сливаем onePart и twoPart
            //в rightPart сливаем threePart и fourPart
            var leftPartBytes = new byte[4]{
                BitConverter.GetBytes(onePart)[1], BitConverter.GetBytes(onePart)[0],
                BitConverter.GetBytes(twoPart)[1], BitConverter.GetBytes(twoPart)[0]
            };
            leftPart = ToUInt32(leftPartBytes);
            var rightPartBytes = new byte[4]{
                BitConverter.GetBytes(threePart)[1], BitConverter.GetBytes(threePart)[0],
                BitConverter.GetBytes(fourPart)[1], BitConverter.GetBytes(fourPart)[0]
            };
            rightPart = ToUInt32(rightPartBytes);
            //слитие закодированных 32-битных блоков назад в 64-битных блок
            var cipherBytes = new byte[8]
        	{
            	BitConverter.GetBytes(leftPart)[3], BitConverter.GetBytes(leftPart)[2], BitConverter.GetBytes(leftPart)[1], BitConverter.GetBytes(leftPart)[0],
				BitConverter.GetBytes(rightPart)[3], BitConverter.GetBytes(rightPart)[2], BitConverter.GetBytes(rightPart)[1], BitConverter.GetBytes(rightPart)[0]
			};
            return ToUInt64(cipherBytes);
        }
        //объединение блоков длиною 16 бит в один блок 32 бита
        private static UInt32 unionUint16_Blocks(UInt16 left, UInt16 right) 
        {   
            UInt16[] parts = {left, right};
            var unitedBytes = new byte[4];
            int index = 0; int length = unitedBytes.Length;
            int number = 2;
            for (int i = 0; i < length; i++) {
                if (i % length == 2)
                {
                    ++index; number = 2;
                }
                unitedBytes[i] = BitConverter.GetBytes(parts[index])[--number];
            }
            return ToUInt32(unitedBytes);
        }        
        //объединение блоков длиною 32 бит в один блок 64 бита
        private static UInt64 unionUint32_Blocks(UInt32 left, UInt32 right)
        {
            UInt32[] parts = {left, right};
            var unitedBytes = new byte[8];
            int index = 0; int length = unitedBytes.Length;
            int number = 4;
            for (int i = 0; i < length; i++)
            {
                if (i % length == 4) {
                    ++index; number = 4;
                }
                unitedBytes[i] = BitConverter.GetBytes(parts[index])[--number];
            }
            return ToUInt64(unitedBytes);
        }        
        //дешифрация блока данных
        public static UInt64 decryptBlock(UInt64 originalBlock)
        {
            byte[] bytes = GetBytes(originalBlock);
            UInt32 leftPart = ToUInt32(bytes.Take(4).ToArray());
            UInt32 rightPart = ToUInt32(bytes.Skip(4).Take(4).ToArray());
            //каждый из блоков еще разбиваем на два подблока длиною в 16 бит
            UInt16 onePart = ToUInt16(GetBytes(leftPart).Take(2).ToArray());
            UInt16 twoPart = ToUInt16(GetBytes(leftPart).Skip(2).Take(2).ToArray());
            UInt16 threePart = ToUInt16(GetBytes(rightPart).Take(2).ToArray());
            UInt16 fourPart = ToUInt16(GetBytes(rightPart).Skip(2).Take(2).ToArray());
            //прогонка раундов
            for (int i = ROUNDS - 1; i >=0; i--)
            {
                UInt32 roundKey = ror64(startKey, i * 8);
                UInt16 f = RoundF(onePart, roundKey);
                if (i > 0)
                {
                    twoPart = onePart;
                    threePart = (UInt16)(f ^ twoPart);
                    fourPart = (UInt16)(f ^ threePart);
                    onePart = (UInt16)(f ^ fourPart);
                }
                else
                {                   
                    twoPart ^= f;
                    threePart ^= f;
                    fourPart ^= f;
                }
            }
            var leftPartBytes = new byte[4]{
                BitConverter.GetBytes(onePart)[1], BitConverter.GetBytes(onePart)[0],
                BitConverter.GetBytes(twoPart)[1], BitConverter.GetBytes(twoPart)[0]
            };
            leftPart = ToUInt32(leftPartBytes);
            var rightPartBytes = new byte[4]{
                BitConverter.GetBytes(threePart)[1], BitConverter.GetBytes(threePart)[0],
                BitConverter.GetBytes(fourPart)[1], BitConverter.GetBytes(fourPart)[0]
            };
            rightPart = ToUInt32(rightPartBytes);
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
                result += (UInt64)Math.Pow(256, 7 - i) * bytes[i];
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
        public static UInt16 RoundF(UInt16 originalBlock, UInt32 roundKey)
        {
            UInt16 first = rol16(originalBlock, 9);
            UInt16 second = (UInt16)~(ror32(roundKey, 11) & originalBlock);
	        return (UInt16)(first ^ second);
        }
        private static UInt32 ror32(UInt32 key, int n)
        {
            UInt32 t1, t2;
            n = n % (sizeof(UInt32) * 8);
            t1 = key >> n;
            t2 = key << (sizeof(UInt32) * 8 - n);
            return t1 | t2;
        }
        //процедура для генерации раундового ключа
        private static UInt32 ror64(UInt64 key, int n) {
            UInt64 t1, t2;
            n = n % (sizeof(UInt64) * 8);
            t1 = key >> n;
            t2 = key << (sizeof(UInt64) * 8 - n);
            return (UInt32)(t1 | t2);//возвращаем половину ключа
        }        
        private static UInt16 ror16(UInt16 a, int n)
        {
            UInt16 t1, t2;
            n = n % (sizeof(UInt16) * 8);  // нормализуем n
            t1 = (UInt16)(a >> n);   // двигаем а вправо на n бит, теряя младшие биты
            t2 = (UInt16)(a << (sizeof(UInt16) * 8 - n)); // перегоняем младшие биты в старшие
            return (UInt16)(t1 | t2);  // объединяем старшие и младшие биты
        }
        private static UInt16 rol16(UInt32 a, int n)
        {
            UInt16 t1, t2;
            n = n % (sizeof(UInt16) * 8);  // нормализуем n
            t1 = (UInt16)(a << n);   // двигаем а вправо на n бит, теряя младшие биты
            t2 = (UInt16)(a >> (sizeof(UInt16) * 8 - n)); // перегоняем младшие биты в старшие
            return (UInt16)(t1 | t2);  // объединяем старшие и младшие биты
        }
        //число переводим в байты
        public static byte[] GetBytes(UInt64 originalBlock)
        {
            return BitConverter.GetBytes(originalBlock).Reverse().ToArray();
        }        
        public static byte[] GetBytes(UInt32 originalBlock)
        {
            return BitConverter.GetBytes(originalBlock).Reverse().ToArray();
        }

    }
}
