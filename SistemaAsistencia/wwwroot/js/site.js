// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Función para actualizar el reloj en tiempo real
function updateClock() {
    const now = new Date();
    
    // Formatear la hora
    const timeString = now.toLocaleTimeString('es-ES', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false
    });
    
    // Formatear la fecha
    const dateString = now.toLocaleDateString('es-ES', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
    
    // Actualizar los elementos del DOM
    const timeElement = document.getElementById('currentTime');
    const dateElement = document.getElementById('currentDate');
    
    if (timeElement) {
        timeElement.textContent = timeString;
    }
    
    if (dateElement) {
        dateElement.textContent = dateString;
    }
}

// Inicializar el reloj cuando se carga la página
document.addEventListener('DOMContentLoaded', function() {
    // Actualizar inmediatamente
    updateClock();
    
    // Actualizar cada segundo
    setInterval(updateClock, 1000);
    
    // Agregar efecto de click a los botones de asistencia
    const attendanceButtons = document.querySelectorAll('.attendance-btn:not(.disabled)');
    attendanceButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Crear efecto de ondas
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.classList.add('ripple');
            
            this.appendChild(ripple);
            
            // Remover el efecto después de la animación
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
});

// Agregar estilos para el efecto ripple
const style = document.createElement('style');
style.textContent = `
    .attendance-btn {
        position: relative;
        overflow: hidden;
    }
    
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s linear;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);