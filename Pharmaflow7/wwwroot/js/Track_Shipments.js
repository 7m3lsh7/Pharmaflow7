let map;
let markers = [];

function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 30.0444, lng: 31.2357 },
        zoom: 6
    });

    const shipments = JSON.parse(document.querySelector('#shipmentData').textContent);
    shipments.forEach(shipment => {
        const marker = new google.maps.Marker({
            position: { lat: shipment.locationLat, lng: shipment.locationLng },
            map: map,
            title: `${shipment.id} - ${shipment.productName}`,
            icon: shipment.status === 'Delivered' ? 'http://maps.google.com/mapfiles/ms/icons/green-dot.png' :
                shipment.status === 'In Transit' ? 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png' :
                    shipment.status === 'Rejected' ? 'http://maps.google.com/mapfiles/ms/icons/red-dot.png' :
                        'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'
        });
        markers.push(marker);

        const infoWindow = new google.maps.InfoWindow({
            content: `<h5>${shipment.id}</h5><p>Product: ${shipment.productName}</p><p>Status: ${shipment.status}</p><p>Location: ${shipment.currentLocation}</p>`
        });
        marker.addListener('click', () => {
            infoWindow.open(map, marker);
        });
    });
}
function acceptShipment(shipmentId) {
    console.log('acceptShipment called with shipmentId:', shipmentId);
    if (!shipmentId || shipmentId === '0' || shipmentId === 0) {
        alert('Invalid shipment ID: ' + shipmentId);
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    console.log('Token:', token);

    // إرسال الـ id في الـ URL كـ query string
    fetch(`/Distributor/AcceptShipment?id=${shipmentId}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token
        }
    })
        .then(response => {
            console.log('Response status:', response.status);
            if (!response.ok) {
                return response.text().then(text => { throw new Error(`Request failed: ${response.status} - ${text}`); });
            }
            return response.json();
        })
        .then(data => {
            console.log('Response data:', data);
            if (data.success) {
                alert(data.message);
                const row = document.querySelector(`tr[data-shipment-id="${shipmentId}"]`);
                row.querySelector('td:nth-child(4)').textContent = 'In Transit';
                row.querySelector('td:last-child').innerHTML = '<span class="text-success">Accepted</span>';
                updateMarker(shipmentId, 'In Transit');
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while accepting the shipment: ' + error.message);
        });
}

function rejectShipment(shipmentId) {
    console.log('rejectShipment called with shipmentId:', shipmentId);
    if (!shipmentId || shipmentId === '0' || shipmentId === 0) {
        alert('Invalid shipment ID: ' + shipmentId);
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    console.log('Token:', token);

    // إرسال الـ id في الـ URL كـ query string
    fetch(`/Distributor/RejectShipment?id=${shipmentId}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token
        }
    })
        .then(response => {
            console.log('Response status:', response.status);
            if (!response.ok) {
                return response.text().then(text => { throw new Error(`Request failed: ${response.status} - ${text}`); });
            }
            return response.json();
        })
        .then(data => {
            console.log('Response data:', data);
            if (data.success) {
                alert(data.message);
                const row = document.querySelector(`tr[data-shipment-id="${shipmentId}"]`);
                row.querySelector('td:nth-child(4)').textContent = 'Rejected';
                row.querySelector('td:last-child').innerHTML = '<span class="text-danger">Rejected</span>';
                updateMarker(shipmentId, 'Rejected');
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while rejecting the shipment: ' + error.message);
        });
}
function updateMarker(shipmentId, newStatus) {
    const marker = markers.find(m => m.title.includes(shipmentId));
    if (marker) {
        marker.setIcon(
            newStatus === 'Delivered' ? 'http://maps.google.com/mapfiles/ms/icons/green-dot.png' :
                newStatus === 'In Transit' ? 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png' :
                    newStatus === 'Rejected' ? 'http://maps.google.com/mapfiles/ms/icons/red-dot.png' :
                        'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'
        );
    }
}

function confirmDelivery(shipmentId) {
    console.log('confirmDelivery called with shipmentId:', shipmentId);
    if (!shipmentId || shipmentId === '0' || shipmentId === 0) {
        alert('Invalid shipment ID: ' + shipmentId);
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    console.log('Token:', token);

    fetch(`/Distributor/ConfirmDelivery?id=${ shipmentId }`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token
        }
    })
        .then(response => {
            console.log('Response status:', response.status);
            if (!response.ok) {
                return response.text().then(text => { throw new Error(`Request failed: ${response.status} - ${text}`); });
            }
            return response.json();
        })
        .then(data => {
            console.log('Response data:', data);
            if (data.success) {
                alert(data.message);
                const row = document.querySelector(`tr[data-shipment-id="${shipmentId}"]`);
                row.querySelector('td:nth-child(4)').textContent = 'Delivered';
                row.querySelector('td:last-child').innerHTML = '<span class="text-success">Delivered</span>';
                updateMarker(shipmentId, 'Delivered');
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while confirming delivery: ' + error.message);
        });
}