const API_BASE = 'http://localhost:5000/api/v1';
let USER_ID = null;
let currentEventId = null;
let currentReservationId = null;
let countdownInterval = null;

// DOM Elements
const elEventTitle = document.getElementById('event-title');
const elEventVenue = document.getElementById('event-venue');
const elSeatsMap = document.getElementById('seats-map');

const modalLogin = document.getElementById('login-modal');
const modalPayment = document.getElementById('payment-modal');
const modalSuccess = document.getElementById('success-modal');

const elCountdown = document.getElementById('countdown');
const elModalSeatInfo = document.getElementById('modal-seat-info');
const elModalResId = document.getElementById('modal-reservation-id');

const btnPay = document.getElementById('btn-pay');
const btnCancel = document.getElementById('btn-cancel');
const btnLogin = document.getElementById('btn-login');
const btnLogout = document.getElementById('btn-logout');

// 1. Initialization
document.addEventListener('DOMContentLoaded', async () => {
    checkSession();
});

// 2. Authentication Logic
function checkSession() {
    const storedUser = localStorage.getItem('loggedUser');
    if (storedUser) {
        const user = JSON.parse(storedUser);
        USER_ID = user.id;
        document.getElementById('user-name').textContent = user.name;
        document.getElementById('user-avatar').textContent = user.name.charAt(0).toUpperCase();
        document.getElementById('user-profile').classList.remove('hidden');
        document.getElementById('user-profile').style.display = 'flex';
        modalLogin.classList.remove('active');
        loadEventDetails();
    } else {
        modalLogin.classList.add('active');
    }
}

btnLogin.onclick = async () => {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const err = document.getElementById('login-error');

    btnLogin.textContent = 'Verificando...';
    btnLogin.disabled = true;
    err.style.display = 'none';

    try {
        const response = await fetch(`${API_BASE}/Users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) throw new Error('Credenciales incorrectas');

        const user = await response.json();
        localStorage.setItem('loggedUser', JSON.stringify(user));
        checkSession();
    } catch (error) {
        err.style.display = 'block';
    } finally {
        btnLogin.textContent = 'Ingresar a mi cuenta';
        btnLogin.disabled = false;
    }
};

btnLogout.onclick = () => {
    localStorage.removeItem('loggedUser');
    location.reload();
};

// 3. Load Data
async function loadEventDetails() {
    try {
        const response = await fetch(`${API_BASE}/Events`);
        const events = await response.json();
        
        if (events && events.length > 0) {
            const event = events[0];
            currentEventId = event.id;
            elEventTitle.textContent = event.name;
            elEventVenue.textContent = event.venue;
            await loadSeatsMap();
        } else {
            elEventTitle.textContent = 'No hay eventos activos';
        }
    } catch (error) {
        console.error('Error cargando evento:', error);
        elEventTitle.textContent = 'Error de conexión';
    }
}

async function loadSeatsMap() {
    try {
        const response = await fetch(`${API_BASE}/Events/${currentEventId}/seats`);
        const seats = await response.json();
        
        elSeatsMap.innerHTML = '';
        
        seats.forEach(seat => {
            const btn = document.createElement('div');
            btn.className = `seat ${seat.status.toLowerCase()}`;
            btn.innerHTML = `<strong>${seat.rowIdentifier}</strong><span>${seat.seatNumber}</span>`;
            
            if (seat.status === 'Available') {
                btn.onclick = () => initiateReservation(seat.id, seat.rowIdentifier, seat.seatNumber);
            } else {
                btn.title = `Asiento ${seat.status}`;
            }
            
            elSeatsMap.appendChild(btn);
        });
    } catch (error) {
        console.error('Error cargando mapa:', error);
    }
}

// 4. Reservation & Payment Flow
async function initiateReservation(seatId, row, number) {
    if(!USER_ID) return;

    try {
        const response = await fetch(`${API_BASE}/Reservations`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ seatId: seatId, userId: USER_ID })
        });
        
        if (response.status === 409) {
            alert('¡Ups! Alguien más reservó este asiento una fracción de segundo antes que tú.');
            loadSeatsMap();
            return;
        }

        if (!response.ok) throw new Error('Error en reserva');

        const result = await response.json();
        currentReservationId = result.reservationId;
        
        elModalSeatInfo.textContent = `${row} - Asiento ${number}`;
        elModalResId.textContent = currentReservationId;
        
        modalPayment.classList.add('active');
        startCountdown(new Date(result.expiresAt));
        loadSeatsMap();
        
    } catch (error) {
        alert('Ocurrió un error al reservar el asiento.');
    }
}

function startCountdown(expiresAt) {
    clearInterval(countdownInterval);
    elCountdown.classList.remove('danger');
    
    countdownInterval = setInterval(() => {
        const now = new Date();
        const diff = expiresAt - now;
        
        if (diff <= 0) {
            clearInterval(countdownInterval);
            elCountdown.textContent = '00:00';
            alert('¡Tiempo excedido! Tu reserva ha sido liberada.');
            closeModal();
            loadSeatsMap();
            return;
        }
        
        const m = Math.floor((diff / 1000 / 60) % 60);
        const s = Math.floor((diff / 1000) % 60);
        elCountdown.textContent = `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        
        if (m === 0 && s <= 30) {
            elCountdown.classList.add('danger');
        }
    }, 1000);
}

btnPay.onclick = async () => {
    btnPay.textContent = 'Procesando Tarjeta... ⏳';
    btnPay.disabled = true;
    
    try {
        const response = await fetch(`${API_BASE}/Reservations/confirm-payment`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ reservationId: currentReservationId, userId: USER_ID })
        });
        
        if (!response.ok) throw new Error('Error en pago');
        
        clearInterval(countdownInterval);
        modalPayment.classList.remove('active');
        modalSuccess.classList.add('active');
        
    } catch (error) {
        alert(error.message);
        btnPay.textContent = 'Pagar y Confirmar';
        btnPay.disabled = false;
    }
};

btnCancel.onclick = closeModal;

function closeModal() {
    modalPayment.classList.remove('active');
    clearInterval(countdownInterval);
    currentReservationId = null;
    loadSeatsMap();
}
