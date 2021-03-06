using System;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Numerics;

namespace CSharp_SDK
{
    public class KeystoreUtils
    {

        private static string t = "1000000000000000000000000000000014def9dea2f79cd65812631a5cf5d3ec";

        public static string PubkeyToAddress(byte[] pubkey)
        {
            Sha3Keccack sha3Keccack = Sha3Keccack.Current;
            byte[] pub256 = sha3Keccack.CalculateHash(pubkey);
            byte[] r1 = RipemdManager.getHash(pub256);
            return PubkeyHashToAddress(r1);
        }

        public static string PubkeyHashToAddress(byte[] publicHash)
        {
            Sha3Keccack sha3Keccack = Sha3Keccack.Current;
            byte[] r1 = publicHash;
            byte[] r2 = Utils.prepend(r1, (byte)0x00);
            byte[] r3 = sha3Keccack.CalculateHash(sha3Keccack.CalculateHash(r1));
            byte[] b4 = Utils.CopyByteArray(r3, 0, 4);
            byte[] b5 = Utils.Combine(r2, b4);
            string s6 = "WX" + Base58Check.Encode(b5);
            return s6;
        }

        public static string PubkeyHashToAddress(string publicHash)
        {
            return PubkeyHashToAddress(publicHash.HexToByteArray());
        }

        private static byte[] AddressToPubkeyHashByteArray(string address)
        {
            byte[] r5;
            if (address.StartsWith("1"))
            {
                r5 = Base58Check.Decode(address);
            }
            else
            {
                r5 = Base58Check.Decode(address.Substring(2));
            }
            byte[] r2 = Utils.CopyByteArray(r5, 0, 21);
            byte[] r1 = Utils.CopyByteArray(r2, 1, 20);
            return r1;
        }

        public static string AddressToPubkeyHash(string address)
        {
            return AddressToPubkeyHashByteArray(address).ToHex();
        }

        public static string PrivatekeyToPublicKey(string privateKey)
        {
            Ed25519PrivateKey eprik = new Ed25519PrivateKey(privateKey.HexToByteArray());
            Ed25519PublicKey epuk = eprik.GeneratePublicKey();
            return epuk.GetEncoded().ToHex();
        }

        public static string PublicKeyToPublicKeyHash(string publicKey)
        {
            byte[] pub256 = Sha3Keccack.Current.CalculateHash(publicKey.HexToByteArray());
            byte[] r1 = RipemdManager.getHash(pub256);
            return r1.ToHex();
        }

        public static int VerifyAddress(String address)
        {
            if (!address.StartsWith("1") && !address.StartsWith("WX") && !address.StartsWith("WR"))
            {
                return -1;
            }
            byte[] r5;
            if (address.StartsWith("1"))
            {
                r5 = Base58Check.Decode(address);
            }
            else
            {
                r5 = Base58Check.Decode(address.Substring(2));
            }
            Sha3Keccack sha3Keccack = Sha3Keccack.Current;
            byte[] r3 = sha3Keccack.CalculateHash(sha3Keccack.CalculateHash(AddressToPubkeyHashByteArray(address)));
            byte[] b4 = Utils.CopyByteArray(r3, 0, 4);
            byte[] _b4 = Utils.CopyByteArray(r5, r5.Length - 4, 4);
            if (Array.Equals(b4, _b4))
            {
                return 0;
            }
            else
            {
                return -2;
            }
        }

    }
}
