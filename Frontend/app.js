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
const timerPath = document.getElementById('timer-path');

const modalLogin = document.getElementById('login-modal');
const modalPayment = document.getElementById('payment-modal');
const modalSuccess = document.getElementById('success-modal');

// Mapped Posters for realism (Restaurado)
const EVENT_POSTERS = [
    'https://images.unsplash.com/photo-1540039155733-5bb30b53aa14?q=80&w=1974&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1501281668745-f7f57925c3b4?q=80&w=2070&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1459749411177-042180ce673c?q=80&w=2070&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1493225255756-d9584f8606e9?q=80&w=2070&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1470225620780-dba8ba36b745?q=80&w=2070&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1514525253361-bee8a19740c1?q=80&w=1974&auto=format&fit=crop',
    'https://images.unsplash.com/photo-1429962714451-bb934ecbb4ec?q=80&w=2070&auto=format&fit=crop'
];

// Initial Load
document.addEventListener('DOMContentLoaded', () => {
    checkSession();
});

// Navigation Functions
function showCatalog() {
    selectionView.classList.add('hidden');
    catalogView.classList.remove('hidden');
    document.getElementById('hero-section').classList.remove('hidden');
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function showSelection() {
    catalogView.classList.add('hidden');
    selectionView.classList.remove('hidden');
    document.getElementById('hero-section').classList.add('hidden');
    window.scrollTo({ top: 0, behavior: 'smooth' });
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

    btn.innerHTML = '<i class="fas fa-circle-notch fa-spin"></i> Entrando...';
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
        showNotification('¡Bienvenido de nuevo!', 'success');
    } catch (err) {
        showNotification(err.message, 'error');
    } finally {
        btn.innerHTML = 'Entrar <i class="fas fa-arrow-right"></i>';
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

// Load Events Catalog (Restaurado con Posters y Formato)
async function loadEvents() {
    try {
        const res = await fetch(`${API_BASE}/Events`);
        const result = await res.json();
        const events = result.data || result;

        elEventsList.innerHTML = '';
        events.forEach((event, idx) => {
            const poster = EVENT_POSTERS[idx % EVENT_POSTERS.length];
            const card = document.createElement('div');
            card.className = 'event-card';
            card.innerHTML = `
                <div class="card-img" style="background-image: url('${poster}')"></div>
                <div class="card-body">
                    <h3>${event.name}</h3>
                    <p><i class="fas fa-map-marker-alt"></i> ${event.venue}</p>
                    <p><i class="fas fa-calendar-day"></i> ${new Date(event.eventDate).toLocaleDateString('es-AR', { day: 'numeric', month: 'long' })}</p>
                </div>
            `;
            card.onclick = () => selectEvent(event, poster);
            elEventsList.appendChild(card);
        });
    } catch (err) {
        showNotification('Error al conectar con el servidor', 'error');
    }
}

async function selectEvent(event, poster) {
    currentEventId = event.id;
    document.getElementById('event-title').textContent = event.name;
    document.getElementById('event-venue').textContent = event.venue;
    document.getElementById('event-date').textContent = new Date(event.eventDate).toLocaleDateString('es-AR', { day: 'numeric', month: 'long', year: 'numeric' });
    document.getElementById('mini-poster').style.backgroundImage = `url('${poster}')`;
    
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
        showNotification('Error al cargar mapa de asientos', 'error');
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
            showNotification('¡Ups! El asiento se ocupó recién.', 'error');
            loadSeats();
            return;
        }

        if (!res.ok) throw new Error('No se pudo reservar');

        const result = await res.json();
        currentReservationId = result.reservationId || result.id;
        
        const eventName = document.getElementById('event-title').textContent;
        document.getElementById('checkout-event-name').textContent = `${eventName}`;
        document.getElementById('modal-seat-info').textContent = `${seat.rowIdentifier}`;
        document.getElementById('modal-seat-number').textContent = `${seat.seatNumber}`;
        document.getElementById('modal-total-price').textContent = `$${seat.price}`;
        
        modalPayment.classList.add('active');
        
        // Optimistic UI update (Manteniendo mejora)
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
    const duration = 5 * 60 * 1000; // 5 minutes

    countdownInterval = setInterval(() => {
        const now = new Date();
        const diff = expiresAt - now;
        
        if (diff <= 0) {
            clearInterval(countdownInterval);
            modalPayment.classList.remove('active');
            showNotification('La reserva expiró por tiempo.', 'error');
            
            // Partial update: set seat back to available (Manteniendo mejora)
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
        
        // Progress ring logic (Manteniendo estética premium)
        if (timerPath) {
            const percent = (diff / duration) * 100;
            timerPath.setAttribute('stroke-dasharray', `${percent}, 100`);
        }
    }, 1000);
}

document.getElementById('btn-pay').onclick = async () => {
    const btn = document.getElementById('btn-pay');
    const originalText = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Procesando...';
    btn.disabled = true;

    try {
        const res = await fetch(`${API_BASE}/Reservations/${currentReservationId}/payments`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: USER_ID })
        });

        if (!res.ok) throw new Error('Error al procesar el pago');

        clearInterval(countdownInterval);
        modalPayment.classList.remove('active');
        modalSuccess.classList.add('active');
        
        // CELEBRATION!
        if (typeof confetti === 'function') {
            confetti({
                particleCount: 150,
                spread: 70,
                origin: { y: 0.6 },
                colors: ['#6366f1', '#a855f7', '#10b981']
            });
        }

    } catch (err) {
        showNotification(err.message, 'error');
        btn.innerHTML = originalText;
        btn.disabled = false;
    }
};

document.getElementById('btn-cancel').onclick = () => {
    modalPayment.classList.remove('active');
    clearInterval(countdownInterval);
    loadSeats();
};
