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
            String text = "darya_markova123encoding";
            UInt64[] blocks = FeistelNetClass.GetBlocks(text);
            UInt64[] resultEncrypted = FeistelNetClass.FeistelEncrypt(blocks);
            //зашифрованный текст
            String cipherText =
                Encoding.ASCII.GetString(resultEncrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
            Console.WriteLine("Зашифрованный текст: {0}", cipherText);

            UInt64[] resultDecrypted = FeistelNetClass.FeistelDecrypt(resultEncrypted);
            String plainText =
                Encoding.ASCII.GetString(resultDecrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
            Console.Write("Расшифрованный текст: {0}", plainText);
            Console.ReadKey();
        }
    }
}
