var recordBtn; //录屏Btn
var voiceBtn; //语音识别 Btn
var toolBtn; //工具栏 Btn
var isRecord=false; //录屏中
var isVoice=false; //语音识别中
var isOpen=false; //工具栏是否打开
RegisterInit();
function RegisterInit() 
{
    if (typeof(gameInstance)=="undefined") {
        setTimeout(() => {
            RegisterFunc();
        }, 2000);
    }else{
        RegisterFunc();
    }
}
function RegisterFunc(){
	toolBtn=document.getElementById("toolBtn");
	toolBtn.addEventListener('click',ToolBarFunc);

	recordBtn=document.getElementById("recordBtn");
	recordBtn.addEventListener('click',RecordBtnFunc);

	voiceBtn=document.getElementById("voiceBtn");
	voiceBtn.addEventListener('click',VoiceBtnFunc);
}

function ToolBarFunc(){
	isOpen=!isOpen;
	recordBtn.hidden=!isOpen;
	voiceBtn.hidden=!isOpen;
}

function RecordBtnFunc(){
	if(isRecord){
		StopRecordScreen();
	}
	else{
		StartRecordScreen()
	}
}
//录屏开始 回调
function RecordStartCallBack(){
    WebSetDataToUnity("startRecordScreen","开始录屏")
    recordBtn.style.backgroundColor="#C8C8C8";
    isRecord=true;
}
//录屏结束 回调
function RecordEndCallBack(){
 	WebSetDataToUnity("stopRecordScreen","结束录屏")
  	recordBtn.style.backgroundColor="#ffffff";
  	isRecord=false;
 	UploadRecordScreen();
}

function VoiceBtnFunc(){
	if(!isVoice){
		voiceRecorder.RecorderStart();
		iatTaste.OnRecognitionStart();
		isVoice=true;
		voiceBtn.style.backgroundColor="#C8C8C8";
	}
	else{
		voiceRecorder.RecorderStop();
		iatTaste.OnRecognitionStop();
		isVoice=false;
		voiceBtn.style.backgroundColor="#ffffff";
	}
}
function StopVoiceFunc(){
	voiceRecorder.RecorderStop(true);
	iatTaste.OnRecognitionStop();
}
//语音识别结果 回调
function RecognitionCallBack(str){
	// window.alert(str);
	WebSetDataToUnity("recognitionCallBack",str);
}
//语音识别结束 回调
function RecognitionEndCallBack(){
	// window.alert("");
	voiceBtn.style.backgroundColor="#ffffff";
}
