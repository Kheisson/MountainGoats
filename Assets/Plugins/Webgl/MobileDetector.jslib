mergeInto(LibraryManager.library, {
  IsMobileDevice: function() {
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    var isMobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(userAgent);

    return isMobile ? 1 : 0;
  },

  RequestFullscreen: function() {
    if (document.fullscreenElement) return;

    var canvas = document.getElementsByTagName('canvas')[0];
    var isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

    if (isIOS) {
      // iOS requires a user gesture to enter fullscreen
      var requestFullscreen = function() {
        if (canvas.webkitRequestFullscreen) {
          canvas.webkitRequestFullscreen();
        } else if (canvas.webkitEnterFullscreen) {
          canvas.webkitEnterFullscreen();
        } else if (canvas.webkitRequestFullScreen) {
          canvas.webkitRequestFullScreen();
        }
      };
      
      // Try to enter fullscreen immediately if we have a user gesture
      try {
        requestFullscreen();
      } catch (e) {
        // If that fails, we'll need to wait for a user gesture
        document.addEventListener('touchstart', function onTouchStart() {
          requestFullscreen();
          document.removeEventListener('touchstart', onTouchStart);
        }, { once: true });
      }
    } else {
      // Standard fullscreen request for other platforms
      if (canvas.requestFullscreen) {
        canvas.requestFullscreen();
      } else if (canvas.mozRequestFullScreen) {
        canvas.mozRequestFullScreen();
      } else if (canvas.webkitRequestFullscreen) {
        canvas.webkitRequestFullscreen();
      } else if (canvas.msRequestFullscreen) {
        canvas.msRequestFullscreen();
      }
    }
  },

  ExitFullscreen: function() {
    if (!document.fullscreenElement) return;

    var isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

    if (isIOS) {
      if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
      } else if (document.webkitCancelFullScreen) {
        document.webkitCancelFullScreen();
      }
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
      } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
      }
    }
  },

  IsInFullscreen: function() {
    return document.fullscreenElement ? 1 : 0;
  }
});