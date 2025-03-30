// Button Interaction
const scanBtn = document.getElementById('scanBtn');
scanBtn.addEventListener('click', () => {
    alert('You will be redirected to the QR scanning page!');
    // Replace with actual navigation if needed:
    // window.location.href = '/verify-medicine';
});

// Scroll Animation for Features Section
window.addEventListener('scroll', () => {
    const features = document.querySelector('.features');
    const position = features.getBoundingClientRect().top;
    const screenPosition = window.innerHeight / 1.3;

    if (position < screenPosition) {
        features.style.opacity = '1';
        features.style.transform = 'translateY(0)';
    }
});