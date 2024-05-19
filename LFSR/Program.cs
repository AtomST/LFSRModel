

using System.Collections;
using System.Text;

namespace LFSR
{
    internal class Program
    {
        //РСЛОС 32бита
        static IList<IList<int>> feedback = new List<IList<int>>()
        {
                new List<int>{2,9,12,17,20},
                new List<int>{5,13,16,27,30},
                new List<int>{4,11,21,26,27,31},
                new List<int>{1,7,14,18,21,29},
        };
        // Позиции обратной связи для РСЛОС длины 8 бит
        //static IList<IList<int>> feedback = new List<IList<int>>()
        //{
        //        new List<int>{2,7},
        //        new List<int>{5,7},
        //        new List<int>{4,5},
        //        new List<int>{1,3,6},
        //};
        static IList<BitArray> SetLFSR(byte[] bytes)
        {
            IList<BitArray> arr = new List<BitArray>();
            for (int i = 0; i < 4; i++)
            {
                arr.Add(new BitArray(bytes.Skip(i).Take(4).ToArray()));
            }
            return arr;
        }
        static bool BoolFunc(BitArray arr)
        {
            return (arr[0] ^ arr[1]) ^ (arr[2] ^ arr[3]);
        }

        static BitArray GetLFSRBits(IList<BitArray> LFSRs)
        {
            BitArray result = new BitArray(4);
            for(int i = 0; i < 4; i++)
            {
                result.Set(i, LFSRs[i][31]);
            }
            return result;
        }

        static void ShiftLFSR(BitArray LFSR, int id)
        {
            bool bit = false;
            foreach (int feddBit in feedback[id])
            {
                bit = bit ^ LFSR[feddBit];
            }
            LFSR.LeftShift(1);
            LFSR.Set(0,bit);

        }

        static BitArray Encrypt(BitArray code, byte[] key)
        {
            IList<BitArray> LFSRs = SetLFSR(key);
            BitArray cripted = new BitArray(code.Length);
            for(int i = 0; i < code.Length; i++)
            {
                BitArray bits4 = GetLFSRBits(LFSRs);
                cripted.Set(i, BoolFunc(bits4) ^ code[i]);
                for (int l = 0; l < 4; l++)
                {
                    ShiftLFSR(LFSRs[l], l);
                }
            }
            return cripted;
        }

        static void Main(string[] args)
        {
            Random rnd = new Random();
            byte[] rndBytes = new byte[16];


            //Буферы для состояний сообщения
            byte[] messageRnd = new byte[8];
            byte[] cryptedM = new byte[8];
            byte[] decryptedM = new byte[8];

            //Генерация ключа
            rnd.NextBytes(rndBytes);
            //Генерация сообщения
            rnd.NextBytes(messageRnd);

            BitArray message = new BitArray(messageRnd);
            //Шифрование
            BitArray criptedMessage = Encrypt(message, rndBytes);
            //Дешифрование
            BitArray encriptedMessage = Encrypt(criptedMessage,rndBytes);

            criptedMessage.CopyTo(cryptedM, 0);
            encriptedMessage.CopyTo(decryptedM, 0);

            Console.Write("Переданное сообщение: ");
            foreach(byte bit in messageRnd)
            {
                Console.Write(bit + " ");
            }
            Console.WriteLine();


            Console.Write("Зашифрованное сообщение: ");
            foreach (byte bit in cryptedM)
            {
                Console.Write(bit + " ");
            }

            Console.WriteLine();

            Console.Write("Дешифрованное сообщение: ");
            foreach (byte bit in decryptedM)
            {
                Console.Write(bit + " ");
            }
            Console.WriteLine();


        }



    }
}