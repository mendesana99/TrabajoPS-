const API_BASE = 'http://localhost:5000/api/v1';
let USER_ID = null;
let currentEventId = null;
let currentReservationId = null;
let countdownInterval = null;

// DOM Elements
const catalogView = document.getElementById('catalog-view');
const selectionView = document.getElementById('selection-view');
const elEventsList = document.getElementById('events-list');
const elSeatsMap = document.getElementById('seats-map');
const elCountdown = document.getElementById('countdown');

const modalLogin = document.getElementById('login-modal');
const modalPayment = document.getElementById('payment-modal');
const modalSuccess = document.getElementById('success-modal');

// Initial Load
document.addEventListener('DOMContentLoaded', () => {
    checkSession();
});

// Navigation Functions
function showCatalog() {
    selectionView.classList.add('hidden');
    catalogView.classList.remove('hidden');
    document.getElementById('hero-section').style.display = 'flex';
}

function showSelection() {
    catalogView.classList.add('hidden');
    selectionView.classList.remove('hidden');
    document.getElementById('hero-section').style.display = 'none';
}

// Session Management
function checkSession() {
    const storedUser = localStorage.getItem('loggedUser');
    if (storedUser) {
        const user = JSON.parse(storedUser);
        USER_ID = user.id;
        document.getElementById('user-name').textContent = user.name;
        document.getElementById('user-profile').classList.remove('hidden');
        modalLogin.classList.remove('active');
        loadEvents();
    } else {
        modalLogin.classList.add('active');
    }
}

