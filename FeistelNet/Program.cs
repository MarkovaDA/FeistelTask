using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FeistelNet
{
    class Program
    {
        static void Main(string[] args)
        {
            //сделать файловый ввод-вывод
            String text = "I love you";
            UInt64[] blocks = FeistelNetClass.GetBlocks(text);
            UInt64[] resultEncrypted = FeistelNetClass.FeistelEncrypt(blocks);
            //зашифрованный текст
            String cipherText =
                Encoding.ASCII.GetString(resultEncrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
            Console.WriteLine("Encrypted text: {0}", cipherText);

            UInt64[] resultDecrypted = FeistelNetClass.FeistelDecrypt(resultEncrypted);
            String plainText =
                Encoding.ASCII.GetString(resultDecrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
            //убираем с конца лишние символы
            plainText = plainText.Substring(0, text.Length);
            Console.WriteLine("Decrypted text: {0}", plainText);
            Console.ReadKey();
        }
    }
}
