const {Unit} = require('./Unit')
const {Server} = require('./Server')
const {Player} = require('./Player')
const {Game} = require('./Game')
const WebSocket = require('ws')
const { Update } = require('./Update')
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

class SpawnUnit{
    constructor(team, unit, newResource){
        this.team = team;
        this.unit = unit;
        this.newResource = newResource;
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

    //createServer(1000, 1337)

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
                spawnUnitRequest(parsedData.info);
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
            case "host_player_ID":
                createServerWrapper(parsedData.info)
                break
            case "start_game":
                startGameFunction(parsedData.info)
                break
            case "update_check":
                checkForUpdate(parsedData.info);
                break;
            case "interval_resource_increase":
                updateGameIntervalResourceIncrease(parsedData.info);
                break;
            default:
                console.log("Unknown command: " + parsedData.type)
        }

        console.log('%o', Object.keys(playerBase))
       
        // Save that in the global state
        console.log('data state %o', states)
    })
});

function findPlayer(playerID, game){
    if(game.player1.playerID === playerID) return game.player1;
    else return game.player2;
}

function findResourceAutumn(game){
    var tempPlayer = game.player1
    if(tempPlayer.team === 1){
        return game.getP1Resource();
    }
    else return game.getP2Resource();
}

function findResourceWinter(game){
    var tempPlayer = game.player1
    if(tempPlayer.team === 2){
        return game.getP1Resource();
    }
    else return game.getP2Resource();
}

function updatePlayerResources(player, game, cost){
    if(game.player1.playerID === player.playerID){
        let tempResource = game.getP1Resource() - cost;
        if(tempResource >= 0) game.p1Resource = tempResource;
        else {
            console.log("ERROR: p1 resource check: " + game.getP1Resource() + " cost: " + cost);
            return false;
        }
    }
    else{
        let tempResource = game.getP2Resource() - cost;
        if(tempResource >= 0) game.p2Resource = tempResource;
        else {
            console.log("ERROR: p2 resource check: " + game.getP2Resource() + " cost: " + cost);
            return false;
        }
    }
}

function spawnUnitRequest(info){
    var nums = info.match(/\d+/g);
    let playerID = parseInt(nums[0], 10);
    let gameID = parseInt(nums[1], 10);
    let unit = parseInt(nums[2]);
    console.log(unit);
    console.log(info);
    let server = states[gameID];
    let playerObject = findPlayer(playerID, server.Game);
    if(playerObject.team === 1)
        switch(unit){
            //knight
            case 1:
                if(!updatePlayerResources(playerObject, server.Game, 30)) return;
                server.Game.AutumnUnits[server.Game.autumnCounter] = Unit.createKnight();
                server.Game.update = true;
                server.Game.updateCommand = "spawn_unit_autumn";
                break;
            default:
                console.log("ERROR: spawn unit request");
                break;
        }
    else{
        switch(unit){
            case 1:
                if(!updatePlayerResources(playerObject, server.Game, 30)) return;
                server.Game.WinterUnits[server.Game.winterCounter] = Unit.createKnight();
                server.Game.update = true;
                server.Game.updateCommand = "spawn_unit_winter";
                break;
            default:
                console.log("ERROR: spawn unit request");
                break;
        }
    }
    checkForUpdate(info);
}

function addTeamComponent(server){
    if(server.player1 === server.aSelect){
        playerBase[server.player1].team = 1;
        playerBase[server.player2].team = 2;
    }
    else{
        playerBase[server.player1].team = 2;
        playerBase[server.player2].team = 1;
    }
}

function updateGameIntervalResourceIncrease(info){
    var gameID = parseInt(info);
    server = states[gameID];
    console.log("informing game state that it is updated via interval resource increase");
    server.Game.intervalResourceIncrease();
}

function startGameFunction(info){
    var gameID = parseInt(info);
    var server = states[gameID];
    addTeamComponent(server);
    server.Game = new Game(playerBase[server.player1], playerBase[server.player2]);
    var obj = new Data("start_game", "");
    console.log(server);
    playersMessage(server, obj);
}

