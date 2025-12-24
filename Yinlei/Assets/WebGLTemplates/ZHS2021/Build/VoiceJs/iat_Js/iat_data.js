/**
 * 	在此Js文件中，配置讯飞语音功能业务参数(若无特殊要求，一般只修改APPID、APIKEY、APISECRET参数即可)，详细可阅读讯飞官网文档，地址如下：
 *	在线语音识别 WebAPI 接口调用示例 接口文档（必看）：https://doc.xfyun.cn/rest_api/语音听写（流式版）.html
 */
 
//讯飞APPID、APIKEY、APISECRET（自行在讯飞官网申请）
const iat_APPID = '5cece5b4'
const iat_APIKEY = '09f3a47bce836bde098242d2cfdba1ff'
const iat_APISECRET = 'cfe22ce2ba7cd16befc0beee00c195d7'

//语种，可选值
const iat_language = 'zh_cn'
//应用领域，可选值
const iat_domain = 'iat'
//方言
const iat_accent = 'mandarin'
//是否开启标点符号添加（仅中文支持），可选值: 0,1
const iat_ptt = 1

