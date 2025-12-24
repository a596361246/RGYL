mergeInto(LibraryManager.library, {
    unityFullScreen: function() {
        if (!document.fullscreenElement) {
            container.requestFullscreen();
        } else if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
});