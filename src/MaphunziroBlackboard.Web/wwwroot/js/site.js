// Maphunziro Blackboard - Main JavaScript File

// SignalR Connection
let connection = null;

// Initialize SignalR connection
function initializeSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('/notificationHub')
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Start connection
    async function start() {
        try {
            await connection.start();
            console.log('SignalR Connected');
        } catch (err) {
            console.log('SignalR Connection Error: ', err);
            setTimeout(start, 5000);
        }
    }

    connection.onclose(async () => {
        await start();
    });

    // Handle incoming notifications
    connection.on('ReceiveNotification', (data) => {
        showNotification(data.title, data.message);
        updateNotificationCount();
    });

    // Start the connection
    start();
}

// Show notification
function showNotification(title, message, type = 'info') {
    Swal.fire({
        title: title,
        text: message,
        icon: type,
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 5000,
        timerProgressBar: true
    });
}

// Update notification count
function updateNotificationCount() {
    fetch('/Notifications/GetUnreadCount')
        .then(response => response.json())
        .then(data => {
            const badge = document.getElementById('notificationCount');
            if (badge) {
                badge.textContent = data.count;
                badge.style.display = data.count > 0 ? 'block' : 'none';
            }
        });
}

// Load notifications
function loadNotifications() {
    fetch('/Notifications/GetRecentNotifications')
        .then(response => response.json())
        .then(data => {
            const notificationList = document.getElementById('notificationList');
            if (notificationList && data.length > 0) {
                notificationList.innerHTML = data.map(notif => `
                    <div class="notification-item ${notif.isRead ? '' : 'unread'}">
                        <div class="d-flex justify-content-between align-items-start">
                            <div class="flex-grow-1">
                                <h6 class="mb-1">${notif.title}</h6>
                                <p class="mb-1 small">${notif.message}</p>
                                <small class="text-muted">${formatDate(notif.createdAt)}</small>
                            </div>
                            ${!notif.isRead ? '<span class="badge bg-primary">New</span>' : ''}
                        </div>
                    </div>
                `).join('');
            }
        });
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now - date;
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
    if (hours < 24) return `${hours} hour${hours > 1 ? 's' : ''} ago`;
    if (days < 7) return `${days} day${days > 1 ? 's' : ''} ago`;
    
    return date.toLocaleDateString();
}

// Initialize tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Initialize popovers
function initializePopovers() {
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
}

// Initialize select2
function initializeSelect2() {
    $('.select2').select2({
        theme: 'bootstrap-5',
        width: '100%'
    });
}

// Show loading spinner
function showLoading(element) {
    if (typeof element === 'string') {
        element = document.getElementById(element);
    }
    if (element) {
        element.innerHTML = '<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';
    }
}

// Hide loading spinner
function hideLoading(element, content = '') {
    if (typeof element === 'string') {
        element = document.getElementById(element);
    }
    if (element) {
        element.innerHTML = content;
    }
}

// Confirm action
function confirmAction(title, text, confirmButtonText = 'Confirm', cancelButtonText = 'Cancel') {
    return Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText
    });
}

// Show success message
function showSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: 'Success',
        text: message,
        timer: 3000,
        timerProgressBar: true
    });
}

// Show error message
function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: message
    });
}

// Show info message
function showInfo(message) {
    Swal.fire({
        icon: 'info',
        title: 'Information',
        text: message
    });
}

// AJAX helper
function ajaxRequest(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        }
    };

    if (data && method !== 'GET') {
        options.body = JSON.stringify(data);
    }

    return fetch(url, options)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        });
}

// Form validation
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return false;

    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;

    inputs.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });

    return isValid;
}

// Clear form
function clearForm(formId) {
    const form = document.getElementById(formId);
    if (form) {
        form.reset();
        form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    }
}

// Initialize calendar
function initializeCalendar(elementId, events = []) {
    const calendarEl = document.getElementById(elementId);
    if (!calendarEl) return;

    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: events,
        eventClick: function(info) {
            showEventDetails(info.event);
        },
        eventDrop: function(info) {
            updateEventDate(info.event);
        },
        editable: true,
        selectable: true,
        selectMirror: true,
        dayMaxEvents: true,
        weekends: true,
        themeSystem: 'bootstrap5'
    });

    calendar.render();
    return calendar;
}

// Show event details
function showEventDetails(event) {
    Swal.fire({
        title: event.title,
        html: `
            <p><strong>Start:</strong> ${event.start.toLocaleString()}</p>
            <p><strong>End:</strong> ${event.end ? event.end.toLocaleString() : 'N/A'}</p>
            ${event.extendedProps.description ? `<p><strong>Description:</strong> ${event.extendedProps.description}</p>` : ''}
        `,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'Edit',
        cancelButtonText: 'Close'
    }).then((result) => {
        if (result.isConfirmed) {
            // Handle edit
            window.location.href = `/Calendar/Edit/${event.id}`;
        }
    });
}

