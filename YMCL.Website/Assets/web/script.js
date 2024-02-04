function TrunSideNav() {
  if (num % 2 === 0) {
    document.getElementById("SideNav").style.width = "100vw";
  } else {
    document.getElementById("SideNav").style.width = "0";
  }
  num++;
}
var num = 0;
