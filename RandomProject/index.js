const {Unit} = require('./Unit')
const {Server} = require('./Server')
const {Player} = require('./Player')
const {Game} = require('./Game')
const WebSocket = require('ws')
const port = 80
// const path =  'wss://Hitokiri-Batosai'

const wss = new WebSocket.Server({port}, () => {
    console.log('server started')
})

states = {}

playerBase = {}

class Data {
    constructor(type, info){
        this.type = type
        this.info = info
    }
}

class TeamsState {
    constructor(a, w, p1, p2){
        this.autumn = a;
        this.winter = w;

        this.player1 = p1;
        this.player2 = p2;
    }
}

wss.on('connection', (ws)=>{
    console.log('connection found')

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
                break
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
                break
            case "update_teams":
                updateTeams(parsedData.info, ws)
                break
            default:
                console.log("Unknown command: " + parsedData.type)
        }

        console.log('%o', Object.keys(playerBase))
       
        // Save that in the global state
        console.log('data state %o', states)
    })
});

function updateTeams(info){
    //num[0] is the gameID, nums[1] is the kind of request, and nums[2] is the requester's playerID
    var nums = info.match(/\d+/g);
    var gameID = parseInt(nums[0])
    var request = parseInt(nums[1])
    var playerID = parseInt(nums[2])
    var obj
    var p1;
    var p2;
    let server = states[gameID]
    switch(request){
        //player is requesting nullification of team selection. Replace requested team with -1, indicating not selected
        case 0:
            if(server.aSelect === playerID) server.aSelect = -1
            else if (server.wSelect === playerID) server.wSelect = -1
            else return
            break;
        //player requesting to join team autumn. If team autumn equals -1 (indicating not chosen), player is fitted to autumn team
        case 1:
            if(server.aSelect === -1) server.aSelect = playerID
            else return
            break;
        //same as case 1, but for team winter.
        case 2:
            if(server.wSelect === -1) server.wSelect = playerID
            else return
            break;
        default:
            console.log("Issue within the 'update teams' function")
            console.log(nums)
    }
    p1 = checkForPlayer1(server)
    p2 = checkForPlayer2(server)
    obj = new TeamsState(server.aSelect, server.wSelect, p1, p2)
    let dObject = new Data("update_teams", JSON.stringify(obj))
    playersMessage(server, dObject)
}

function checkForPlayer1(server){
    if(server.aSelect === server.player1 || server.wSelect === server.player1) return true;
    else return false;
}

function checkForPlayer2(server){
    if(server.aSelect === server.player2 || server.wSelect === server.player2) return true;
    else return false;
}

function playersMessage(server, obj){
    var player1, player2
    player1 = playerBase[server.player1]
    let ran = new Data("message", "This is for player 1")
    player1.ws.send(JSON.stringify(ran))
    player1.ws.send(JSON.stringify(obj))
    player2 = playerBase[server.player2]
    let ran2 = new Data("message", "This is for player 2")
    player2.ws.send(JSON.stringify(ran2))
    if(player2 !== null) player2.ws.send(JSON.stringify(obj))
}

function deletePlayer(info){
    console.log(info)
    var nums = info.match(/\d+/g);
    delete playerBase[nums[0]]
    delete states[nums[1]]
}

function sendServerHostID(info, ws){
    let server = states[parseInt(info)]
    let obj = new Data("host_player_ID", toString(server.player1))
    ws.send(JSON.stringify(obj))
    let host = playerBase[server.player1]
    let obj2 = new Data("guest_player_ID", toString(server.player2))
    host.ws.send(JSON.stringify(obj2))
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
    playerBase[info] = new Player(ws, info)
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
        server.player2 = parseInt(nums[1]);
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
