using System.Net;
using System.Text;
using UnityEngine;

namespace Victorina
{
    public class IpCodeSystem
    {
        private const int LettersAmount = 26;
        
        public string GetCode(string ipString)
        {
            byte[] ipBytes = GetIpBytes(ipString);
            string code = GetCode(ipBytes);
            Debug.Log($"Get code: {ipString} {code}");
            return code;
        }
        
        private byte[] GetIpBytes(string ipString)
        {
            if (IPAddress.TryParse(ipString, out var ipAddress))
            {
                return ipAddress.GetAddressBytes();
            }
            return null;
        }
        
        private string GetCode(byte[] ipNumbers)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte number in ipNumbers)
            {
                int letterNumber = number % LettersAmount;
                int letterCircles = number / LettersAmount;
                sb.Append((char) ('A' + letterNumber));
                sb.Append((char) ('A' + letterCircles));
            }
            return sb.ToString();
        }

        public string GetIp(string code)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetNumber(code[0], code[1]));
            sb.Append('.');
            sb.Append(GetNumber(code[2], code[3]));
            sb.Append('.');
            sb.Append(GetNumber(code[4], code[5]));
            sb.Append('.');
            sb.Append(GetNumber(code[6], code[7]));
            return sb.ToString();
        }

        private int GetNumber(char ch1, char ch2)
        {
            int n1 = ch1 - 'A';
            int n2 = ch2 - 'A';
            return n2 * LettersAmount + n1;
        }

        public bool IsValidGameCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;
            
            if (code.Length != 8)
                return false;

            for (int i = 0; i < 8; i += 2)
            {
                int number = GetNumber(code[i], code[i + 1]);
                if (number < 0 || number > 255)
                    return false;
            }

            return true;
        }
    }
}