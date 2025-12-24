
window.recorder = new MP3Recorder({
    debug:true,
    funOk: function () {
        //console.log('初始化成功');
    },
    funCancel: function (msg) {
        recorder = null;
    }
});

window.voiceRecorder = {};
var mp3Blob;
var audio;

//开始录音
window.voiceRecorder.RecorderStart = function() {
	if(window.recorder === null){
		console.log('没有检测到麦克风');
		return;		
	}	
	if(audio){
		audio.pause()		
	}
    window.recorder.start();
};

//停止录音
window.voiceRecorder.RecorderStop = function(_isBackToU3d) {
	if(window.recorder === null){
		console.log('没有检测到麦克风');
		return;		
	}		
    window.recorder.stop();
    window.recorder.getMp3Blob(function (blob) {     
		var fileReader = new FileReader();
		fileReader.readAsArrayBuffer(blob);	
		fileReader.onload = function(event) {		
			mp3Blob = blob;
		
			//录音数据若要回传到Unity,则开始数据处理
			if(_isBackToU3d){
				var base64Str = _arrayBufferToBase64(fileReader.result);
				//对base64录音数据分段回传处理，防止数据过大程序崩溃
				_splitBase64Data(base64Str); 
			}							
		};
    });
};

//播放录音
window.voiceRecorder.AudioPlay = function(){
	if(window.recorder === null){
		console.log('没有检测到麦克风');
		return;		
	}	
	if(mp3Blob)	{
		if(audio){
			audio.pause()		
		}
		var audioBlob = URL.createObjectURL(mp3Blob);
		audio = new Audio(audioBlob)
		audio.play()
	}		
};

//Base64转码
function _arrayBufferToBase64( buffer ) {
    var binary = '';
    var bytes = new Uint8Array( buffer );
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode( bytes[ i ] );
    }
    return window.btoa( binary );
};

//Base64数据分段处理
function _splitBase64Data(data){
	if( data.length < 2000000){
		var num = 1;
	}else{
		num = Math.ceil(data.length/2000000);
	}
	length = data.length/num;
	console.log('录音数据回传次数'+num);
	var result = [];
	
	for(var i=0; i<data.length; i+=length){
		result.push(data.slice(i,i+length));
	}
	
	for (i=0;i<result.length;i++){
		//window.gameInstance.SendMessage('VoiceManager','RecorderCallBack',result[i].toString()+'|'+data.length)
		//VoiceCallBack(result[i].toString()+'|'+data.length);
	}
	console.log('MP3导出成功，Base64长度',data.length);
}




