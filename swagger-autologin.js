(function () {
    fetch('/api/dev-auth/swagger-login')
        .then(res => res.json())
        .then(data => {
            const token = data.token;
            function tryPreauth() {
                if (window.ui && typeof window.ui.preauthorizeApiKey === 'function') {
                    window.ui.preauthorizeApiKey("Bearer", token);
                    console.log('[Swagger] Preauthorized dev account');
                    return true;
                }
                return false;
            }
            let attempts = 0;
            const max = 30;
            const interval = setInterval(() => {
                attempts++;
                if (tryPreauth() || attempts > max) clearInterval(interval);
            }, 200);
        })
        .catch(err => console.warn('[Swagger] Auto-login error', err));
})();
