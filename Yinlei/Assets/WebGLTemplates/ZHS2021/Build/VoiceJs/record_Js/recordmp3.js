(function (exports) {

    var MP3Recorder = function (config) {

        var recorder = this;
        config = config || {};
        config.sampleRate = config.sampleRate || 44100;
        config.bitRate = config.bitRate || 128;
		
			// 老的浏览器可能根本没有实现 mediaDevices，所以我们可以先设置一个空的对象
			if (navigator.mediaDevices === undefined) {
			  navigator.mediaDevices = {};
			}
					
			// 一些浏览器部分支持 mediaDevices。我们不能直接给对象设置 getUserMedia 
			// 因为这样可能会覆盖已有的属性。这里我们只会在没有getUserMedia属性的时候添加它。
			if (navigator.mediaDevices.getUserMedia === undefined) {
				navigator.mediaDevices.getUserMedia = function(constraints) {

				// 首先，如果有getUserMedia的话，就获得它
					var getUserMedia = navigator.getUserMedia ||
								   navigator.webkitGetUserMedia ||
                                   navigator.mozGetUserMedia ||
                                   navigator.msGetUserMedia;

					// 一些浏览器根本没实现它 - 那么就返回一个error到promise的reject来保持一个统一的接口
					if (!getUserMedia) {
						return Promise.reject(new Error('getUserMedia is not implemented in this browser'));
					}

					// 否则，为老的navigator.getUserMedia方法包裹一个Promise
					return new Promise(function(resolve, reject) {
						getUserMedia.call(navigator, constraints, resolve, reject);
					});
				}
			}

navigator.mediaDevices.getUserMedia({ audio: true})
.then(function (stream) {
                var context = new AudioContext(),
                    microphone = context.createMediaStreamSource(stream),
                    processor = context.createScriptProcessor(16384, 1, 1),//bufferSize大小，输入channel数，输出channel数
                    mp3ReceiveSuccess, currentErrorCallback;

                config.sampleRate = context.sampleRate;
                processor.onaudioprocess = function (event) {
                    //边录音边转换
                    var array = event.inputBuffer.getChannelData(0);
                    realTimeWorker.postMessage({ cmd: 'encode', buf: array });
                };

                var realTimeWorker = new Worker('Build/VoiceJs/record_Js/worker-realtime.js');
                realTimeWorker.onmessage = function (e) {
                    switch (e.data.cmd) {
                        case 'init':
                            log('初始化成功');
                            if (config.funOk) {
                                config.funOk();
                            }
                            break;
                        case 'end':
                            log('MP3大小：', e.data.buf.length);
                            if (mp3ReceiveSuccess) {
                                mp3ReceiveSuccess(new Blob(e.data.buf, { type: 'audio/mp3' }));
                            }
                            break;
                        case 'error':
                            log('错误信息：' + e.data.error);
                            if (currentErrorCallback) {
                                currentErrorCallback(e.data.error);
                            }
                            break;
                        default:
                            log('未知信息：', e.data);
                    }
                };

                recorder.getMp3Blob = function (onSuccess, onError) {
                    currentErrorCallback = onError;
                    mp3ReceiveSuccess = onSuccess;
                    realTimeWorker.postMessage({ cmd: 'finish' });
                };

                recorder.start = function () {
                    if (processor && microphone) {
                        microphone.connect(processor);
                        processor.connect(context.destination);
                        log('开始录音');
                    }
                }

                recorder.stop = function () {
                    if (processor && microphone) {
                        microphone.disconnect();
                        processor.disconnect();
                        log('录音结束');
                    }
                }

                realTimeWorker.postMessage({
                    cmd: 'init',
                    config: {
                        sampleRate: config.sampleRate,
                        bitRate: config.bitRate
                    }
                });
            })
.catch(function (error) {
                var msg;
                switch (error.code || error.name) {
                    case 'PERMISSION_DENIED':
                    case 'PermissionDeniedError':
                        msg = '用户拒绝访问麦客风';
                        break;
                    case 'NOT_SUPPORTED_ERROR':
                    case 'NotSupportedError':
                        msg = '浏览器不支持麦客风';
                        break;
                    case 'MANDATORY_UNSATISFIED_ERROR':
                    case 'MandatoryUnsatisfiedError':
                        msg = '找不到麦客风设备';
                        break;
                    default:
                        msg = '无法打开麦克风，异常信息:' + (error.code || error.name);
                        break;
                }
                if (config.funCancel) {
                    config.funCancel(msg);
                }
            });
   

        function log(str) {
            if (config.debug) {
                console.log(str);
            }
        }
    }

    exports.MP3Recorder = MP3Recorder;
})(window);
