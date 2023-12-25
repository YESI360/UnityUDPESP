using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MoverUDPOri02 : MonoBehaviour
{
    int cont;
    public float speed;
    public float pote = 0;
    public float Antpote = 1;
    AudioSource source;
    Thread m_Thread;
    UdpClient m_Client;

    void Start()
    {
        m_Thread = new Thread(new ThreadStart(ReceiveData));
        m_Thread.IsBackground = true;
        m_Thread.Start();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (pote == 1)
        {
            transform.localScale += new Vector3(pote * speed, pote * speed, pote * speed);
        }

        if (pote == 2)
        {
            //source.Play();//SE REPRODUCE TODO EL TIEMPO, SUENA COMO UN GLITCH
            transform.localScale -= new Vector3(pote * speed, pote * speed, pote * speed);

        }


        if (pote == 2 && pote != Antpote)
        {
            //Debug.Log("play");
            source.Play();
        }

        Antpote = pote;//actualizo estados

    }


    void ReceiveData()
    {
        try
        {
            m_Client = new UdpClient(1234);
            m_Client.EnableBroadcast = true;
            while (true)
            {
                IPEndPoint hostIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = m_Client.Receive(ref hostIP);
                string returnData = Encoding.ASCII.GetString(data);
                Debug.Log(returnData);

                 if (!float.TryParse(returnData, System.Globalization.NumberStyles.Number,null,out float auxi)){
                    Debug.Log("Lo recibido no es un número");
                };
                pote = auxi;////////////////////////////////////////////
                Debug.Log(auxi);

            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            OnApplicationQuit();
        }
    }

    private void OnApplicationQuit()
    {
        if (m_Thread != null)
        {
            m_Thread.Abort();
        }

        if (m_Client != null)
        {
            m_Client.Close();
        }
    }
    void udpSend(Vector3 posx)
    {
        //var IP = IPAddress.Parse("192.168.0.74"); // Conecta al IP indicado (Modo Unicast) 
        var IP = IPAddress.Parse("239.1.2.3"); // Conecta al grupo Multicast indicado       
        int port = 1234;


        var udpClient1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var sendEndPoint = new IPEndPoint(IP, port);


        try
        {

            //Sends a message to the host to which you have connected.
            //byte[] sendBytes = Encoding.ASCII.GetBytes("hello from unity");
            byte[] sendBytes = Encoding.ASCII.GetBytes(posx.ToString());
            udpClient1.SendTo(sendBytes, sendEndPoint);



        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

    }
}


