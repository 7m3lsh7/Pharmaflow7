// wwwroot/js/scripts.js
document.getElementById('addProductForm')?.addEventListener('submit', function(event) {
    event.preventDefault();

    const productName = document.getElementById('productName').value.trim();
    const productionDate = document.getElementById('productionDate').value;
    const expirationDate = document.getElementById('expirationDate').value;
    const description = document.getElementById('description').value.trim();

    // Basic validation
    if (!productName || !productionDate || !expirationDate || !description) {
        alert('Please fill in all fields.');
        return;
    }
    if (new Date(productionDate) >= new Date(expirationDate)) {
        alert('Expiration date must be after production date.');
        return;
    }

    // Generate unique product ID (for demo purposes, using timestamp)
    const productId = 'PROD-' + Date.now();

    // Data to encode in QR Code
    const qrData = JSON.stringify({
        id: productId,
        name: productName,
        productionDate: productionDate,
        expirationDate: expirationDate,
        description: description
    });

    // Generate QR Code
    const qrCodeContainer = document.getElementById('qrCodeContainer');
    const qrCodeDiv = document.getElementById('qrCode');
    qrCodeDiv.innerHTML = ''; // Clear previous QR code
    new QRCode(qrCodeDiv, {
        text: qrData,
        width: 200,
        height: 200,
        colorDark: "#000000",
        colorLight: "#ffffff"
    });

    qrCodeContainer.style.display = 'block';
    alert('Product added successfully! QR Code generated.');
});

function downloadQR() {
    const qrCanvas = document.querySelector('#qrCode canvas');
    if (qrCanvas) {
        const link = document.createElement('a');
        link.download = 'product-qr-code.png';
        link.href = qrCanvas.toDataURL('image/png');
        link.click();
    }
}