function checkForUpdate(info){
    var nums = info.match(/\d+/g);
    let playerID = parseInt(nums[0], 10);
    let gameID = parseInt(nums[1], 10);
    let game = states[gameID].Game;
    let dataObj = new Data("update", "");
    let updateObj = new Update();
    if(game.update = true){
        switch (game.updateCommand){
            case "p1_resource_change":
                updateObj.command = "resources_update";
                updateObj.updateObj = JSON.stringify(game.getP1Resource());
                dataObj.info = JSON.stringify(updateObj);
                game.player1.ws.send(JSON.stringify(dataObj));
                break;
            case "p2_resource_change":
                updateObj.command = "resources_update";
                updateObj.updateObj = JSON.stringify(game.getP2Resource());
                dataObj.info = JSON.stringify(updateObj);
                game.player2.ws.send(JSON.stringify(dataObj));
                break;
            case "both_resource_change":
                updateObj.command = "resources_update";
                updateObj.updateObj = JSON.stringify(game.getP1Resource());
                dataObj.info = JSON.stringify(updateObj);
                game.player1.ws.send(JSON.stringify(dataObj));
                updateObj.updateObj = JSON.stringify(game.getP2Resource());
                dataObj.info = JSON.stringify(updateObj);
                game.player2.ws.send(JSON.stringify(dataObj));
                break;
            case "spawn_unit_autumn":
                updateObj.command = "spawn_unit_autumn";
                updateObj.updateObj = new SpawnUnit(1, "knight", findResourceAutumn(game));
                dataObj.info = JSON.stringify(updateObj);
                game.player1.ws.send(JSON.stringify(dataObj));
                game.player2.ws.send(JSON.stringify(dataObj));
                game.autumnCounter++;
                break;
            case "spawn_unit_winter":
                updateObj.command = "spawn_unit_winter";
                updateObj.updateObj = new SpawnUnit(2, "knight", findResourceWinter(game));
                dataObj.info = JSON.stringify(updateObj);
                game.player1.ws.send(JSON.stringify(dataObj));
                game.player2.ws.send(JSON.stringify(dataObj));
                game.winterCounter++;
                break;
            default:
                console.log("ERROR: " + game.updateCommand);
                break;
        }
    }
    console.log(dataObj);
    dataObj = null;
    game.update = false;
}

function updateTeams(info){
    //num[0] is the gameID, nums[1] is the kind of request, and nums[2] is the requester's playerID
    var nums = info.match(/\d+/g);
    var gameID = parseInt(nums[0], 10)
    var request = parseInt(nums[1], 10)
    var playerID = parseInt(nums[2], 10)
    var obj
    var p1;
    var p2;
    let server = states[gameID]
    switch(request){
        //player is requesting nullification of team selection. Replace requested team with -1, indicating not selected
        case 0:
            if (server.aSelect === playerID) server.aSelect = -1
            else if (server.wSelect === playerID) server.wSelect = -1
            else return
            break;
        //player requesting to join team autumn. If team autumn equals -1 (indicating not chosen), player is fitted to autumn team
        case 1:
            if (server.aSelect === -1) server.aSelect = playerID
            else return
            if (server.wSelect === playerID) server.wSelect = -1
            break;
        //same as case 1, but for team winter.
        case 2:
            if (server.wSelect === -1) server.wSelect = playerID
            else return
            if (server.aSelect === playerID) server.aSelect = -1
            break;
        default:
            console.log("Issue within the 'update teams' function")
            console.log(nums)
    }
    p1 = checkForPlayer1(server)
    p2 = checkForPlayer2(server)
    obj = new TeamsState(server.aSelect, server.wSelect, p1, p2)
    console.log(obj)
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

function createServerWrapper(info){
    var nums = info.match(/\d+/g);
    var playerID = parseInt(nums[0], 10)
    var gameID = parseInt(nums[1], 10)
    createServer(gameID, playerID)
}

function playersMessage(server, obj){
    var player1, player2
    player1 = playerBase[server.player1]
    player1.ws.send(JSON.stringify(obj))
    player2 = playerBase[server.player2]
    if(player2 !== null) player2.ws.send(JSON.stringify(obj))
}

function deletePlayer(info){
    console.log(info)
    var nums = info.match(/\d+/g);
    var player = parseInt(nums[0])
    var server = states[nums[1]]
    if(server.player1 === player){
        if(server.player2 !== null){
            let player2 = playerBase[server.player2]
            let obj = new Data("kick", "");
            player2.ws.send(JSON.stringify(obj))
        }
        delete states[nums[1]]
    }
    else{
        server.player2 = null;
        server.full = false;
        server.aSelect = -1;
        server.wSelect = -1;
        let obj = new Data("kick", "")
        let player1 = playerBase[server.player1];
        player1.ws.send(JSON.stringify(obj));
    }
    delete playerBase[nums[0]]
}

function sendServerHostID(info, ws){
    let server = states[parseInt(info)]
    console.log(server)
    if (server === null) return;
    console.log(server.player1)
    var hostID = server.player1.toString();
    let obj = new Data("host_player_ID", hostID)
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
    ws.send(JSON.stringify(req))
}

function sendServerList(ws){
    for (const gameID in states ){
        let server = states[gameID]
        if(!server.full)
            ws.send(JSON.stringify(new Data("server", JSON.stringify(server.gameID))))
        } // sends the gameID for each server

    let final = new Data("create_server_list", "")
    ws.send(JSON.stringify(final))
}

function joinServerRequest(s, ws){
    var nums = s.match(/\d+/g);
    var server = states[nums[0]];
    var obj;
    if(!server.full) {
        server.player2 = parseInt(nums[1]);
        server.full = true;
        let num = server.gameID
        let str = num.toString();
        obj = new Data("successful_join", str);
    } else {
        obj = new Data("unsuccessful_join", "");
    }
    playersMessage(server, obj)
}

function createServer(gameID, info){
    var num = parseInt(info, 10)
    states[gameID] = new Server(false, gameID, num)
    var player = playerBase[num];
    player.ws.send(JSON.stringify(new Data("server_created", "")))
}



wss.on('listening', ()=>{
    console.log(`server is listening on port ${port}`)
})
