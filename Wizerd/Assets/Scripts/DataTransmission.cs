using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class DataTransmission : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    [SerializeField] int port = 2912;
    IPEndPoint localEndPoint;

    public bool isReceiving = true;
    
    public string leftLandmarksData;
    public string rightLandmarksData;
    List<byte> frameBytesList = new List<byte>();
    public byte[] frameBytes;

    public void Start()
    {
        localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

        receiveThread = new Thread(new ThreadStart(ReceiveData)) {
            IsBackground = true
        };
        receiveThread.Start();
    }

    // Receive Thread
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (isReceiving) {
            try {
                byte[] dataByte = client.Receive(ref localEndPoint);

                // If data received is hand landmarks (Flag is L)
                if (dataByte[0] == 76) {
                    // If left hand landmarks received (Flag is L)
                    if (dataByte[1] == 76) {
                        leftLandmarksData = Encoding.UTF8.GetString(dataByte, 3, dataByte.Length - 4);
                    }
                    // If right hand landmarks received (Flag is R)
                    else if (dataByte[1] == 82) {
                        rightLandmarksData = Encoding.UTF8.GetString(dataByte, 3, dataByte.Length - 4);
                    }
                }
                // Data received is the last frame chunk (Flag is F)
                else if (dataByte[0] == 70) {
                    byte[] chunk = new byte[dataByte.Length - 1];
                    Buffer.BlockCopy(dataByte, 1, chunk, 0, chunk.Length);

                    frameBytesList.AddRange(chunk);

                    frameBytes = frameBytesList.ToArray();
                    frameBytesList.Clear();
                }
                // Data received is frame chunk (Flag is C)
                else if (dataByte[0] == 67) {
                    byte[] chunk = new byte[dataByte.Length - 1];
                    Buffer.BlockCopy(dataByte, 1, chunk, 0, chunk.Length);

                    frameBytesList.AddRange(chunk);
                }
                // Data received is none (Flag is N)
                else if (dataByte[0] == 78) {
                    frameBytes = new byte[0];
                    frameBytesList.Clear();
                }
            } catch (Exception err) {
                print(err.ToString());
            }
        }
    }
}