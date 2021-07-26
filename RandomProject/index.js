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

playerBase = {
    
}


class Data {
    constructor(type, info){
        this.type = type
        this.info = info
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
            case "spawn_unit":
                ws.send("Spawn Message Accepted")
                break
            case "get_server_list":
                //expand to more info: players, etc.
                sendServerList(ws)
                break
            case "join_server":
                break
        }

        console.log('%o', playerBase)
       
        // Save that in the global state
        console.log('data state %o', states)
    })
});

function deletePlayer(info){
    console.log(info)
    delete playerBase[parseInt(info)]
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
    console.log(info)
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
                console.log('data state %o', states)
                return
            default:
                console.log("ERROR: gameIDConfirm" + parsedData.type)
                return
        }
    })
}

function sendServerList(ws){
    let obj = new Data("server_list", JSON.stringify(states))
    console.log(JSON.stringify(obj))
    ws.send(JSON.stringify(obj))
    let keys = Object.keys(states)
    keys.forEach(element => {
        let key = new Data("key", element)
        ws.send(JSON.stringify(key))
    });
    obj = new Data("create_server_list", "")
    ws.send(JSON.stringify(obj))
}

function createServer(gameID, info){
    states[gameID] = new Server(false, gameID, parseInt(info))
}

wss.on('listening', ()=>{
    console.log(`server is listening on port ${port}`)
})
