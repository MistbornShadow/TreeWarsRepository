const {Unit} = require('./Unit')
const {Server} = require('./Server')
const WebSocket = require('ws')
const port = 8080
const wss = new WebSocket.Server({port}, ()=>{
    console.log('server started')
})

class Player{
    constructor(state){
        this.state = state
    }
}

states = {}

playerBase = {}

class Data {
    constructor(type, info){
        this.type = type
        this.info = info
    }
}

wss.on('connection', (ws)=>{
    console.log('connection found')
    console.log(ws)
    console.log(toString(ws))
    ws.send("welcome")

    ws.on('message', (data)=>{
        parsedData = JSON.parse(data)
        console.log(parsedData)
         // Check what kind of data we received
        switch(parsedData.type) {
            case "game_exit":
                deletePlayer(parsedData.info)
                break
            case "player_ID":
                playerIDConfirm(parsedData.info, ws)
                break
            case "game_ID":
                gameIDConfirm(parsedData.info, ws)
            case "spawn_unit":
                ws.send("Spawn Message Accepted")
                break
            case "get_server_list":
                //expand to more info: players, etc.
                sendServerList(ws)
                break
            case "join_server":
                joinServerRequest(parsedData.info, ws)
                break
            case "request_host":
                sendServerHostID(parsedData.info, ws)
                break
            case "request_guest":
                sendServerGuestID(parsedData.info, ws)
            default:
                console.log("Unknown command: " + parsedData.type)
        }

        //console.log('%o', playerBase)
       
        // Save that in the global state
        console.log('data state %o', states)
    })
});

function deletePlayer(info){
    console.log(info)
    delete playerBase[parseInt(info, 10)]
}

function sendServerHostID(info, ws){
    let server = states[parseInt(info)]
    let obj = new Data("host_player_ID", toString(server.player1))
    ws.send(JSON.stringify(obj))
}

function sendServerGuestID(info, ws){
    let server = states[parseInt(info)]
    let obj = new Data("host_player_ID", toString(server.player2))
    ws.send(JSON.stringify(obj))
}

function playerIDConfirm(info, ws){
    console.log(info)
    if(playerBase[info] != null) {
        let obj = new Data("generate_player_ID", "")
        ws.send(JSON.stringify(obj))
        return
    }
    playerBase[info] = new Player(true)
}

function gameIDConfirm(info, ws){
    if(states[info] != null) {
        let obj = new Data("generate_game_ID", "")
        ws.send(JSON.stringify(obj))
        return
    }
    let req = new Data("send_player_ID", "")
    console.log(req);
    ws.send(JSON.stringify(req))
    ws.on('message', (data)=>{
        parsedData = JSON.parse(data)
        switch(parsedData.type){
            case "host_player_ID":
                createServer(parseInt(info), parsedData.info)
                return
            default:
                console.log("ERROR: gameIDConfirm" + parsedData.type)
                return
        }
    })
}

function sendServerList(ws){
    let keys = Object.keys(states)
    keys.forEach(element =>{
        var obj = states[element]
        console.log(obj)
        if(!Boolean(obj.full)){
            var server = new Data("server", JSON.stringify(obj))
            console.log(JSON.stringify(server))
            ws.send(JSON.stringify(server))
        }
    })
    var final = new Data("create_server_list", "")
    ws.send(JSON.stringify(final))
}

function joinServerRequest(s, ws){
    var nums = s.match(/\d+/g);
    var server = states[nums[0]];
    var obj = new Data("", "");
    if(!server.full) {
        server.player2 = nums[1];
        server.full = true;
        obj = new Data("successful_join", toString(server.gameID));
    } else {
        obj = new Data("unsuccessful_join", "");
    }
    ws.send(JSON.stringify(obj));
}

function createServer(gameID, info){
    states[gameID] = new Server(false, gameID, parseInt(info))
}

wss.on('listening', ()=>{
    console.log(`server is listening on port ${port}`)
})
