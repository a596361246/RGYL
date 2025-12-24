var u2;
var url="https://studyservice.zhihuishu.com/api/stuExperiment/submitExperimentResult";
var timeUrl="https://studyservice.zhihuishu.com/api/stuExperiment/systemTime";
//var url="studyservice.zhihuishu.com/api/videoquestion/mytest";
var id= -1;
var startTime=-1000000000000;
var endTime=-1000000000000;
var openLinkTime=-1000000000000
function UnityGetDataFromWeb(identifier)
{                                   
}
function UnitySetDataFromWeb(identifier, data)
{
    if(identifier == "eval")
    {
        eval(data);
    }

    if(identifier == 'getQueryVariable')
    {
        WebSetDataToUnity('getQueryVariable', data + '=' + getQueryVariable(data))
    }

    if(identifier == 'getCookie')
    {
        WebSetDataToUnity('getCookie', data + '=' + getUUID())
    }
    
    if (identifier == 'getToken')
    {
        WebSetDataToUnity('getToken', data + '=' + getToken())
    }

    if (identifier == 'getAudio')
    {
        WebSetDataToUnity('getAudio', "audio" + '=' + data)
    }

    if(identifier =="getExperimentId")
    {
        WebSetDataToUnity('getExperimentId', data + '=' + GetMeta("experimentId"))
    }
    if(identifier =="getCourseId")
    {
        WebSetDataToUnity('getCourseId', data + '=' + GetMeta("courseId"))
    }
    if(identifier =="getAppId")
    {
        WebSetDataToUnity('getAppId', data + '=' + GetMeta("appId"))
    }
    if(identifier =="getPlatformType")
    {
        WebSetDataToUnity('platformType', data + '=' + GetMeta("platformType"))
    }
    if(identifier =="getSecret")
    {
        WebSetDataToUnity('getSecret', data + '=' + GetMeta("secret"))
    }
    if (identifier == 'getTicket')
    {
        WebSetDataToUnity('getTicket', data + '=' + getTicket())
    }
    if (identifier == 'getUrls')
    {
        readJson('Build/urls.json', getUrls);
    }
    if (identifier=="getFileUrl")
    {
        WebSetDataToUnity('getFileUrl', data)
    }
    if(identifier=="getLocationParams")
    {
        WebSetDataToUnity('getLocationParams', data + '#' + getLocationParams())
    }
    if(identifier=="getLocationUrl"){
        WebSetDataToUnity('getLocationUrl', data + '=' + getLocationUrl())
    }
    if(identifier=="getOpenLinkTime"){
        WebSetDataToUnity('getOpenLinkTime',data+"="+openLinkTime)
    }

    if("startRecording"==identifier)
    {
        startRecording();
        return;
    }
    if("stopRecording"==identifier)
    {
        stopRecording();
        return;
    }
    if("playRecording"==identifier)
    {
        playRecording();
        return;
    }
    if("stopPlay"==identifier)
    {
        stopPlay();
        return;
    }
    if("pausePlay"==identifier)
    {
        pausePlay();
        return;
    }
    if("continuePlay"==identifier)
    {
        continuePlay();
        return;
    }
    if("uploadRecording"==identifier)
    {
        uploadRecording();
        return;
    }

    if (identifier=="startRecordScreen")
    {
        StartRecordScreen();
    }
    if (identifier=="stopRecordScreen"){
        StopRecordScreen();
    }
    if (identifier=="downloadRecordScreen"){
        DownloadRecordScreen();
    }
    if(identifier=="uploadRecordScreen"){
        UploadRecordScreen();
    }

    if(identifier == "command")
    {
        WebSetDataToUnity("command", data);
    }
}
function WebSetDataToUnity(identifier, data) 
{
    if (typeof(gameInstance)=="undefined") {
        setTimeout(() => {
            gameInstance.SendMessage('_Communication', 'Receive', identifier + '_' + data);
        }, 2000);
    }else{
        gameInstance.SendMessage('_Communication', 'Receive', identifier + '_' + data);
    }
}
function GetMeta(metaName)
{
    var metas = document.getElementsByTagName("meta");
    for (let i = 0; i < metas.length; i++) {
        if (metas[i].getAttribute('name') == metaName) {
            return metas[i].getAttribute('content');
        }
    }
    return '';
}
function SetMeta(metaName, data)
{
    var metas = document.getElementsByTagName("meta");
    for (let i = 0; i < metas.length; i++) {
        if (metas[i].getAttribute('name') == metaName) {
            metas[i].setAttribute('content', data);
            return;
        }
    }
}
function getQueryVariable(variable)
{
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i=0;i<vars.length;i++) {
        var pair = vars[i].split("=");
        if(pair[0] == variable){return pair[1];}
    }
    return(-1);
}
function getToken()
{
    var query = window.location.search.substring(1);
    var vars = query.split("token=");
    return vars[1];
}
function getLocationParams()
{
    var query = window.location.search.substring(1);
    return query;
}
function getLocationUrl(){
    var query=window.location;
    return query;
}
function getTicket()
{
    var query = window.location.search.substring(1);
    if(query.indexOf("ticket=") == -1) return "";
    var vars = query.split("ticket=");
    return vars[1];
}

function readJson(path, loadFunc)
{
    var url = path;
    var request = new XMLHttpRequest();
    request.open("get", url);
    request.send(null);
    request.onload = function () {
        if (request.status == 200) {
            loadFunc(request.responseText);
        }
        else
        {
            loadFunc("");
        }
    }
}

function getUrls(json)
{
    WebSetDataToUnity('getUrls', 'urls=' + json);
}

function getUUID()
{
    var info = getQueryVariable("info");
    var uuid ="";

    if(info != -1)
    {
        var json = JSON.parse(decodeURIComponent(info));
        console.log(json);
        uuid=json['uuid'];
        //return json['uuid'];
    }
    else uuid=getCASLOGC("uuid");
    var taskid=getQueryVariable("taskId");
    if(taskid==-1)
        return uuid;
    else
        return uuid+"&"+taskid;
}

function getCASLOGC(key)
{
    var r =  unescape(getCookie("CASLOGC"));
    var json = JSON.parse(r);
    if (json[key] == null) return "";
    return json[key];
}

function getCookie(name)
{
    var strcookie = document.cookie;
    var arrcookie = strcookie.split("; ");
    for (var i = 0; i < arrcookie.length; i++)
    {
        var arr = arrcookie[i].split("=");
        if (arr[0] == name)
        {
            return arr[1];
        }
    }
    return "{}";
}
//代码调用这个方法
function uploadImage(){
    if(document.getElementById("inputimg")==null){
        var input = document.createElement("input");
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/png');
        input.setAttribute('id', 'inputimg');
        input.setAttribute('hidden', 'true');
        input.setAttribute('onchange', 'uploadImg(event,this)');
        document.body.appendChild(input);
    }
    document.getElementById("inputimg").click();
}

function uploadImg(e,dom) {
    var URL = window.URL || window.webkitURL || window.mozURL
    var e = event || e
    var fileObj =
        dom instanceof HTMLElement
        ? dom.files[0]
        : $(dom)[0].files[0]
    console.log(e)
    console.log(dom)
    var container = document.querySelector('.preview')
    var img = new Image()
    img.src = URL.createObjectURL(fileObj)
    console.log(img);
    img.onload = function() {
        //后缀名使用的时候需要判断一下,这里的r即为图片字符串数据
        var r = getImageBase64(img,'png')
        console.log(r);
    }
}

function getImageBase64(img, ext) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, img.width, img.height);
    var dataURL = canvas.toDataURL("image/" + ext);
    canvas = null;
    return dataURL;
}
