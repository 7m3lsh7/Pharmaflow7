// wwwroot/js/scripts.js
// Sample product data (to be replaced with real data from Backend)
let products = [
    { id: 'PROD-12345', name: 'Paracetamol 500mg', productionDate: '2024-01-15', expirationDate: '2026-01-15', description: 'Pain reliever and fever reducer' },
    { id: 'PROD-12346', name: 'Amoxicillin 250mg', productionDate: '2024-03-10', expirationDate: '2026-03-10', description: 'Antibiotic for infections' },
    { id: 'PROD-12347', name: 'Ibuprofen 400mg', productionDate: '2024-05-20', expirationDate: '2026-05-20', description: 'Anti-inflammatory drug' }
];

// Load products into the table
function loadProducts() {
    const tbody = document.getElementById('productTableBody');
    if (tbody) {
        tbody.innerHTML = '';
        products.forEach(product => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${product.id}</td>
                <td>${product.name}</td>
                <td>${product.productionDate}</td>
                <td>${product.expirationDate}</td>
                <td>${product.description}</td>
                <td>
                    <button class="btn btn-primary btn-sm me-2" onclick="editProduct('${product.id}')"><i class="fas fa-edit"></i> Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteProduct('${product.id}')"><i class="fas fa-trash"></i> Delete</button>
                </td>
            `;
            tbody.appendChild(row);
        });
    }
}

// Edit product
function editProduct(id) {
    const product = products.find(p => p.id === id);
    if (product) {
        document.getElementById('editProductId').value = product.id;
        document.getElementById('editProductName').value = product.name;
        document.getElementById('editProductionDate').value = product.productionDate;
        document.getElementById('editExpirationDate').value = product.expirationDate;
        document.getElementById('editDescription').value = product.description;
        new bootstrap.Modal(document.getElementById('editProductModal')).show();
    }
}

// Save edited product
function saveEditedProduct() {
    const id = document.getElementById('editProductId').value;
    const name = document.getElementById('editProductName').value.trim();
    const productionDate = document.getElementById('editProductionDate').value;
    const expirationDate = document.getElementById('editExpirationDate').value;
    const description = document.getElementById('editDescription').value.trim();

    if (!name || !productionDate || !expirationDate || !description) {
        alert('Please fill in all fields.');
        return;
    }
    if (new Date(productionDate) >= new Date(expirationDate)) {
        alert('Expiration date must be after production date.');
        return;
    }

    const productIndex = products.findIndex(p => p.id === id);
    products[productIndex] = { id, name, productionDate, expirationDate, description };
    loadProducts();
    bootstrap.Modal.getInstance(document.getElementById('editProductModal')).hide();
    alert('Product updated successfully!');
}

// Delete product
function deleteProduct(id) {
    if (confirm('Are you sure you want to delete this product?')) {
        products = products.filter(p => p.id !== id);
        loadProducts();
        alert('Product deleted successfully!');
    }
}

// Initial load
loadProducts();