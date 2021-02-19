var websocket = require('ws');

var websocketServer = new websocket.Server({port:8080}, ()=>{
    console.log ("The server is running");
});

var wsList = [];
var roomList = [];

websocketServer.on("connection", (ws)=>{

    {
        //Lobby
        ws.on("message",(data)=>{

            console.log('user connected.');

        //Reception
            console.log("send from user :"+ data);

        //Convert jsonStr into jsonObj
            var toJsonObj = { 
                roomName:"",
                data:""
            }
            toJsonObj = JSON.parse(data);


            if(toJsonObj.eventName == "CreateRoom")//CreateRoom
            {
                console.log("user request CreateRoom["+toJsonObj.data+"]");

                var isFoundRoom = false; //Find Room with roonName

                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if(isFoundRoom == true)// Found room
                {
                    //Callback to Client : roomName is exist
                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("create room fail.");

                }
                else
                {
                    console.log("Room is not Found")
                    //Create Room Here
                    var newRoom = {
                        roomName: toJsonObj.data,
                        wsList: []
                    }
    
                    newRoom.wsList.push(ws);//Add User to roomList
    
                    roomList.push(newRoom);

                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:toJsonObj.data
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("create room success.");
                }

                
            }
            else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
            {
                console.log("user request JoinRoom");//Homework
            }

            else if(toJsonObj.eventName == "LeaveRoom");//LeaveRoom
            {
                var isLeaveSuccess = false;
                for(var i = 0; i < roomList.length; i++)//Loop in roomList
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                    {
                        if(ws == roomList[i].wsList[j])//If user found.
                        {
                            roomList[i].wsList.splice(j, 1);//Remove user When found.
    
                            if(roomList[i].wsList.length <= 0)//Remove room.
                            {
                                roomList.splice(i, 1);//Remove at index one time. When no one left in room.
                            }
                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }
                
                if(isLeaveSuccess)
                {
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"success"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("leave room success");
                }
                else
                {
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("leave room fail");
                }
            }
        })
    }
    
    
    wsList.push(ws);

    ws.on("message",(data)=>{
        
        console.log("send from user : "+data);
        
        Boardcast(data);
    });

    ws.on("close" , ()=>{

        console.log('user disconnected');

        for(var i = 0; i < roomList.length; i++)//Loop in roomList
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
            {
                if(ws == roomList[i].wsList[j])//If founded client.
                {
                    roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                    if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                    {
                        roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                    }
                    break;
                }
            }
        }
    });
});


function Boardcast(data)
{
   /* for (var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }*/
}