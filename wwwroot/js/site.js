// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Home page motion and carousel behavior.
(() => {
  const animated = document.querySelectorAll(".cv-animate");
  if (animated.length > 0) {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("in-view");
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.18, rootMargin: "0px 0px -30px 0px" }
    );
    animated.forEach((el) => observer.observe(el));
  }

  const boutiqueCarousels = document.querySelectorAll(".cv-boutique-carousel");
  boutiqueCarousels.forEach((el) => {
    if (window.bootstrap && window.bootstrap.Carousel) {
      window.bootstrap.Carousel.getOrCreateInstance(el, {
        interval: 4500,
        ride: "carousel",
        pause: "hover",
        touch: true,
        wrap: true
      });
    }
  });

  const dismissToast = (item) => {
    if (!item || item.classList.contains("cv-toast-item--leaving")) return;
    item.classList.add("cv-toast-item--leaving");
    item.addEventListener(
      "animationend",
      () => {
        item.remove();
        const host = document.querySelector(".cv-toast-host");
        if (host && !host.querySelector(".cv-toast-item")) host.remove();
      },
      { once: true }
    );
  };

  document.querySelectorAll(".cv-toast-item").forEach((item) => {
    const closeBtn = item.querySelector(".cv-toast-close");
    closeBtn?.addEventListener("click", () => dismissToast(item));
    window.setTimeout(() => dismissToast(item), 4500);
  });
})();

// Admin panel — sidebar toggle (all breakpoints)
(() => {
  const body = document.body;
  if (!body.classList.contains("cv-admin-body")) return;

  const toggle = document.querySelector("[data-cv-admin-sidebar-toggle]");
  const backdrop = document.querySelector("[data-cv-admin-sidebar-backdrop]");
  const sidebar = document.getElementById("cvAdminSidebar");
  const desktopMq = window.matchMedia("(min-width: 992px)");

  const isOpen = () => body.classList.contains("cv-admin-sidebar-is-open");

  const setOpen = (open) => {
    body.classList.toggle("cv-admin-sidebar-is-open", open);
    toggle?.setAttribute("aria-expanded", String(open));
    if (open && !desktopMq.matches) {
      body.style.overflow = "hidden";
    } else {
      body.style.overflow = "";
    }
  };

  const syncForBreakpoint = () => {
    if (desktopMq.matches) {
      setOpen(true);
    } else {
      setOpen(false);
    }
  };

  toggle?.addEventListener("click", () => setOpen(!isOpen()));
  backdrop?.addEventListener("click", () => setOpen(false));

  sidebar?.querySelectorAll(".nav-link").forEach((link) => {
    link.addEventListener("click", () => {
      if (!desktopMq.matches) setOpen(false);
    });
  });

  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && isOpen()) setOpen(false);
  });

  desktopMq.addEventListener("change", syncForBreakpoint);
  syncForBreakpoint();
})();
