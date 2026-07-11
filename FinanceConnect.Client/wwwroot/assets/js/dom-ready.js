// DOM ready callback
function P() {
    console.log("DOM is ready!");
    // Your chart rendering code here
}

// DOM ready helper
function domReady(callback) {
    if (document.readyState === "complete" ||
        (document.readyState !== "loading" && !document.documentElement.doScroll)) {
        callback(); // DOM already ready → immediately call
    } else {
        document.addEventListener("DOMContentLoaded", callback); // wait until DOM ready
    }
}

// Call with P
domReady(P);
