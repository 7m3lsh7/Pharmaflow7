// Button Interaction
const scanBtn = document.getElementById('scanBtn');
scanBtn.addEventListener('click', () => {
    alert('You will be redirected to the QR scanning page!');
    // Replace with actual navigation if needed:
    // window.location.href = '/verify-medicine';
});

// Scroll Animation for Sections
window.addEventListener('scroll', () => {
    const sections = document.querySelectorAll('.features, .how-it-works, .statistics, .testimonials');
    sections.forEach(section => {
        const position = section.getBoundingClientRect().top;
        const screenPosition = window.innerHeight / 1.3;

        if (position < screenPosition) {
            section.style.opacity = '1';
            section.style.transform = 'translateY(0)';
        }
    });
});