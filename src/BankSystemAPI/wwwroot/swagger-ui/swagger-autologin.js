(function () {
    fetch('/api/dev-auth/swagger-login', { credentials: 'same-origin' })
        .then(res => res.json())
        .then(data => {
            const token = data?.token;
            if (!token) return console.warn('[swagger-autologin] no token returned');

            const bearer = 'Bearer ' + token;

            const interval = setInterval(() => {
                if (window.ui?.preauthorizeApiKey) {
                    window.ui.preauthorizeApiKey("Bearer", bearer);
                    console.info('[swagger-autologin] token applied');
                    clearInterval(interval);
                }
            }, 200);
        })
        .catch(err => console.error('[swagger-autologin] fetch error', err));
})();
