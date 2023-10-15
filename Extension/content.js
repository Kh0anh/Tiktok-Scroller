(async () => {
    var sleep = (ms) => {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    var connect = () => {
        try {
            const socket = new WebSocket('ws://127.0.0.1:8572');

            socket.onopen = (event) => {
                socket.send('TiktokScroll');
            };

            socket.onmessage = (event) => {
                if (event.data == "Next") {
                    (document.querySelector('[data-e2e="arrow-right"]')).click()
                }
                if (event.data == "Like") {
                    (document.querySelector('[data-e2e="browse-like-icon"]')).click()
                }
                if (event.data == "Trash") {
                    var nameF = ((Object.keys(document.querySelector('[data-e2e="browse-ellipsis"]')))[1]).toString();
                    ((document.querySelector('[data-e2e="browse-ellipsis"]'))[nameF]).onMouseEnter();
                    (document.querySelector('[class="tiktok-a5doy7-LiItemWrapper ebh6by61"]')).click();
                }
            };
        } catch {
            connect();
        }
    }
    connect()
})()