/**
 * Created by iflytek on 2019/11/19.
 *  
 * 在线语音合成 WebAPI 接口调用示例 接口文档（必看）：https://www.xfyun.cn/doc/tts/online_tts/API.html
 * 错误码链接：
 * https://www.xfyun.cn/doc/tts/online_tts/API.html
 * https://www.xfyun.cn/document/error-code （code返回错误码时必看）
 * 
 */

let isChrome = navigator.userAgent.toLowerCase().match(/chrome/)
let tts_notSupportTip = isChrome ? '您的浏览器暂时不支持体验功能，请升级您的浏览器' : '您现在使用的浏览器暂时不支持体验功能，<br />推荐使用谷歌浏览器Chrome'

function getWebsocketUrl () {
  return new Promise((resolve, reject) => {
    var apiKey = tts_APIKEY
    var apiSecret = tts_APISECRET
    var url = 'wss://tts-api.xfyun.cn/v2/tts'
    var host = location.host 
    var date = new Date().toGMTString()
    var algorithm = 'hmac-sha256'
    var headers = 'host date request-line'
    var signatureOrigin = `host: ${host}\ndate: ${date}\nGET /v2/tts HTTP/1.1`
    var signatureSha = CryptoJS.HmacSHA256(signatureOrigin, apiSecret)
    var signature = CryptoJS.enc.Base64.stringify(signatureSha)
    var authorizationOrigin = `api_key="${apiKey}", algorithm="${algorithm}", headers="${headers}", signature="${signature}"`
    var authorization = btoa(authorizationOrigin)
    url = `${url}?authorization=${authorization}&date=${date}&host=${host}`
    resolve(url)
  })
}

let audioCtx
let source
let btnState = {
  unTTS: '立即合成',
  ttsing: '正在合成',
  endTTS: '立即播放',
  play: '停止播放',
  pause: '继续播放',
  endPlay: '重新播放',
  errorTTS: '合成失败'
}
class Experience {
  constructor ({
                 speed = tts_speed,
                 voice = tts_volume,
                 pitch = tts_pitch,
				 bgs = tts_bgs,
				 ram = tts_ram,
				 rdn = tts_rdn,
                 text = '',
                 engineType = tts_ent,
                 voiceName = tts_vcn,
                 playBtn = '.js-base-play',
                 defaultText = ''
               } = {}) {
    this.speed = speed
    this.voice = voice
    this.pitch = pitch
	this.bgs = bgs
	this.ram = ram
	this.rdn = rdn
    this.text = text
    this.defaultText = defaultText
    this.engineType = engineType
    this.voiceName = voiceName
    this.playBtn = playBtn
    this.playState = ''
    this.audioDatas = []
    this.pcmPlayWork = new Worker('Build/VoiceJs/tts_Js/transform.worker.js')
    this.pcmPlayWork.onmessage = (e) => {
      this.onmessageWork(e)
    }
  }

  setConfig ({
               speed,
               voice,
               pitch,
			   bgs,
			   ram,
			   rdn,
               text,
               defaultText,
               engineType,
               voiceName
             }) {
    speed && (this.speed = speed)
    voice && (this.voice = voice)
    pitch && (this.pitch = pitch)
	bgs && (this.bgs = bgs)
	ram && (this.ram = ram)
	rdn && (this.rdn = rdn)
    text && (this.text = text)
    defaultText && (this.defaultText = defaultText)
    engineType && (this.engineType = engineType)
    voiceName && (this.voiceName = voiceName)
    this.resetAudio()
  }

  onmessageWork (e) {
    switch (e.data.command) {
      case 'newAudioData': {
        this.audioDatas.push(e.data.data)
        if (this.playState === 'ttsing' && this.audioDatas.length === 1) {
          this.playTimeout = setTimeout(() => {
            this.audioPlay()
          }, 1000)
        }
        break
      }
    }
  }

  setBtnState (state) {
    let oldState = this.playState
    this.playState = state
    $(this.playBtn).removeClass(oldState).addClass(state).text(btnState[state])
  }

  getAudio () {
    this.setBtnState('ttsing')
    getWebsocketUrl().then((url) => {
      this.connectWebsocket(url)
    })
  }

