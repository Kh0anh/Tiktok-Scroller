using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TiktokScroller_Listener.Connection
{
    public static class Common
    {
        public static byte[] EncodeWebSocketMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            int messageLength = messageBytes.Length;
            byte[] encodedMessage;

            if (messageLength < 126)
            {
                encodedMessage = new byte[messageLength + 2];
                encodedMessage[0] = 0x81; // Text frame opcode
                encodedMessage[1] = (byte)messageLength;
                Array.Copy(messageBytes, 0, encodedMessage, 2, messageLength);
            }
            else if (messageLength < 65536)
            {
                encodedMessage = new byte[messageLength + 4];
                encodedMessage[0] = 0x81; // Text frame opcode
                encodedMessage[1] = 126;
                encodedMessage[2] = (byte)((messageLength >> 8) & 255);
                encodedMessage[3] = (byte)(messageLength & 255);
                Array.Copy(messageBytes, 0, encodedMessage, 4, messageLength);
            }
            else
            {
                // Currently not handling payloads longer than 65535 bytes
                throw new Exception("Message is too long to encode.");
            }

            return encodedMessage;
        }
        public static string DecodeWebSocketMessage(byte[] message)
        {
            if (message.Length < 2)
            {
                Console.WriteLine("Invalid WebSocket message.");
                return "error";
            }

            byte firstByte = message[0];
            byte secondByte = message[1];

            bool isFinalFrame = (firstByte & 0x80) != 0;
            bool isMasked = (secondByte & 0x80) != 0;
            int payloadLength = secondByte & 0x7F;
            int dataStartIndex = 2;
            int maskStartIndex = dataStartIndex;
            int payloadStartIndex = dataStartIndex;
            byte[] mask = null;

            if (payloadLength == 126)
            {
                payloadLength = BitConverter.ToUInt16(new byte[] { message[3], message[2] }, 0);
                maskStartIndex = 4;
                payloadStartIndex = 6;
            }
            else if (payloadLength == 127)
            {
                payloadLength = (int)BitConverter.ToUInt64(new byte[] { message[9], message[8], message[7], message[6], message[5], message[4], message[3], message[2] }, 0);
                maskStartIndex = 10;
                payloadStartIndex = 14;
            }

            if (isMasked)
            {
                mask = new byte[] { message[maskStartIndex], message[maskStartIndex + 1], message[maskStartIndex + 2], message[maskStartIndex + 3] };
                payloadStartIndex += 4;
            }

            byte[] payload = new byte[payloadLength];
            for (int i = 0; i < payloadLength; i++)
            {
                payload[i] = (byte)(message[payloadStartIndex + i] ^ mask[i % 4]);
            }

            string decodedMessage = Encoding.UTF8.GetString(payload);
            return decodedMessage;
        }
        public static string ComputeWebSocketHandshakeResponse(string request)
        {
            // Extract the Sec-WebSocket-Key from the client's HTTP request
            string key = request.Substring(request.IndexOf("Sec-WebSocket-Key: ") + 19);
            key = key.Substring(0, key.IndexOf("\r\n"));

            // Concatenate the key with a predefined WebSocket GUID and calculate a SHA-1 hash
            string concatenated = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            byte[] concatenatedBytes = Encoding.UTF8.GetBytes(concatenated);
            byte[] hashBytes;
            using (SHA1 sha1 = SHA1.Create())
            {
                hashBytes = sha1.ComputeHash(concatenatedBytes);
            }

            // Convert the hash to Base64 to get the Sec-WebSocket-Accept value
            string responseKey = Convert.ToBase64String(hashBytes);

            return responseKey;
        }
    }
}
