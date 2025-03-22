let currentStep = 1;

function nextStep(step) {
    document.getElementById(`step${currentStep}`).classList.remove('active');
    document.getElementById(`step${step}`).classList.add('active');
    currentStep = step;

    const progress = (step / 2) * 100;
    document.querySelector('.progress-bar').style.width = `${progress}%`;
}

function prevStep(step) {
    document.getElementById(`step${currentStep}`).classList.remove('active');
    document.getElementById(`step${step}`).classList.add('active');
    currentStep = step;

    const progress = (step / 2) * 100;
    document.querySelector('.progress-bar').style.width = `${progress}%`;
}

function adjustFields() {
    const userType = document.getElementById('userType').value;
    document.getElementById('consumerFields').style.display = userType === 'consumer' ? 'block' : 'none';
    document.getElementById('companyFields').style.display = userType === 'company' ? 'block' : 'none';
    document.getElementById('distributorFields').style.display = userType === 'distributor' ? 'block' : 'none';
}

function validateStep1() {
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value.trim();
    const confirmPassword = document.getElementById('confirmPassword').value.trim();
    const userType = document.getElementById('userType').value;

    let isValid = true;

    if (!userType) {
        document.getElementById('emailError').textContent = 'Please select a user type.';
        document.getElementById('emailError').style.display = 'block';
        isValid = false;
    } else {
        document.getElementById('emailError').style.display = 'none';
    }

    if (!email || !email.includes('@')) {
        document.getElementById('emailError').textContent = 'Please enter a valid email address.';
        document.getElementById('emailError').style.display = 'block';
        isValid = false;
    } else {
        document.getElementById('emailError').style.display = 'none';
    }

    if (password.length < 8) {
        document.getElementById('passwordError').textContent = 'Password must be at least 8 characters.';
        document.getElementById('passwordError').style.display = 'block';
        isValid = false;
    } else {
        document.getElementById('passwordError').style.display = 'none';
    }

    if (password !== confirmPassword) {
        document.getElementById('confirmPasswordError').textContent = 'Passwords do not match.';
        document.getElementById('confirmPasswordError').style.display = 'block';
        isValid = false;
    } else {
        document.getElementById('confirmPasswordError').style.display = 'none';
    }

    if (isValid) {
        adjustFields();
        nextStep(2);
    }
}

function validateStep2() {
    const userType = document.getElementById('userType').value;
    let isValid = true;

    if (userType === 'distributor') {
        const distributorName = document.getElementById('distributorName').value.trim();
        const warehouseAddress = document.getElementById('warehouseAddress').value.trim();
        const contactNumber = document.getElementById('distributorContact').value.trim();

        if (!distributorName) {
            document.getElementById('distributorName').classList.add('is-invalid');
            isValid = false;
        } else {
            document.getElementById('distributorName').classList.remove('is-invalid');
        }
        if (!warehouseAddress) {
            document.getElementById('warehouseAddress').classList.add('is-invalid');
            isValid = false;
        } else {
            document.getElementById('warehouseAddress').classList.remove('is-invalid');
        }
        if (!contactNumber) {
            document.getElementById('distributorContact').classList.add('is-invalid');
            isValid = false;
        } else {
            document.getElementById('distributorContact').classList.remove('is-invalid');
        }
    }

    if (isValid) {
        document.getElementById('signupForm').submit();
    }
}
function loginWithGoogle() {
    alert('Logging in with Google...');
}

function loginWithFacebook() {
    alert('Logging in with Facebook...');
}