  connectWebsocket (url) {
    if ('WebSocket' in window) {
      this.websocket = new WebSocket(url)
    } else if ('MozWebSocket' in window) {
      this.websocket = new MozWebSocket(url)
    } else {
      alert(tts_notSupportTip)
      return
    }
    let self = this
    this.websocket.onopen = (e) => {
      var params = {
        'common': {
          'app_id': tts_APPID 
        },
        'business': {
          'ent': self.engineType,
          'aue': 'raw',
          'auf': 'audio/L16;rate=16000',
          'vcn': self.voiceName,
          'speed': self.speed,
          'volume': self.voice * 10,
          'pitch': self.pitch,
          'bgs': self.bgs,
		  'ram':self.ram,
		  'rdn':self.rdn,
          'tte': 'UTF8'
        },
        'data': {
          'status': 2,
          'text': Base64.encode(self.text)
        }
      }
      this.websocket.send(JSON.stringify(params))
    }
    this.websocket.onmessage = (e) => {
      let jsonData = JSON.parse(e.data)
      // 合成失败
      if (jsonData.code !== 0) {		
        alert(`${jsonData.code}:${jsonData.message}`)
        self.resetAudio()
        this.websocket.close()
        return
      }
      self.pcmPlayWork.postMessage({
        command: 'transData',
        data: jsonData.data.audio
      })
		
      if (jsonData.code === 0 && jsonData.data.status === 2) {
		////////////////////////////回调Unity函数////////////////////////////////////
		window.gameInstance.SendMessage('RainierVoiceSystem','SynthesisCallBack',jsonData.data.audio);
        this.websocket.close()
      }
    }
    this.websocket.onerror = (e) => {
      console.log(e)
      console.log(e.data)
    }
    this.websocket.onclose = (e) => {
      console.log(e)
    }
  }
  
  
  resetAudio () {
    this.audioPause()
    this.setBtnState('unTTS')
    this.audioDatasIndex = 0
    this.audioDatas = []
    this.websocket && this.websocket.close()
    clearTimeout(this.playTimeout)
  }

  audioPlay () {
    try {
      if (!audioCtx) {
        audioCtx = new (window.AudioContext || window.webkitAudioContext)()
      }
      if (!audioCtx) {
        alert(tts_notSupportTip)
        return
      }
    } catch (e) {
      alert(tts_notSupportTip)
      return
    }
    this.audioDatasIndex = 0
    if (this.audioDatas.length) {
      this.playSource()
      this.setBtnState('play')
    } else {
      this.getAudio()
    }
  }

  audioPause (state) {
    if (this.playState === 'play') {
      this.setBtnState(state || 'endPlay')
    }
    clearTimeout(this.playTimeout)
    try {
      source && source.stop()
    } catch (e) {
      console.log(e)
    }
  }

  playSource () {
    let bufferLength = 0
    let dataLength = this.audioDatas.length
    for (let i = this.audioDatasIndex; i < dataLength; i++) {
      bufferLength += this.audioDatas[i].length
    }
    let audioBuffer = audioCtx.createBuffer(1, bufferLength, 22050)
    let offset = 0
    let nowBuffering = audioBuffer.getChannelData(0)
    for (let i = this.audioDatasIndex; i < dataLength; i++) {
      let audioData = this.audioDatas[i]
      if (audioBuffer.copyToChannel) {
        audioBuffer.copyToChannel(audioData, 0, offset)
      } else {
        for (let j = 0; j < audioData.length; j++) {
          nowBuffering[offset + j] = audioData[j]
        }
      }
      offset += audioData.length
      this.audioDatasIndex++
    }

    source = audioCtx.createBufferSource()
    source.buffer = audioBuffer
    source.connect(audioCtx.destination)
    source.start()
    source.onended = (event) => {
      if (this.playState !== 'play') {
        return
      }
      if (this.audioDatasIndex < this.audioDatas.length) {
        this.playSource()
      } else {
        this.audioPause('endPlay')
      }
    }
  }
}

let experience = new Experience({
  speed: 50,
  voice: 50,
  pitch: 50,
  playBtn: `.audio-ctrl-btn`
})



$('.audio-ctrl-btn').on('click', function () {
  let text = $('textarea').val()
  if (text !== experience.text) {
    experience.setConfig({
      text
    })
  }
  if (experience.playState === 'play') {
    experience.audioPause()
  } else {
    experience.audioPlay()
  }
})
$('textarea').on('change', function () {
  var text = this.value || ''
  experience.setConfig({text})
  $(this).keyup()
})


/////////////Unity中调用接口///////////////////////

class Synthesis
{
	//开始合成
	OnSynthesisStart(str) 
	{
		if(experience.text !== str){
			let text = str;
			experience.setConfig({
				text
			})
		}
		if (experience.playState === 'play') {
			experience.audioPause();
		} 
		else {
		experience.audioPlay();
		}
	}	
}
var synthesis = new Synthesis()
