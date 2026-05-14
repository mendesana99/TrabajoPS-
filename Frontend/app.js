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

// Notification System
function showNotification(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);
    
    // Add simple inline style for the toast since it wasn't in CSS
    toast.style.position = 'fixed';
    toast.style.bottom = '20px';
    toast.style.right = '20px';
    toast.style.padding = '15px 25px';
    toast.style.borderRadius = '8px';
    toast.style.color = 'white';
    toast.style.fontWeight = 'bold';
    toast.style.zIndex = '9999';
    toast.style.boxShadow = '0 4px 12px rgba(0,0,0,0.15)';
    toast.style.transition = 'opacity 0.3s ease';
    
    if(type === 'error') toast.style.background = '#e74c3c';
    else if(type === 'success') toast.style.background = '#2ecc71';
    else toast.style.background = '#3498db';

    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// 3. Load Data
async function loadEventDetails() {
    try {
        const response = await fetch(`${API_BASE}/Events`);
        const result = await response.json();
        const events = result.data || result; // Fallback in case backend doesn't return PaginatedResult for some reason
        
        const eventsListEl = document.getElementById('events-list');
        eventsListEl.innerHTML = '';
        
        if (events && events.length > 0) {
            events.forEach(event => {
                const card = document.createElement('div');
                card.style.border = '1px solid var(--border-color)';
                card.style.padding = '1rem';
                card.style.borderRadius = '8px';
                card.style.cursor = 'pointer';
                card.style.background = 'var(--panel-bg)';
                card.style.minWidth = '200px';
                card.innerHTML = `
                    <h3>${event.name}</h3>
                    <p>📍 ${event.venue}</p>
                    <small>📅 ${new Date(event.eventDate).toLocaleDateString()}</small>
                `;
                card.onclick = () => selectEvent(event);
                eventsListEl.appendChild(card);
            });
        } else {
            eventsListEl.innerHTML = '<p>No hay eventos activos.</p>';
        }
    } catch (error) {
        console.error('Error cargando eventos:', error);
        document.getElementById('events-list').innerHTML = '<p>Error de conexión al cargar eventos.</p>';
    }
}

async function selectEvent(event) {
    currentEventId = event.id;
    elEventTitle.textContent = event.name;
    elEventVenue.textContent = event.venue;
    
    document.getElementById('event-details-section').style.display = 'block';
    document.getElementById('seats-area-section').style.display = 'block';
    
    await loadSeatsMap();
}

async function loadSeatsMap() {
    try {
        const response = await fetch(`${API_BASE}/Events/${currentEventId}/seats`);
        const seats = await response.json();
        
        elSeatsMap.innerHTML = '';
        
        seats.forEach(seat => {
            const btn = document.createElement('div');
            btn.className = `seat ${seat.status.toLowerCase()}`;
            btn.id = `seat-${seat.id}`;
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
            showNotification('¡Ups! Alguien más reservó este asiento una fracción de segundo antes que tú.', 'error');
            const btn = document.getElementById(`seat-${seatId}`);
            if (btn) {
                btn.className = 'seat reserved';
                btn.onclick = null;
            }
            return;
        }

        if (!response.ok) throw new Error('Error en reserva');

        const result = await response.json();
        currentReservationId = result.reservationId || result.id;
        
        elModalSeatInfo.textContent = `${row} - Asiento ${number}`;
        elModalResId.textContent = currentReservationId;
        
        modalPayment.classList.add('active');
        
        // Optimistic UI update
        const btn = document.getElementById(`seat-${seatId}`);
        if (btn) {
            btn.className = 'seat reserved';
            btn.onclick = null;
        }

        startCountdown(new Date(result.expiresAt));
        
    } catch (error) {
        showNotification('Ocurrió un error al reservar el asiento.', 'error');
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
            showNotification('¡Tiempo excedido! Tu reserva ha sido liberada.', 'error');
            closeModal();
            loadSeatsMap(); // Here we reload all since we don't track the exact seat id in the timer to revert it
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
        const response = await fetch(`${API_BASE}/Reservations/${currentReservationId}/payment`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: USER_ID })
        });
        
        if (!response.ok) throw new Error('Error en pago');
        
        clearInterval(countdownInterval);
        modalPayment.classList.remove('active');
        modalSuccess.classList.add('active');
        
    } catch (error) {
        showNotification(error.message, 'error');
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
