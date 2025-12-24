/**
 * 	在此Js文件中，配置讯飞语音功能业务参数(若无特殊要求，一般只修改APPID、APIKEY、APISECRET参数即可)，详细可阅读讯飞官网文档，地址如下：
 *	在线语音合成 WebAPI 接口调用示例 接口文档（必看）：https://www.xfyun.cn/doc/tts/online_tts/API.html
 */
 
//讯飞APPID、APIKEY、APISECRET（自行在讯飞官网申请）
const tts_APPID = '5cece5b4'
const tts_APIKEY = '09f3a47bce836bde098242d2cfdba1ff'
const tts_APISECRET = 'cfe22ce2ba7cd16befc0beee00c195d7'


//引擎类型，可选值
const tts_ent = 'intp65'
//发音人，可选值: aisjiuxu、aisxping、aisjinger、aisbabyxu、xiaoyan
const tts_vcn = 'xiaoyan'
//语速，可选值：[0-100]
const tts_speed = 50
//音量，可选值：[0-100]
const tts_volume = 100
//音高，可选值：[0-100]
const tts_pitch = 50
//合成音频的背景音，可选值: 0,1
const tts_bgs = 0
//是否读出标点，可选值: 0,1
const tts_ram = '0'
//合成音频数字发音方式，可选值: 0,1,2,3
const tts_rdn = '0'



