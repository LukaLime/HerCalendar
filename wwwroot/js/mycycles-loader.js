

// Stores the last URL used in fetchWithRetry so it can be retried if needed
let lastUrl = "";

// UI elements for displaying status and progress
const statusText = document.getElementById("loading-message");
const defaultMessage = "🔄 Loading... please do a little stretch while we get things ready 🧘‍♀️";
const progressBar = document.getElementById("loader-progress-bar");

// Timer/interval control variables
let progressInterval = null;
let messageInterval = null;
let loaderTimer = null; 
let loaderWasShown = false;

// Shows the global loader overlay with progress bar and quirky messages
function showLoader() {
    const loader = document.getElementById("global-loader");
    const progressBar = document.getElementById("loader-progress-bar");
    const message = document.getElementById("loading-message");

    const quirkyMessages = [
        "🔄 Loading... please do a little stretch while we get things ready 🧘‍♀️",
        "🔄 Loading... brewing bits and bytes 🌀",
        "🔄 Loading... stretching the hamsters 🐢",
        "🔄 Loading... calculating awesomeness ⏳",
        "🔄 Loading... pulling data from the magic hat 🎩"
    ];

    if (loader && progressBar) {
        loader.style.display = "flex";
        progressBar.style.width = "0%";
        let progress = 0;

        // Display an initial quirky message and begin rotating them every 10 seconds
        if (message) {
            const firstMessage = quirkyMessages[Math.floor(Math.random() * quirkyMessages.length)];
            message.textContent = firstMessage;

            clearInterval(messageInterval); 
            messageInterval = setInterval(() => {
                const next = quirkyMessages[Math.floor(Math.random() * quirkyMessages.length)];
                message.textContent = next;
            }, 10000);
        }

        // Animate progress bar up to 90%
        clearInterval(progressInterval);
        progressInterval = setInterval(() => {
            if (progress < 90) {
                progress++;
                progressBar.style.width = progress + "%";
            }
        }, 50);
    }
}


// Hides the global loader and resets the intervals
function hideLoader() {
    const loader = document.getElementById("global-loader");
    const progressBar = document.getElementById("loader-progress-bar");

    if (loader && progressBar) {
        progressBar.style.width = "100%";
        clearInterval(progressInterval);
        clearInterval(messageInterval); 

        setTimeout(() => {
            loader.style.display = "none";
        }, 1000);
    }
}

// Fetch a URL with automatic retry logic and loader management
async function fetchWithRetry(url, retries = 2, delay = 3000) {
    lastUrl = url;
    loaderWasShown = false;

    loaderTimer = setTimeout(() => {
        loaderWasShown = true;
        showLoader();  
    }, 3000); // 3 second delay before showing loader

    for (let attempt = 1; attempt <= retries; attempt++) {
        try {
            console.log(`Attempt ${attempt}/${retries} - fetching ${url}`);
            const response = await fetch(url);

            console.log(`Response status: ${response.status}`);
            if (response.status === 503) throw new Error("503 Service Unavailable");
            if (!response.ok) throw new Error(`Status: ${response.status}`);

            // Replace main content with response HTML
            const html = await response.text();
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, "text/html");
            const newMain = doc.querySelector("main");

            if (newMain) {
                document.querySelector("main").innerHTML = newMain.innerHTML;

                // // Reset progress bar and status on success
                if (progressBar) {
                    progressBar.classList.remove("bg-danger", "bg-warning");
                    progressBar.classList.add("bg-primary", "progress-bar-animated");
                }

                if (statusText) {
                    statusText.innerText = defaultMessage;
                }

            }

            // Success — exit loop
            break; 

        } catch (err) {
            console.warn(`Attempt ${attempt} failed: ${err.message}`);

            if (statusText) {
                statusText.innerText = `🔄 Failed to reach server... - Attempting to wake the server`;
            }
            if (progressBar) {
                progressBar.classList.remove("bg-primary");
                progressBar.classList.add("bg-warning");
            }

            if (attempt < retries) {
                // Wait before retrying
                await new Promise(res => setTimeout(res, delay));
            } else {
                // Final failure after all retries
                if (statusText) {
                    statusText.innerText = `❌ We couldn’t reach the server after several tries. Please refresh or try again.`;
                }
                if (progressBar) {
                    progressBar.style.width = "100%";
                    progressBar.classList.remove("bg-warning");
                    progressBar.classList.remove("progress-bar-animated");
                    progressBar.classList.add("bg-danger");
                }

                const retryBtn = document.getElementById("retryBtn");

                if (retryBtn) {
                    retryBtn.style.display = "inline-block";
                }
                // If loader hasn't shown yet, show it now for failure UI
                if (!loaderWasShown) {
                    showLoader(); 
                }
            }
        }
    }
    // Prevent loader from being shown too late
    clearTimeout(loaderTimer);

    if (loaderWasShown) {
        hideLoader(); // Hide loader if it was shown
    }
}

// Retry the last failed fetch
function retryLastFetch() {
    const retryBtn = document.getElementById("retryBtn");

    if (statusText) {
        statusText.innerText = defaultMessage;
    }

    if (retryBtn) retryBtn.style.display = "none";

    if (progressBar) {
        progressBar.style.width = "0%";
        progressBar.classList.remove("bg-danger");
        progressBar.classList.add("bg-primary");
    }

    if (lastUrl) {
        fetchWithRetry(lastUrl, 2, 3000);
    }
}

// Automatically start fetching when landing on MyCycles page
window.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    if (path === '/mycycles' || path === '/mycycles/index') {
        fetchWithRetry('/MyCycles/IndexPartial', 2, 3000);
    }
});


