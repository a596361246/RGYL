function Command()
{
    var str = "";
    for (i = 0; i < arguments.length; i++) {
        if(i!=0){
            str += "_";
        }

        str += arguments[i];
    }
    UnitySetDataFromWeb("command", str);
    return str;
}
function CommandManager(){
    var u = new Object();
    u.unityid = 0;
    u.getbyname = function(name){
        var newid = this.unityid ++;
        Command("getbyname", name, newid);
        return UnityObj(newid);
    }

    u.getbyid = function(id){
        var newid = this.unityid ++;
        Command("getbyid", id, newid);
        return UnityObj(newid);
    }

    u.getbytype = function(type){
        var newid = this.unityid ++;
        Command("getbytype", type, newid);
        return UnityObj(newid);
    }

    u.showlist = function(){
        Command("showlist");
    }

    u.object = function (type, value) {
        var newid = this.unityid ++;
        Command("create", type, value, newid);
        return UnityObj(newid);
    }

    u.gameObject = function (name){
        var newid = this.unityid ++;
        Command("newgameobject", name, newid);
        return UnityObj(newid);
    }

    u.instantiate = function (resourcesPath){
        var newid = this.unityid ++;
        Command("instantiate", resourcesPath, newid);
        return UnityObj(newid);
    }

    u.vector2 = function (x, y){
        var newid = this.unityid ++;
        Command("create", "Vector2", "{x:" + x + ",y:" + y + "}", newid);
        return UnityObj(newid);
    }
    u.vector3 = function (x, y, z){
        var newid = this.unityid ++;
        Command("create", "Vector3", "{x:" + x + ",y:" + y + ",z:" + z + "}", newid);
        return UnityObj(newid);
    }

    u.color = function (r, g, b, a){
        var newid = this.unityid ++;
        Command("create", "Color", "{r:" + r + ",g:" + g + ",b:" + b + ",a:" + a + "}", newid);
        return UnityObj(newid);
    }

    return u;
}
function UnityObj(name){
    var u = new Object();
    u.name = name;

    u.showMember = function(){
        Command("showmember", name);
    }

    u.showTypeMember = function(){
        Command("showtypemember", name);
    }

    u.getproperty = function(propertyName){
        var newid = UManager.unityid ++;
        Command("getproperty", name, propertyName, newid);
        return UnityObj(newid);
    }

    u.setproperty = function(propertyName, value){
        if(typeof value == "object"){
            Command("setproperty", name, propertyName, '@' + value.name)
        }else if(typeof value == "string"){
            Command("setproperty", name, propertyName, value)
        }else{
            console.log("值不对");
        }
    }

    u.getfield = function(fieldName){
        var newid = UManager.unityid ++;
        Command("getfield", name, fieldName, newid);
        return UnityObj(newid);
    }

    u.setfield = function(fieldName, value){
        if(typeof value == "object"){
            Command("sefield", name, fieldName, '@' + value.name)
        }else if(typeof value == "string"){
            Command("sefield", name, fieldName, value)
        }else{
            console.log("值不对");
        }
    }

    u.method = function(methodName, ...arr){
        var str = "";
        for (i = 0; i < arr.length; i++) {
            if(i!=0){
                str += ",";
            }

            str += arr[i];
        }
        var newid = UManager.unityid ++;
        Command("method", name, methodName, newid, str);
        return UnityObj(newid);
    }

    u.getcompoment = function (type) {
        var newid = UManager.unityid ++;
        Command("getcompoment", name, type, newid);
        return UnityObj(newid);
    }

    u.add = function (toAdd){
        var newid = UManager.unityid ++;
        Command("operate", "+", name, toAdd.name, newid);
        return UnityObj(newid);
    }

    u.sub = function (toAdd){
        var newid = UManager.unityid ++;
        Command("operate", "-", name, toAdd.name, newid);
        return UnityObj(newid);
    }

    u.mul = function (toAdd){
        var newid = UManager.unityid ++;
        Command("operate", "*", name, toAdd.name, newid);
        return UnityObj(newid);
    }

    u.div = function (toAdd){
        var newid = UManager.unityid ++;
        Command("operate", "/", name, toAdd.name, newid);
        return UnityObj(newid);
    }

    u.show = function (){
        var newid = UManager.unityid ++;
        Command("show", name, newid);
        return UnityObj(newid);
    }

    return u;
}
var UManager = new CommandManager();
