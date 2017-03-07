using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FeistelNet
{
    class FeistelNetClassV1
    {
        private static int rounds = 12;//число раундов
        private static UInt64 key = 1234567890123456;//исходный ключ

        public static void feistelCrypt() {
            using (BinaryReader reader = new BinaryReader(File.Open("input.txt", FileMode.Open)))
            {
                //BinaryWriter writer = new BinaryWriter(File.Open("encrypt_output.txt", FileMode.OpenOrCreate));
                //while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    UInt64 currentBlock = 12345678;//reader.ReadUInt64();//блок данных
                    UInt64 encryptedBlock = blockCrypt(currentBlock, key); //зашифрованный блок данных
                    Console.WriteLine("исходный блок {0}", currentBlock.ToString());
                    Console.WriteLine("encrypted блок {0}", encryptedBlock.ToString());
                    UInt64 decryptedBlock = blockDecrypt(encryptedBlock, key);
                    Console.WriteLine("decrypted блок {0}", decryptedBlock.ToString());
                   //writer.Write(encryptedBlock);
                }
                //writer.Close();
            }
        }
        public static void feistelDecrypt()
        {
            using (BinaryReader reader = new BinaryReader(File.Open("encrypt_output.txt", FileMode.Open)))
            {
                BinaryWriter writer = new BinaryWriter(File.Open("decrypt_output.txt", FileMode.OpenOrCreate));
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {                    
                    UInt64 currentBlock = reader.ReadUInt64();//блок данных
                    UInt64 decryptedBlock = blockDecrypt(currentBlock, key); //расшифрованный блок данных
                    writer.Write(decryptedBlock);
                }
                writer.Close();
            }
           
        }
        private static UInt64 blockCrypt(UInt64 block, UInt64 key) {
            //Console.WriteLine("input {0:X}", block);
            UInt32 leftPart = (UInt32)(block >> 32);
            UInt32 rightPart = (UInt32)block;
            UInt32 temp, roundKey;
            for (int i = 0; i < rounds; i++) {
		        temp = leftPart;
                roundKey = (UInt32)ror64(key, i * 8);
                leftPart = f(leftPart, roundKey) ^ rightPart;
                rightPart = temp;
	        }
            UInt64 result = rightPart;
            rightPart <<= 32;
            result |= leftPart;
            //Console.WriteLine("encrypt {0:X}", result);
            return result;
        }
        private static UInt64 blockDecrypt(UInt64 block, UInt64 key)
        {
            UInt32 leftPart = (UInt32)(block >> 32);
            UInt32 rightPart = (UInt32)block;
            UInt32 temp, roundKey;
            for (int i = rounds - 1; i >= 0; i--)
            {
                temp = leftPart;
                roundKey = (UInt32)ror64(key, i * 8);
                leftPart = f(leftPart, roundKey) ^ rightPart;
                rightPart = temp;
            }
            UInt64 result = rightPart;
            rightPart <<= 32;
            result |= leftPart;
            return result;
        }
        
        private static UInt64 ror64(UInt64 a, int n)
        {
            UInt64 t1, t2;
            n = n % (sizeof(UInt64) * 8);  // нормализуем n
            t1 = a >> n;   // двигаем а вправо на n бит, теряя младшие биты
            t2 = a << (sizeof(UInt64) * 8 - n); // перегоняем младшие биты в старшие
            return t1 | t2;  // объединяем старшие и младшие биты
        }

        private static UInt32 ror32(UInt32 a, int n)
        {
            UInt32 t1, t2;
            n = n % (sizeof(UInt32) * 8);  // нормализуем n
            t1 = a >> n;   // двигаем а вправо на n бит, теряя младшие биты
            t2 = a << (sizeof(UInt32) * 8 - n); // перегоняем младшие биты в старшие
            return t1 | t2;  // объединяем старшие и младшие биты
        }

        private static UInt32 rol32(UInt32 a, int n)
        {
            UInt32 t1, t2;
            n = n % (sizeof(UInt32) * 8);  // нормализуем n
            t1 = a << n;   // двигаем а влево на n бит, теряя старшие биты
            t2 = a >> (sizeof(UInt32) * 8 - n); // перегоняем старшие биты в младшие
            return t1 | t2;  // объединяем старшие и младшие биты
        }

        private static UInt32 f(UInt32 block, UInt32 key) {
            UInt32 first = rol32(block, 9);
            UInt32 second = ~((ror32(key, 11)) &  block);
            return first * second;
        }
    }
}
