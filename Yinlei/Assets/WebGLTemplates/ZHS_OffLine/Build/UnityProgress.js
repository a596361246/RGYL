function UnityProgress(gameInstance, progress) {
  if (!gameInstance.Module)
    return;
  if (!hasinit) {
    init();
  }
  if(hasinit){
    ele.innerHTML = (parseInt(100 * progress)) + "%";
    if (progress == 1)
    {
      gameInstance.progress.style.display = "none";
      document.getElementById("version").hidden = true;
      document.getElementById("explanation").hidden = true;
    }
  }
}

var hasinit = false;
var ele;
function init()
{
  hasinit = true;
  SetMeta('experimentId', getQueryVariable('experimentId'));
  SetMeta('courseId', getQueryVariable('courseId'));
  ele = document.createElement("div");
  ele.className = "title";
  ele.style.color = GetMeta("progressColor");
  ele.style.textAlign = 'center';
  ele.style.position = 'relative';
  initload();

  gameInstance.progress = document.createElement("div");
  gameInstance.progress.className = "progress " + gameInstance.Module.splashScreenStyle;
  gameInstance.container.appendChild(gameInstance.progress);
  gameInstance.container.style.color = '#ff0000';
  gameInstance.progress.appendChild(ele);
  gameInstance.progress.appendChild(document.getElementById("loadcanvas"));
  var tip = document.createElement("div");
  tip.className = "tip";
  gameInstance.progress.appendChild(tip);	
  tip.innerHTML = GetMeta('tipContent');
  tip.style.color = GetMeta("tipColor");

  var experimentId = getQueryVariable('experimentId');
  var courseId = getQueryVariable('courseId');
  var data = new FormData();
  data.append("experimentId", experimentId);
  data.append("courseId", courseId);
  var xhr = new XMLHttpRequest();
  xhr.withCredentials = true;
   gameInstance.container.style.backgroundImage="url(Build/background.png)";
  // xhr.addEventListener("readystatechange", function() {
  //   if(this.readyState === 4) {
  //     var j = JSON.parse(this.responseText);
  //     if(j['code']==0){
  //       var img_back = 'url('+j['data']['experimentInfo']['picUrl1']+')';
  //       gameInstance.container.style.backgroundImage=img_back;
  //       document.title = "è™šæ‹Ÿå®žéªŒ | "+ j['data']['courseName'];
  //     }else{
  //       gameInstance.container.style.backgroundImage="url(https://ar.zhihuishu.com/static/source/defaultbackground.png)";
  //     }
  //   }
  // });
  // xhr.open("POST", "https://virtualcourse.zhihuishu.com/vcCourse/findExperimentInfo");
  // xhr.send(data);
}


function initload() {
  var $ = {};
  
  $.Particle = function( opt ) {
    this.radius = 7;
    this.x = opt.x;
    this.y = opt.y;
    this.angle = opt.angle;
    this.speed = opt.speed;
    this.accel = opt.accel;
    this.decay = 0.01;
    this.life = 1;
  };
  
  $.Particle.prototype.step = function( i ) {
    this.speed += this.accel;
    this.x += Math.cos( this.angle ) * this.speed;
    this.y += Math.sin( this.angle ) * this.speed;
    this.angle += $.PI / 64;
    this.accel *= 1.01;
    this.life -= this.decay;
    
    if( this.life <= 0 ) {
      $.particles.splice( i, 1 );
    }
  };
  
  $.Particle.prototype.draw = function( i ) {
    var circleColor =GetMeta("circleColor");
    if(circleColor==''){
      $.ctx.fillStyle = $.ctx.strokeStyle = 'hsla(' + ( $.tick + ( this.life * 120 ) ) + ', 100%, 60%, ' + this.life + ')';
    }
    else{
      $.ctx.fillStyle=$.ctx.strokeStyle = 'rgba('+parseInt(circleColor.substring(1,3),16)+','+parseInt(circleColor.substring(3,5),16)+','
        +parseInt(circleColor.substring(5,7),16)+','+this.life+')';
    }
    $.ctx.beginPath();
    if( $.particles[ i - 1 ] ) {
      $.ctx.moveTo( this.x, this.y );
      $.ctx.lineTo( $.particles[ i - 1 ].x, $.particles[ i - 1 ].y );
    }
    $.ctx.stroke();
    
    $.ctx.beginPath();
    $.ctx.arc( this.x, this.y, Math.max( 0.001, this.life * this.radius ), 0, $.TWO_PI );
    $.ctx.fill();
    
    var size = Math.random() * 1.25;
    $.ctx.fillRect( ~~( this.x + ( ( Math.random() - 0.5 ) * 35 ) * this.life ), ~~( this.y + ( ( Math.random() - 0.5 ) * 35 ) * this.life ), size, size );
  }
  
  $.step = function() {
    $.particles.push( new $.Particle({
      x: $.width / 2 + Math.cos( $.tick / 20 ) * $.min / 2,
      y: $.height / 2 + Math.sin( $.tick / 20 ) * $.min / 2,
      angle: $.globalRotation + $.globalAngle,
      speed: 0,
      accel: 0.01
    }));
    
    $.particles.forEach( function( elem, index ) {
      elem.step( index );
    });
    
    $.globalRotation += $.PI / 6;
    $.globalAngle += $.PI / 6;
  };
  
  $.draw = function() {
    $.ctx.clearRect( 0, 0, $.width, $.height );
    
    $.particles.forEach( function( elem, index ) {
      elem.draw( index );
    });
  };
  
  $.init = function() {
    $.canvas = document.createElement( 'canvas' );
    $.canvas.id = "loadcanvas";
    $.ctx = $.canvas.getContext( '2d' );
    $.width = 300;
    $.height = 300;
    $.canvas.width = $.width * window.devicePixelRatio;
    $.canvas.height = $.height * window.devicePixelRatio;
    $.canvas.style.width = $.width + 'px';
    $.canvas.style.height = $.height + 'px';
    $.canvas.style.position = 'absolute';
    $.canvas.style.left = '250px';
    $.canvas.style.top = '-150px';
    $.ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
    $.min = $.width * 0.5;
    $.particles = [];
    $.globalAngle = 0;
    $.globalRotation = 0;
    $.tick = 0;
    $.PI = Math.PI;
    $.TWO_PI = $.PI * 2;
    $.ctx.globalCompositeOperation = 'lighter';
    document.body.appendChild( $.canvas );
    $.loop();
  };
  
  $.loop = function() {
    requestAnimationFrame( $.loop );
    $.step();
    $.draw();
    $.tick++;
  };
  
  $.init();
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