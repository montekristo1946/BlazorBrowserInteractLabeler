// function GetSizeElement(id) {
//     const element = document.getElementById(id);
//     if (element == null) {
//         console.log("Run GetSizeElement null", id)
//         return {
//             Width: -1,
//             Height: -1
//         };
//     }
//     return {
//         Width: element.clientWidth,
//         Height: element.clientHeight
//     };
// }

// function GetScaleBrowse() {
//     var zoom = ( window.outerWidth  ) / window.innerWidth
//     return zoom
// }

function GetBrowseSize() {
    return {
        Width: window.innerWidth,
        Height: window.innerHeight
    };
}

// function GetBackgroundPosition() {
//     const element = document.getElementById("drawing_panel_background");
//     if (element == null) {
//         console.log("Run GetBackgroundPosition null", id)
//         return -1
//     }
//      var t= element.style.backgroundImage
//     var img = new Image();
//     img.src = t
//     console.log("Run GetBackgroundPosition", img.width)
//     return 0
// }

// function GetWidthBrowse() {
//     // console.log(document.body.scrollWidth)
//     // console.log(document.documentElement.scrollWidth)
//     // console.log(document.body.offsetWidth)
//     // console.log(document.documentElement.offsetWidth)
//     // console.log(document.documentElement.clientWidth)
//
//        
//     // var element = document.querySelector('panel_naviation');
//     // var element = document.getElementById('drawing_main_panel');
//     // var scaleX = element.getBoundingClientRect().width / element.offsetWidth;
//
//     // console.log(element.getBoundingClientRect().width,element.offsetWidth)
//     // console.log(scaleX)
//
//     // var zoom = ( window.outerWidth  ) / window.innerWidth
//     // console.log(zoom,window.outerWidth,window.innerWidth)
//    
//     return Math.max(
//         // document.body.scrollWidth,
//         // document.documentElement.scrollWidth,
//         // document.body.offsetWidth,
//         // document.documentElement.offsetWidth,
//         document.documentElement.clientWidth
//     );
// }

function FocusElement (id){
    const element = document.getElementById(id);
    if (element == null)
        return
    element.focus({ preventScroll: true });
}

async function LoadImg (idCanvas,imgBase64, width,height){
    if (imgBase64 == null || width <=0 || height<=0 ) {
        console.log("Fail input data");
        return;
    }
    const img = new Image();
    await new Promise(r => {
        img.onload = r;
        img.src = imgBase64;
    });

    // console.log("chartCanvas  ",imgBase64);
    
    const canvas = document.getElementById(idCanvas);
    if (canvas == null) {
        console.log("canvas null",idCanvas);
        return -1;
    }

    
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0,width,height);
    
    ctx.drawImage(img, 0, 0,width,height);

}