document.getElementById('btn-login').onclick = async () => {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const btn = document.getElementById('btn-login');

    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
    btn.disabled = true;

    try {
        const response = await fetch(`${API_BASE}/Users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) throw new Error('Credenciales inválidas');

        const user = await response.json();
        localStorage.setItem('loggedUser', JSON.stringify(user));
        checkSession();
        showNotification('Bienvenido de nuevo!', 'success');
    } catch (err) {
        showNotification(err.message, 'error');
    } finally {
        btn.innerHTML = 'Iniciar Sesión';
        btn.disabled = false;
    }
};

document.getElementById('btn-logout').onclick = () => {
    localStorage.removeItem('loggedUser');
    location.reload();
};

// UI Notifications (Toasts)
function showNotification(message, type = 'success') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    const icon = type === 'success' ? 'check-circle' : 'exclamation-circle';
    
    toast.innerHTML = `
        <i class="fas fa-${icon}"></i>
        <span>${message}</span>
    `;
    
    container.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => toast.remove(), 400);
    }, 4000);
}

// Load Events Catalog
async function loadEvents() {
    try {
        const res = await fetch(`${API_BASE}/Events`);
        const result = await res.json();
        const events = result.data || result;

        elEventsList.innerHTML = '';
        events.forEach(event => {
            const card = document.createElement('div');
            card.className = 'event-card';
            card.innerHTML = `
                <div class="card-img"><i class="fas fa-ticket-alt"></i></div>
                <div class="card-body">
                    <div class="event-tag">Tour 2026</div>
                    <h3>${event.name}</h3>
                    <p><i class="fas fa-map-marker-alt"></i> ${event.venue}</p>
                    <p><i class="fas fa-calendar-day"></i> ${new Date(event.eventDate).toLocaleDateString()}</p>
                </div>
            `;
            card.onclick = () => selectEvent(event);
            elEventsList.appendChild(card);
        });
    } catch (err) {
        showNotification('Error al cargar catálogo', 'error');
    }
}

async function selectEvent(event) {
    currentEventId = event.id;
    document.getElementById('event-title').textContent = event.name;
    document.getElementById('event-venue').textContent = event.venue;
    document.getElementById('checkout-event-name').textContent = event.name;
    
    showSelection();
    await loadSeats();
}

async function loadSeats() {
    try {
        const res = await fetch(`${API_BASE}/Events/${currentEventId}/seats`);
        const seats = await res.json();
        
        elSeatsMap.innerHTML = '';
        seats.forEach(seat => {
            const el = document.createElement('div');
            el.className = `seat ${seat.status.toLowerCase()}`;
            el.id = `seat-${seat.id}`;
            
            // Show Row + Number for clarity (e.g. A1, B5)
            el.innerHTML = `<span class="row-id">${seat.rowIdentifier}</span>${seat.seatNumber}`;
            el.title = `Fila ${seat.rowIdentifier} - Asiento ${seat.seatNumber} - $${seat.price}`;
            
            if (seat.status === 'Available') {
                el.onclick = () => reserveSeat(seat);
            }
            elSeatsMap.appendChild(el);
        });
    } catch (err) {
        showNotification('Error al cargar mapa', 'error');
    }
}

// Reservation Flow
async function reserveSeat(seat) {
    if (!USER_ID) return;

    try {
        const res = await fetch(`${API_BASE}/Reservations`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ seatId: seat.id, userId: USER_ID })
        });

        if (res.status === 409) {
            showNotification('El asiento acaba de ser ocupado', 'error');
            loadSeats();
            return;
        }

        if (!res.ok) throw new Error('Error al reservar');

        const result = await res.json();
        currentReservationId = result.reservationId || result.id;
        
        document.getElementById('modal-seat-info').textContent = `${seat.rowIdentifier}${seat.seatNumber}`;
        document.getElementById('modal-total-price').textContent = `$${seat.price}`;
        
        modalPayment.classList.add('active');
        
        // Optimistic UI update
        const el = document.getElementById(`seat-${seat.id}`);
        if (el) {
            el.className = 'seat reserved';
            el.onclick = null;
        }

        startTimer(new Date(result.expiresAt), seat.id);
        showNotification('Asiento bloqueado temporalmente', 'success');
        
    } catch (err) {
        showNotification(err.message, 'error');
    }
}

function startTimer(expiresAt, seatId) {
    clearInterval(countdownInterval);
    countdownInterval = setInterval(() => {
        const now = new Date();
        const diff = expiresAt - now;
        
        if (diff <= 0) {
            clearInterval(countdownInterval);
            modalPayment.classList.remove('active');
            showNotification('La reserva expiró por tiempo.', 'error');
            
            // Partial update: set seat back to available
            const el = document.getElementById(`seat-${seatId}`);
            if (el) {
                el.className = 'seat available';
                el.onclick = () => {
                    const row = el.querySelector('.row-id').textContent;
                    const num = el.textContent.replace(row, '');
                    reserveSeat({ id: seatId, rowIdentifier: row, seatNumber: num, price: document.getElementById('modal-total-price').textContent.replace('$', '') });
                };
            }
            return;
        }
        
        const m = Math.floor((diff / 1000 / 60) % 60);
        const s = Math.floor((diff / 1000) % 60);
        elCountdown.textContent = `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
    }, 1000);
}

document.getElementById('btn-pay').onclick = async () => {
    const btn = document.getElementById('btn-pay');
    btn.innerHTML = 'Procesando... <i class="fas fa-spinner fa-spin"></i>';
    btn.disabled = true;

    try {
        const res = await fetch(`${API_BASE}/Reservations/${currentReservationId}/payments`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: USER_ID })
        });

        if (!res.ok) throw new Error('Error en el pago');

        clearInterval(countdownInterval);
        modalPayment.classList.remove('active');
        modalSuccess.classList.add('active');
    } catch (err) {
        showNotification(err.message, 'error');
    } finally {
        btn.innerHTML = 'Pagar Ahora <i class="fas fa-credit-card"></i>';
        btn.disabled = false;
    }
};

document.getElementById('btn-cancel').onclick = () => {
    modalPayment.classList.remove('active');
    clearInterval(countdownInterval);
    loadSeats();
};
