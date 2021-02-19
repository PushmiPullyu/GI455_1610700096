using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {

        /*struct MessageData
        {
            public string username;
            public string message;

            public MessageData(string username , string message)
            {
                this.username = username;
                this.message = message;
            }
        }*/

        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        public GameObject rootConnection;
        public GameObject rootLobby;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootMessenger;
        public GameObject rootError;

        

        public InputField inputText;
        public InputField inputUsername;
        public InputField inputCreateRoom;
        public InputField inputJoinRoom;
        public Text roomName;
        public Text textError;
        //public Text sendText;
        //public Text receiveText;

        public string username;
        
        private WebSocket ws;

        private string tempMessageString;

        private bool ErrorStatus = false;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootMessenger.SetActive(false);
            rootError.SetActive(false);
        }
        private void Update()
        {
            UpdateNotifyMessage();
        }
        public void Connect(InputField username)
        {
            string url = "ws://127.0.0.1:8080/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            this.username = username.text;

            rootConnection.SetActive(false);
            rootLobby.SetActive(true);
        }

        public void LoobyCreate()
        {
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(true);
        }

        public void LoobyJoin()
        {
            rootLobby.SetActive(false);
            rootJoinRoom.SetActive(true);
        }

        public void toCreateRoom()
        {
            CreateRoom(inputCreateRoom.text);
            
        }

        public void toJoinRoom()
        {
            JoinRoom(inputJoinRoom.text);  

        }

        public void okButton()
        {
            rootError.SetActive(false);
        }


        public void CreateRoom(string roomName)
        {
            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void JoinRoom(string roomName)
        {
            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            rootMessenger.SetActive(false);
            rootLobby.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
           /* if (string.IsNullOrEmpty(inputText.text) || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData messageData = new MessageData(inputUsername.text, inputText.text);

            string toJsonStr = JsonUtility.ToJson(messageData);

            ws.Send(toJsonStr);
            inputText.text = "";*/
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void ErrorCheck(SocketEvent socket)
        { 
            switch (socket.eventName)
            {
                case "CreateRoom":

                    Debug.Log(socket.data);

                    if (socket.data == "roomExist")
                    {
                        
                        ErrorStatus = true;
                        Debug.Log(ErrorStatus);
                        rootError.SetActive(true);
                        textError.text = "Room existed";
                    }
                    else
                    {
                        Debug.Log("Success");
                        rootCreateRoom.SetActive(false);
                        rootMessenger.SetActive(true);

                        roomName.text = "Room : " + inputCreateRoom.text;
                    }

                    break;
                case "JoinRoom":

                    if (socket.data == "notFound")
                    {
                        ErrorStatus = true;
                        rootError.SetActive(true);
                        textError.text = "Room not found";
                    }
                    else if(socket.data == "joinFail")
                    {
                        ErrorStatus = true;
                        rootError.SetActive(true);
                        textError.text = "Join room failed";
                    }
                    else
                    {
                        rootJoinRoom.SetActive(false);
                        rootMessenger.SetActive(true);

                        roomName.text = "Room : " + inputJoinRoom.text;
                    }

                    break;
            }

            /*if(socket.eventName == "CreateRoom")
            {
                if (socket.data == "roomExist")
                {
                    ErrorStatus = true;
                    textError.text = "Room existed";
                }
                else
                {
                    Debug.Log(rootMessenger.gameObject.name);
                    OpenMessage();
                    
                    roomName.text = "Room : " + inputCreateRoom.text;
                }
            }*/

            /*if (ErrorStatus)
            {
                rootError.SetActive(true);
            }*/
            Debug.Log(ErrorStatus);
        }


        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;

            SocketEvent se = JsonUtility.FromJson<SocketEvent>(tempMessageString);

            ErrorCheck(se);
        }
    }
}


