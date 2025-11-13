function animateCount(elementId, start, end) {
    const element = document.getElementById(elementId);
    if (!element) return;

    let startTimestamp = null;
    const duration = 1000; // 1 second

    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        element.innerText = Math.floor(progress * (end - start) + start);
        if (progress < 1) {
            window.requestAnimationFrame(step);
        }
    };

    window.requestAnimationFrame(step);
}
