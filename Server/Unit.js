class Unit {
    constructor(type, hpMax, hpCurr, attack) {
        this.type = type;
        this.hpMax = hpMax;
        this.hpCurr = hpCurr;
        this.attack = attack;
    }

    static createKnight(){
        return new Unit("knight", 100, 100, 20);
    }
}
exports.Unit = Unit