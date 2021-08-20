class Server {
    constructor(full, gameID, player1) {
        this.full = full
        this.gameID = gameID
        this.player1 = player1
        this.player2 = null
        this.aSelect = -1
        this.wSelect = -1
        this.Game = null
    }
}
exports.Server = Server