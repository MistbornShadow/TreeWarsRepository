class Game {
    constructor(player1, player2) {
        this.player1 = player1;
        this.player2 = player2;
        this.update = false;
        this.updateCommand = null;
        this.p1Resource = 0;
        this.p2Resource = 0;
        this.p1IntervalResourceIncrease = 10;
        this.p2IntervalResourceIncrease = 10;
        this.AutumnUnits = {};
        this.WinterUnits = {};
        this.autumnUnitCounter = 0;
        this.winterUnitCounter = 0;
        this.autumnUnitDeathCounter = 0;
        this.winterUnitDeathCounter = 0;
    }

    addResourceP1(resource){
        this.p1Resource += resource;
        this.update = true;
        this.updateCommand = "p1_resource_change";
    }

    addResourceP2(resource){
        this.p2Resource += resource;
        this.update = true;
        this.updateCommand = "p2_resource_change";
    }

    addResourceBoth(resource){
        this.p1Resource += resource;
        this.p2Resource += resource;
        this.update = true;
        this.updateCommand = "both_resource_change";
    }

    subtractResourceP1(cost){
        this.p1Resource -= cost;
        this.update = true;
        this.updateCommand = "p1_resource_change";
    }

    subtractResourceP2(cost){
        this.p2Resource -= cost;
        this.update = true;
        this.updateCommand = "p2_resource_change";
    }

    subtractResourceBoth(cost){
        this.p1Resource -= cost;
        this.p2Resource -= cost;
        this.update = true;
        this.updateCommand = "both_resource_change";
    }

    intervalResourceIncrease(){
        this.addResourceP1(this.p1IntervalResourceIncrease);
        this.addResourceP2(this.p2IntervalResourceIncrease);
        this.updateCommand = "both_resource_change";
        this.update = true;
    }

    getP1Resource(){
        return this.p1Resource;
    }

    getP2Resource(){
        return this.p2Resource;
    }
}
exports.Game = Game