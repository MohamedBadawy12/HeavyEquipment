

window.addEventListener('scroll', function () {
    var nb = document.getElementById('navbar');
    if (nb) nb.classList.toggle('scrolled', window.scrollY > 50);
});

document.addEventListener('click', function (e) {
    var navLinks = document.querySelector('.nav-links');
    var mobileBtn = document.querySelector('.nav-mobile-btn');
    if (!navLinks) return;
    if (!navLinks.contains(e.target) && (!mobileBtn || !mobileBtn.contains(e.target))) {
        navLinks.classList.remove('mobile-open');
        if (mobileBtn) mobileBtn.textContent = '☰';
    }
});

document.querySelectorAll('.alert').forEach(function (a) {
    setTimeout(function () {
        a.style.transition = 'opacity 0.5s';
        a.style.opacity = '0';
        setTimeout(function () { a.remove(); }, 500);
    }, 4000);
});