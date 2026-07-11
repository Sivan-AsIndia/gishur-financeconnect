window.fullscreen = function () {
    const btn = document.getElementById("themeToggleBtn1");
    const icon = document.getElementById("fsIcon");

    if (!btn || !icon) return;

    btn.addEventListener("click", function (event) {
        event.preventDefault();
        event.stopPropagation();

        if (!document.fullscreenElement) {
            // ENTER
            document.documentElement.requestFullscreen().then(() => {
                icon.classList.remove("ti-arrows-maximize");
                icon.classList.add("ti-arrows-minimize");
            });
        } else {
            // EXIT
            document.exitFullscreen().then(() => {
                icon.classList.remove("ti-arrows-minimize");
                icon.classList.add("ti-arrows-maximize");
            });
        }
    });
};
