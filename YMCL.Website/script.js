var openw = false;
function list() {
  if (openw == true) {
    document.getElementById("barr").style.display = "none";
    openw = false
  } else {
    document.getElementById("barr").style.display = "block";
    openw = true
  }
}

var url = window.location.href;
if (url == 'https://ymcl.daiyu-233.top/') {
  window.location = 'https://ymcl.daiyu.fun'
}

function updateTime() {
  const now = new Date();
  let hours = now.getHours();
  let minutes = now.getMinutes();
  let seconds = now.getSeconds();

  // 添加前导零
  hours = String(hours).padStart(2, '0');
  minutes = String(minutes).padStart(2, '0');
  seconds = String(seconds).padStart(2, '0');

  const currentTime = `${hours}:${minutes}:${seconds}`;

  // 更新页面上的时间显示
  document.getElementById('clock').innerHTML = currentTime;
}
updateTime()
// 每秒钟更新一次时间
setInterval(updateTime, 1000);

const DownloadBtn = document.getElementById("DownloadBtn");
DownloadBtn.onclick = function () {
  window.open("https://github.com/DaiYu-233/YMCL/releases");
};

const w = document.getElementById("fk");
w.onclick = function () {
  window.open("https://support.qq.com/product/542108");
};
// const wwwww = document.getElementById("DownloadBtnx86");
// wwwww.onclick = function () {
//   window.open("./YMCL-x86.exe");
// };
const ck = document.getElementById("ck");
ck.onclick = function () {
  window.open("https://github.com/DaiYu-233/YMCL");
};
const wg = document.getElementById("git");
wg.onclick = function () {
  window.open("https://github.com/DaiYu-233/YMCL");
};
const csh = document.getElementById("csh");
csh.onclick = function () {
  window.open("https://support.qq.com/products/542108/blog/927530");
};
const yx = document.getElementById("yx");
yx.onclick = function () {
  window.open("https://txc.qq.com/products/542108/blog/1027959");
};
window.onscroll = function () {
  var scrollTop = document.documentElement.scrollTop;
  if (scrollTop < 4648) {
    document.getElementById("innerr").style.left =
      (scrollTop - 840) * -1 + "px";
    // console.log((scrollTop - 840) * -1);
  }
};

function Launch() {
  const id = document.getElementById("keywords");
  window.open("ymcl://" + id.value);
}
