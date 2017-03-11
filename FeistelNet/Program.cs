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
            String text ="darya_markova123";
            uint[] blocks = FeistelNetClassV2.GetBlocks(text);
            uint[] resultEncrypted = FeistelNetClassV2.FeistelEncrypt(blocks);
            String cipherText = 
                Encoding.ASCII.GetString(resultEncrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
<<<<<<< Updated upstream
            uint[] resultDecrypted = FeistelNetClassV2.FeistelDecrypt(resultEncrypted);
=======
            Console.WriteLine("Зашифрованный текст: {0}", cipherText);            
            UInt64[] resultDecrypted = FeistelNetClass.FeistelDecrypt(resultEncrypted);
>>>>>>> Stashed changes
            String plainText = 
                Encoding.ASCII.GetString(resultDecrypted.SelectMany(r => BitConverter.GetBytes(r).Reverse()).ToArray());
            Console.Write(plainText);
            Console.ReadKey();
        }
    }
}
