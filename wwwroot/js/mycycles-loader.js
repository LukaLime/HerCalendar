
// On page load, automatically fetch cycle data if on the MyCycles page


window.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    if (path === '/mycycles' || path === '/mycycles/index') {
        fetchWithRetry('/MyCycles/IndexPartial', 2, 3000);
    }
});

let lastUrl = "";

async function fetchWithRetry(url, retries = 2, delay = 3000) {
    lastUrl = url;

    const loader = document.getElementById('page-loader');
    const statusText = document.getElementById('loader-status');
    const retryBtn = document.getElementById('retry-btn-container');

    //loader.style.display = "block";
    retryBtn.style.display = "none";
    statusText.innerText = "Fetching your data ⏳";

    let loaderVisible = false;

    // Show loader after a small delay (e.g., 300ms), and only if still loading
    const showLoaderTimeout = setTimeout(() => {
        loader.classList.add('visible');
        loaderVisible = true;
    }, 300);

    for (let attempt = 1; attempt <= retries; attempt++) {
        try {
            console.log(`Attempt ${attempt}/${retries} - fetching ${url}`);
            const response = await fetch(url);
            console.log(`Response status: ${response.status}`);

            if (response.status === 503) {
                throw new Error("503 Service Unavailable");
            }

            if (!response.ok) {
                throw new Error(`Status: ${response.status}`);
            }

            const html = await response.text();
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, "text/html");
            const newMain = doc.querySelector("main");

            if (newMain) {
                document.querySelector("main").innerHTML = newMain.innerHTML;
            }

            //loader.style.display = "none";
            // Then inside successful fetch or final catch, always clear it
            clearTimeout(showLoaderTimeout);

            if (loaderVisible) {
                loader.classList.remove('visible');
            }

            return;

        } catch (err) {
            console.warn(`Attempt ${attempt} failed: ${err.message}`);

            if (attempt < retries) {
                const dots = '.'.repeat(attempt % 4);
                statusText.innerText = `🔄 Attempt ${attempt}/${retries}${dots} - Waking the server`;
                await new Promise(res => setTimeout(res, delay));
            } else {
                clearTimeout(showLoaderTimeout);
                loader.classList.add('visible');
                statusText.innerText = "❌ We couldn’t reach the server after several tries. Please refresh or try again.";
                retryBtn.style.display = "block";
            }
        }
    }
}

// Retry handler (called by Retry button)
function retryLastFetch() {
    if (lastUrl) {
        fetchWithRetry(lastUrl, 2, 3000);
    }
}