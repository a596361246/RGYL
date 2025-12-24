var screenRecorder;
var screenVideo;
var videoData;

async function StartRecordScreen() //开始录屏
{

    if (screenVideo==null)
    {
        screenVideo=document.createElement("sceen-video");
        screenVideo.setAttribute('type','video');
        screenVideo.setAttribute('autoplay','true');
        screenVideo.setAttribute('id','sceen-video');
        screenVideo.setAttribute('controls','true');
        screenVideo.setAttribute('controlsList','nodownload');
    }
    let captureStream

    try{
      captureStream = await navigator.mediaDevices.getDisplayMedia({
        video: true,
        // audio: true,   not support
        cursor: 'always'
      })
    }catch(e){
        // 取消录屏或者报错
      alert("Could not get stream")
      return
    }

     
    // 删除之前的 Blob
    window.URL.revokeObjectURL(screenVideo.src)

    screenVideo.autoplay = true
     
    // 实时的播放录屏
    screenVideo.srcObject = captureStream
     
    // new 一个媒体记录
    screenRecorder = new MediaRecorder(captureStream)
    screenRecorder.start()
     
    captureStream.getVideoTracks()[0].onended = () => {
        // 录屏结束完成
      screenRecorder.stop()
    }

    screenRecorder.addEventListener("dataavailable", event => {
        // 录屏结束，并且数据可用
        console.log("dataavailable------------")
        videoData=event.data;
      let videoUrl = URL.createObjectURL(event.data, {type: 'video/mp4'})
      screenVideo.srcObject = null
      screenVideo.src = videoUrl
      screenVideo.autoplay = false
      RecordEndCallBack();
    })
    RecordStartCallBack();
};

function StopRecordScreen() //停止录屏
{
	try{
		let tracks = screenVideo.srcObject.getTracks()
		tracks.forEach(track => track.stop())
		screenRecorder.stop()
		WebSetDataToUnity("stopRecordScreen","结束录屏")
    }catch(e){
        // 录屏失败，未选择屏幕录制
      alert("no screen recording selected")
      return
    }
};

 function DownloadRecordScreen() //下载 录屏 到本地
 {
    const url = screenVideo.src
    const name = new Date().toISOString().slice(0, 19).replace('T',' ').replace(" ","_").replace(/:/g,"-")
    const a = document.createElement('a')
    a.style = 'display: none'
    a.download = `${name}.mp4`
    a.href = url
    document.body.appendChild(a)
    a.click()
    WebSetDataToUnity("downloadRecordScreen","下载录屏")
};

function UploadRecordScreen()
{
    let fd = new FormData()
    const name = new Date().toISOString().slice(0, 19).replace('T',' ').replace(" ","_").replace(/:/g,"-")
    fd.append("file", videoData, `${name}.mp4`)
    let xhr = new XMLHttpRequest();
    xhr.addEventListener("error", function (e) {
        WebSetDataToUnity("uploadVideoError",e)
    }, false);
    xhr.addEventListener("load", function (e) {
        let jsonStr = JSON.parse(e.target.response);
		WebSetDataToUnity("uploadVideoOK",JSON.stringify(jsonStr.rt));
        console.log(jsonStr.rt);
        // console.log(JSON.stringify(jsonStr.rt));
    }, false);

    xhr.open("POST", "https://newbase.zhihuishu.com/upload/commonUploadFile");
    xhr.send(fd);
};


