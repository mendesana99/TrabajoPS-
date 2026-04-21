const API_BASE = 'http://localhost:5000/api/v1'; //dirección base del back
const USER_ID = 1; // usuario por defecto

//variabels evento, número ticket, cronometro
let currentEventId = null;
let currentReservationId = null;
let countdownInterval = null;

// DOM Elements
const elEventTitle = document.getElementById('event-title');
const elEventVenue = document.getElementById('event-venue');
const elSeatsMap = document.getElementById('seats-map');
const modalPayment = document.getElementById('payment-modal');
const modalSuccess = document.getElementById('success-modal');
const elCountdown = document.getElementById('countdown');
const elModalSeatInfo = document.getElementById('modal-seat-info');
const elModalResId = document.getElementById('modal-reservation-id');
const btnPay = document.getElementById('btn-pay');
const btnCancel = document.getElementById('btn-cancel');

// 1. Iniciar Aplicación --> carga html
document.addEventListener('DOMContentLoaded', async () => {
    await loadEventDetails();
});

// 2. Cargar Evento desde bd 
async function loadEventDetails() {
    try {
        //pedido http get al controlador events
        const response = await fetch(`${API_BASE}/Events`);
        const events = await response.json(); //convierte a json
        
        if (events && events.length > 0) {
            const event = events[0]; // Tomamos el primer evento (Concierto Rock)
            currentEventId = event.id;
            elEventTitle.textContent = event.name;
            elEventVenue.textContent = event.venue;
            
            await loadSeatsMap();
        } else {
            elEventTitle.textContent = 'No hay eventos activos';
        }
    } catch (error) {
        console.error('Error cargando evento:', error);
        elEventTitle.textContent = 'Error de conexión con API';
    }
}

// 3. Inicia compra "reserva temporal"
async function loadSeatsMap() {
    try {
        // Pedimos TODAS las butacas del evento para dibujar el estadio completo
        const response = await fetch(`${API_BASE}/Events/${currentEventId}/seats`);
        const seats = await response.json();
        
        elSeatsMap.innerHTML = ''; // Limpiar mapa
        
        // Simular un grid ordenado
        seats.forEach(seat => {
            const btn = document.createElement('div');
            btn.className = `seat ${seat.status.toLowerCase()}`;
            btn.innerHTML = `<strong>${seat.rowIdentifier}</strong><span>${seat.seatNumber}</span>`;
            
            // Solo las Available se pueden cliquear
            if (seat.status === 'Available') {
                btn.onclick = () => initiateReservation(seat.id, seat.rowIdentifier, seat.seatNumber);
            } else {
                btn.title = `Asiento ${seat.status}`;
            }
            
            elSeatsMap.appendChild(btn);
        });
    } catch (error) {
        console.error('Error cargando map de butacas:', error);
    }
}

// 4. Iniciar Reserva (POST)
async function initiateReservation(seatId, row, number) {
    try {
        const response = await fetch(`${API_BASE}/Reservations`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ seatId: seatId, userId: USER_ID })
        });
        
        if (response.status === 409) {
            alert('¡Ups! Alguien más reservó este asiento una fracción de segundo antes que tú.');
            loadSeatsMap(); // Refrescar para ver que desapareció
            return;
        }

        if (!response.ok) throw new Error('Error en reserva');

        const result = await response.json();
        currentReservationId = result.reservationId;
        
        // Actualizar UI del Modal
        elModalSeatInfo.textContent = `Fila ${row} - Asiento ${number}`;
        elModalResId.textContent = currentReservationId;
        
        // Mostrar Modal y arrancar reloj
        modalPayment.classList.remove('hidden');
        startCountdown(new Date(result.expiresAt));
        
        // Repintar mapa de fondo (para que se ponga naranja "Reserved")
        loadSeatsMap();
        
    } catch (error) {
        alert('Ocurrió un error al reservar el asiento.');
        console.error(error);
    }
}

// 5. Reloj de 5 minutos
function startCountdown(expiresAt) {
    clearInterval(countdownInterval);
    
    countdownInterval = setInterval(() => {
        const now = new Date();
        const diff = expiresAt - now;
        
        if (diff <= 0) {
            clearInterval(countdownInterval);
            elCountdown.textContent = '00:00';
            alert('¡Tiempo excedido! Tu reserva ha sido liberada.');
            closeModal();
            loadSeatsMap(); // Refrescar mapa porque el Worker liberará la butaca
            return;
        }
        
        const m = Math.floor((diff / 1000 / 60) % 60);
        const s = Math.floor((diff / 1000) % 60);
        elCountdown.textContent = `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
        
        // Efecto visual si queda poco tiempo
        if (m === 0 && s < 30) elCountdown.style.color = '#ff0000';
    }, 1000);
}

// 6. Confirmar Pago (POST)
btnPay.onclick = async () => {
    btnPay.textContent = 'Procesando Tarjeta... ⏳';
    btnPay.disabled = true;
    
    try {
        const response = await fetch(`${API_BASE}/Reservations/confirm-payment`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ reservationId: currentReservationId, userId: USER_ID })
        });
        
        if (!response.ok) {
            const err = await response.json();
            throw new Error(err.error || 'Error en pago');
        }
        
        // ÉXITO
        clearInterval(countdownInterval);
        modalPayment.classList.add('hidden');
        modalSuccess.classList.remove('hidden');
        
    } catch (error) {
        alert(error.message);
        btnPay.textContent = '💰 Confirmar Pago y Comprar';
        btnPay.disabled = false;
    }
};

// Utilidades
btnCancel.onclick = closeModal;

function closeModal() {
    modalPayment.classList.add('hidden');
    clearInterval(countdownInterval);
    currentReservationId = null;
    loadSeatsMap(); // Refrescar porque cancelamos
}