// Update event date
function updateEventDate(event) {
    ajaxRequest(`/Calendar/UpdateDate/${event.id}`, 'POST', {
        start: event.start,
        end: event.end
    }).then(data => {
        if (data.success) {
            showSuccess('Event updated successfully');
        } else {
            showError('Failed to update event');
            event.revert();
        }
    }).catch(error => {
        console.error('Error updating event:', error);
        showError('Failed to update event');
        event.revert();
    });
}

// Initialize charts
function initializeChart(elementId, type, data, options = {}) {
    const ctx = document.getElementById(elementId);
    if (!ctx) return;

    const defaultOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'top',
            },
            tooltip: {
                mode: 'index',
                intersect: false,
            }
        }
    };

    const chartOptions = { ...defaultOptions, ...options };
    
    return new Chart(ctx, {
        type: type,
        data: data,
        options: chartOptions
    });
}

// File upload helper
function handleFileUpload(input, maxSize = 10 * 1024 * 1024, allowedTypes = ['image/jpeg', 'image/png', 'application/pdf']) {
    const file = input.files[0];
    if (!file) return true;

    if (file.size > maxSize) {
        showError(`File size must be less than ${maxSize / (1024 * 1024)}MB`);
        input.value = '';
        return false;
    }

    if (!allowedTypes.includes(file.type)) {
        showError('Invalid file type');
        input.value = '';
        return false;
    }

    return true;
}

// Preview image
function previewImage(input, previewId) {
    const file = input.files[0];
    const preview = document.getElementById(previewId);
    
    if (file && file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = function(e) {
            preview.src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
}

// Toggle sidebar
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (sidebar) {
        sidebar.classList.toggle('show');
    }
}

// Dark mode toggle
function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    const isDarkMode = document.body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDarkMode);
}

// Initialize dark mode
function initializeDarkMode() {
    const isDarkMode = localStorage.getItem('darkMode') === 'true';
    if (isDarkMode) {
        document.body.classList.add('dark-mode');
    }
}

// Print helper
function printElement(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const printWindow = window.open('', '_blank');
    printWindow.document.write(`
        <html>
            <head>
                <title>Print</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    table { width: 100%; border-collapse: collapse; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .no-print { display: none; }
                </style>
            </head>
            <body>
                ${element.innerHTML}
            </body>
        </html>
    `);
    printWindow.document.close();
    printWindow.print();
}

// Export to CSV
function exportToCSV(data, filename) {
    const csv = convertToCSV(data);
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
}

// Convert data to CSV
function convertToCSV(data) {
    if (!data || data.length === 0) return '';

    const headers = Object.keys(data[0]);
    const csvHeaders = headers.join(',');
    const csvRows = data.map(row => 
        headers.map(header => {
            const value = row[header];
            return typeof value === 'string' && value.includes(',') 
                ? `"${value}"` 
                : value;
        }).join(',')
    );

    return [csvHeaders, ...csvRows].join('\n');
}

// Initialize everything when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize SignalR if user is authenticated
    if (document.body.dataset.authenticated === 'true') {
        initializeSignalR();
    }

    // Initialize components
    initializeTooltips();
    initializePopovers();
    initializeSelect2();
    initializeDarkMode();

    // Load notifications if user is authenticated
    if (document.body.dataset.authenticated === 'true') {
        loadNotifications();
        updateNotificationCount();
    }

    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 5000);
    });

    // Handle form submissions with AJAX
    document.querySelectorAll('form[data-ajax]').forEach(form => {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (!validateForm(form.id)) {
                return;
            }

            const formData = new FormData(form);
            const url = form.action;
            const method = form.method || 'POST';

            fetch(url, {
                method: method,
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showSuccess(data.message || 'Operation completed successfully');
                    if (data.redirect) {
                        window.location.href = data.redirect;
                    }
                } else {
                    showError(data.message || 'Operation failed');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showError('An error occurred while processing your request');
            });
        });
    });
});

// Global functions
window.MaphunziroBlackboard = {
    showNotification,
    confirmAction,
    showSuccess,
    showError,
    showInfo,
    ajaxRequest,
    validateForm,
    clearForm,
    initializeCalendar,
    initializeChart,
    handleFileUpload,
    previewImage,
    toggleSidebar,
    toggleDarkMode,
    printElement,
    exportToCSV
};
