let map;
let polyline;

// Initialize the map
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 30.0444, lng: 31.2357 }, // Default center (Cairo, Egypt as an example)
        zoom: 5,
    });
}

function trackShipment() {
    const trackingNumber = document.getElementById('trackingNumber').value;
    const resultDiv = document.getElementById('result');

    // Reset result and map
    resultDiv.innerHTML = '';
    resultDiv.className = 'result';
    if (polyline) {
        polyline.setMap(null); // Clear previous polyline if exists
    }

    if (!trackingNumber) {
        resultDiv.innerHTML = 'Please enter a valid tracking number';
        resultDiv.classList.add('error', 'show');
        return;
    }

    // Simulate tracking process (can be replaced with a real API later)
    setTimeout(() => {
        if (trackingNumber.length === 10 && !isNaN(trackingNumber)) {
            resultDiv.innerHTML = `
                <strong>Tracking Number:</strong> ${trackingNumber}<br>
                <strong>Status:</strong> In Transit<br>
                <strong>Expected Delivery Date:</strong> March 25, 2025
            `;
            resultDiv.classList.add('success', 'show');

            // Simulate shipment path (replace with real data from an API if available)
            const shipmentPath = [
                { lat: 30.0444, lng: 31.2357 }, // Cairo, Egypt (Starting point)
                { lat: 31.2001, lng: 29.9187 }, // Alexandria, Egypt
                { lat: 33.3152, lng: 44.3661 }, // Baghdad, Iraq (Destination)
            ];

            // Draw the path on the map
            polyline = new google.maps.Polyline({
                path: shipmentPath,
                geodesic: true,
                strokeColor: '#FF0000',
                strokeOpacity: 1.0,
                strokeWeight: 2,
            });

            polyline.setMap(map);

            // Adjust map to fit the path
            const bounds = new google.maps.LatLngBounds();
            shipmentPath.forEach(point => bounds.extend(point));
            map.fitBounds(bounds);

            // Add markers for start and end points
            new google.maps.Marker({
                position: shipmentPath[0],
                map: map,
                label: 'Start',
            });

            new google.maps.Marker({
                position: shipmentPath[shipmentPath.length - 1],
                map: map,
                label: 'End',
            });

        } else {
            resultDiv.innerHTML = 'Invalid tracking number. Please check the number.';
            resultDiv.classList.add('error', 'show');
        }
    }, 1000); // Delay to simulate server response
}