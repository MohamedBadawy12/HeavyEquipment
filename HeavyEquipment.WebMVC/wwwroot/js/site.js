// ===================================================
//   HeavyHub — Site-wide Shared Scripts
//   File: wwwroot/js/site.js
// ===================================================

// ===== NAVBAR SCROLL EFFECT =====
const navbar = document.querySelector('.navbar');

if (navbar) {
    window.addEventListener('scroll', () => {
        navbar.classList.toggle('scrolled', window.scrollY > 50);
    });
}

// ===== ACTIVE NAV LINK =====
const currentPath = window.location.pathname.toLowerCase();
document.querySelectorAll('.nav-links a').forEach(link => {
    const href = link.getAttribute('href')?.toLowerCase();
    if (href && currentPath.startsWith(href) && href !== '/') {
        link.classList.add('active');
    }
});

// ===== SCROLL REVEAL =====
const revealObserver = new IntersectionObserver((entries) => {
    entries.forEach((entry, i) => {
        if (entry.isIntersecting) {
            setTimeout(() => entry.target.classList.add('visible'), i * 100);
        }
    });
}, { threshold: 0.1 });

document.querySelectorAll('.reveal').forEach(el => revealObserver.observe(el));

// ===== AUTO-HIDE ALERTS =====
document.querySelectorAll('.alert').forEach(alert => {
    setTimeout(() => {
        alert.style.transition = 'opacity 0.5s ease';
        alert.style.opacity = '0';
        setTimeout(() => alert.remove(), 500);
    }, 4000);
});