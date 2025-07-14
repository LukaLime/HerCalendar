
// On page load, automatically fetch cycle data if on the MyCycles page


window.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    if (path === '/mycycles' || path === '/mycycles/index') {
        fetchWithRetry('/MyCycles/IndexPartial', 2, 3000);
    }
});

let lastUrl = "";
let progress = 0;
let progressInterval = null;

async function fetchWithRetry(url, retries = 2, delay = 3000) {
    lastUrl = url;

    const loader = document.getElementById('page-loader');
    const statusText = document.getElementById('loader-status');
    const retryBtn = document.getElementById('retry-btn-container');

    // Show loader
    loader.classList.remove('d-none');
    retryBtn.classList.add('d-none');
    startSimulatedProgress();

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

            finishProgress();
            loader.classList.add('d-none'); // Hide loader
            return;

        } catch (err) {
            console.warn(`Attempt ${attempt} failed: ${err.message}`);

            if (attempt < retries) {
                const dots = '.'.repeat(attempt % 4);
                statusText.innerText = `🔄 Attempt ${attempt}/${retries}${dots} - Waking the server`;
                await new Promise(res => setTimeout(res, delay));
            } else {
                finishProgress();
                statusText.innerText = "❌ We couldn’t reach the server after several tries. Please refresh or try again.";
                retryBtn.classList.remove('d-none');
            }
        }
    }
}

function retryLastFetch() {
    if (lastUrl) {
        fetchWithRetry(lastUrl, 2, 3000);
    }
}

function startSimulatedProgress() {
    const progressBar = document.getElementById('loader-progress');
    progress = 0;
    progressBar.style.width = '0%';
    progressBar.setAttribute('aria-valuenow', 0);

    progressInterval = setInterval(() => {
        if (progress < 95) {
            progress += Math.random() * 5;
            progressBar.style.width = `${progress}%`;
            progressBar.setAttribute('aria-valuenow', Math.floor(progress));
        } else {
            clearInterval(progressInterval); // finish manually on success
        }
    }, 200);
}

function finishProgress() {
    clearInterval(progressInterval);
    const progressBar = document.getElementById('loader-progress');
    progressBar.style.width = '100%';
    progressBar.setAttribute('aria-valuenow', 100);
    progress = 0;
}
