(function () {

    function initVideoNavScroll() {
        const links = document.querySelectorAll('.video-nav-bar a[href^="#"]');
        if (!links.length) return;

        function getHeaderOffset() {
            const nav = document.querySelector('nav.navbar');
            return nav ? Math.ceil(nav.getBoundingClientRect().height) + 12 : 12;
        }

        function smoothScrollToElement(el) {
            if (!el) return;
            const rect = el.getBoundingClientRect();
            const offset = getHeaderOffset();
            const top = window.scrollY + rect.top - offset;
            window.scrollTo({
                top: Math.max(0, Math.floor(top)),
                behavior: 'smooth'
            });
        }

        links.forEach(link => {
            link.onclick = null;

            link.addEventListener('click', function (ev) {
                const href = this.getAttribute('href');
                if (!href || href === '#') return;

                const id = href.slice(1);
                const target = document.getElementById(id);
                if (!target) return;

                ev.preventDefault();

                smoothScrollToElement(target);

                history.replaceState(null, '', `#${id}`);

                const bsCollapse = document.querySelector('.navbar-collapse.show');
                if (bsCollapse && window.bootstrap?.Collapse) {
                    bootstrap.Collapse.getOrCreateInstance(bsCollapse).hide();
                }

                target.classList.add('scroll-target-highlight');
                setTimeout(() => target.classList.remove('scroll-target-highlight'), 1200);
            });
        });

        // scroll-behavior: smooth jest teraz ustawiane globalnie w CSS (html { scroll-behavior: smooth; }),
        // niezależnie od tego, czy ta funkcja się wykona.
    }

    window.initVideoNavScroll = initVideoNavScroll;

    window.scrollToHashIfAny = () => {
        const hash = window.location.hash;
        if (!hash) return;

        const id = hash.substring(1);
        const target = document.getElementById(id);
        if (!target) return;

        const nav = document.querySelector('nav.navbar');
        const offset = nav ? Math.ceil(nav.getBoundingClientRect().height) + 12 : 12;

        const rect = target.getBoundingClientRect();
        const top = window.scrollY + rect.top - offset;

        window.scrollTo({
            top: Math.max(0, Math.floor(top)),
            behavior: 'smooth'
        });
    };
})();


document.addEventListener('click', function (e) {
    const img = e.target.closest('[data-bs-image]');
    if (!img) return;

    const src = img.getAttribute('data-bs-image');
    const modalImg = document.getElementById('imageModalImg');
    if (modalImg) {
        modalImg.src = src;
    }
});



function initDayOverlay(descriptions) {
    const cells = document.querySelectorAll(".cell");
    const overlay = document.getElementById("overlay");
    const overlayText = document.getElementById("overlay-text");

    if (!cells.length || !overlay || !overlayText) return;

    cells.forEach((cell, index) => {
        cell.addEventListener("click", () => {
            overlayText.innerHTML = descriptions[index % descriptions.length] ?? "Programme à venir…";
            overlay.classList.add("active");
        });
    });
}

window.closeOverlay = function () {
    document.getElementById("overlay")?.classList.remove("active");
};

window.stopPropagation = function (e) {
    if (e) e.stopPropagation();
};


// ===== NIL =====
(function () {

    function preload(frames) {
        return frames.map(src => {
            const img = new Image();
            img.src = src;
            return img;
        });
    }

    function createFilmPlayer(frameElementId, urls) {
        const frames = preload(urls);
        let interval = null;
        let i = 0;

        return {
            start() {
                if (interval !== null) return;

                interval = setInterval(() => {
                    const img = document.getElementById(frameElementId);
                    if (!img) return;

                    i = (i + 1) % frames.length;
                    img.src = frames[i].src;
                }, 1000);
            },
            stop() {
                clearInterval(interval);
                interval = null;
            }
        };
    }

    const nilPlayer1 = createFilmPlayer("nilFrame", [
        "https://worldcruises.blob.core.windows.net/images/Nil/nile1.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/luxor1.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile2.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile3.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile4.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile11.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile5.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile6.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile7.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile8.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile9.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/nile10.jpg"
    ]);

    const nilPlayer2 = createFilmPlayer("nilFrame2", [
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt1.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt13.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt2.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt3.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt4.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt5.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt12.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt6.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt7.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt8.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt9.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt10.jpg",
        "https://worldcruises.blob.core.windows.net/images/Nil/egypt11.jpg"
    ]);

    window.startNilFilm = () => nilPlayer1.start();
    window.stopNilFilm = () => nilPlayer1.stop();

    window.startNilFilm2 = () => nilPlayer2.start();
    window.stopNilFilm2 = () => nilPlayer2.stop();

})();


// ===== SCROLL BUTTONS =====
window.addEventListener("scroll", function () {
    const btn = document.getElementById("backToTopBtn");
    if (!btn) return;

    if (window.scrollY > 250) {
        btn.classList.add("show");
    } else {
        btn.classList.remove("show");
    }
});

function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
}

function scrollDown() {
    window.scrollBy({ top: 800, behavior: "smooth" });
}

window.addEventListener("scroll", () => {
    const btn = document.getElementById("scrollDownBtn");
    if (!btn) return;

    if (window.scrollY > 100) {
        btn.style.opacity = "0";
        btn.style.pointerEvents = "none";
    } else {
        btn.style.opacity = "1";
        btn.style.pointerEvents = "auto";
    }
});

